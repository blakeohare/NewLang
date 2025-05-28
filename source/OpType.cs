namespace NewLang;

public enum OpType
{
    UNKNOWN = 0,

    ADDITION = 1,
    SUBTRACTION = 2,
    MULTIPLICATION = 3,
    DIVISION = 4,
    MODULO = 5,
    EQUAL = 6,
    NOT_EQUAL = 7,
    LESS_THAN = 8,
    GREATER_THAN = 9,
    LESS_THAN_EQ = 10,
    GREATER_THAN_EQ = 11,
    BITWISE_AND = 12,
    BITWISE_OR = 13,
    BITWISE_XOR = 14,
    BOOLEAN_AND = 15,
    BOOLEAN_OR = 16,
    EXPONENT = 17,
    BITSHIFT_LEFT = 18, 
    BITSHIFT_RIGHT = 19,
    NULL_COALESCE = 20,

    MAX_BOUND = 21,
}

public static class OpUtil
{
    private static readonly Dictionary<string, OpType> OP_TO_OP_ID = [];
    private static readonly Dictionary<string, OpType> ASSIGN_TO_OP_ID = [];

    private static void EnsureInitialized()
    {
        if (OP_TO_OP_ID.Count > 0) return;

        Dictionary<string, OpType> lookup = [];
        lookup["+"] = OpType.ADDITION;
        lookup["-"] = OpType.SUBTRACTION;
        lookup["*"] = OpType.MULTIPLICATION;
        lookup["/"] = OpType.DIVISION;
        lookup["%"] = OpType.MODULO;
        lookup["=="] = OpType.EQUAL;
        lookup["!="] = OpType.NOT_EQUAL;
        lookup["<"] = OpType.LESS_THAN;
        lookup["<="] = OpType.LESS_THAN_EQ;
        lookup[">"] = OpType.GREATER_THAN;
        lookup[">="] = OpType.GREATER_THAN_EQ;
        lookup["&"] = OpType.BITWISE_AND;
        lookup["|"] = OpType.BITWISE_OR;
        lookup["^"] = OpType.BITWISE_XOR;
        lookup["&&"] = OpType.BOOLEAN_AND;
        lookup["||"] = OpType.BOOLEAN_OR;
        lookup["**"] = OpType.EXPONENT;
        lookup["<<"] = OpType.BITSHIFT_LEFT;
        lookup[">>"] = OpType.BITSHIFT_RIGHT;
        lookup["??"] = OpType.NULL_COALESCE;

        foreach (string op in lookup.Keys)
        {
            OP_TO_OP_ID[op] = lookup[op];
            ASSIGN_TO_OP_ID["=" + op] = lookup[op];
        }

        ASSIGN_TO_OP_ID["="] = OpType.EQUAL;
    }

    public static OpType GetOpFromOpString(string str)
    {
        EnsureInitialized();
        return OP_TO_OP_ID.TryGetValue(str, out OpType t) ? t : OpType.UNKNOWN;
    }

    public static OpType GetOpFromAssignmentString(string str)
    {
        EnsureInitialized();
        return ASSIGN_TO_OP_ID.TryGetValue(str, out OpType t) ? t : OpType.UNKNOWN;
    }
}
