using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class WhileLoop : Statement
{
    public Expression Condition { get; set; }
    public Statement[] Code { get; set; }

    public WhileLoop(Token whileToken, Expression condition, IList<Statement> code)
        : base(whileToken)
    {
        this.Condition = condition;
        this.Code = [..code];
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer bufCond = this.Condition.Serialize();
        ByteCodeBuffer bufCode = Statement.Serialize(this.Code);
        int codeLength = bufCode == null ? 0 : bufCode.Length;
        return ByteCodeBuffer.Join4(
            bufCond, 
            ByteCodeRow.BufferOf(ByteCodeOp.POP_AND_JUMP_IF_FALSE, this.FirstToken, codeLength + 1), 
            bufCode, 
            ByteCodeRow.BufferOf(ByteCodeOp.JUMP, null, -1 - codeLength - 1 - bufCond.Length));
    }
}
