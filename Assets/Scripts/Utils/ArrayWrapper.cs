using System;

[Serializable]
public class ArrayWrapper<T>
{
    public T[] Data;

    public ArrayWrapper(T[] data)
    {
        Data = data;
    }
}
