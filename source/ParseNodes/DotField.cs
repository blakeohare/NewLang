using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class DotField : Expression
{
    public Expression Root { get; set; }
    public Token DotToken { get; set; }
    public Token FieldNameToken { get; set; }
    public string FieldName { get; set; }

    public DotField(Expression root, Token dotToken, Token fieldNameToken)
        : base(root.FirstToken)
    {
        this.Root = root;
        this.DotToken = dotToken;
        this.FieldNameToken = fieldNameToken;
        this.FieldName = fieldNameToken.Value;
    }

    public override ByteCodeBuffer Serialize()
    {
        return ByteCodeBuffer.Join(
            this.Root.Serialize(), 
            ByteCodeRow.BufferOf(ByteCodeOp.DOT_FIELD, this.DotToken, this.FieldName));
    }
}
