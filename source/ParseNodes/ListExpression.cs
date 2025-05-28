using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class ListExpression : Expression
{
    public Expression[] Items { get; private set; }
    
    public ListExpression(Token firstToken, IList<Expression> items)
        : base(firstToken)
    {
        this.Items = [..items];
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer buf = null;
        for (int i = 0; i < this.Items.Length; i++)
        {
            buf = ByteCodeBuffer.Join(buf, this.Items[i].Serialize());
        }

        return ByteCodeBuffer.Join(
            buf,
            ByteCodeRow.BufferOf(ByteCodeOp.DEFINE_LIST, this.FirstToken, this.Items.Length));
    }
}
