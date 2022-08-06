using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

[System.Serializable]
public class ThirdPersonComponent : CustomComponent
{
    [SerializeField]
    private Transform camera;
    [SerializeField]
    private Transform cameraHandle;

    [Space(10), SerializeField]
    private Transform lookAtTarget;

    [Space(10), SerializeField]
    private Vector3 distance;
    [Space(5), SerializeField]
    private float radius;
    [SerializeField]
    private Vector2 radiusRange;

    [Space(10), SerializeField]
    private Vector2 verticalRange;


    public void InitializeCamera()
    {
        camera.position = cameraHandle.position + distance.normalized * radius;
    }

    public void HandleMouseInput(float h, float v)
    {
        var spherical = CoordinationSystem.CartesianToSpherical(camera.localPosition);

        spherical.x = Mathf.Clamp(radius, radiusRange.x, radiusRange.y);
        spherical.y = (spherical.y * Mathf.Rad2Deg + h) * Mathf.Deg2Rad;
        spherical.z = Mathf.Clamp(spherical.z * Mathf.Rad2Deg - v, verticalRange.x, verticalRange.y) * Mathf.Deg2Rad;

        camera.localPosition = CoordinationSystem.SphericalToCartesian(spherical);
    }

    public void HandleCameraLook()
    {
        if (lookAtTarget == null) return;
        camera.LookAt(lookAtTarget, Vector3.up);
    }

    public void CalculateCameraDistance()
    {
        var result = Physics.SphereCast(cameraHandle.position, 0.5f, (camera.position - cameraHandle.position).normalized, out var hit, radius, 1 << LayerMask.NameToLayer("FlexibleCameraHit"));

        if (!result) return;

        var spherical = CoordinationSystem.CartesianToSpherical(camera.localPosition);

        spherical.x = Mathf.Clamp(hit.distance, radiusRange.x, radiusRange.y);

        camera.localPosition = CoordinationSystem.SphericalToCartesian(spherical);
    }


    public Quaternion GetForwardQuaternion()
    {
        return Quaternion.AngleAxis(
            CoordinationSystem.CartesianToSpherical(camera.position).y * Mathf.Rad2Deg,
            Vector3.up);
    }

    public Vector3 GetForwardVector()
    {
        var pos = cameraHandle.position - camera.position;
        pos.y = 0F;
        return pos.normalized;
    }
}
