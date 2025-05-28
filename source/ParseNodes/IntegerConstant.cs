using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class IntegerConstant : Expression
{
    public long Value { get; set; }
    
    public IntegerConstant(Token firstToken, long value) : base(firstToken)
    {
        this.Value = value;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeRow.BufferOf(ByteCodeOp.PUSH_INT, this.FirstToken, (int)this.Value);
    }
}