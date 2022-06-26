using System.Collections.Generic;
using UnityEngine;

namespace Anomaly
{
    public class PopupController : UIController<Popup>
    {
        private static Stack<Popup> popupStack = new Stack<Popup>();

        public static async void Show(string popupName, UIEventParam param)
        {
            Debug.Assert(layoutDictionary.ContainsKey(popupName));

            popupStack.Push(layoutDictionary[popupName]);

            Current = popupStack.Peek();
            Current.gameObject.SetActive(true);
            await Current.OnEnter(param);
        }

        public static async void Hide()
        {
            if (popupStack.Count == 0) return;

            await popupStack.Peek().OnExit();

            popupStack.Peek().gameObject.SetActive(false);
            popupStack.Pop();

            Current = popupStack.Count == 0 ? null : popupStack.Peek();
        }
    }
}