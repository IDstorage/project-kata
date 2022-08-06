using UnityEngine;
using Anomaly;

[System.Serializable]
public class AnimatorComponent : CustomComponent
{
    [SerializeField]
    private Animator animator;


    public void SetFloat(string key, float value)
    {
        animator.SetFloat(key, value);
    }

    public void SetInteger(string key, int value)
    {
        animator.SetInteger(key, value);
    }

    public void SetBool(string key, bool value)
    {
        animator.SetBool(key, value);
    }

    public void SetTrigger(string key)
    {
        animator.SetTrigger(key);
    }


    public AnimatorStateInfo GetCurrentState(int layer = 0)
    {
        return animator.GetCurrentAnimatorStateInfo(layer);
    }
    public AnimatorStateInfo GetNextState(int layer = 0)
    {
        return animator.GetNextAnimatorStateInfo(layer);
    }
}