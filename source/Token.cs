namespace NewLang;

public enum TokenType
{
    WORD,
    KEYWORD,
    INTEGER, 
    FLOAT, 
    HEX_INTEGER, 
    STRING, 
    PUNCTUATION,
}

public class Token
{
    public string FileName { get; set; }
    public string Value { get; set; }
    public TokenType Type { get; set; }
    public int Line { get; set; }
    public int Col { get; set; }

    public Token(string filename, string value, TokenType type, int line, int col)
    {
        this.FileName = filename;
        this.Value = value;
        this.Type = type;
        this.Line = line;
        this.Col = col;
    }
}
