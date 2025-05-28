using System.Globalization;
using NewLang.ParseNodes;

namespace NewLang.Runtime;

public enum RuntimeType
{
    NULL = 1,
    INTEGER = 2,
    BOOLEAN = 3,
    FLOAT = 4,  
    STRING = 5, 
    LIST = 6,
    DICTIONARY = 7,
    INSTANCE = 8, 
    FUNCTION = 9,
    NATIVE_HANDLE = 10,
    
    MAX_BOUND = 11,
}

public class RuntimeValue
{
    public object Value { get; set; }
    public RuntimeType Type { get; set; }

    private static RuntimeValue EMPTY_STRING = new RuntimeValue() { Type = RuntimeType.STRING, Value = "" };
    private static Dictionary<char, RuntimeValue> SINGLE_CHAR_STRING = new Dictionary<char, RuntimeValue>();
    public static RuntimeValue OfString(string val)
    {
        if (val.Length <= 1)
        {
            if (val.Length == 0) return EMPTY_STRING;
            if (SINGLE_CHAR_STRING.TryGetValue(val[0], out RuntimeValue rtval)) return rtval;
            rtval = new RuntimeValue() { Type = RuntimeType.STRING, Value = val };
            SINGLE_CHAR_STRING[val[0]] = rtval;
            return rtval;
        }

        return new RuntimeValue() { Type = RuntimeType.STRING, Value = val };
    }

    public static readonly RuntimeValue TRUE = new RuntimeValue() { Type = RuntimeType.BOOLEAN, Value = true };
    public static readonly RuntimeValue FALSE = new RuntimeValue() { Type = RuntimeType.BOOLEAN, Value = false };

    public static RuntimeValue OfBoolean(bool val)
    {
        return val ? TRUE : FALSE;
    }


    private static readonly RuntimeValue ZERO_F = new RuntimeValue() { Type = RuntimeType.FLOAT, Value = 0.0 };
    private static readonly RuntimeValue ONE_F = new RuntimeValue() { Type = RuntimeType.FLOAT, Value = 1.0 };
    private static readonly RuntimeValue HALF_F = new RuntimeValue() { Type = RuntimeType.FLOAT, Value = 0.5 };

    public static RuntimeValue OfFloat(double val)
    {
        if (val <= 1)
        {
            if (val == 0) return ZERO_F;
            if (val == 1) return ONE_F;
            if (val == 0.5) return HALF_F;
        }

        return new RuntimeValue() { Type = RuntimeType.FLOAT, Value = val };
    }

    private static readonly RuntimeValue[] POSITIVE_INTEGERS;
    private static readonly RuntimeValue[] NEGATIVE_INTEGERS;

    static RuntimeValue()
    {
        POSITIVE_INTEGERS = new RuntimeValue[1024];
        NEGATIVE_INTEGERS = new RuntimeValue[1024];
        for (int i = 0; i < 1024; i++)
        {
            POSITIVE_INTEGERS[i] = new RuntimeValue() { Type = RuntimeType.INTEGER, Value = i };
            NEGATIVE_INTEGERS[i] = new RuntimeValue() { Type = RuntimeType.INTEGER, Value = -i };
        }

        NEGATIVE_INTEGERS[0] = POSITIVE_INTEGERS[0];
    }

    public static RuntimeValue OfInteger(int val)
    {
        if (val < 1024 && val > -1024)
        {
            if (val >= 0) return POSITIVE_INTEGERS[val];
            return NEGATIVE_INTEGERS[-val];
        }

        return new RuntimeValue() { Type = RuntimeType.INTEGER, Value = val };
    }

    public static RuntimeValue OfArray(IList<RuntimeValue> values)
    {
        return new RuntimeValue() { Type = RuntimeType.LIST, Value = new List<RuntimeValue>(values) };
    }

    public static RuntimeValue OfDictionary(DictImpl dictionary)
    {
        return new RuntimeValue() { Type = RuntimeType.DICTIONARY, Value = dictionary };
    }

    public static RuntimeValue NULL = new RuntimeValue() { Type = RuntimeType.NULL, Value = null };

    public static RuntimeValue OfNull()
    {
        return NULL;
    }

    public static RuntimeValue OfFunction(FunctionDefinition? fd, string name)
    {
        return new RuntimeValue() { Type = RuntimeType.FUNCTION, Value = new RuntimeFunctionRef(null, fd, name) };
    }

    public static string ToSimpleString(RuntimeValue val)
    {
        switch (val.Type)
        {
            case RuntimeType.NULL: return "null";
            case RuntimeType.BOOLEAN: return (bool)val.Value ? "true" : "false";
            case RuntimeType.INTEGER: return ((int)val.Value).ToString(CultureInfo.InvariantCulture);
            case RuntimeType.STRING: return (string)val.Value;
            case RuntimeType.LIST: return "<LIST:sz=" + ((List<RuntimeValue>)val.Value).Count + ">";
            case RuntimeType.DICTIONARY: return "<DICTIONAR:sz=" + ((DictImpl)val.Value).Size + ">";
            case RuntimeType.INSTANCE: return "<INSTANCE:" + "ClassName" + "#" + "12345" + ">";
            case RuntimeType.FUNCTION: return "<FUNC:" + ((RuntimeFunctionRef)val.Value).Name + ">";
            case RuntimeType.NATIVE_HANDLE: return "<NATIVEOBJ>";
            
            case RuntimeType.FLOAT: 
                string floatStr = ((double)val.Value).ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
                if (floatStr.Contains('e')) return floatStr;
                if (!floatStr.Contains('.')) floatStr += ".0";
                return floatStr;
            
            default: throw new NotImplementedException();
        }
    }
}
