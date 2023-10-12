using System;
using System.Collections.Generic;
using System.Linq;



public class ObjectsListSpawner<T> where T : WorldObject
{
    public int counter = 0;
    List<T> pool;
    Func<int, T> MakeObject;
    public Action<T> onTakeBack;

    public ObjectsListSpawner(Func<int, T> makeObject)
    {
        pool = new List<T>();
        MakeObject = makeObject;
    }

    public T GetByID(int id)
    {
        T result = pool.FirstOrDefault(item => item.Data.ID == id);
        if (result != null)
        {
            pool.Remove(result);
            return result;
        }
        else
        {
            counter++;
            return MakeObject(id);
        }
    }

    public void TakeBack(T other)
    {
        if (!pool.Contains(other))
        {
            pool.Add(other);
            onTakeBack?.Invoke(other);
        }
    }
}

public class ObjectsQueueSpawner<T> where T : WorldObject
{
    public int counter = 0;
    Stack<T> pool;
    Func<int, T> MakeObject;
    public Action<T> onTakeBack;

    public ObjectsQueueSpawner(Func<int, T> makeObject)
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
            counter++;
            return MakeObject(-1);
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