using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerBehavior : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public GameObject boat;
    private FixedJoint2D joint;
    // The attached variable is for when you want the player to be flung into the air
    public bool attached = true;
    public float rotateSpeed = 0.03f;
    public BoxCollider2D boatCollider;

    public float rotatePower = 5f;

    private Vector3 startScale;

    public bool dead = false;

    public float minDepth = -2;

    public bool godMode = false;

    public bool currentlySurprised;

    // Start is called before the first frame update
    void Start()
    {
        boatCollider = boat.GetComponent<BoxCollider2D>();
        playerBody = GetComponent<Rigidbody2D>();
        // joint = GetComponent<FixedJoint2D>();

        startScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            FindObjectOfType<GameManager>().audioMixer.SetFloat("musicPitch", 1f);

            if (!CheckerForHasStartedTheRealGameOnce.instance.hasStartedTheRealGameOnce)
                SceneManager.LoadScene("SethScene-init");
            else
                SceneManager.LoadScene("SethScene");
        }
        GetComponent<Animator>().SetBool("Reeling", Input.GetKey(KeyCode.Space));
    }

    public void TriggerSurprise(bool surprise)
    {
        GetComponent<Animator>().SetBool("Surprised", surprise);
        currentlySurprised = surprise;
    }

    public bool GetSurprise()
    {
        return currentlySurprised;
    }

    void FixedUpdate()
    {
        if (playerBody.position.y <= minDepth)
        {
            playerBody.velocity = new Vector3(playerBody.velocity.x, 0);
        }

        var boatRot = boat.transform.rotation.eulerAngles.z;

        if (attached)
        {
            // // Boat is rotating right
            if (boatRot <= 270 && boatRot >= 90)
            {
                OnDeath();
            }
            playerBody.simulated = false;
            transform.parent = boatCollider.transform;
        }
        else
        {
            playerBody.simulated = true;
            transform.parent = null;
        }
    }

    public void OnDeath()
    {
        if (godMode) return;

        attached = false;
        dead = true;

        playerBody.velocity = Vector3.zero;

        GameManager.instance.TriggerLose();
    }
}
