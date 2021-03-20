using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    Rigidbody2D rb;

    float cameraRightPos;

    public float rotateForce = 10.0f;
    public float moveWaterEdgeBuffer = 5.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        // gets right bound of camera
        cameraRightPos = transform.position.x + (Camera.main.orthographicSize * Screen.width / Screen.height);

        if (cameraRightPos > (WaterManager.instance.finalNodePos.x - moveWaterEdgeBuffer))
        {
            // print(WaterManager.instance.finalNodePos);
            WaterManager.instance.SwapFirstWithLast();
            // print(WaterManager.instance.finalNodePos);
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.AddTorque(-horizontal * rotateForce);

        rb.AddForce(new Vector2(5.0f, 0.0f));
    }
}
