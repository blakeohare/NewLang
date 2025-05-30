using System.Runtime.CompilerServices;
using System.Text;

namespace NewLang.Runtime;

public static class Interpreter
{
    private const int OP_OFFSET = (int)OpType.MAX_BOUND + (int)RuntimeType.MAX_BOUND;

    private const int TYPE_INT = (int)RuntimeType.INTEGER;
    private const int TYPE_FLOAT = (int)RuntimeType.FLOAT;
    private const int TYPE_BOOLEAN = (int)RuntimeType.BOOLEAN;
    private const int TYPE_NULL = (int)RuntimeType.NULL;
    private const int TYPE_STRING = (int)RuntimeType.STRING;
    private const int TYPE_LIST = (int)RuntimeType.LIST;
    private const int TYPE_DICTIONARY = (int)RuntimeType.DICTIONARY;
    private const int TYPE_INSTANCE = (int)RuntimeType.INSTANCE;
    private const int TYPE_FUNCTION = (int)RuntimeType.FUNCTION;
    private const int TYPE_NATIVE_HANDLE = (int)RuntimeType.NATIVE_HANDLE;

    private const int OP_ADD = (int)OpType.ADDITION;
    private const int OP_SUB = (int)OpType.SUBTRACTION;
    private const int OP_MULTIPLY = (int)OpType.MULTIPLICATION;
    private const int OP_DIVIDE = (int)OpType.DIVISION;
    private const int OP_MOD = (int)OpType.MODULO;
    private const int OP_EQ = (int)OpType.EQUAL;
    private const int OP_NOT_EQ = (int)OpType.NOT_EQUAL;
    private const int OP_LT = (int)OpType.LESS_THAN;
    private const int OP_GT = (int)OpType.GREATER_THAN;
    private const int OP_LTEQ = (int)OpType.LESS_THAN_EQ;
    private const int OP_GTEQ = (int)OpType.GREATER_THAN_EQ;
    private const int OP_BITWISE_AND = (int)OpType.BITWISE_AND;
    private const int OP_BITWISE_OR = (int)OpType.BITWISE_OR;
    private const int OP_BITWISE_XOR = (int)OpType.BITWISE_XOR;
    private const int OP_BOOL_AND = (int)OpType.BOOLEAN_AND;
    private const int OP_BOOL_OR = (int)OpType.BOOLEAN_OR;
    private const int OP_EXP = (int)OpType.EXPONENT;
    private const int OP_BITSHIFT_LEFT = (int)OpType.BITSHIFT_LEFT;
    private const int OP_BITSHIFT_RIGHT = (int)OpType.BITSHIFT_RIGHT;
    private const int OP_NULL_COAL = (int)OpType.NULL_COALESCE;

