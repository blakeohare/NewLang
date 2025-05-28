using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class ReturnStatement : Statement
{
    public Expression? Expression { get; private set; }

    public ReturnStatement(Token returnToken, Expression? value) : base(returnToken)
    {
        this.Expression = value;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join(
            this.Expression == null
                ? ByteCodeRow.BufferOf(ByteCodeOp.PUSH_NULL, null)
                : this.Expression.Serialize(),
            ByteCodeRow.BufferOf(ByteCodeOp.RETURN, this.FirstToken));
    }
}
