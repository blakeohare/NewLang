using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class ForLoop : Statement
{
    public Expression? Condition { get; set; }
    public Statement[] Init { get; set; }
    public Statement[] Step { get; set; }
    public Statement[] Code { get; set; }

    public ForLoop(Token forToken, IList<Statement> init, Expression? condition, IList<Statement> step,
        IList<Statement> code)
        : base(forToken)
    {
        this.Init = [..init];
        this.Condition = condition;
        this.Step = [..step];
        this.Code = [..code];
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer bufInit = null;
        foreach (Statement stmnt in this.Init)
        {
            bufInit = ByteCodeBuffer.Join(bufInit, stmnt.Serialize());
        }
        
        ByteCodeBuffer bufCond = this.Condition == null 
            ? ByteCodeRow.BufferOf(ByteCodeOp.PUSH_BOOL, this.FirstToken, 1)
            : this.Condition.Serialize();
        int condLength = bufCond.Length;

        ByteCodeBuffer bufStep = null;
        foreach (Statement stmnt in this.Step)
        {
            bufStep = ByteCodeBuffer.Join(bufStep, stmnt.Serialize());
        }

        int stepLength = bufStep == null ? 0 : bufStep.Length;

        ByteCodeBuffer bufLoopBody = Statement.Serialize(this.Code);
        int loopBodyLength = bufLoopBody == null ? 0 : bufLoopBody.Length;
        
        // TODO: resolve break and continue
        
        return ByteCodeBuffer.Join6(
            bufInit, 
            bufCond,
            ByteCodeRow.BufferOf(ByteCodeOp.POP_AND_JUMP_IF_FALSE, this.FirstToken, loopBodyLength + stepLength + 1),
            bufLoopBody,
            bufStep,
            ByteCodeRow.BufferOf(ByteCodeOp.JUMP, null, -1 - stepLength - loopBodyLength - 1 - condLength));
    }
}
