using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class FunctionInvocation : Expression
{
    public Expression Root { get; set; }
    public Token InvokeToken { get; set; }
    public Expression[] Args { get; set; }

    public FunctionInvocation(Expression root, Token invokeToken, IList<Expression> args)
        : base(root.FirstToken)
    {
        this.Root = root;
        this.InvokeToken = invokeToken;
        this.Args = [..args];
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer buf = this.Root.Serialize();
        for (int i = 0; i < this.Args.Length; i++)
        {
            buf = ByteCodeBuffer.Join(buf, this.Args[i].Serialize());
        }

        return ByteCodeBuffer.Join(
            buf,
            ByteCodeRow.BufferOf(ByteCodeOp.INVOKE_FUNCTION, this.InvokeToken, this.Args.Length));
    }
}
