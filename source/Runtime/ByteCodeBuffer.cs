namespace NewLang.Runtime;

public class ByteCodeBuffer
{
    public ByteCodeRow LeafRow;
    public ByteCodeBuffer Left;
    public ByteCodeBuffer Right;
    public int Length;
    public bool IsLeaf;

    public static ByteCodeBuffer Of(ByteCodeRow row)
    {
        return new ByteCodeBuffer()
        {
            LeafRow = row,
            IsLeaf = true,
            Left = null,
            Right = null,
            Length = 1,
        };
    }

    public static ByteCodeRow[] Flatten(ByteCodeBuffer buffer)
    {
        if (buffer == null) return [];
        
        List<ByteCodeRow> output = [];
        int expectedSize = buffer.Length;
        Stack<ByteCodeBuffer> queue = new Stack<ByteCodeBuffer>();
        queue.Push(buffer);
        
        while (queue.Count > 0)
        {
            ByteCodeBuffer current = queue.Pop();
            if (current.IsLeaf)
            {
                output.Add(current.LeafRow);
            }
            else
            {
                queue.Push(current.Right);
                queue.Push(current.Left);
            }
        }

        if (output.Count != expectedSize) throw new InvalidOperationException();
        return [..output];
    }
    
    public static ByteCodeBuffer Join(ByteCodeBuffer a, ByteCodeBuffer b)
    {
        if (a == null) return b;
        if (b == null) return a;
        return new ByteCodeBuffer()
        {
            LeafRow = null,
            IsLeaf = false,
            Left = a,
            Right = b,
            Length = a.Length + b.Length,
        };
    }

    public static ByteCodeBuffer Join4(ByteCodeBuffer a, ByteCodeBuffer b, ByteCodeBuffer c, ByteCodeBuffer d)
    {
        return Join(Join(a, b), Join(c, d));
    }

    public static ByteCodeBuffer Join5(
        ByteCodeBuffer a,
        ByteCodeBuffer b,
        ByteCodeBuffer c,
        ByteCodeBuffer d,
        ByteCodeBuffer e)
    {
        return Join8(a, b, c, d, e, null, null, null);
    }

    public static ByteCodeBuffer Join6(
        ByteCodeBuffer a,
        ByteCodeBuffer b,
        ByteCodeBuffer c,
        ByteCodeBuffer d,
        ByteCodeBuffer e,
        ByteCodeBuffer f)
    {
        return Join8(a, b, c, d, e, f, null, null);
    }

    public static ByteCodeBuffer Join8(
        ByteCodeBuffer a,
        ByteCodeBuffer b,
        ByteCodeBuffer c,
        ByteCodeBuffer d,
        ByteCodeBuffer e,
        ByteCodeBuffer f,
        ByteCodeBuffer g,
        ByteCodeBuffer h)
    {
        return Join(Join4(a, b, c, d), Join4(e, f, g, h));
    }

    public static ByteCodeBuffer Join3(ByteCodeBuffer a, ByteCodeBuffer b, ByteCodeBuffer c)
    {
        return Join(Join(a, b), c);
    }
}
