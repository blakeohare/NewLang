using NewLang.ParseNodes;

namespace NewLang.Runtime;

public class RuntimeContext
{
    public StackFrame CallStackHead { get; set; }
    public Dictionary<string, FunctionDefinition> FunctionDefinitions { get; set; }

    public RuntimeContext(Dictionary<string, FunctionDefinition> functions)
    {
        FunctionDefinition mainFunc = functions["main"];
        this.FunctionDefinitions = functions;
        this.CallStackHead = new StackFrame(mainFunc, [RuntimeValue.OfArray([])]);
    }
}
