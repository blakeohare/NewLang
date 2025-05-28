using NewLang.Runtime;

namespace NewLang.ParseNodes;

public abstract class Statement
{
    public Token FirstToken { get; set; }

    public Statement(Token firstToken)
    {
        this.FirstToken = firstToken;
    }

    public abstract ByteCodeBuffer Serialize();

    public static ByteCodeBuffer Serialize(Statement[] statements)
    {
        ByteCodeBuffer buf = null;
        foreach (Statement stmnt in statements)
        {
            buf = ByteCodeBuffer.Join(buf, stmnt.Serialize());
        }

        return buf;
    }
}
