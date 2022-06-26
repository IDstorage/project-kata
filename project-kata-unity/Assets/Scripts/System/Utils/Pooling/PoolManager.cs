using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly.Utils
{
    public class PoolManager
    {
        private PoolManager() { }
        private static PoolManager instance = null;
        public static PoolManager Instance => (instance ?? (instance = new PoolManager()));


        Dictionary<string, List<PoolObject>> pool = new Dictionary<string, List<PoolObject>>();
        Dictionary<string, PoolObject> setting = new Dictionary<string, PoolObject>();

        public void Init(params PoolObject[] list)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                if (setting.ContainsKey(list[i].uniqueName) || list[i].uniqueName.Length == 0) continue;
                setting.Add(list[i].uniqueName, list[i]);
            }
        }

        public void Preparing(string name, int count)
        {
            Debug.Assert(setting.ContainsKey(name));
            if (!pool.ContainsKey(name)) pool.Add(name, new List<PoolObject>());
            for (int i = 0; i < count; ++i)
            {
                pool[name].Add(Object.Instantiate(setting[name]));
                pool[name][pool[name].Count - 1].Init();
            }
        }

        public PoolObject Get(string name)
        {
            if (!pool.ContainsKey(name)) Preparing(name, 1);
            if (pool[name].Count == 0) Preparing(name, 1);

            var obj = pool[name][0].Use();
            pool[name].RemoveAt(0);
            return obj;
        }

        public T Get<T>(string name)
        {
            return Get(name).GetComponent<T>();
        }

        public void Abandon(PoolObject obj)
        {
            obj.Abandon();
            if (!pool.ContainsKey(obj.uniqueName)) Object.Destroy(obj);
            pool[obj.uniqueName].Add(obj);
        }

    }
}
