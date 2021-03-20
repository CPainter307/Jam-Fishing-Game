using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineController : MonoBehaviour
{
    public static PlayerLineController instance;
    [HideInInspector] public CircleCollider2D reelCircle;
    public FishingPole pole;
    public Boat boat;
    public PlayerBehavior player;
    [HideInInspector] public RopeBridge ropeBridge;

    public Cinemachine.CinemachineVirtualCamera vcam;

    [HideInInspector] public bool reeling = false;
    public float reelInSpeed = 5f;
    public float reelOutSpeed = 15f;

    public float maxReel = 5f;
    public float minReel = 20f;

    public float zoom = 2.0f;
    public float minZoom = 10.0f;

    public float frontTorqueHandicap = 2f;

    private bool followPlayer = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            print("Destroying existing singleton");
        }
    }

    private void Start()
    {
        reelCircle = GetComponent<CircleCollider2D>();
        ropeBridge = GetComponentInChildren<RopeBridge>();
    }

    private void Update()
    {
        followPlayer = player.attached;
    }

    private void FixedUpdate()
    {
        float dist = Vector3.Distance(boat.GetComponent<Rigidbody2D>().position, reelCircle.transform.position);
        if (dist > reelCircle.radius)
        {
            ropeBridge.scaleFactor = 100;
            boat.GetComponent<Rigidbody2D>().position = reelCircle.ClosestPoint(boat.GetComponent<Rigidbody2D>().position);
        }
        else
        {
            ropeBridge.scaleFactor = -1;
        }
        // if (player.attached)
        // {
        //     print("player attached");
        //     dist = Vector3.Distance(boat.GetComponent<Rigidbody2D>().position, reelCircle.transform.position);
        //     if (dist > reelCircle.radius)
        //     {
        //         ropeBridge.scaleFactor = 100;
        //         boat.GetComponent<Rigidbody2D>().position = reelCircle.ClosestPoint(boat.GetComponent<Rigidbody2D>().position);
        //     }
        //     else
        //     {
        //         ropeBridge.scaleFactor = -1;
        //     }
        // }
        // else if (!player.attached)
        // {
        //     print("player not attached");
        //     dist = Vector3.Distance(player.GetComponent<Rigidbody2D>().position, reelCircle.transform.position);
        //     if (dist > reelCircle.radius)
        //     {
        //         ropeBridge.scaleFactor = 100;
        //         player.GetComponent<Rigidbody2D>().position = reelCircle.ClosestPoint(player.GetComponent<Rigidbody2D>().position);
        //     }
        //     else
        //     {
        //         ropeBridge.scaleFactor = -1;
        //     }
        // }

        reeling = Input.GetButton("Jump");

        if (!reeling)
        {
            if (reelCircle.radius < minReel)
                reelCircle.radius += reelOutSpeed * Time.deltaTime;
        }
        else
        {
            GameObject.FindObjectOfType<FishBehavior>().DecreaseHealth(2f);
            if (reelCircle.radius > maxReel)
                reelCircle.radius -= reelInSpeed * Time.deltaTime;
        }
    }
}
