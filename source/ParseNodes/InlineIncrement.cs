using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class InlineIncrement : Expression
{
    public Expression Root { get; set; }
    public Token IncrementToken { get; set; }
    public bool IsPrefix { get; set; }

    public bool IsSuffix
    {
        get { return !this.IsPrefix; }
    }

    public bool IsAddition { get; set; }

    public InlineIncrement(Token firstToken, Token incrOpToken, Expression root, bool isPrefix, bool isAddition)
        : base(firstToken)
    {
        this.Root = root;
        this.IncrementToken = incrOpToken;
        this.IsPrefix = isPrefix;
        this.IsAddition = isAddition;
    }

    public override ByteCodeBuffer Serialize()
    {
        if (this.Root is Variable v)
        {
            return ByteCodeRow.BufferOf(
                ByteCodeOp.INLINE_INCR_VAR,
                this.IncrementToken,
                v.Name,
                this.IsPrefix ? 1 : 0,
                this.IsAddition ? 1 : -1);
        }

        throw new NotImplementedException();
    }
}
