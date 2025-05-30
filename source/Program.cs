﻿using NewLang.ParseNodes;
using NewLang.Runtime;

namespace NewLang;

internal static class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please specify a path");
            return;
        }

        string file = args[0];
#if DEBUG
        MainImpl(file);
#else
        try
        {
            MainImpl(file);
        }
        catch (UserFacingException ufe)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ufe.Message);
            Console.ForegroundColor = color;
        }
#endif
    }

    private static void MainImpl(string rawPath)
    {
        string path = System.IO.Path.GetFullPath(rawPath);
        string content = DiskUtil.ReadTextFile(path);
        FunctionDefinition[] funcs = FileParser.ParseFile(path);

        Dictionary<string, FunctionDefinition> funcLookup = new Dictionary<string, FunctionDefinition>();
        foreach (FunctionDefinition fd in funcs)
        {
            if (funcLookup.ContainsKey(fd.Name))
                throw new ParserException(fd.FirstToken, "Duplicate function definition: '" + fd.Name + "'.");
            funcLookup[fd.Name] = fd;
            fd.SerializeInPlace();
        }

        if (!funcLookup.ContainsKey("main"))
        {
            throw new UserFacingException("No function definition for main()");
        }

        RuntimeContext execCtx = new RuntimeContext(funcLookup);
        Interpreter.Interpret(execCtx);
    }
}
