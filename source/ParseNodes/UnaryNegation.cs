using NewLang.Runtime;

namespace NewLang.ParseNodes;

public abstract class UnaryNegation : Expression
{
    public Expression Root { get; set; }
    public Token OpToken { get; set; }

    public UnaryNegation(Token prefixToken, Expression root) : base(prefixToken)
    {
        this.Root = root;
        this.OpToken = prefixToken;
    }
}

public class NegativeSign : UnaryNegation
{
    public NegativeSign(Token notToken, Expression root) : base(notToken, root)
    {
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join(
            this.Root.Serialize(),
            ByteCodeRow.BufferOf(ByteCodeOp.NEGATIVE_SIGN, this.FirstToken));
    }
}

public class BooleanNot : UnaryNegation
{
    public BooleanNot(Token notToken, Expression root) : base(notToken, root)
    {
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join(
            this.Root.Serialize(),
            ByteCodeRow.BufferOf(ByteCodeOp.BOOLEAN_NOT, this.FirstToken));
    }
}

public class BitwiseNot : UnaryNegation
{
    public BitwiseNot(Token notToken, Expression root) : base(notToken, root)
    {
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join(
            this.Root.Serialize(),
            ByteCodeRow.BufferOf(ByteCodeOp.BITWISE_NOT, this.FirstToken));
    }
}
