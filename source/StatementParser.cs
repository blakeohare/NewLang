using System.Diagnostics;
using Microsoft.VisualBasic.CompilerServices;
using NewLang.ParseNodes;

namespace NewLang;

public static class StatementParser
{
    public static Statement[] ParseCodeBlock(TokenStream tokens, bool curlyBraceRequired)
    {
        List<Statement> statements = [];
        if (curlyBraceRequired || tokens.IsNext("{"))
        {
            tokens.PopExpected("{");
            while (!tokens.PopIfPresent("}"))
            {
                statements.Add(StatementParser.ParseStatement(tokens, false));
            }
        }
        else
        {
            statements.Add(StatementParser.ParseStatement(tokens, false));
        }

        return [..statements];
    }

    private static readonly HashSet<string> ASSIGN_OPS = [
        .."= += -= *= /= &= |= ^=".Split(' '),
    ];

    public static Statement ParseStatement(TokenStream tokens, bool insideForLoopStatements)
    {
        tokens.EnsureMore();
        
        if (!insideForLoopStatements)
        {
            switch (tokens.PeekValueNonNull())
            {
                case "if": return ParseIfStatement(tokens);
                case "for": return ParseForLoop(tokens);
                case "return": return ParseReturnStatement(tokens);
                case "while": return ParseWhileLoop(tokens);
                case "break": return ParseBreakStatement(tokens);
                case "continue": return ParseContinueStatement(tokens);
                case "switch": return ParseSwitchStatement(tokens);
                case "throw": return ParseThrowStatement(tokens);
            }
        }

        Expression expr = ExpressionParser.ParseExpression(tokens);

        Statement stmnt;
        if (ASSIGN_OPS.Contains(tokens.PeekValueNonNull()))
        {
            Token opToken = tokens.Pop();
            Expression assignValue = ExpressionParser.ParseExpression(tokens);
            stmnt = new Assignment(expr, opToken, assignValue);
        }
        else
        {
            stmnt = new ExpressionAsStatement(expr);
        }

        if (!insideForLoopStatements)
        {
            tokens.PopExpected(";");
        }
        
        return stmnt;
    }

    private static Statement ParseIfStatement(TokenStream tokens)
    {
        Token ifToken = tokens.PopExpected("if");
        tokens.PopExpected("(");
        Expression condition = ExpressionParser.ParseExpression(tokens);
        tokens.PopExpected(")");
        Statement[] ifCode = StatementParser.ParseCodeBlock(tokens, false);
        Statement[] elseCode = [];
        if (tokens.PopIfPresent("else"))
        {
            elseCode = StatementParser.ParseCodeBlock(tokens, false);
        }

        return new IfStatement(ifToken, condition, ifCode, elseCode);
    }

    private static Statement ParseForLoop(TokenStream tokens)
    {
        Token forToken = tokens.PopExpected("for");
        tokens.PopExpected("(");
        List<Statement> init = [];
        Expression condition = null;
        List<Statement> step = [];
        while (!tokens.PopIfPresent(";"))
        {
            if (init.Count > 0) tokens.PopExpected(",");
            init.Add(StatementParser.ParseStatement(tokens, true));
        }

        if (!tokens.IsNext(";"))
        {
            condition = ExpressionParser.ParseExpression(tokens);
            tokens.PopExpected(";");
        }

        while (!tokens.PopIfPresent(")"))
        {
            if (step.Count > 0) tokens.PopExpected(",");
            step.Add(StatementParser.ParseStatement(tokens, true));
        }

        Statement[] code = StatementParser.ParseCodeBlock(tokens, false);
        return new ForLoop(forToken, init, condition, step, code);
    }

    private static Statement ParseWhileLoop(TokenStream tokens)
    {
        Token whileToken = tokens.PopExpected("while");
        tokens.PopExpected("(");
        Expression condition = ExpressionParser.ParseExpression(tokens);
        tokens.PopExpected(")");
        Statement[] code = StatementParser.ParseCodeBlock(tokens, false);
        return new WhileLoop(whileToken, condition, code);
    }

    private static Statement ParseReturnStatement(TokenStream tokens)
    {
        Token returnTok = tokens.PopExpected("return");
        Expression expr = null;
        if (!tokens.PopIfPresent(";"))
        {
            expr = ExpressionParser.ParseExpression(tokens);
            tokens.PopExpected(";");
        }

        return new ReturnStatement(returnTok, expr);
    }

    private static Statement ParseBreakStatement(TokenStream tokens)
    {
        throw new NotImplementedException();
    }

    private static Statement ParseContinueStatement(TokenStream tokens)
    {
        throw new NotImplementedException();
    }

    private static Statement ParseSwitchStatement(TokenStream tokens)
    {
        throw new NotImplementedException();
    }

    private static Statement ParseThrowStatement(TokenStream tokens)
    {
        Token throwToken = tokens.PopExpected("throw");
        Expression thrownExpr = ExpressionParser.ParseExpression(tokens);
        tokens.PopExpected(";");
        return new ThrowStatement(throwToken, thrownExpr);
    }
}
