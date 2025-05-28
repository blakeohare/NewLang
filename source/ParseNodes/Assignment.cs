using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class Assignment : Statement
{
    public Expression Target { get; set; }
    public Expression Value { get; set; }
    public Token OpToken { get; set; }
    public string Op { get; set; }

    public Assignment(Expression target, Token opToken, Expression value) : base(target.FirstToken)
    {
        this.Target = target;
        this.Value = value;
        this.OpToken = opToken;
        this.Op = opToken.Value;
    }

    public override ByteCodeBuffer Serialize()
    {
        ByteCodeBuffer valueBuf = this.Value.Serialize();
        bool isIncremental = this.Op != "=";
        string incrementalOp = this.Op.Substring(1);
        
        if (this.Target is Variable varTarget)
        {
            if (isIncremental)
            {
                throw new NotImplementedException();
            }

            return ByteCodeBuffer.Join(
                valueBuf, 
                ByteCodeRow.BufferOf(ByteCodeOp.ASSIGN_VARIABLE, varTarget.FirstToken, varTarget.Name));
        }

        throw new NotImplementedException();
    }
}
