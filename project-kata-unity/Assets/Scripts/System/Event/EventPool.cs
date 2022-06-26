using System.Collections.Generic;

namespace Anomaly 
{
    public static class EventPool
    {
        private static Dictionary<string, BaseEvent> eventSet = new Dictionary<string, BaseEvent>();

        public static T Get<T>() where T : BaseEvent, new()
        {
            string name = typeof(T).Name;
            if (!eventSet.ContainsKey(name))
            {
                eventSet.Add(name, new T());
            }
            return eventSet[name] as T;
        }
    }
}