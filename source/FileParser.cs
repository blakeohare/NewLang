using NewLang.ParseNodes;

namespace NewLang;

public static class FileParser
{
    public static FunctionDefinition[] ParseFile(string filename)
    {
        string filenameAbsolute = System.IO.Path.GetFullPath(filename);
        List<FunctionDefinition> functionsOut = [];
        ParseFileImpl(null, filenameAbsolute, functionsOut, [filenameAbsolute]);
        return [..functionsOut];
    }

    public static void ParseFileImpl(Token? importThrowToken, string fileAbsolutePath,
        List<FunctionDefinition> functionsOut,
        HashSet<string> cycleChecks)
    {
        string? content = DiskUtil.TryReadTextFile(fileAbsolutePath);
        if (content == null)
        {
            throw new ParserException(importThrowToken, "Path not found: '" + fileAbsolutePath + "'");
        }

        string currentDirectoryAbsolute = System.IO.Path.GetDirectoryName(fileAbsolutePath);

        string displayPath = fileAbsolutePath; // TODO: display path should be shown relative to the initial root.
        TokenStream tokens = Tokenizer.Tokenize(displayPath, content);
        List<FunctionDefinition> funcDefs = [];
        while (tokens.HasMore)
        {
            if (tokens.IsNext("function"))
            {
                functionsOut.Add(EntityParser.ParseFunction(tokens));
            }
            else if (tokens.IsNext("import"))
            {
                Token importToken = tokens.PopExpected("import");
                tokens.EnsureMore();
                Token pathToken = tokens.Pop();
                if (pathToken.Type != TokenType.STRING)
                {
                    throw new ParserException(pathToken,
                        "Unexpected token: '" + pathToken.Value + "'. Expected import path.");
                }

                tokens.PopExpected(";");
                string path = ExpressionParser.ParseStringValueFromToken(pathToken);
                string newPathAbsolute =
                    System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectoryAbsolute, path));
                if (!cycleChecks.Contains(newPathAbsolute))
                {
                    cycleChecks.Add(newPathAbsolute);
                    ParseFileImpl(importToken, newPathAbsolute, functionsOut, cycleChecks);
                }
            }
            else
            {
                throw new ParserException(tokens.Peek(), "Unexpected top-level code.");
            }
        }
    }
}
