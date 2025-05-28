using System.Globalization;
using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class FloatConstant : Expression
{
    public double Value { get; set; }
    
    public FloatConstant(Token firstToken, double value) : base(firstToken)
    {
        this.Value = value;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeRow.BufferOf(
            ByteCodeOp.PUSH_FLOAT, this.FirstToken, this.Value.ToString(CultureInfo.InvariantCulture));
    }
}