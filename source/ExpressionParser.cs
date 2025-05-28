using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using NewLang.ParseNodes;
using Expression = NewLang.ParseNodes.Expression;

namespace NewLang;

public static class ExpressionParser
{
    private class BinaryOpParser
    {
        private HashSet<string> opChoices;
        private Func<TokenStream, Expression> nextParser;
        private bool isShortCircuit;
    
        public BinaryOpParser(ICollection<string> ops, Func<TokenStream, Expression> nextParser)
        {
            this.opChoices = [..ops];
            this.nextParser = nextParser;
            string sampleOp = this.opChoices.FirstOrDefault();
            this.isShortCircuit = sampleOp == "||" || sampleOp == "&&" || sampleOp == "??";
        }

        public Expression Parse(TokenStream tokens)
        {
            Expression root = this.nextParser(tokens);
            if (this.opChoices.Contains(tokens.PeekValueNonNull()))
            {
                List<Expression> expressions = [root];
                List<Token> ops = [];
                while (this.opChoices.Contains(tokens.PeekValueNonNull()))
                {
                    ops.Add(tokens.Pop());
                    expressions.Add(this.nextParser(tokens));
                }

                Expression acc;
                if (this.isShortCircuit)
                {
                    int last = expressions.Count - 1;
                    acc = expressions[last];
                    for (int i = last - 1; i >= 0; i--)
                    {
                        acc = new BinaryOp(expressions[i], ops[i], acc);
                    }
                }
                else
                {
                    acc = root;
                    for (int i = 1; i < expressions.Count; i++)
                    {
                        acc = new BinaryOp(acc, ops[i - 1], expressions[i]);
                    }
                }

                return acc;
            }

            return root;
        }
    }

    private static readonly BinaryOpParser MULTIPLICATION = new BinaryOpParser(["*", "/", "%"], ParseUnary);
    private static readonly BinaryOpParser ADDITION = new BinaryOpParser(["+", "-"], MULTIPLICATION.Parse);
    private static readonly BinaryOpParser BITSHIFT = new BinaryOpParser(["<<", ">>"], ADDITION.Parse);
    private static readonly BinaryOpParser INEQUALITY = new BinaryOpParser(["<", ">", "<=", ">="], BITSHIFT.Parse);
    private static readonly BinaryOpParser EQUALITY = new BinaryOpParser(["==", "!="], INEQUALITY.Parse);
    private static readonly BinaryOpParser BITWISE_OP = new BinaryOpParser(["&", "|", "^"], EQUALITY.Parse);
    private static readonly BinaryOpParser BOOLEAN_COMBO = new BinaryOpParser(["&&", "||"], BITWISE_OP.Parse);
    private static readonly BinaryOpParser NULL_COALESCE = new BinaryOpParser(["??"], BOOLEAN_COMBO.Parse);
    
    public static Expression ParseExpression(TokenStream tokens)
    {
        return ParseTernary(tokens);
    }

    private static Expression ParseTernary(TokenStream tokens)
    {
        Expression root = NULL_COALESCE.Parse(tokens);
        if (tokens.IsNext("?"))
        {
            Token qmark = tokens.Pop();
            Expression trueVal = ParseTernary(tokens);
            tokens.PopExpected(":");
            Expression falseVal = ParseTernary(tokens);
            return new Ternary(root, qmark, trueVal, falseVal);
        }

        return root;
    }

