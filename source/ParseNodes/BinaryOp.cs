using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class BinaryOp : Expression
{

    public Expression Left { get; set; }
    public Expression Right { get; set; }
    public Token OpToken { get; set; }
    public OpType Op { get; set; }

    public BinaryOp(Expression left, Token opToken, Expression right) : base(left.FirstToken)
    {
        this.Left = left;
        this.Right = right;
        this.OpToken = opToken;
        this.Op = OpUtil.GetOpFromOpString(opToken.Value);
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer bufLeft = this.Left.Serialize();
        ByteCodeBuffer bufRight = this.Right.Serialize();

        if (this.Op == OpType.BOOLEAN_AND)
        {
            return ByteCodeBuffer.Join3(
                bufLeft,
                ByteCodeRow.BufferOf(ByteCodeOp.JUMP_IF_FALSE_NO_POP, this.OpToken, bufRight.Length),
                bufRight);
        }

        if (this.Op == OpType.BOOLEAN_OR)
        {
            return ByteCodeBuffer.Join3(
                bufLeft,
                ByteCodeRow.BufferOf(ByteCodeOp.JUMP_IF_TRUE_NO_POP, this.OpToken, bufRight.Length),
                bufRight);
        }

        if (this.Op == OpType.NULL_COALESCE)
        {
            return ByteCodeBuffer.Join3(
                bufLeft,
                ByteCodeRow.BufferOf(ByteCodeOp.JUMP_IF_NON_NULL_NO_POP, this.OpToken, bufRight.Length),
                bufRight);
        }

        return ByteCodeBuffer.Join3(
            this.Left.Serialize(),
            this.Right.Serialize(),
            ByteCodeRow.BufferOf(ByteCodeOp.BINARY_OP, this.OpToken, (int)this.Op));
    }
}
