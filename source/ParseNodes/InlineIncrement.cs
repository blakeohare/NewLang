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
        throw new NotImplementedException();
    }
}
