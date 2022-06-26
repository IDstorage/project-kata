using UnityEngine;

namespace Anomaly
{
    public class SceneController : UIController<Scene>
    {
        public static async void Change(string name, UIEventParam param)
        {
            Debug.Assert(layoutDictionary.ContainsKey(name));

            await Current.OnExit();
            Current.gameObject.SetActive(false);

            Current = layoutDictionary[name];
            Current.gameObject.SetActive(true);
            await Current.OnEnter(param);
        }
    }
}