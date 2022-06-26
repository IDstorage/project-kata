using System.Reflection;
using UnityEngine;

namespace Anomaly
{
    public class CustomBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            InitializeMagicFunc();
        }

        public void InitializeMagicFunc()
        {
            MethodInfo GetMethod(string methodName)
            {
                var type = GetType();

                while (type != typeof(System.Object))
                {
                    MethodInfo info = GetType()
                        .GetMethod(methodName,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (info != null) return info;
                    type = type.BaseType;
                }
                return null;
            }
            bool IsValidMagicFunction(MethodInfo method)
            {
                return method != null
                    && method.GetParameters().Length == 0
                    && method.ReturnType == typeof(void);
            }

            var manager = Managers.Update;

            var method = GetMethod("OnFixedUpdate");
            if (IsValidMagicFunction(method)) manager.RegisterFixedUpdate(this, method.CreateDelegate(typeof(System.Action), this) as System.Action);

            method = GetMethod("OnUpdate");
            if (IsValidMagicFunction(method)) manager.RegisterUpdate(this, method.CreateDelegate(typeof(System.Action), this) as System.Action);
            //if (IsValidMagicFunction(method)) manager.RegisterUpdate(Delegate.CreateDelegate(typeof(System.Action), this, method) as System.Action);

            method = GetMethod("OnLateUpdate");
            if (IsValidMagicFunction(method)) manager.RegisterLateUpdate(this, method.CreateDelegate(typeof(System.Action), this) as System.Action);

            // for (int i = 0; i < methodList.Length; ++i)
            // {
            //     var method = GetMethod(methodList[i].Item1);
            //     if (!IsValidMagicFunction(method)) continue;
            //     Managers.Update.Register(this, method, methodList[i].Item2);
            // }
        }
    }
}
