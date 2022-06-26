using System.Collections.Generic;

namespace Anomaly 
{
    public class ComponentPool
    {
        private static Dictionary<System.Type, CustomComponent> eventSet = new Dictionary<System.Type, CustomComponent>();

        public static T Get<T>() where T : CustomComponent, new()
        {
            if (!eventSet.ContainsKey(typeof(T)))
            {
                eventSet.Add(typeof(T), new T());
            }
            return eventSet[typeof(T)] as T;
        }
    }
}