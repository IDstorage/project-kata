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
    private Vector3 distance;
    [Space(5), SerializeField]
    private float radius;
    [SerializeField]
    private Vector2 radiusRange;

    [Space(10), SerializeField]
    private Vector2 verticalRange;


    [Space(10), SerializeField]
    private Transform followTarget;

    [SerializeField, Range(0F, 2F)]
    private float targetingHorizontalScale = 1F;
    [SerializeField, Range(0F, 1F)]
    private float targetingVerticalScale = 0.375f;

    [SerializeField, Range(0F, 50F)]
    private float targetingRadius = 10F;


    public bool HasFollowTarget => followTarget != null;


    public void InitializeCamera()
    {
        camera.position = cameraHandle.position + distance.normalized * radius;
    }

    public void HandleMouseInput(float h, float v)
    {
        if (HasFollowTarget) return;

        var spherical = CoordinationSystem.CartesianToSpherical(camera.localPosition);

        spherical.x = Mathf.Clamp(radius, radiusRange.x, radiusRange.y);
        spherical.y = (spherical.y * Mathf.Rad2Deg + h) * Mathf.Deg2Rad;
        spherical.z = Mathf.Clamp(spherical.z * Mathf.Rad2Deg - v, verticalRange.x, verticalRange.y) * Mathf.Deg2Rad;

        camera.localPosition = CoordinationSystem.SphericalToCartesian(spherical);
    }

    public void HandleCameraLook()
    {
        if (!HasFollowTarget) return;

        var targetDir = followTarget.position - cameraHandle.position;

        var spherical = CoordinationSystem.CartesianToSpherical(camera.localPosition);

        spherical.y = Mathf.Atan2(targetDir.x, targetDir.z) + Mathf.PI * targetingHorizontalScale;
        spherical.z = Mathf.PI * targetingVerticalScale;

        camera.localPosition = Vector3.Slerp(camera.localPosition, CoordinationSystem.SphericalToCartesian(spherical), Time.deltaTime * 10F);

        camera.LookAt(followTarget, Vector3.up);
    }

    public void CalculateCameraDistance()
    {
        var result = Physics.SphereCast(cameraHandle.position, 0.5f, (camera.position - cameraHandle.position).normalized, out var hit, radius, 1 << LayerMask.NameToLayer("FlexibleCameraHit"));

        if (!result) return;

        var spherical = CoordinationSystem.CartesianToSpherical(camera.localPosition);

        spherical.x = Mathf.Clamp(hit.distance, radiusRange.x, radiusRange.y);

        camera.localPosition = CoordinationSystem.SphericalToCartesian(spherical);
    }

    public Vector3 GetForwardVector()
    {
        var pos = cameraHandle.position - camera.position;
        pos.y = 0F;
        return pos.normalized;
    }


    public void SetTarget(Transform target)
    {
        followTarget = target;
    }


    public bool Targeting(CustomBehaviour root, out Transform target)
    {
        target = null;

        if (HasFollowTarget) return true;

        var targets = Physics.OverlapSphere(root.transform.position, targetingRadius, 1 << LayerMask.NameToLayer("Targeting"));

        float min = float.MaxValue;
        for (int i = 0; i < targets.Length; ++i)
        {
            float distance = (targets[i].transform.position - root.transform.position).sqrMagnitude;
            if (distance >= min) continue;

            distance = min;
            target = targets[i].transform;
        }

        return true;
    }
}
