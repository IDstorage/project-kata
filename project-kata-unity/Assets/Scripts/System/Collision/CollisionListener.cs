using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Anomaly
{
    [RequireComponent(typeof(Collider))]
    public class CollisionListener : CustomBehaviour
    {
        public UnityEvent<Collision> onCollisionEnter, onCollisionStay, onCollisionEnd;

        private void OnCollisionEnter(Collision other)
        {
            onCollisionEnter?.Invoke(other);
        }
        private void OnCollisionStay(Collision other)
        {
            onCollisionStay?.Invoke(other);
        }
        private void OnCollisionExit(Collision other)
        {
            onCollisionEnd?.Invoke(other);
        }
    }
}