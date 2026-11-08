namespace CincoVertice.Common.Utils.Lexer;

public class GenericTokens
{
    /// <summary>Tokens processed.</summary>
    protected List<GenericToken> _tokens = [];

    /// <summary>
    ///     Get Token by index.
    /// </summary>
    /// <param name="index">Token index.</param>
    /// <returns>Return token. If index is not valid, return Null Token with zero length.</returns>
    public GenericToken Token(int index)
    {
        if (index < _tokens.Count)
        {
            return _tokens[index];
        }

        return new GenericToken(startPos: -1, length: 0, type: 0);
    }

    /// <summary>Gets tokens count.</summary>
    public int Count
    {
        get
        {
            return _tokens.Count;
        }
    }

    public void Add(GenericToken token)
    {
        _tokens.Add(token);
    }

    public void Clear()
    {
        _tokens.Clear();
    }
}
