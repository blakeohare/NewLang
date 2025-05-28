using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class ExpressionAsStatement : Statement
{
    public Expression Expression { get; set; }

    public ExpressionAsStatement(Expression expr) : base(expr.FirstToken)
    {
        this.Expression = expr;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join(
            this.Expression.Serialize(), 
            ByteCodeRow.BufferOf(ByteCodeOp.POP, null));
    }
}
