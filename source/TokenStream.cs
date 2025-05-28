namespace NewLang;

public class TokenStream
{
    private Token[] tokens;
    private string filename;
    private int index;
    private int length;
    
    public TokenStream(string filename, IList<Token> tokens)
    {
        this.filename = filename;
        this.tokens = [..tokens];
        this.index = 0;
        this.length = this.tokens.Length;
    }

    public Token Peek()
    {
        if (this.index < this.length) return this.tokens[this.index];
        return null;
    }

    public string PeekValue()
    {
        Token t = this.Peek();
        if (t == null) return null;
        return t.Value;
    }

    public string PeekValueNonNull()
    {
        return this.PeekValue() ?? "";
    }

    public Token Pop()
    {
        if (this.index < this.length) return this.tokens[this.index++];
        throw new EndOfFileException(this.filename, " Unexpected end of file.");
    }

    public bool PopIfPresent(string value)
    {
        if (this.IsNext(value))
        {
            this.index++;
            return true;
        }

        return false;
    }
    
    public Token PopExpected(string value)
    {
        Token t = this.Pop();
        if (t.Value == value) return t;
        throw new ParserException(t, "Expected '" + value + "' but found '" + t.Value + "' instead.");
    }

    public Token PopNameToken(string purposeForError)
    {
        Token t = this.Pop();
        if (t.Type == TokenType.WORD) return t;
        throw new ParserException(t, "Expected " + purposeForError + " but found '" + t.Value + "'.");
    }

    public bool IsNext(string value)
    {
        return this.index < this.length && this.tokens[this.index].Value == value;
    }

    public bool HasMore
    {
        get { return this.index < this.length; }
    }

    public void EnsureMore()
    {
        if (this.index >= this.length) throw new EndOfFileException(this.filename, "Unexpected end of file.");
    }
}
