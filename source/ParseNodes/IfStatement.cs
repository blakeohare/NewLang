using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class IfStatement : Statement
{
    public Expression Condition { get; set; }
    public Statement[] IfCode { get; set; }
    public Statement[] ElseCode { get; set; }

    public IfStatement(Token ifToken, Expression condition, IList<Statement> ifCode, IList<Statement> elseCode)
        : base(ifToken)
    {
        this.Condition = condition;
        this.IfCode = [..ifCode];
        this.ElseCode = [..elseCode];
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer conditionBuf = this.Condition.Serialize();
        ByteCodeBuffer ifBuf = Statement.Serialize(this.IfCode);
        int ifLen = ifBuf == null ? 0 : ifBuf.Length;
        ByteCodeBuffer elseBuf = Statement.Serialize(this.ElseCode);
        int elseLen = elseBuf == null ? 0 : elseBuf.Length;

        if (elseBuf == null)
        {
            return ByteCodeBuffer.Join3(
                conditionBuf,
                ByteCodeRow.BufferOf(ByteCodeOp.POP_AND_JUMP_IF_FALSE, this.FirstToken, ifLen),
                ifBuf);
        }

        return ByteCodeBuffer.Join5(
            conditionBuf,
            ByteCodeRow.BufferOf(ByteCodeOp.POP_AND_JUMP_IF_FALSE, this.FirstToken, ifLen + 1),
            ifBuf,
            ByteCodeRow.BufferOf(ByteCodeOp.JUMP, null, elseLen),
            elseBuf);
    }
}
