using System.Globalization;
using System.Text;
using CincoVertice.Common.Application.Scripting.Models;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Splits a script into tokens. # starts a comment to the end of the line. A new line ends a statement, except
///     inside parentheses or brackets, so a long call or list can span lines. Text literals use double quotes,
///     with the escapes \" \\ \n and \t. A name can have dotted parts, as in the function clipboard.gettext.
/// </summary>
public class ScriptLexer
{
    private static readonly HashSet<string> _keywords =
        new(StringComparer.OrdinalIgnoreCase) { "if", "elseif", "else", "end", "and", "or", "not", "true", "false" };

    private static readonly string[] _operators = ["==", "!=", "<=", ">=", "+", "-", "*", "/", "%", "=", "<", ">"];

    private string _source = string.Empty;
    private List<Token> _tokens = [];
    private int _index;
    private int _line;
    private int _lineStart;

    private int Column => _index - _lineStart + 1;

    /// <summary>
    ///     Splits the script into tokens, ending with <see cref="TokenType.EndOfScript"/>.
    /// </summary>
    /// <exception cref="ScriptException">An unexpected character, an unclosed text, or a number too large.</exception>
    public List<Token> Tokenize(string source)
    {
        _source = source;
        _tokens = [];
        _index = 0;
        _line = 1;
        _lineStart = 0;
        // Open ( and [ together, since a new line inside either does not end the statement
        int bracketDepth = 0;

        while (_index < _source.Length)
        {
            char current = _source[_index];

            if (current == '\n')
            {
                if (bracketDepth == 0)
                {
                    AddNewLine();
                }

                _index++;
                _line++;
                _lineStart = _index;
            }
            else if (current is ' ' or '\t' or '\r')
            {
                _index++;
            }
            else if (current == '#')
            {
                while (_index < _source.Length && _source[_index] != '\n')
                {
                    _index++;
                }
            }
            else if (char.IsAsciiDigit(current) || current == '.' && char.IsAsciiDigit(Peek(1)))
            {
                ReadNumber();
            }
            else if (char.IsAsciiLetter(current) || current == '_')
            {
                ReadIdentifier();
            }
            else if (current == '"')
            {
                ReadText();
            }
            else if (current == '(')
            {
                bracketDepth++;
                Add(TokenType.LeftParenthesis, "(", 1);
            }
            else if (current == ')')
            {
                bracketDepth = Math.Max(0, bracketDepth - 1);
                Add(TokenType.RightParenthesis, ")", 1);
            }
            else if (current == '[')
            {
                bracketDepth++;
                Add(TokenType.LeftBracket, "[", 1);
            }
            else if (current == ']')
            {
                bracketDepth = Math.Max(0, bracketDepth - 1);
                Add(TokenType.RightBracket, "]", 1);
            }
            else if (current == ',')
            {
                Add(TokenType.Comma, ",", 1);
            }
            else
            {
                ReadOperator();
            }
        }

        AddNewLine();
        _tokens.Add(new Token(TokenType.EndOfScript, string.Empty, _line, Column));

        return _tokens;
    }

    private char Peek(int offset) => _index + offset < _source.Length ? _source[_index + offset] : '\0';

    private void Add(TokenType type, string value, int length)
    {
        _tokens.Add(new Token(type, value, _line, Column));
        _index += length;
    }

    /// <summary>
    ///     Ends the current statement, once: blank lines and lines with only a comment add nothing.
    /// </summary>
    private void AddNewLine()
    {
        if (_tokens.Count > 0 && _tokens[^1].Type != TokenType.NewLine)
        {
            _tokens.Add(new Token(TokenType.NewLine, string.Empty, _line, Column));
        }
    }

    private void ReadNumber()
    {
        int start = _index;
        int column = Column;

        while (char.IsAsciiDigit(Peek(0)))
        {
            _index++;
        }

        if (Peek(0) == '.' && char.IsAsciiDigit(Peek(1)))
        {
            _index++;

            while (char.IsAsciiDigit(Peek(0)))
            {
                _index++;
            }
        }

        string text = _source[start.._index];

        if (!decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
        {
            throw new ScriptException($"The number {text} is too large", _line, column);
        }

        _tokens.Add(new Token(TokenType.Number, text, _line, column));
    }

    private void ReadIdentifier()
    {
        int start = _index;
        int column = Column;

        while (char.IsAsciiLetterOrDigit(Peek(0)) || Peek(0) == '_'
            || Peek(0) == '.' && (char.IsAsciiLetter(Peek(1)) || Peek(1) == '_'))
        {
            _index++;
        }

        // A dotted name such as clipboard.gettext is a function of a module, never a keyword
        string name = _source[start.._index];

        _tokens.Add(_keywords.Contains(name)
            ? new Token(TokenType.Keyword, name.ToLowerInvariant(), _line, column)
            : new Token(TokenType.Identifier, name, _line, column));
    }

    private void ReadText()
    {
        int column = Column;
        var sb = new StringBuilder();

        // Skip the opening quote
        _index++;

        while (true)
        {
            char current = Peek(0);

            if (_index >= _source.Length || current is '\n' or '\r')
            {
                throw new ScriptException("The text is not closed: add a \" at its end", _line, column);
            }

            _index++;

            if (current == '"')
            {
                break;
            }

            if (current != '\\')
            {
                sb.Append(current);

                continue;
            }

            char escaped = Peek(0);

            sb.Append(escaped switch
            {
                '"' => '"',
                '\\' => '\\',
                'n' => '\n',
                't' => '\t',
                _ => throw new ScriptException(
                    $"Unknown escape \\{escaped}: use \\\" \\\\ \\n or \\t", _line, Column - 1)
            });
            _index++;
        }

        _tokens.Add(new Token(TokenType.Text, sb.ToString(), _line, column));
    }

    private void ReadOperator()
    {
        foreach (string op in _operators)
        {
            if (string.CompareOrdinal(_source, _index, op, 0, op.Length) == 0)
            {
                Add(TokenType.Operator, op, op.Length);

                return;
            }
        }

        string hint = _source[_index] switch
        {
            '!' => ": use 'not' instead of '!'",
            '&' => ": use 'and' instead of '&&'",
            '|' => ": use 'or' instead of '||'",
            '\'' => ": texts use double quotes, \"like this\"",
            _ => string.Empty
        };

        throw new ScriptException($"Unexpected character '{_source[_index]}'{hint}", _line, Column);
    }
}
