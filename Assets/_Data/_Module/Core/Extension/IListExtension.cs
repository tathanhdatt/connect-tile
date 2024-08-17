using System;
using System.Collections;
using System.Collections.Generic;

public static class IListExtension
{
    public static T First<T>(this IList<T> list)
    {
        return list[0];
    }

    public static T Last<T>(this IList<T> list)
    {
        return list[list.Count - 1];
    }
    
    public static void RemoveFirst(this IList list)
    {
        list.RemoveAt(0);
    }

    public static void RemoveLast(this IList list)
    {
        list.RemoveAt(list.Count - 1);        
    }
    
    public static void Shuffle(this IList list)
    {
        var numberItem = list.Count;
        var random = new Random();
        for (var i = 0; i < numberItem; i++)
        {
            var randomIndex = random.Next(0, numberItem);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public static IList Concat(this IList list, IList other)
    {
        foreach (var item in other)
        {
            list.Add(item);
        }

        return list;
    }

    public static bool IsEmpty(this IList list)
    {
        return list.Count <= 0;
    }
}
