using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class ThrowStatement : Statement
{
    public Expression ThrownExpression { get; private set; }
    
    public ThrowStatement(Token throwToken, Expression expr) : base(throwToken)
    {
        this.ThrownExpression = expr;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join(
            this.ThrownExpression.Serialize(),
            ByteCodeRow.BufferOf(ByteCodeOp.THROW, this.FirstToken));
    }
}