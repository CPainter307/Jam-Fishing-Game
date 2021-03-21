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

    private IEnumerator co;

    public float maxReel = 5f;
    public float minReel = 20f;

    public float zoom = 2.0f;
    public float minZoom = 10.0f;

    public float frontTorqueHandicap = 2f;

    private bool followPlayer = false;

    public bool isAtMaxReel = false;

    public float loseTimer = 0f;
    public float maxLoseTimer = 3f;

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

    public enum State
    {
        None,
        ReelingIn,
        ReelingOut,
        MaxReel,
        MinReel
    }

    State currentState = State.None;

    private void Start()
    {
        reelCircle = GetComponent<CircleCollider2D>();
        ropeBridge = GetComponentInChildren<RopeBridge>();

        co = StartReelLose();
    }

    private void Update()
    {
        followPlayer = player.attached;

        if (currentState == State.MaxReel)
        {
            loseTimer += Time.deltaTime;
            if (loseTimer > maxLoseTimer)
            {
                player.OnDeath();
                loseTimer = 0.0f;
            }
        }
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

        FishBehavior fish = GameObject.FindObjectOfType<FishBehavior>();

        if (fish == null) return;

        if (!reeling)
        {
            if (reelCircle.radius < minReel)
            {
                SetState(State.ReelingIn);
                reelCircle.radius += reelOutSpeed * Time.fixedDeltaTime;
                GameManager.instance.UpdateReelUI(reelCircle.radius, currentState, reelOutSpeed * Time.fixedDeltaTime);
            }
            else
            {
                SetState(State.MaxReel);
                GameManager.instance.UpdateReelUI(reelCircle.radius, currentState, 0.0f);
            }
        }
        else
        {
            fish.DecreaseHealth(2f);
            if (reelCircle.radius > maxReel)
            {
                SetState(State.ReelingOut);
                reelCircle.radius -= reelInSpeed * Time.fixedDeltaTime;
                GameManager.instance.UpdateReelUI(reelCircle.radius, currentState, reelInSpeed * Time.fixedDeltaTime);
            }
            else
            {
                SetState(State.MinReel);
                GameManager.instance.UpdateReelUI(reelCircle.radius, currentState, 0.0f);
            }
        }
    }

    IEnumerator StartReelLose()
    {
        yield return new WaitForSeconds(3f);
    }

    void SetState(State state)
    {
        if (currentState == state) return;

        GameManager.instance.StartState();
        loseTimer = 0.0f;

        currentState = state;
        switch (state)
        {
            case State.None:
                break;

            case State.MinReel:
                break;

            case State.MaxReel:
                break;
        }
    }

    private void OnDestroy()
    {
        // GameManager.instance.StartState();
        // SetState(State.None);
    }
}
