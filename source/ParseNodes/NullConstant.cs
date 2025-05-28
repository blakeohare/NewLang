using System.Globalization;
using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class NullConstant : Expression
{
    public bool Value { get; set; }
    
    public NullConstant(Token firstToken) : base(firstToken)
    {
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeRow.BufferOf(ByteCodeOp.PUSH_NULL, this.FirstToken);
    }
}