using NewLang.ParseNodes;

namespace NewLang;

public static class FileParser
{
    public static FunctionDefinition[] ParseFile(string filename, string content)
    {
        TokenStream tokens = Tokenizer.Tokenize(filename, content);
        List<FunctionDefinition> funcDefs = [];
        while (tokens.HasMore)
        {
            if (tokens.IsNext("function"))
            {
                funcDefs.Add(EntityParser.ParseFunction(tokens));
            }
        }

        return [..funcDefs];
    }
}