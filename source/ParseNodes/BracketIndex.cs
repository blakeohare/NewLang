using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class BracketIndex : Expression
{
    public Expression Root { get; set; }
    public Token BracketToken { get; set; }
    public Expression Index { get; set; }

    public BracketIndex(Expression root, Token bracketToken, Expression indexExpr)
        : base(root.FirstToken)
    {
        this.Root = root;
        this.BracketToken = bracketToken;
        this.Index = indexExpr;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join3(
            this.Root.Serialize(),
            this.Index.Serialize(),
            ByteCodeRow.BufferOf(ByteCodeOp.INDEX, this.BracketToken));
    }
}