    private static Expression ParseUnary(TokenStream tokens)
    {
        Token prefixToken;
        switch (tokens.PeekValueNonNull())
        {
            case "-":
                prefixToken = tokens.Pop();
                return new NegativeSign(prefixToken, ParseUnary(tokens));
            case "!":
                prefixToken = tokens.Pop();
                return new BooleanNot(prefixToken, ParseUnary(tokens));
            case "~":
                prefixToken = tokens.Pop();
                return new BitwiseNot(prefixToken, ParseUnary(tokens));

            case "++":
            case "--":
                prefixToken = tokens.Pop();
                return new InlineIncrement(prefixToken, prefixToken, ParseUnary(tokens), true, prefixToken.Value == "++");
        }

        Expression root = ParseAtomicWithSuffixes(tokens);

        switch (tokens.PeekValueNonNull())
        {
            case "++":
            case "--":
                Token inlineIncr = tokens.Pop();
                return new InlineIncrement(root.FirstToken, inlineIncr, root, false, inlineIncr.Value == "++");
        }

        return root;
    }

    private static Expression ParseAtomicWithSuffixes(TokenStream tokens)
    {
        Expression root = ParseAtomic(tokens);
        while (true)
        {
            switch (tokens.PeekValueNonNull())
            {
                case ".":
                    Token dotToken = tokens.Pop();
                    Token fieldName = tokens.PopNameToken("field name");
                    root = new DotField(root, dotToken, fieldName);
                    break;

                case "[":
                    Token bracketToken = tokens.Pop();
                    Expression indexExpr = ParseExpression(tokens);
                    tokens.PopExpected("]");
                    root = new BracketIndex(root, bracketToken, indexExpr);
                    break;

                case "(":
                    Token openParen = tokens.Pop();
                    List<Expression> args = [];
                    while (!tokens.PopIfPresent(")"))
                    {
                        if (args.Count > 0) tokens.PopExpected(",");
                        args.Add(ParseExpression(tokens));
                    }

                    root = new FunctionInvocation(root, openParen, args);
                    break;

                default:
                    return root;
            }
        }
    }

    private static Expression ParseAtomic(TokenStream tokens)
    {
        if (tokens.IsNext("("))
        {
            tokens.Pop();
            Expression wrappedExpr = ParseExpression(tokens);
            tokens.PopExpected(")");
            return wrappedExpr;
        }
        tokens.EnsureMore();
        switch (tokens.Peek().Type)
        {
            case TokenType.WORD:
                Token varToken = tokens.Pop();
                return new Variable(varToken, varToken.Value);
            
            case TokenType.INTEGER:
                Token intTok = tokens.Pop();
                long intValue = ParseIntegerValueFromToken(intTok, 10);
                return new IntegerConstant(intTok, intValue);
                break;
            
            case TokenType.HEX_INTEGER:
                Token hexTok = tokens.Pop();
                long hexValue = ParseIntegerValueFromToken(hexTok, 16);
                return new IntegerConstant(hexTok, hexValue);
            
            case TokenType.FLOAT:
                Token floatTok = tokens.Pop();
                double floatValue = ParseFloatValueFromToken(floatTok);
                return new FloatConstant(floatTok, floatValue);
            
            case TokenType.KEYWORD:
                switch (tokens.PeekValueNonNull())
                {
                    case "true":
                    case "false":
                        Token t = tokens.Pop();
                        return new BooleanConstant(t, t.Value == "true");
                    case "null":
                        return new NullConstant(tokens.Pop());
                    case "new":
                        throw new NotImplementedException(); // we are not ready for such things.
                }

                break;
            case TokenType.STRING:
                Token strTok = tokens.Pop();
                string strValue = ParseStringValueFromToken(strTok);
                return new StringConstant(strTok, strValue);
            
            case TokenType.PUNCTUATION:
                switch (tokens.PeekValueNonNull())
                {
                    case "[": return ParseListExpression(tokens);
                    case "{": return ParseDictionaryExpression(tokens);
                }

                break;
        }

        Token unexpectedToken = tokens.Pop();
        throw new ParserException(unexpectedToken,
            "Expected expression but found '" + unexpectedToken.Value + "' instead.");
    }

