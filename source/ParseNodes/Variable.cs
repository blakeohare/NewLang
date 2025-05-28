using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class Variable : Expression
{
    public string Name { get; set; }

    public Variable(Token token, string varName) : base(token)
    {
        this.Name = varName;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeRow.BufferOf(ByteCodeOp.PUSH_VARIABLE, this.FirstToken, this.Name);
    }
}
