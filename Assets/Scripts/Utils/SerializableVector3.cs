using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }

    public static implicit operator Vector2(SerializableVector3 rValue)
    {
        return new Vector2(rValue.x, rValue.y);
    }

    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerializableVector3(Vector2 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, 0F);
    }

    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}

public static class VectorArrayExtension
{
    public static SerializableVector3[] ToSerializable(this Vector2[] array)
    {
        SerializableVector3[] result = new SerializableVector3[array.Length];
        for (int i = 0; i < result.Length; ++i) result[i] = (Vector3)array[i];
        return result;
    }

    public static SerializableVector3[] ToSerializable(this Vector3[] array)
    {
        SerializableVector3[] result = new SerializableVector3[array.Length];
        for (int i = 0; i < result.Length; ++i) result[i] = array[i];
        return result;
    }

    public static List<SerializableVector3> ToSerializable(this List<Vector2> list)
    {
        List<SerializableVector3> result = new List<SerializableVector3>(list.Count);
        for (int i = 0; i < list.Count; ++i) result.Add(list[i]);
        return result;
    }


    public static Vector2[] ToVector2(this SerializableVector3[] array)
    {
        Vector2[] result = new Vector2[array.Length];
        for (int i = 0; i < result.Length; ++i) result[i] = (Vector3)array[i];
        return result;
    }

    public static Vector2[] ToVector2(this List<SerializableVector3> list)
    {
        Vector2[] result = new Vector2[list.Count];
        for (int i = 0; i < result.Length; ++i) result[i] = list[i];
        return result;
    }

    public static Vector3[] ToVector3(this SerializableVector3[] array)
    {
        Vector3[] result = new Vector3[array.Length];
        for (int i = 0; i < result.Length; ++i) result[i] = array[i];
        return result;
    }


    public static Vector3[] ToVector3(this Vector2[] array)
    {
        Vector3[] result = new Vector3[array.Length];
        for (int i = 0; i < result.Length; ++i) result[i] = (Vector3)array[i];
        return result;
    }
}
