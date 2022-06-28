using UnityEngine;
using Anomaly;

[System.Serializable]
public class AnimatorComponent : CustomComponent
{
    [System.Serializable]
    [SharedComponentData(typeof(AnimatorComponent))]
    public class Data : CustomComponent.BaseData
    {
        public Animator animator;
    }


    public void SetFloat(Data target, string key, float value)
    {
        target.animator.SetFloat(key, value);
    }

    public void SetInteger(Data target, string key, int value)
    {
        target.animator.SetInteger(key, value);
    }

    public void SetBool(Data target, string key, bool value)
    {
        target.animator.SetBool(key, value);
    }

    public void SetTrigger(Data target, string key)
    {
        target.animator.SetTrigger(key);
    }
}