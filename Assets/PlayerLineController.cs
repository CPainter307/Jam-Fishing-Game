using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineController : MonoBehaviour
{
    public static PlayerLineController instance;
    [HideInInspector] public CircleCollider2D reelCircle;
    public FishingPole pole;
    public Boat boat;
    [HideInInspector] public RopeBridge ropeBridge;

    [HideInInspector] public bool reeling = false;
    public float reelSpeed = 5f;

    public float maxReel = 5f;
    public float minReel = 20f;

    public float frontTorqueHandicap = 2f;

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

    private void FixedUpdate()
    {
        if (Vector3.Distance(boat.GetComponent<Rigidbody2D>().position, reelCircle.transform.position) > reelCircle.radius)
        {
            ropeBridge.scaleFactor = -1;
            boat.GetComponent<Rigidbody2D>().position = reelCircle.ClosestPoint(boat.GetComponent<Rigidbody2D>().position);
            
            // boat.GetComponent<Rigidbody2D>().AddTorque(-frontTorqueHandicap);
        }
        else
        {
            ropeBridge.scaleFactor = 100;
        }

        reeling = Input.GetButton("Jump");

        if (!reeling)
        {
            if (reelCircle.radius < minReel)
                reelCircle.radius += reelSpeed * Time.deltaTime;
        }
        else
        {
            if (reelCircle.radius > maxReel)
                reelCircle.radius -= reelSpeed * Time.deltaTime;
        }
    }
}
