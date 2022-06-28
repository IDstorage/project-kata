using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class ThirdPersonComponent : CustomComponent
{
    [System.Serializable]
    [SharedComponentData(typeof(ThirdPersonComponent))]
    public class Data : CustomComponent.BaseData
    {
        public Transform camera;
        public Transform cameraHandle;

        [Space(10)]
        public Transform lookAtTarget;

        [Space(10)]
        public Vector3 distance;
        [Space(5)]
        public float radius;
        public Vector2 radiusRange;

        [Space(10)]
        public Vector2 verticalRange;
    }

    public void InitializeCamera(Data target)
    {
        target.camera.position = target.cameraHandle.position + target.distance.normalized * target.radius;
    }

    public void HandleMouseInput(Data target, float h, float v)
    {
        var spherical = CoordinationSystem.CartesianToSpherical(target.camera.localPosition);

        spherical.x = Mathf.Clamp(target.radius, target.radiusRange.x, target.radiusRange.y);
        spherical.y = (spherical.y * Mathf.Rad2Deg + h) * Mathf.Deg2Rad;
        spherical.z = Mathf.Clamp(spherical.z * Mathf.Rad2Deg - v, target.verticalRange.x, target.verticalRange.y) * Mathf.Deg2Rad;

        target.camera.localPosition = CoordinationSystem.SphericalToCartesian(spherical);
    }

    public void HandleCameraLook(Data target)
    {
        if (target.lookAtTarget == null) return;
        target.camera.LookAt(target.lookAtTarget, Vector3.up);
    }

    public void CalculateCameraDistance(Data target)
    {
        //if (!Physics.SphereCast(target.camera.position, 0.5f, (target.cameraHandle.position - target.camera.position).normalized, out var hit, target.radius)) return;
        Debug.DrawRay(target.camera.position, (target.cameraHandle.position - target.camera.position).normalized * target.radius, Color.red);
        Debug.Log(LayerMask.NameToLayer("FlexibleCameraHit"));
        if (!Physics.Raycast(target.camera.position, (target.cameraHandle.position - target.camera.position).normalized, out var hit, target.verticalRange.y, 0)) return;//~LayerMask.NameToLayer("FlexibleCameraHit"))) return;

        Debug.Log(hit.collider.name);

        var spherical = CoordinationSystem.CartesianToSpherical(target.camera.localPosition);

        spherical.x = Mathf.Clamp(hit.distance, target.radiusRange.x, target.radiusRange.y);

        target.camera.localPosition = CoordinationSystem.SphericalToCartesian(spherical);
    }


    public Quaternion GetForwardQuaternion(Data target)
    {
        return Quaternion.AngleAxis(
            CoordinationSystem.CartesianToSpherical(target.camera.position).y * Mathf.Rad2Deg,
            Vector3.up);
    }

    public Vector3 GetForwardVector(Data target)
    {
        var pos = target.cameraHandle.position - target.camera.position;
        pos.y = 0F;
        return pos.normalized;
    }
}
