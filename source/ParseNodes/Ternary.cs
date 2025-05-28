using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class Ternary : Expression
{
    public Expression Condition { get; set; }
    public Expression TrueValue { get; set; }
    public Expression FalseValue { get; set; }
    public Token QMarkOp { get; set; }

    public Ternary(Expression condition, Token qmark, Expression trueValue, Expression falseValue)
        : base(condition.FirstToken)
    {
        this.Condition = condition;
        this.TrueValue = trueValue;
        this.FalseValue = falseValue;
        this.QMarkOp = qmark;
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer bufCond = this.Condition.Serialize();
        ByteCodeBuffer bufTrue = this.TrueValue.Serialize();
        ByteCodeBuffer bufFalse = this.FalseValue.Serialize();

        bufTrue = ByteCodeBuffer.Join(bufTrue, ByteCodeRow.BufferOf(ByteCodeOp.JUMP, null, bufFalse.Length));
        return ByteCodeBuffer.Join4(
            bufCond,
            ByteCodeRow.BufferOf(ByteCodeOp.POP_AND_JUMP_IF_FALSE, this.QMarkOp, bufTrue.Length),
            bufTrue,
            bufFalse);
    }
}
