using NewLang.ParseNodes;

namespace NewLang;

public static class EntityParser
{
    public static FunctionDefinition ParseFunction(TokenStream tokens)
    {
        Token firstToken = tokens.PopExpected("function");
        Token nameToken = tokens.PopNameToken("function name");
        tokens.PopExpected("(");
        List<Token> args = [];
        while (!tokens.PopIfPresent(")"))
        {
            if (args.Count > 0) tokens.PopExpected(",");
            args.Add(tokens.PopNameToken("argument name"));
        }

        IList<Statement> code = StatementParser.ParseCodeBlock(tokens, true);
        return new FunctionDefinition(firstToken, nameToken, args, code);
    }
}
