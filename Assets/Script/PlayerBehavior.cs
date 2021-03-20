using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public GameObject boat;
    // The attached variable is for when you want the player to be flung into the air
    public bool attached;
    public float rotateSpeed = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attached)
        {
            playerBody.gravityScale = 1.5f;
        }
        else
        {
            playerBody.gravityScale = 1f;
        }
    }

    void FixedUpdate()
    {
        var boatRot = boat.transform.rotation.eulerAngles.z;
        if (attached)
        {
            // Boat is rotating right
            if (boatRot >= 275 && boatRot <= 355)
            {
                transform.Translate(Vector2.right * boatRot * rotateSpeed * Time.deltaTime);
            }
            // Boat is rotating left
            else if (boatRot >= 5 && boatRot <= 85)
            {
                transform.Translate(Vector2.right * (boatRot - 360) * rotateSpeed * Time.deltaTime);
            }
        }
        if (attached)
        {
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                boat.transform.rotation.eulerAngles.z
            );
        }
    }

    
}
