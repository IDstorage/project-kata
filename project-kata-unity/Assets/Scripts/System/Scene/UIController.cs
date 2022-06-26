using System.Collections.Generic;
using UnityEngine;

namespace Anomaly
{
    public class UIController<T> : MonoBehaviour where T : UILayout
    {
        protected static Dictionary<string, T> layoutDictionary = new Dictionary<string, T>();
        public static T Current { get; protected set; }

        protected virtual void Awake()
        {
            if (layoutDictionary.Count != 0) return;
            var layouts = GetComponentsInChildren<T>(true);
            for (int i = 0; i < layouts.Length; ++i)
            {
                layoutDictionary.Add(layouts[i].name, layouts[i]);
            }
        }
    }
}