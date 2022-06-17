using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTest : MonoBehaviour
{
    [SerializeField]
    private Transform body;

    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private Transform cameraHandle;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private TargetingSystem targeting;

    [SerializeField]
    private float animationFadeMultiplier = 1F;

    [SerializeField]
    private float moveSpeedMultiplier = 1F;

    private float hFollower = 0F, vFollower = 0F;
    private float floatCutoff = 0.005f;
    private Quaternion originHandleRot;

    private float GetSign(float value) 
    {
        if (Mathf.Abs(value) <= floatCutoff) return 0F;
        return Mathf.Sign(value);
    }

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;

        originHandleRot = cameraHandle.localRotation;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Mouse X");
        float v = Input.GetAxisRaw("Mouse Y");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        var euler = cameraHandle.eulerAngles;
        euler.x = Mathf.Clamp(euler.x - v, 0F, 70F);
        euler.y += h;

        if (targeting.IsTargeting) cameraHandle.localRotation = Quaternion.Slerp(cameraHandle.localRotation, Quaternion.LookRotation(targeting.target.position - body.position, Vector3.up), Time.deltaTime * 20F);
        else cameraHandle.eulerAngles = euler;
        

        var forward = targeting.IsTargeting ? body.forward : Quaternion.AngleAxis(euler.y, Vector3.up) * Vector3.forward;
        // Debug forward vector
        Debug.DrawRay(body.position, forward, Color.red);


        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (!targeting.IsTargeting && (h != 0 || v != 0))
        {
            body.rotation = Quaternion.Slerp(body.rotation, Quaternion.AngleAxis(euler.y, Vector3.up), Time.deltaTime * 10F);
        }
        else if (targeting.IsTargeting) 
        {
            var targetRot = Quaternion.LookRotation(targeting.target.position - body.position, Vector3.up);
            body.rotation = Quaternion.Lerp(body.rotation, targetRot, Time.deltaTime * 10F);
        }        
        
        hFollower += GetSign(h - hFollower) * Time.deltaTime * animationFadeMultiplier;
        if (Mathf.Abs(hFollower) <= floatCutoff) hFollower = 0F;

        vFollower += GetSign(v - vFollower) * Time.deltaTime * animationFadeMultiplier;
        if (Mathf.Abs(vFollower) <= floatCutoff) vFollower = 0F;
    
        var moveDir = (forward * v + body.right * h) * Time.deltaTime * moveSpeedMultiplier * (Mathf.Sign(v) > 0F && targeting.IsTargeting ? 2F : 1F);
        controller.Move(moveDir);
        

        animator.SetFloat("HSpeed", hFollower);
        animator.SetFloat("VSpeed", vFollower);

        animator.SetBool("Sprint", isSprinting);

        animator.SetBool("OnTargeting", targeting.IsTargeting);
    }

    private void OnGUI() 
    {
        Rect rt = new Rect(Vector2.zero, new Vector2(Screen.width * 0.3f, Screen.height));
        GUI.Label(rt, $"HF: {hFollower}\nVF: {vFollower}");
    }
}
