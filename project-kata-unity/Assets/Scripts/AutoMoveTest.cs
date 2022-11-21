using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveTest : MonoBehaviour
{
    [SerializeField] private float radius = 1F;
    [SerializeField] private float speed = 1F;
    private float angle = 0F;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Cos(angle) * radius, 0F, Mathf.Sin(angle) * radius);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.right);

        angle += Mathf.PI * 2 * Time.deltaTime * speed;
        if (angle > Mathf.PI * 2)
        {
            angle -= Mathf.PI * 2;
        }
    }
}