    private static Expression ParseListExpression(TokenStream tokens)
    {
        Token firstToken = tokens.PopExpected("[");
        List<Expression> list = [];
        bool nextAllowed = true;
        while (nextAllowed && !tokens.IsNext("]"))
        {
            list.Add(ParseExpression(tokens));
            nextAllowed = tokens.PopIfPresent(",");
        }
        tokens.PopExpected("]");
        return new ListExpression(firstToken, list);
    }

    private static Expression ParseDictionaryExpression(TokenStream tokens)
    {
        Token firstToken = tokens.PopExpected("{");
        List<Expression> keys = [];
        List<Expression> values = [];
        bool nextAllowed = true;
        while (nextAllowed && !tokens.IsNext("}"))
        {
            Expression keyExpr = ParseExpression(tokens);
            if (keyExpr is StringConstant)
            {
                // This is fine.
            }
            else if (keyExpr is Variable varExpr)
            {
                keyExpr = new StringConstant(varExpr.FirstToken, varExpr.Name);
            }
            else
            {
                throw new ParserException(keyExpr.FirstToken, "Invalid expression for dictionary key.");
            }

            keys.Add(keyExpr);
            tokens.PopExpected(":");
            values.Add(ParseExpression(tokens));
            nextAllowed = tokens.PopIfPresent(",");
        }

        tokens.PopExpected("}");
        return new DictionaryExpression(firstToken, keys, values);
    }

    private static Dictionary<char, int> DEC_LOOKUP = new Dictionary<char, int>();
    private static Dictionary<char, int> HEX_LOOKUP = new Dictionary<char, int>();
    
    private static long ParseIntegerValueFromToken(Token tok, int radix)
    {
        if (DEC_LOOKUP.Count == 0)
        {
            string chars = "0123456789abcdef";
            for (int iter = 0; iter < 2; iter++)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (i < 10) DEC_LOOKUP[chars[i]] = i;
                    HEX_LOOKUP[chars[i]] = i;
                }

                chars = chars.ToUpperInvariant();
            }
        }
        
        string rawValue = tok.Value;
        Dictionary<char, int> lookup = DEC_LOOKUP;
        bool isHex = radix == 16;
        if (radix == 16)
        {
            lookup = HEX_LOOKUP;
            rawValue = rawValue.Substring(2);
            if (rawValue.Length == 0) throw new ParserException(tok, "Invalid hexadecimal constant: '" + rawValue);
        }
        else if (radix != 10)
        {
            throw new InvalidOperationException();
        }

        long output = 0;
        for (int i = 0; i < rawValue.Length; i++)
        {
            char c = rawValue[i];
            if (!lookup.ContainsKey(c))
            {
                throw new ParserException(tok, isHex ? "Invalid hexadecimal constant" : "Invalid integer constant");
            }

            int digitValue = lookup[c];
            output = output * radix + digitValue;
        }

        return output;
    }

    internal static string ParseStringValueFromToken(Token tok)
    {
        StringBuilder sb = new StringBuilder();
        string rawValue = tok.Value;
        int length = rawValue.Length - 1;
        for (int i = 1; i < length; i++)
        {
            char c = rawValue[i];
            if (c == '\\')
            {
                if (i + 1 == length)
                {
                    throw new ParserException(tok, "String cannot end with a stray backslash.");
                }

                c = rawValue[i++];
                switch (c)
                {
                    case 'n': c = '\n'; break;
                    case 'r': c = '\r'; break;
                    case 't': c = '\t'; break;
                    case '\\': c = '\\'; break;
                    case '"': c = '"'; break;
                    case '\'': c = '\''; break;
                    case 'u': throw new NotImplementedException(); // TODO: Unicode
                    default: throw new ParserException(tok, "Invalid string escape sequence '\\" + c + "'.");
                }
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private static double ParseFloatValueFromToken(Token tok)
    {
        string rawValue = tok.Value;

        if (double.TryParse(rawValue, CultureInfo.InvariantCulture, out double outVal))
        {
            return outVal;
        }

        throw new ParserException(tok, "Invalid float constant.");
    }
}
