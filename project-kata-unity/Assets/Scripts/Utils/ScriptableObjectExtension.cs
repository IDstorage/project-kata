using UnityEngine;

public static class ScriptableObjectExtension
{
    public static T Clone<T>(this T so) where T : ScriptableObject
    {
        if (so == null)
        {
            return ScriptableObject.CreateInstance<T>();
        }

        T instance = Object.Instantiate(so);
        instance.name = so.name;
        return instance;
    }
}

