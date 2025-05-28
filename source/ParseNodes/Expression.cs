using NewLang.Runtime;

namespace NewLang.ParseNodes;

public abstract class Expression
{
    public Token FirstToken { get; set; }

    public Expression(Token firstToken)
    {
        this.FirstToken = firstToken;
    }

    public abstract ByteCodeBuffer Serialize();
}
