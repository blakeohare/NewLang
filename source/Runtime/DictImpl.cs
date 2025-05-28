using System.Runtime.CompilerServices;

namespace NewLang.Runtime;

public class DictImpl
{
    public RuntimeValue[] Values;
    public RuntimeValue[] Keys;
    public Dictionary<string, int> strLookup;
    public Dictionary<int, int> intLookup;
    public bool IsString;
    public int Size;
    public int Capacity;

    public DictImpl()
    {
        int cap = 4;
        this.Values = new RuntimeValue[cap];
        this.Keys = new RuntimeValue[cap];
        this.Size = 0;
        this.Capacity = cap;
    }

    public void Add(StackFrame throwFrame, RuntimeValue key, RuntimeValue value)
    {
        bool isString = key.Type == RuntimeType.STRING;
        bool isInt = key.Type == RuntimeType.INTEGER;
        if (!isString && !isInt) throw new RuntimeException(throwFrame, "Cannot use keys of this type in dictionaries.");
        if (this.Size == 0)
        {
            this.IsString = isString;
            if (isString) this.strLookup = new Dictionary<string, int>();
            else this.intLookup = new Dictionary<int, int>();
        }
        else if (this.IsString != isString)
        {
            throw new RuntimeException(throwFrame, "Cannot use mixed key types in a single dictionary.");
        }

        int targetIndex = -1;
        if (isString)
        {
            string sKey = (string) key.Value;
            if (this.strLookup.ContainsKey(sKey))
            {
                targetIndex = this.strLookup[sKey];
            }
            else
            {
                targetIndex = this.Size++;
                this.strLookup[sKey] = targetIndex;
            }
        }
        else
        {
            int iKey = (int) key.Value;
            if (this.intLookup.ContainsKey(iKey))
            {
                targetIndex = this.intLookup[iKey];
            }
            else
            {
                targetIndex = this.Size++;
                this.intLookup[iKey] = targetIndex;
            }
        }

        if (targetIndex == this.Capacity)
        {
            this.Keys = ExpandCapacity(this.Keys);
            this.Values = ExpandCapacity(this.Values);
            this.Capacity = this.Keys.Length;
        }

        this.Keys[targetIndex] = key;
        this.Values[targetIndex] = value;
    }

    public RuntimeValue? TryGet(RuntimeValue key)
    {
        bool isString = key.Type == RuntimeType.STRING;
        if (!isString && key.Type != RuntimeType.INTEGER) return null;
        if (isString != this.IsString) return null;
        int index;
        if (isString)
        {
            string sKey = (string)key.Value;
            if (!this.strLookup.ContainsKey(sKey)) return null;
            index = this.strLookup[sKey];
        }
        else
        {
            int iKey = (int)key.Value;
            if (!this.intLookup.ContainsKey(iKey)) return null;
            index = this.intLookup[iKey];
        }

        return this.Values[index];
    }

    private static RuntimeValue[] ExpandCapacity(RuntimeValue[] arr)
    {
        int newSize = arr.Length * 2;
        if (newSize < 4) newSize = 4;
        RuntimeValue[] newArr = new RuntimeValue[newSize];
        for (int i = 0; i < arr.Length; i++)
        {
            newArr[i] = arr[i];
        }

        return newArr;
    }
}