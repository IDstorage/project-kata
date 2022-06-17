using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TargetingSystem : MonoBehaviour
{
    [SerializeField]
    private Transform body;

    public Transform target;

    public bool IsTargeting => target != null;

    private List<Transform> detectedTargets = new List<Transform>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Targeting();
        }
    }


    public void Targeting()
    {
        if (target != null)
        {
            target = null;

            return;
        }

        target = FindTarget();
    }

    public Transform FindTarget()
    {
        var forward = body.forward;
        float min = float.MaxValue;
        for (int i = 0; i < detectedTargets.Count; ++i)
        {
            var dir = detectedTargets[i].position - body.position;
            float distance = Mathf.Sin(Mathf.Acos(Vector3.Dot(forward, dir))) * dir.magnitude;
            if (distance > min) continue;
            target = detectedTargets[i];
            min = distance;
        }
        return target;
    }



    private void OnTriggerEnter(Collider other)
    {
        detectedTargets.Add(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        detectedTargets.Remove(other.transform);
    }
}
