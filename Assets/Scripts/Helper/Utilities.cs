using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static List<GameObject> GetChildOnlyObjects(this GameObject[] array)
    {
        var list = new List<GameObject>();
        list.AddRange(array);
        list.RemoveAt(0);
        return list;
    }
}