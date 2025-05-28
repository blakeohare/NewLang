using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class StringConstant : Expression
{
    public string Value { get; set; }
    
    public StringConstant(Token firstToken, string value) : base(firstToken)
    {
        this.Value = value;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeRow.BufferOf(ByteCodeOp.PUSH_STRING, this.FirstToken, this.Value);
    }
}