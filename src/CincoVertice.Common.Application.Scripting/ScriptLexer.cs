using System.Globalization;
using System.Text;
using CincoVertice.Common.Application.Scripting.Helpers;
using CincoVertice.Common.Application.Scripting.Models;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Splits a script into tokens. # starts a comment to the end of the line. A new line ends a statement, except
///     inside parentheses or brackets, so a long call or list can span lines. Text literals use double quotes,
///     with the escapes \" \\ \n and \t; a text of several lines goes between lines with """ and has no escapes.
///     A name can have dotted parts, as in the function clipboard.gettext.
/// </summary>
public static class ScriptLexer
{
    private static readonly HashSet<string> _keywords =
        new(StringComparer.OrdinalIgnoreCase) { "if", "elseif", "else", "end", "and", "or", "not", "true", "false" };

    private static readonly string[] _operators = ["==", "!=", "<=", ">=", "+", "-", "*", "/", "%", "=", "<", ">"];

    public static bool IsKeyword(string name) => _keywords.Contains(name);

    /// <summary>
    ///     Splits the script into tokens, ending with <see cref="TokenType.EndOfScript"/>. Each call uses its own
    ///     <see cref="Scanner"/>, so calls never share state.
    /// </summary>
    /// <exception cref="ScriptException">An unexpected character, an unclosed text, or a number too large.</exception>
    public static List<Token> Tokenize(string source) => new Scanner(source).Tokenize();

    /// <summary>
    ///     One pass over one script: where it is in the source, and the tokens so far.
    /// </summary>
    private sealed class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = [];
        private int _index;
        private int _line = 1;
        private int _lineStart;

        public Scanner(string source)
        {
            _source = source;
        }

        private int Column => _index - _lineStart + 1;

        public List<Token> Tokenize()
        {
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
                else if (current == '"' && Peek(1) == '"' && Peek(2) == '"')
                {
                    ReadMultiLineText();
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

        /// <summary>
        ///     A text of several lines: the lines between a """ at the end of a line and a line starting with """.
        ///     It is taken as it is, without escapes. The spaces before the closing """ are removed from every line
        ///     (<see cref="TextIndent"/>), so the text can be indented with the script.
        /// </summary>
        private void ReadMultiLineText()
        {
            int line = _line;
            int column = Column;

            _index += 3;

            while (Peek(0) is ' ' or '\t' or '\r')
            {
                _index++;
            }

            if (_index < _source.Length && Peek(0) != '\n')
            {
                throw new ScriptException("Start the text on the line after \"\"\"", _line, Column);
            }

            List<string> lines = [];

            while (true)
            {
                if (_index >= _source.Length)
                {
                    throw new ScriptException(
                        "The text is not closed: add a line starting with \"\"\" after it", line, column);
                }

                // Past the new line, to the start of the next line
                _index++;
                _line++;
                _lineStart = _index;

                int end = _source.IndexOf('\n', _index);
                string text = _source[_index..(end < 0 ? _source.Length : end)].TrimEnd('\r');

                // The closing line may go on, e.g. """) to end a call
                if (text.TrimStart().StartsWith("\"\"\"", StringComparison.Ordinal))
                {
                    int indent = text.Length - text.TrimStart().Length;
                    _index += indent + 3;

                    if (!TextIndent.TryRemove(lines, text[..indent], out string value, out int badLine))
                    {
                        throw new ScriptException(
                            "This line starts left of the closing \"\"\": indent it at least as much",
                            line + 1 + badLine,
                            1);
                    }

                    _tokens.Add(new Token(TokenType.Text, value, line, column));

                    return;
                }

                lines.Add(text);
                _index = end < 0 ? _source.Length : end;
            }
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
}
