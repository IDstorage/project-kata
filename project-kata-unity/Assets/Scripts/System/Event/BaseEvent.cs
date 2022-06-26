using UnityEngine;

namespace Anomaly 
{
    public class EventParam
    {
        public GameObject sender, receiver;

        public bool IsValid()
        {
            return !ReferenceEquals(sender, null) && !ReferenceEquals(receiver, null);
        }
    }


    public abstract class BaseEvent
    {
        public abstract void Invoke(EventParam param);
    }
}