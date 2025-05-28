using NewLang.ParseNodes;
using NewLang.Runtime;

namespace NewLang;

public class UserFacingException : Exception
{
      public UserFacingException(string msg) : base(msg)
      {
      }
}

public class RuntimeException : UserFacingException
{
      private static string BuildRuntimeExceptionString(StackFrame frame, string msg)
      {
            List<string> lines = [msg, "", "Stack Trace:"];
            for (StackFrame? walker = frame; walker != null; walker = walker.PreviousFrame)
            {
                  string row = walker.FuncDef.Name + " [" + walker.FuncDef.FirstToken.FileName + "] ";
                  Token traceToken = walker.FuncDef.ByteCode[walker.PC].Token;
                  if (traceToken == null)
                  {
                        row += " Line ?? Col ??";
                  }
                  else
                  {
                        row += " Line " + traceToken.Line + " Col " + traceToken.Col;
                  }

                  lines.Add(row);
            }

            return string.Join("\n", lines);
      }

      public RuntimeException(StackFrame frame, string msg) : base(BuildRuntimeExceptionString(frame, msg))
      {
      }
}

public class ParserException : UserFacingException
{
      private static string TokenToLocInfo(Token? tok)
      {
            if (tok == null) return "";
            return tok.FileName + " Line " + tok.Line + " Col " + tok.Col + ": ";
      }

      public ParserException(Token? token, string msg) : base(TokenToLocInfo(token) + msg)
      {
      }

      public ParserException(string msg) : base(msg)
      {
      }
}

public class EndOfFileException : ParserException
{
      public EndOfFileException(string filename, string msg) : base(filename + ": " + msg)
      {
      }
}
