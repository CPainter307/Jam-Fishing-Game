using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public GameObject boat;
    private FixedJoint2D joint;
    // The attached variable is for when you want the player to be flung into the air
    public bool attached;
    public float rotateSpeed = 0.03f;
    public BoxCollider2D boatCollider;

    public float rotatePower = 5f;

    private Vector3 startScale;

    // Start is called before the first frame update
    void Start()
    {
        boatCollider = boat.GetComponent<BoxCollider2D>();
        playerBody = GetComponent<Rigidbody2D>();
        joint = GetComponent<FixedJoint2D>();

        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (attached)
        {
            // playerBody.gravityScale = 1.5f;
        }
        else
        {
            // playerBody.gravityScale = 1f;
        }
    }

    void FixedUpdate()
    {
        var boatRot = boat.transform.rotation.eulerAngles.z;
        joint.enabled = attached;
        if (attached)
        {
            

            // // Boat is rotating right
            // if (boatRot >= 270 + rotatePower && boatRot <= 360 - rotatePower)
            // {
            //     transform.Translate(Vector2.right * boatRot * rotateSpeed * Time.deltaTime);
            // }
            // // Boat is rotating left
            // else if (boatRot >= 0 + rotatePower && boatRot <= 90 - rotatePower)
            // {
            //     transform.Translate(Vector2.right * (boatRot - 360) * rotateSpeed * Time.deltaTime);
            // }

            // transform.eulerAngles = new Vector3(
            //     transform.eulerAngles.x,
            //     transform.eulerAngles.y,
            //     boat.transform.rotation.eulerAngles.z
            // );

            // transform.parent = boatCollider.transform;
            // playerBody.position = new Vector3(playerBody.position.x, boatCollider.bounds.max.y);
            // playerBody.constraints = RigidbodyConstraints2D.FreezePosition;
            // transform.position = new Vector3(transform.position.x, boatCollider.bounds.max.y, transform.position.z);
            // transform.localScale = new Vector3(
            //     startScale.x / transform.parent.localScale.x,
            //     startScale.y / transform.parent.localScale.y,
            //     startScale.z / transform.parent.localScale.z);
        }
        else
        {
            // transform.parent = null;
        }
    }
}
