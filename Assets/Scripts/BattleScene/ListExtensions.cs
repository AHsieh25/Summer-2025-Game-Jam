using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static List<T> Shuffle<T>(this List<T> list)
    {
        List<T> copy = new List<T>(list);
        for (int i = 0; i < copy.Count; i++)
        {
            int rand = Random.Range(i, copy.Count);
            (copy[i], copy[rand]) = (copy[rand], copy[i]);
        }
        return copy;
    }
}