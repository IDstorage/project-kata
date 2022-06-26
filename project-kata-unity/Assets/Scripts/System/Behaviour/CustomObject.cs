using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly
{
    public class CustomObject : CustomBehaviour
    {
        private Dictionary<Type, CustomComponent.BaseData> componentsData = new Dictionary<Type, CustomComponent.BaseData>();

        protected override void Initialize()
        {
            base.Initialize();
            InitializeComponents(this.GetType());
        }


        public void InitializeComponents(params CustomComponent.BaseData[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                var attribute = Attribute.GetCustomAttribute(data[i].GetType(), typeof(SharedComponentDataAttribute)) as SharedComponentDataAttribute;
                componentsData.Add(attribute.OuterType, data[i]);
            }
        }

        public void InitializeComponents(System.Type targetType)
        {
            var fields = targetType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(CustomComponent.BaseData))) continue;

                var attribute = Attribute.GetCustomAttribute(field.FieldType, typeof(SharedComponentDataAttribute)) as SharedComponentDataAttribute;
                if (attribute == null)
                {
                    Debug.LogError($"Wrong Data Format!! You must add SharedComponentData attribute. See {field.FieldType}.");
                    continue;
                }

                if (componentsData.ContainsKey(attribute.OuterType)) continue;

                componentsData.Add(attribute.OuterType, field.GetValue(this) as CustomComponent.BaseData);
            }
        }


        public T GetComponentData<T>() where T : CustomComponent
        {
            if (!componentsData.ContainsKey(typeof(T))) throw new System.Exception("Wrong ComponentData Access");
            return componentsData[typeof(T)] as T;
        }

        public T GetSharedComponent<T>() where T : CustomComponent, new()
        {
            if (!componentsData.ContainsKey(typeof(T))) throw new System.Exception("Wrong Component Access");
            return ComponentPool.Get<T>();
        }
    }
}
