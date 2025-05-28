using NewLang.ParseNodes;

namespace NewLang.Runtime;

public class RuntimeFunctionRef
{
    public bool IsUserDefined { get; private set; }
    public bool IsBuiltIn { get; private set; }
    public FunctionDefinition UserFunction { get; private set; }
    public string Name { get; private set; }
    public int ArgcMax { get; private set; }
    public int ArgcMin { get; private set; }
    public RuntimeValue MethodContext { get; private set; }

    private static readonly Dictionary<string, BuiltInFuncInfo> BUILT_IN_FUNCTIONS =
        new BuiltInFuncInfo[]
        {
            BuiltInFuncInfo.Make("print", 1, 99999),
            BuiltInFuncInfo.Make("floor", 1),

            BuiltInFuncInfo.MakeMethod(RuntimeType.DICTIONARY, "get", 1, 2),

            BuiltInFuncInfo.MakeMethod(RuntimeType.LIST, "add", 1),

            BuiltInFuncInfo.MakeMethod(RuntimeType.STRING, "trim", 0),
        }.ToDictionary(v => (v.MethodRootType != null ? v.MethodRootType + "." : "") + v.Name);

    private class BuiltInFuncInfo
    {
        public string Name { get; set; }
        public int ArgcMin { get; set; }
        public int ArgcMax { get; set; }
        public RuntimeType? MethodRootType { get; set; } = null;

        public static BuiltInFuncInfo Make(string name, int argcMin, int argcMax)
        {
            return new BuiltInFuncInfo() { Name = name, ArgcMin = argcMin, ArgcMax = argcMax };
        }

        public static BuiltInFuncInfo Make(string name, int argc)
        {
            return Make(name, argc, argc);
        }

        public static BuiltInFuncInfo MakeMethod(RuntimeType rtType, string name, int argcMin, int argcMax)
        {
            BuiltInFuncInfo fn = Make(name, argcMin, argcMax);
            fn.MethodRootType = rtType;
            return fn;
        }

        public static BuiltInFuncInfo MakeMethod(RuntimeType rtType, string name, int argc)
        {
            return MakeMethod(rtType, name, argc, argc);
        }
    }

    public static bool IsBuiltInFunctionName(string name)
    {
        return BUILT_IN_FUNCTIONS.ContainsKey(name);
    }

    public RuntimeFunctionRef(RuntimeType? methodRootType, FunctionDefinition funcDef, string name)
    {
        this.Name = name;
        if (funcDef != null)
        {
            this.IsUserDefined = true;
            this.IsBuiltIn = false;
            this.UserFunction = funcDef;
        }
        else if (methodRootType == null && BUILT_IN_FUNCTIONS.ContainsKey(name))
        {
            this.IsBuiltIn = true;
            this.IsUserDefined = false;
            this.ArgcMin = BUILT_IN_FUNCTIONS[name].ArgcMin;
            this.ArgcMax = BUILT_IN_FUNCTIONS[name].ArgcMax;
        }
        else if (methodRootType != null && BUILT_IN_FUNCTIONS.TryGetValue(methodRootType + "." + name, out BuiltInFuncInfo funcInfo))
        {
            this.IsBuiltIn = true;
            this.IsUserDefined = false;
            this.ArgcMin = funcInfo.ArgcMin;
            this.ArgcMax = funcInfo.ArgcMax;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public static RuntimeValue? TryGetBuiltInMethod(RuntimeValue methodRoot, string name)
    {
        string lookupKey = methodRoot.Type + "." + name;
        if (!BUILT_IN_FUNCTIONS.TryGetValue(lookupKey, out BuiltInFuncInfo funcInfo)) return null;
        RuntimeFunctionRef func = new RuntimeFunctionRef(methodRoot.Type, null, name) { MethodContext = methodRoot };
        return new RuntimeValue() { Type = RuntimeType.FUNCTION, Value = func };
    }
}
