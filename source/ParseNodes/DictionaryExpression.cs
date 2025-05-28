using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class DictionaryExpression : Expression
{
    public Expression[] Keys { get; private set; }
    public Expression[] Values { get; private set; }


    public DictionaryExpression(Token firstToken, IList<Expression> keys, IList<Expression> values)
        : base(firstToken)
    {
        this.Keys = [..keys];
        this.Values = [..values];
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer buf = null;
        for (int i = 0; i < this.Keys.Length; i++)
        {
            buf = ByteCodeBuffer.Join3(
                buf, 
                this.Keys[i].Serialize(), 
                this.Values[i].Serialize());
        }

        return ByteCodeBuffer.Join(
            buf,
            ByteCodeRow.BufferOf(ByteCodeOp.DEFINE_DICTIONARY, this.FirstToken, this.Keys.Length));
    }
}

