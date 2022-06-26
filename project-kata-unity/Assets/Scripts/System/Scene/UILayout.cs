using System.Threading.Tasks;
using UnityEngine;

namespace Anomaly
{
    public class UIEventParam
    {

    }

    public abstract class UILayout : MonoBehaviour
    {
        public abstract Task OnEnter(UIEventParam param);
        public abstract Task OnExit();
    }
}