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

    public AudioClip hitWaterSound;
    public AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        // gets right bound of camera
        cameraRightPos = rb.position.x + (Camera.main.orthographicSize * Screen.width / Screen.height);

        if (rb.position.y < -.5f)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = hitWaterSound;
                audioSource.pitch = UnityEngine.Random.Range(.7f, .9f);
                audioSource.volume = .5f;
                audioSource.Play();
            }
        }

        while (cameraRightPos > (WaterManager.instance.finalNodePos.x - moveWaterEdgeBuffer))
        {
            WaterManager.instance.SwapFirstWithLast();


        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "FishSpike")
        {
            print("test");
            GameObject.FindObjectOfType<PlayerBehavior>().OnDeath();
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.AddTorque(-horizontal * rotateForce);

        rb.AddForce(new Vector2(moveSpeed, 0.0f));
    }
}
