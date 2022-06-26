using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly
{
    public class Managers : MonoBehaviour
    {
        private static bool isInitialized = false;

        public static EventManager Event { get; private set; }
        public static UpdateManager Update { get; private set; }


        private void Awake()
        {
            if (isInitialized)
            {
                Destroy(gameObject);
                return;
            }

            isInitialized = true;
            DontDestroyOnLoad(gameObject);

            Event = gameObject.AddComponent<EventManager>();
            Update = gameObject.AddComponent<UpdateManager>();
        }
    }
}