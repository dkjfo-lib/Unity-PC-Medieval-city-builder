using System.Globalization;

[System.Serializable]
public class BaseIndexer<T>
{
    public T[] arr;

    public BaseIndexer(int length)
    {
        arr = new T[length];
    }

    protected T this[int i]
    {
        get { return arr[i]; }
        set { arr[i] = value; }
    }
}

[System.Serializable]
public class BaseEnumMap<TE, TV> : BaseIndexer<TV>
    where TE : struct, System.IConvertible
{
    static System.IFormatProvider format = new CultureInfo("en-US");

    public BaseEnumMap() : base(System.Enum.GetValues(typeof(TE)).Length) { }
    public TV Get(TE enumValue) => this[GetEnumId(enumValue)];
    public void Set(TE enumValue, TV value) => this[GetEnumId(enumValue)] = value;
    protected int GetEnumId(TE enumValue) => enumValue.ToInt32(format);
}