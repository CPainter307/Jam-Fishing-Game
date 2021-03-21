using System;
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

    private int amountOfFakeFishReeled = 0;

    private IEnumerator co;

    private FakeReeler fakeReeler;

    public float maxReel = 5f;
    public float minReel = 20f;

    public float zoom = 2.0f;
    public float minZoom = 10.0f;

    public float frontTorqueHandicap = 2f;

    private bool followPlayer = false;

    public bool isAtMaxReel = false;

    public float loseTimer = 0f;
    public float maxLoseTimer = 3f;

    public bool realGameHasStarted = false;

    public SpriteRenderer fishSprite;

    FishReeled fishReeled;

    [System.Serializable]
    public class FishReeled
    {
        public Sprite fishSprite;
        public string fishName;
    }

    public GameObject theBigOnePrefab;

    public FishReeled[] fishes;

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
        MinReel,
        ReelingOutHoldFake
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
        reeling = Input.GetButton("Jump");

        if (realGameHasStarted)
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

            FishBehavior fish = GameObject.FindObjectOfType<FishBehavior>();
            if (fish == null) return;
            RealGameReeling(fish);
        }
        else
        {
            FakeReeler reeler = GameObject.FindObjectOfType<FakeReeler>();
            if (reeler == null) return;
            fakeReeler = reeler;
            FakeGameReeling(reeler);
        }
    }

    private void FakeGameReeling(FakeReeler reeler)
    {
        // if (Input.GetButtonUp("Jump"))
        // {
        //     SetState(State.ReelingOutHoldFake);
        // }

        if (!reeling)
        {
            // LeanTween.cancelAll(fakeReeler.gameObject);
        }
        else
        {
            if (currentState == State.ReelingOutHoldFake)
            {
                LeanTween.moveLocalY(fakeReeler.gameObject, 0.0f, 1.0f).setOnComplete(FishReeledComplete);
            }
            else
            {
                SetState(State.ReelingOut);
            }
        }
    }

    private void RealGameReeling(FishBehavior fish)
    {
        if (!reeling)
        {
            if (reelCircle.radius < minReel)
            {
                SetState(State.ReelingIn);
                reelCircle.radius += reelOutSpeed * Time.fixedDeltaTime;
                GameManager.instance.UpdateReelUI(reelCircle.radius, currentState, (reelOutSpeed / 4.5f) * Time.fixedDeltaTime);
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
                GameManager.instance.UpdateReelUI(reelCircle.radius, currentState, (reelInSpeed / 4.5f) * Time.fixedDeltaTime);
            }
            else
            {
                print("test");
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

            case State.ReelingOut:
                if (!realGameHasStarted)
                {
                    LeanTween.moveLocalY(fakeReeler.gameObject, -20.0f, 1.0f).setOnComplete(OnCompleteMoveBack);
                    GameManager.instance.ResetFishOverlay();
                    fishSprite.sprite = null;
                    //LeanTween.moveLocalY(fakeReeler.gameObject, 0.0f, 1.0f);
                }
                break;
            case State.ReelingOutHoldFake:
                if (!realGameHasStarted)
                {

                }
                break;

            case State.ReelingIn:
                break;
        }
    }

    void OnCompleteMoveBack()
    {
        SetState(State.ReelingOutHoldFake);

        fishReeled = fishes[UnityEngine.Random.Range(0, fishes.Length)];

        fishSprite.sprite = fishReeled.fishSprite;
        GameManager.instance.SetFishSprite(fishSprite.sprite);

        if (amountOfFakeFishReeled > 2)
        {
            if (UnityEngine.Random.Range(0, 1) == 0)
            {
                StartRealGame();
            }
        }
        amountOfFakeFishReeled++;
    }

    private void StartRealGame()
    {
        GameObject go = transform.parent.gameObject;
        transform.parent = null;
        Destroy(go);
        GameObject theBigOne = Instantiate(theBigOnePrefab, transform.position, Quaternion.identity);

        transform.parent = theBigOne.transform;

        realGameHasStarted = true;

        CheckerForHasStartedTheRealGameOnce.instance.hasStartedTheRealGameOnce = true;
    }

    void FishReeledComplete()
    {
        SetState(State.None);

        GameManager.instance.SetFishText(fishReeled.fishName);
    }

    private void OnDestroy()
    {
        // GameManager.instance.StartState();
        // SetState(State.None);
    }
}
