using System.Threading.Tasks;
using UnityEngine;
using Anomaly.Utils;
using System.Collections;

public abstract class BaseTrail : PoolObject
{
    public abstract void OnTrailStarted();
    public abstract void OnTrailEnded();

    public abstract void OnTracingBegan();
    public abstract void OnTracingEnded();

    public abstract bool IsFinished();

    private SmartCoroutine returnCoroutine;

    private void OnEnable()
    {
        OnTrailStarted();
    }

    private void OnDisable()
    {
        OnTrailEnded();
    }


    public void BeginTracing(Transform parent = null)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;

        OnTracingBegan();
    }

    public void EndTracing()
    {
        transform.SetParent(null);
        OnTracingEnded();

        if (returnCoroutine == null) returnCoroutine = SmartCoroutine.Create(CoReturn);
        returnCoroutine.Stop();
        returnCoroutine.Start();

        IEnumerator CoReturn()
        {
            while (!IsFinished()) yield return null;

            Debug.Log("Return");
            Return();
        }
    }
}
