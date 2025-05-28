using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class BooleanConstant : Expression
{
    public bool Value { get; set; }
    
    public BooleanConstant(Token firstToken, bool value) : base(firstToken)
    {
        this.Value = value;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeRow.BufferOf(ByteCodeOp.PUSH_BOOL, this.FirstToken, this.Value ? 1 : 0);
    }
}