using NewLang.Runtime;

namespace NewLang.ParseNodes;

public class FunctionDefinition
{
    public Token FirstToken { get; set; }
    public string Name { get; set; }
    public Token NameToken { get; set; }
    public Token[] ArgTokens { get; set; }
    public string[] Args { get; set; }
    public Statement[] Code { get; set; }
    public ByteCodeRow[] ByteCode { get; set; }

    public FunctionDefinition(Token firstToken, Token nameToken, IList<Token> args, IList<Statement> code)
    {
        this.FirstToken = firstToken;
        this.NameToken = nameToken;
        this.Name = nameToken.Value;
        this.ArgTokens = [..args];
        this.Args = [..args.Select(t => t.Value)];
        this.Code = [..code];
    }

    public void SerializeInPlace()
    {
        ByteCodeBuffer buf = null;
        for (int i = 0; i < this.Args.Length; i++)
        {
            buf = ByteCodeBuffer.Join3(
                buf,
                ByteCodeRow.BufferOf(ByteCodeOp.PUSH_ARG, null, i),
                ByteCodeRow.BufferOf(ByteCodeOp.ASSIGN_VARIABLE, this.ArgTokens[i], this.Args[i]));
        }

        buf = ByteCodeBuffer.Join4(
            buf,
            Statement.Serialize(this.Code),
            ByteCodeRow.BufferOf(ByteCodeOp.PUSH_NULL, null),
            ByteCodeRow.BufferOf(ByteCodeOp.RETURN, null));

        this.ByteCode = ByteCodeBuffer.Flatten(buf);
    }
}