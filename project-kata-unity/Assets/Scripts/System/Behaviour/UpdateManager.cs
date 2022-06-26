using System.Collections.Generic;
using System;
using UnityEngine;

namespace Anomaly
{
    public class UpdateManager : MonoBehaviour
    {
        private List<(CustomBehaviour target, Action method)> fixedUpdateObjectList = new List<(CustomBehaviour, Action)>();
        private List<(CustomBehaviour target, Action method)> updateObjectList = new List<(CustomBehaviour, Action)>();
        private List<(CustomBehaviour target, Action method)> lateUpdateObjectList = new List<(CustomBehaviour, Action)>();

        public void RegisterFixedUpdate(CustomBehaviour target, System.Action action)
        {
            fixedUpdateObjectList.Add((target, action));
        }
        public void RegisterUpdate(CustomBehaviour target, System.Action action)
        {
            updateObjectList.Add((target, action));
        }
        public void RegisterLateUpdate(CustomBehaviour target, System.Action action)
        {
            lateUpdateObjectList.Add((target, action));
        }


        void FixedUpdate()
        {
            UpdateLoop(fixedUpdateObjectList);
        }

        void Update()
        {
            UpdateLoop(updateObjectList);
        }

        void LateUpdate()
        {
            UpdateLoop(lateUpdateObjectList);
        }

        private void UpdateLoop(List<(CustomBehaviour target, System.Action method)> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var current = list[i];
                if (current.target == null)
                {
                    list.RemoveAt(i);
                    continue;
                }
                if (!current.target.gameObject.activeInHierarchy)
                {
                    continue;
                }
                current.method.Invoke();
            }
        }
    }

}