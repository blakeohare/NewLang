namespace NewLang.Runtime;

public class ByteCodeRow
{
    public ByteCodeOp Op;
    public string StringValue;
    public Token? Token;
    public int[] Args;
    public int FirstArg;
    public RuntimeValue CacheValue;

    public static ByteCodeBuffer BufferOf(ByteCodeOp op, Token? token, params int[] args)
    {
        return ByteCodeBuffer.Of(ByteCodeRow.Of(op, token, args));
    }
    
    public static ByteCodeBuffer BufferOf(ByteCodeOp op, Token? token, string strArg, params int[] args)
    {
        return ByteCodeBuffer.Of(ByteCodeRow.Of(op, token, strArg, args));
    }
    
    public static ByteCodeRow Of(ByteCodeOp op, Token? token, params int[] args)
    {
        return new ByteCodeRow()
        {
            Op = op,
            Token = token,
            Args = [..args],
            FirstArg = args.Length == 0 ? 0 : args[0],
            CacheValue = null,
            StringValue = null,
        };
    }

    public static ByteCodeRow Of(ByteCodeOp op, Token? token, string strArg, params int[] args)
    {
        ByteCodeRow row = ByteCodeRow.Of(op, token, args);
        row.StringValue = strArg;
        return row;
    }
}
