using UnityEngine;

public class TestTrail : BaseTrail
{
    [SerializeField] private TrailRenderer trailRenderer;

    public override void OnTrailStarted()
    {
        trailRenderer.enabled = true;

        Debug.Log("Trail started");
    }

    public override void OnTrailEnded()
    {
        trailRenderer.enabled = false;

        Debug.Log("Trail ended");
    }

    public override void OnTracingBegan()
    {
        Debug.Log("Start tracing");
    }

    public override void OnTracingEnded()
    {
        // Stop particles...
        Debug.Log("Stop tracing");
    }

    public override bool IsFinished()
    {
        return trailRenderer.positionCount == 0;
    }
}
