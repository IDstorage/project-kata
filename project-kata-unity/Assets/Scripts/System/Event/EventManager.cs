using System.Collections.Generic;
using UnityEngine;

namespace Anomaly
{
    public class EventManager : MonoBehaviour
    {
        private Queue<(BaseEvent targetEvent, EventParam param)> eventQueue = new Queue<(BaseEvent, EventParam)>();


        public void AddEvent(BaseEvent e, EventParam p)
        {
            eventQueue.Enqueue((e, p));
        }

        private void Update()
        {
            while (eventQueue.Count > 0)
            {
                var current = eventQueue.Dequeue();
                current.targetEvent.Invoke(current.param);
            }
        }
    }
}