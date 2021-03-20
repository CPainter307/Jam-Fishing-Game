using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    Rigidbody2D rb;

    float cameraRightPos;

    public float rotateForce = 10.0f;
    public float moveWaterEdgeBuffer = 5.0f;
    public float moveSpeed = 5.0f;

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
        cameraRightPos = rb.position.x + (Camera.main.orthographicSize * Screen.width / Screen.height);

        while (cameraRightPos > (WaterManager.instance.finalNodePos.x - moveWaterEdgeBuffer))
        {
            WaterManager.instance.SwapFirstWithLast();

            
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.AddTorque(-horizontal * rotateForce);

        rb.AddForce(new Vector2(moveSpeed, 0.0f));
    }
}
