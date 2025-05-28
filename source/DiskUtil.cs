namespace NewLang;

public static class DiskUtil
{
    public static string ReadTextFile(string path)
    {
        string? output = TryReadTextFile(path);
        if (output != null) return output;
        throw new UserFacingException("File not found: " + System.IO.Path.GetFullPath(path));
    }

    public static string? TryReadTextFile(string path)
    {
        if (!System.IO.File.Exists(path)) return null;
        try
        {
            string data = System.IO.File.ReadAllText(path);
            return data;
        }
        catch (System.IO.IOException)
        {
            return null;
        }
    }
}
