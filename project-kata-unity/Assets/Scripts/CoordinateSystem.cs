using UnityEngine;

public static class CoordinationSystem
{
    public static Vector3 CartesianToSpherical(Vector3 coord)
    {
        float r = Mathf.Sqrt(coord.x * coord.x + coord.y * coord.y + coord.z * coord.z);
        return new Vector3(
            r,
            Mathf.Atan2(coord.x, coord.z),
            Mathf.Acos(coord.y / r)
        );
    }

    public static Vector3 SphericalToCartesian(Vector3 coord)
    {
        return new Vector3(
            coord.x * Mathf.Sin(coord.z) * Mathf.Sin(coord.y),
            coord.x * Mathf.Cos(coord.z),
            coord.x * Mathf.Sin(coord.z) * Mathf.Cos(coord.y)
        );
    }
}
