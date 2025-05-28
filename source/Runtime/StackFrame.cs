using NewLang.ParseNodes;

namespace NewLang.Runtime;

public class StackFrame
{
    public FunctionDefinition FuncDef { get; set; }
    public int PC;
    public Dictionary<string, RuntimeValue> LocalVariables { get; private set; }
    public Stack<RuntimeValue> ValueStack { get; private set; }
    public RuntimeValue[] FunctionArgs { get; private set; }
    public StackFrame PreviousFrame { get; set; }

    public StackFrame(FunctionDefinition funcDef, RuntimeValue[] args)
    {
        this.PC = 0;
        this.FuncDef = funcDef;
        this.LocalVariables = [];
        this.ValueStack = [];
        this.FunctionArgs = [..args];
    }
}
