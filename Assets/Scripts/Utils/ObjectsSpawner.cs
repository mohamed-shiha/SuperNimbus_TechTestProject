using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsSpawner<T> where T : MonoBehaviour
{
    static int counter = 0;
    Stack<T> pool;
    Func<int, T> MakeObject;
    public Action<T> onTakeBack;

    public ObjectsSpawner(Func<int, T> makeObject)
    {
        pool = new Stack<T>();
        MakeObject = makeObject;
    }

    public T GiveMeOne()
    {
        T result;
        if (pool.Count > 0)
        {
            result = pool.Pop();
        }
        else
        {
            return MakeObject(counter++);
        }

        return result;
    }

    public void TakeBack(T other)
    {
        if (!pool.Contains(other))
        {
            pool.Push(other);
            onTakeBack?.Invoke(other);
        }
    }
}