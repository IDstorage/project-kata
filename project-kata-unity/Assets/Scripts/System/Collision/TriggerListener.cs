using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Anomaly
{
    [RequireComponent(typeof(Collider))]
    public class TriggerListener : CustomBehaviour
    {
        public UnityEvent<Collider> onTriggerEnter, onTriggerStay, onTriggerEnd;
        public Dictionary<string, HashSet<Collider>> colliderList = new Dictionary<string, HashSet<Collider>>();

        private void OnTriggerEnter(Collider other)
        {
            if (!colliderList.ContainsKey(other.tag))
            {
                colliderList.Add(other.tag, new HashSet<Collider>());
            }
            colliderList[other.tag].Add(other);
            onTriggerEnter?.Invoke(other);
        }
        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }
        private void OnTriggerExit(Collider other)
        {
            colliderList[other.tag].Remove(other);
            onTriggerEnd?.Invoke(other);
        }
    }
}