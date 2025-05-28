namespace NewLang;

public class Tokenizer
{
    private enum TokenizerState
    {
        READY,
        STRING,
        COMMENT,
        WORD,
    }

    private static readonly HashSet<string> KEYWORDS =
    [
        "true", "false",
        "null",
        "function",
        "return", "if", "for", "while",
        "break", "continue",
        "new",
        "import",
        "throw",
    ];

    private static readonly HashSet<string> MULTICHAR_TOKENS =
    [
        "==", "!=",
        "<=", ">=",
        "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", 
        "&&", "||",
        "<<", ">>", "<<=", ">>=",
        "++", "--",
        "**",
        "??",
    ];

    private static readonly HashSet<char> MUTLICHAR_STARTERS =
    [
        ..MULTICHAR_TOKENS.Select(s => s[0])
    ];

    private static readonly HashSet<char> WHITESPACE = [.." \r\n\t".ToCharArray()];
    private static readonly HashSet<char> DIGITS = [.."0123456789".ToCharArray()];

    private static readonly HashSet<char> VALID_IDENTIFIER_CHARS =
    [
        .."abcdefghijklmnopqrstuvwxyz".ToCharArray(),
        .."ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
        ..DIGITS,
        '_',
    ];

    public static TokenStream Tokenize(string filename, string code)
    {
        return new TokenStream(filename, TokenizeImpl(filename, code));
    }
    
    private static IList<Token> TokenizeImpl(string filename, string code)
    {
        code = code.Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd() + "\n";
        int length = code.Length;
        int[] lines = new int[length];
        int[] cols = new int[length];
        int line = 1;
        int col = 1;
        for (int i = 0; i < length; i++)
        {
            lines[i] = line;
            cols[i] = col;
            if (code[i] == '\n')
            {
                col = 1;
                line++;
            }
            else
            {
                col++;
            }
        }

        List<Token> tokens = [];

        TokenizerState state = TokenizerState.READY;
        int tokenStart = -1;
        char tokenSubType = ' ';

        for (int i = 0; i < length; i++)
        {
            char c = code[i];
            switch (state)
            {
                case TokenizerState.READY:
                    if (WHITESPACE.Contains(c))
                    {
                        // skip!
                    }
                    else if (VALID_IDENTIFIER_CHARS.Contains(c))
                    {
                        tokenStart = i;
                        state = TokenizerState.WORD;
                    }
                    else if (c == '"' || c == '\'')
                    {
                        tokenStart = i;
                        state = TokenizerState.STRING;
                        tokenSubType = c;
                    }
                    else if (c == '/' && (code[i + 1] == '/' || code[i + 1] == '*'))
                    {
                        i++;
                        tokenSubType = code[i];
                        state = TokenizerState.COMMENT;
                    }
                    else
                    {
                        int tokenLength = 1;
                        if (MUTLICHAR_STARTERS.Contains(c))
                        {
                            string c2 = code.Substring(i, 2);
                            if (MULTICHAR_TOKENS.Contains(c2))
                            {

                                tokenLength = 2;
                                if ((c2 == "<<" || c2 == ">>") && code[i + 2] == '=')
                                {
                                    tokenLength = 3;
                                }
                            }
                        }

                        string token = code.Substring(i, tokenLength);
                        tokens.Add(new Token(filename, token, TokenType.PUNCTUATION, lines[i], cols[i]));
                        i += tokenLength - 1;
                    }

                    break;

                case TokenizerState.STRING:
                    if (c == tokenSubType)
                    {
                        string stringToken = code.Substring(tokenStart, i - tokenStart + 1);
                        tokens.Add(new Token(filename, stringToken, TokenType.STRING, lines[tokenStart],
                            cols[tokenStart]));
                        state = TokenizerState.READY;
                    }
                    else if (c == '\\')
                    {
                        // valid escape sequences are filtered later.
                        i++;
                    }
                    else
                    {
                        // continue
                    }

                    break;

                case TokenizerState.COMMENT:
                    if (tokenSubType == '/')
                    {
                        if (c == '\n')
                        {
                            state = TokenizerState.READY;
                        }
                    }
                    else if (tokenSubType == '*')
                    {
                        if (c == '*' && code[i + 1] == '/')
                        {
                            state = TokenizerState.READY;
                            i++;
                        }
                    }
                    else
                    {
                        // ignore
                    }

                    break;

                case TokenizerState.WORD:
                    if (!VALID_IDENTIFIER_CHARS.Contains(c))
                    {
                        string wordToken = code.Substring(tokenStart, i - tokenStart);
                        TokenType type;
                        if (KEYWORDS.Contains(wordToken))
                        {
                            type = TokenType.KEYWORD;
                        }
                        else if (wordToken[0] == '0' && wordToken.ToLowerInvariant().StartsWith("0x"))
                        {
                            type = TokenType.HEX_INTEGER;
                        }
                        else if (DIGITS.Contains(wordToken[0]))
                        {
                            type = TokenType.INTEGER;
                        }
                        else
                        {
                            type = TokenType.WORD;
                        }

                        tokens.Add(new Token(filename, wordToken, type, lines[tokenStart], cols[tokenStart]));
                        state = TokenizerState.READY;
                        --i;
                    }

                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        for (int i = 0; i < tokens.Count; i++)
        {
            Token current = tokens[i];
            if (current != null && current.Value == ".")
            {
                Token left = i > 0 ? tokens[i - 1] : null;
                bool leftAdj = left != null &&
                               left.Type == TokenType.INTEGER &&
                               left.Line == current.Line &&
                               left.Col + left.Value.Length == current.Col;
                if (leftAdj)
                {
                    left.Value += ".";
                    tokens[i] = left;
                    tokens[i - 1] = null;
                    current = left;
                    current.Type = TokenType.FLOAT;
                }
                
                Token right = i + 1 < tokens.Count ? tokens[i + 1] : null;
                bool rightAdj = right != null &&
                                right.Type == TokenType.INTEGER &&
                                right.Line == current.Line &&
                                current.Col + current.Value.Length == right.Col;

                if (rightAdj)
                {
                    current.Type = TokenType.FLOAT;
                    current.Value += right.Value;
                    tokens[i + 1] = null;
                }
            }
        }

        return tokens.Where(t => t != null).ToArray();
    }
}
