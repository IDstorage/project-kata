using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

[System.Serializable]
public class PhysicsComponent : CustomComponent, IUpdater
{
    [SerializeField]
    private Transform body;

    [SerializeField]
    private float forceAttenScale = 5F;

    private float defaultForceAttenScale = -1F;

    private (Vector3 force, Vector3 impulse) extraForce = (Vector3.zero, Vector3.zero);

    public void AddImpulse(Vector3 direction, float power)
    {
        extraForce.impulse += direction.normalized * power;
    }

    public void AddForce(Vector3 direction, float power)
    {
        extraForce.force += direction.normalized * power;
    }

    public void RemoveForce(Vector3 direction, float power)
    {
        extraForce.force -= direction.normalized * power;
    }

    public void RemoveAllForce(Vector3 direction, float power)
    {
        extraForce.force = Vector3.zero;
    }

    public void RemoveAllImpulse(Vector3 direction, float power)
    {
        extraForce.impulse = Vector3.zero;
    }


    public void SetForceAttenScale(float scale)
    {
        if (defaultForceAttenScale < 0F) defaultForceAttenScale = forceAttenScale;

        forceAttenScale = scale <= 0F ? defaultForceAttenScale : scale;
    }


    public void Update()
    {
        extraForce.impulse = Vector3.Lerp(extraForce.impulse, Vector3.zero, Time.deltaTime * forceAttenScale);
        if (extraForce.impulse.sqrMagnitude <= 0.0025f) extraForce.impulse = Vector3.zero;

        body.position += (extraForce.force + extraForce.impulse) * Time.deltaTime;
    }
}