    public static void Interpret(RuntimeContext rtCtx)
    {
        StackFrame frame = rtCtx.CallStackHead;
        ByteCodeRow[] byteCode = frame.FuncDef.ByteCode;
        ByteCodeRow row = null;
        List<RuntimeValue> args = [];
        List<RuntimeValue> list1 = null;
        RuntimeValue value1 = null;
        RuntimeValue value2 = null;
        RuntimeValue outputValue = null;
        DictImpl dict1 = null;
        int int1 = 0;
        int i = 0;
        string str1 = "";
        string name = "";
        object left = null;
        object right = null;
        bool found = false;
        bool match = false;
        StringBuilder sb = null;

        while (frame != null)
        {
            row = byteCode[frame.PC];
            switch (row.Op)
            {
                case ByteCodeOp.ASSIGN_VARIABLE:
                    frame.LocalVariables[row.StringValue] = frame.ValueStack.Pop();
                    break;

                case ByteCodeOp.BINARY_OP:

                    value2 = frame.ValueStack.Pop();
                    value1 = frame.ValueStack.Pop();
                    left = value1.Value;
                    right = value2.Value;

                    switch (((int)value1.Type * OP_OFFSET + row.FirstArg) * OP_OFFSET + (int)value2.Type)
                    {
                        case (TYPE_INT * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left + (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_FLOAT:
                            outputValue = RuntimeValue.OfFloat((int)left + (double)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfFloat((double)left + (int)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_FLOAT:
                            outputValue = RuntimeValue.OfFloat((double)left + (double)right);
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_SUB) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left - (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_SUB) * OP_OFFSET + TYPE_FLOAT:
                            outputValue = RuntimeValue.OfFloat((int)left - (double)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_SUB) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfFloat((double)left - (int)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_SUB) * OP_OFFSET + TYPE_FLOAT:
                            outputValue = RuntimeValue.OfFloat((double)left - (double)right);
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_MULTIPLY) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left * (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_MULTIPLY) * OP_OFFSET + TYPE_FLOAT:
                            outputValue = RuntimeValue.OfFloat((int)left * (double)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_MULTIPLY) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfFloat((double)left * (int)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_MULTIPLY) * OP_OFFSET + TYPE_FLOAT:
                            outputValue = RuntimeValue.OfFloat((double)left * (double)right);
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_DIVIDE) * OP_OFFSET + TYPE_INT:
                            if ((int)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfInteger((int)left / (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_DIVIDE) * OP_OFFSET + TYPE_FLOAT:
                            if ((double)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfFloat((int)left / (double)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_DIVIDE) * OP_OFFSET + TYPE_INT:
                            if ((int)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfFloat((double)left / (int)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_DIVIDE) * OP_OFFSET + TYPE_FLOAT:
                            if ((double)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfFloat((double)left / (double)right);
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_MOD) * OP_OFFSET + TYPE_INT:
                            if ((int)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfInteger((int)left % (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_MOD) * OP_OFFSET + TYPE_FLOAT:
                            if ((double)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfFloat((int)left % (double)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_MOD) * OP_OFFSET + TYPE_INT:
                            if ((int)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfFloat((double)left % (int)right);
                            break;
                        case (TYPE_FLOAT * OP_OFFSET + OP_MOD) * OP_OFFSET + TYPE_FLOAT:
                            if ((double)right == 0) throw new RuntimeException(frame, "Division by zero");
                            outputValue = RuntimeValue.OfFloat((double)left % (double)right);
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_LT) * OP_OFFSET + TYPE_INT:
                            outputValue = (int)left < (int)right ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_GT) * OP_OFFSET + TYPE_INT:
                            outputValue = (int)left > (int)right ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_LTEQ) * OP_OFFSET + TYPE_INT:
                            outputValue = (int)left <= (int)right ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_GTEQ) * OP_OFFSET + TYPE_INT:
                            outputValue = (int)left >= (int)right ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_FLOAT * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_BOOLEAN * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_LIST * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_DICTIONARY * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_INSTANCE * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_NATIVE_HANDLE * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_INT:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_FLOAT:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_BOOLEAN:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_LIST:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_DICTIONARY:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_INSTANCE:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_NATIVE_HANDLE:
                        case (TYPE_STRING * OP_OFFSET + OP_ADD) * OP_OFFSET + TYPE_STRING:
                            outputValue = RuntimeValue.OfString(RuntimeValue.ToSimpleString(value1) +
                                                                RuntimeValue.ToSimpleString(value2));
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_EQ) * OP_OFFSET + TYPE_FLOAT:
                        case (TYPE_FLOAT * OP_OFFSET + OP_EQ) * OP_OFFSET + TYPE_INT:
                        case (TYPE_INT * OP_OFFSET + OP_NOT_EQ) * OP_OFFSET + TYPE_FLOAT:
                        case (TYPE_FLOAT * OP_OFFSET + OP_NOT_EQ) * OP_OFFSET + TYPE_INT:
                            match =
                                (value1.Type == RuntimeType.INTEGER ? (int)left : (double)left) -
                                (value2.Type == RuntimeType.INTEGER ? (int)right : (double)right) == 0;

                            outputValue = (row.FirstArg == OP_EQ) == match
                                ? RuntimeValue.TRUE
                                : RuntimeValue.FALSE;
                            break;

                        case (TYPE_NULL * OP_OFFSET + OP_EQ) * OP_OFFSET + TYPE_NULL:
                            outputValue = RuntimeValue.TRUE;
                            break;
                        case (TYPE_NULL * OP_OFFSET + OP_NOT_EQ) * OP_OFFSET + TYPE_NULL:
                            outputValue = RuntimeValue.FALSE;
                            break;

                        case (TYPE_BOOLEAN * OP_OFFSET + OP_EQ) * OP_OFFSET + TYPE_BOOLEAN:
                        case (TYPE_BOOLEAN * OP_OFFSET + OP_NOT_EQ) * OP_OFFSET + TYPE_BOOLEAN:
                            match = (bool)left == (bool)right;
                            outputValue = match == (row.FirstArg == OP_EQ) ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_EQ) * OP_OFFSET + TYPE_INT:
                        case (TYPE_INT * OP_OFFSET + OP_NOT_EQ) * OP_OFFSET + TYPE_INT:
                            match = (int)left == (int)right;
                            outputValue = match == (row.FirstArg == OP_EQ) ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;

                        case (TYPE_FLOAT * OP_OFFSET + OP_EQ) * OP_OFFSET + TYPE_FLOAT:
                        case (TYPE_FLOAT * OP_OFFSET + OP_NOT_EQ) * OP_OFFSET + TYPE_FLOAT:
                            match = (double)left - (double)right == 0;
                            outputValue = match == (row.FirstArg == OP_EQ) ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;

                        case (TYPE_STRING * OP_OFFSET + OP_EQ) * OP_OFFSET + TYPE_STRING:
                        case (TYPE_STRING * OP_OFFSET + OP_NOT_EQ) * OP_OFFSET + TYPE_STRING:
                            match = (string)left == (string)right;
                            outputValue = match == (row.FirstArg == OP_EQ) ? RuntimeValue.TRUE : RuntimeValue.FALSE;
                            break;

                        case (TYPE_INT * OP_OFFSET + OP_BITWISE_AND) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left & (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_BITWISE_OR) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left | (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_BITWISE_XOR) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left ^ (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_BITSHIFT_LEFT) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left << (int)right);
                            break;
                        case (TYPE_INT * OP_OFFSET + OP_BITSHIFT_RIGHT) * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfInteger((int)left >> (int)right);
                            break;

                        case (TYPE_STRING * OP_OFFSET + OP_MULTIPLY) * OP_OFFSET + TYPE_INT:
                        case (TYPE_INT * OP_OFFSET + OP_MULTIPLY) * OP_OFFSET + TYPE_STRING:
                            if (value1.Type == RuntimeType.INTEGER)
                            {
                                int1 = (int)left;
                                str1 = (string)right;
                            }
                            else
                            {
                                int1 = (int)right;
                                str1 = (string)left;
                            }

                            sb = new StringBuilder();
                            while (int1-- > 0)
                            {
                                sb.Append(str1);
                            }

                            outputValue = RuntimeValue.OfString(sb.ToString());
                            sb = null;
                            break;

                        default:
                            if (row.FirstArg == OP_EQ)
                            {
                                outputValue = RuntimeValue.FALSE;
                            }
                            else if (row.FirstArg == OP_NOT_EQ)
                            {
                                outputValue = RuntimeValue.TRUE;
                            }
                            else
                            {
                                throw new NotImplementedException(str1);
                            }

                            break;
                    }

                    frame.ValueStack.Push(outputValue);
                    break;

                case ByteCodeOp.DEFINE_DICTIONARY:
                    args.Clear();
                    int1 = row.FirstArg * 2;
                    while (int1-- > 0)
                    {
                        args.Add(frame.ValueStack.Pop());
                    }

                    args.Reverse();
                    int1 = row.FirstArg * 2;
                    dict1 = new DictImpl();
                    for (i = 0; i < int1; i += 2)
                    {
                        dict1.Add(frame, args[i], args[i + 1]);
                    }

                    frame.ValueStack.Push(RuntimeValue.OfDictionary(dict1));

                    break;

                case ByteCodeOp.DEFINE_LIST:
                    args.Clear();
                    int1 = row.FirstArg;
                    while (int1-- > 0)
                    {
                        args.Add(frame.ValueStack.Pop());
                    }

                    args.Reverse();
                    frame.ValueStack.Push(RuntimeValue.OfArray([..args]));
                    args.Clear();
                    break;

                case ByteCodeOp.DOT_FIELD:
                    value1 = frame.ValueStack.Pop();
                    outputValue = RuntimeFunctionRef.TryGetBuiltInMethod(value1, row.StringValue);
                    if (outputValue == null)
                    {
                        switch (value1.Type)
                        {
                            case RuntimeType.LIST:
                                if (row.StringValue == "length")
                                {
                                    outputValue = RuntimeValue.OfInteger(((List<RuntimeValue>)value1.Value).Count);
                                }

                                break;

                            case RuntimeType.STRING:
                                if (row.StringValue == "length")
                                {
                                    outputValue = RuntimeValue.OfInteger(((string)value1.Value).Length);
                                }

                                break;
                        }
                    }

                    if (outputValue == null)
                    {
                        throw new RuntimeException(frame,
                            "There is no property or method named ." + row.StringValue + " on this type.");
                    }

                    frame.ValueStack.Push(outputValue ?? RuntimeValue.NULL);
                    break;

                case ByteCodeOp.INDEX:
                    value2 = frame.ValueStack.Pop();
                    value1 = frame.ValueStack.Pop();
                    outputValue = null;
                    switch (((int)value1.Type * OP_OFFSET + (int)value2.Type))
                    {
                        case TYPE_LIST * OP_OFFSET + TYPE_INT:
                            outputValue = ((List<RuntimeValue>)value1.Value)[(int)value2.Value];
                            break;

                        case TYPE_STRING * OP_OFFSET + TYPE_INT:
                            outputValue = RuntimeValue.OfString(((string)value1.Value)[(int)value2.Value] + "");
                            break;

                        case TYPE_DICTIONARY * OP_OFFSET + TYPE_STRING:
                            dict1 = (DictImpl)value1.Value;
                            str1 = (string)value2.Value;
                            if (!dict1.IsString || !dict1.strLookup.ContainsKey(str1))
                            {
                                throw new RuntimeException(frame, "Key not found in dictionary.");
                            }

                            outputValue = dict1.Values[dict1.strLookup[str1]];
                            break;

                        case TYPE_DICTIONARY * OP_OFFSET + TYPE_INT:
                            dict1 = (DictImpl)value1.Value;
                            int1 = (int)value2.Value;
                            if (dict1.IsString || !dict1.intLookup.ContainsKey(int1))
                            {
                                throw new RuntimeException(frame, "Key not found in dictionary.");
                            }

                            outputValue = dict1.Values[dict1.intLookup[int1]];
                            break;

                        default:
                            throw new RuntimeException(frame, "Cannot index into this type with this value.");
                    }

                    frame.ValueStack.Push(outputValue);
                    break;

                case ByteCodeOp.INLINE_INCR_VAR:
                    if (!frame.LocalVariables.ContainsKey(row.StringValue))
                    {
                        throw new RuntimeException(frame, "Variable '" + row.StringValue + "' is not defined.");
                    }

                    value1 = frame.LocalVariables[row.StringValue];
                    if (value1.Type != RuntimeType.INTEGER)
                    {
                        throw new RuntimeException(frame, "Inline increment operators can only be used on integers.");
                    }

                    value2 = RuntimeValue.OfInteger((int)value1.Value + row.Args[1]);
                    frame.ValueStack.Push(row.FirstArg == 1 ? value2 : value1);
                    frame.LocalVariables[row.StringValue] = value2;

                    break;

                case ByteCodeOp.INVOKE_FUNCTION:
                    int1 = row.FirstArg;
                    args.Clear();
                    while (int1-- > 0)
                    {
                        args.Add(frame.ValueStack.Pop());
                    }

                    value1 = frame.ValueStack.Pop();
                    if (value1.Value is RuntimeFunctionRef funcRef)
                    {
                        args.Reverse();

                        if (funcRef.IsBuiltIn)
                        {
                            if (args.Count < funcRef.ArgcMin || args.Count > funcRef.ArgcMax)
                            {
                                throw new RuntimeException(
                                    frame,
                                    "Invalid number of arguments provided. " +
                                    (funcRef.ArgcMin == funcRef.ArgcMax
                                        ? "Expected " + funcRef.ArgcMin
                                        : ("Between " + funcRef.ArgcMin + " and " + funcRef.ArgcMax)) +
                                    " but found " + args.Count + " instead.");
                            }

                            RuntimeValue output = null;
                            switch (funcRef.MethodContext == null ? RuntimeType.NULL : funcRef.MethodContext.Type)
                            {
                                case RuntimeType.NULL:

                                    switch (funcRef.Name)
                                    {
                                        case "floor":
                                            value1 = args[0];
                                            if (value1.Type == RuntimeType.FLOAT)
                                                output = RuntimeValue.OfInteger((int)Math.Floor((double)value1.Value));
                                            else if (value1.Type == RuntimeType.INTEGER) output = value1;
                                            else
                                                throw new RuntimeException(frame,
                                                    "Expected a numeric input to floor()");

                                            break;

                                        case "print":
                                            sb = new StringBuilder();
                                            for (i = 0; i < args.Count; i++)
                                            {
                                                if (i > 0) sb.Append(' ');
                                                sb.Append(RuntimeValue.ToSimpleString(args[i]));
                                            }

                                            str1 = sb.ToString();
                                            sb = null;
                                            Console.WriteLine(str1);
                                            break;

                                        default:
                                            throw new NotImplementedException(funcRef.Name);
                                    }

                                    break;

                                case RuntimeType.STRING:
                                    switch (funcRef.Name)
                                    {
                                        case "trim":
                                            output = RuntimeValue.OfString(((string)funcRef.MethodContext.Value)
                                                .Trim());
                                            break;
                                        default:
                                            throw new NotImplementedException("string." + funcRef.Name);
                                    }

                                    break;

                                case RuntimeType.LIST:
                                    switch (funcRef.Name)
                                    {
                                        case "add":
                                            list1 = (List<RuntimeValue>)funcRef.MethodContext.Value;
                                            list1.Add(args[0]);
                                            outputValue = RuntimeValue.NULL;
                                            break;

                                        default:
                                            throw new NotImplementedException("list." + funcRef.Name);
                                    }

                                    break;

                                case RuntimeType.DICTIONARY:

                                    switch (funcRef.Name)
                                    {
                                        case "get":
                                            dict1 = (DictImpl)funcRef.MethodContext.Value;
                                            output = dict1.TryGet(args[0]) ??
                                                     (args.Count == 2 ? args[1] : RuntimeValue.NULL);
                                            break;

                                        default:
                                            throw new NotImplementedException("list." + funcRef.Name);
                                    }

                                    break;
                            }

                            if (output == null)
                            {
                                output = RuntimeValue.OfNull();
                            }

                            frame.ValueStack.Push(output);
                        }
                        else
                        {
                            StackFrame newFrame = new StackFrame(funcRef.UserFunction, [..args]);
                            args.Clear();
                            newFrame.PreviousFrame = frame;
                            frame = newFrame;
                            rtCtx.CallStackHead = frame;
                            frame.PC = -1;
                            byteCode = frame.FuncDef.ByteCode;

                        }
                    }
                    else
                    {
                        throw new RuntimeException(frame,
                            "Attempted to invoke a value that was not a function as a function.");
                    }

                    break;

                case ByteCodeOp.JUMP:
                    frame.PC += row.FirstArg;
                    break;

                case ByteCodeOp.NEGATIVE_SIGN:
                    value1 = frame.ValueStack.Pop();
                    if (value1.Type == RuntimeType.INTEGER)
                    {
                        value1 = RuntimeValue.OfInteger(-(int)value1.Value);
                    }
                    else if (value1.Type == RuntimeType.FLOAT)
                    {
                        value1 = RuntimeValue.OfFloat(-(double)value1.Value);
                    }
                    else
                    {
                        throw new RuntimeException(frame, "Cannot use negative sign on this type of value.");
                    }

                    frame.ValueStack.Push(value1);

                    break;


                case ByteCodeOp.POP:
                    frame.ValueStack.Pop();
                    break;

                case ByteCodeOp.POP_AND_JUMP_IF_FALSE:
                    value1 = frame.ValueStack.Pop();
                    if (value1.Type == RuntimeType.BOOLEAN && !(bool)value1.Value)
                    {
                        frame.PC += row.FirstArg;
                    }

                    break;

                case ByteCodeOp.PUSH_ARG:
                    frame.ValueStack.Push(frame.FunctionArgs[row.FirstArg]);
                    break;

                case ByteCodeOp.PUSH_FLOAT:
                    row.CacheValue = RuntimeValue.OfFloat(double.Parse(row.StringValue));
                    row.Op = ByteCodeOp.PUSH_VALUE;
                    frame.PC--;
                    break;

                case ByteCodeOp.PUSH_INT:
                    row.CacheValue = RuntimeValue.OfInteger(row.FirstArg);
                    row.Op = ByteCodeOp.PUSH_VALUE;
                    frame.PC--;
                    break;

                case ByteCodeOp.PUSH_NULL:
                    row.CacheValue = RuntimeValue.OfNull();
                    row.Op = ByteCodeOp.PUSH_VALUE;
                    frame.PC--;
                    break;

                case ByteCodeOp.PUSH_STRING:
                    row.CacheValue = RuntimeValue.OfString(row.StringValue);
                    row.Op = ByteCodeOp.PUSH_VALUE;
                    frame.PC--;
                    break;

                case ByteCodeOp.PUSH_VALUE:
                    frame.ValueStack.Push(row.CacheValue);
                    break;

                case ByteCodeOp.PUSH_VARIABLE:
                    name = row.StringValue;
                    if (frame.LocalVariables.ContainsKey(name))
                    {
                        frame.ValueStack.Push(frame.LocalVariables[name]);
                    }
                    else
                    {
                        if (rtCtx.FunctionDefinitions.ContainsKey(name))
                        {
                            row.CacheValue = RuntimeValue.OfFunction(rtCtx.FunctionDefinitions[name], name);
                        }
                        else if (RuntimeFunctionRef.IsBuiltInFunctionName(name))
                        {
                            row.CacheValue = RuntimeValue.OfFunction(null, name);
                        }
                        else
                        {
                            throw new RuntimeException(frame, "Use of undeclared variable: " + name);
                        }

                        row.Op = ByteCodeOp.PUSH_VALUE;
                        frame.PC--;
                    }

                    break;

                case ByteCodeOp.RETURN:
                    value1 = frame.ValueStack.Pop();
                    frame = frame.PreviousFrame;
                    if (frame == null) return;
                    frame.ValueStack.Push(value1);
                    rtCtx.CallStackHead = frame;
                    byteCode = frame.FuncDef.ByteCode;
                    break;

                case ByteCodeOp.THROW:
                    str1 = RuntimeValue.ToSimpleString(frame.ValueStack.Pop());
                    throw new RuntimeException(frame, str1);
                
                default:
                    throw new NotImplementedException("OP: " + row.Op);
            }

            frame.PC++;
        }
    }
}
