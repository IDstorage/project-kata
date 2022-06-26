using UnityEngine;

namespace Anomaly
{
    public class OverlayController : UIController<Overlay>
    {
        public static async void Show(string name, UIEventParam param)
        {
            Debug.Assert(layoutDictionary.ContainsKey(name));
            layoutDictionary[name].gameObject.SetActive(true);
            await layoutDictionary[name].OnEnter(param);
        }

        public static async void Hide(string name)
        {
            Debug.Assert(layoutDictionary.ContainsKey(name));
            await layoutDictionary[name].OnExit();
            layoutDictionary[name].gameObject.SetActive(false);
        }
    }
}