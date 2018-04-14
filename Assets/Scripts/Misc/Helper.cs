using UnityEngine;

public static class Helper
{
    public static bool IsOdd(this int num)
    {
        return num % 2 != 0;
    }
    
    public static Transform FindChildRecursively(this Transform parent, string name)
    {
        int count = parent.childCount;
        for (int i=0;i< count;i++)
        {
            var child = parent.GetChild(i);
            if (child.name == name)
                return child;
            else
            {
                var result = child.FindChildRecursively(name);
                if (result != null)
                    return result;
            }
        }
        return null;
    }
}
