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
    private Transform reelerPosition;

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

    public AudioSource music;

    public AudioClip calmTheme;
    public AudioClip extremeTheme;

    public float fishTimer = 0f;
    public float minFishTimer;
    public float maxFishTimer;
    float actualFishTime;

    public float fishEscapeTimer = 3f;

    public bool fishTimerRunning = false;
    public bool escapeTimerRunning = false;

    public bool lineUp = true;
    public bool fishOnTheLine;
    public bool bigOneOnTheLine = false;

    bool controllable = true;

    FishReeled fishReeled;

    [System.Serializable]
    public class FishReeled
    {
        public Sprite fishSprite;
        public string fishName;
    }

    public GameObject theBigOnePrefab;

    public FishReeled[] fishes;


    public AudioClip reelingIn;
    public AudioClip reelingOut;
    public AudioClip deployRod;

    private AudioSource audioSource;
    public AudioSource playerSounds;
    public AudioClip surprisedSound;

    public GameObject exclamation;

    public GameObject timer;

    bool releasing;

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
        audioSource = GetComponent<AudioSource>();

        co = StartReelLose();
    }

    private void Update()
    {
        releasing = Input.GetButtonDown("Jump");
        if (currentState == State.MaxReel && realGameHasStarted)
        {
            loseTimer += Time.deltaTime;
            if (loseTimer > maxLoseTimer)
            {
                player.OnDeath();
                loseTimer = 0.0f;
            }
        }

        if (fishTimerRunning)
        {
            RunFishTimer();
        }
        else
        {
            //StopFishTimer();
        }

        if (escapeTimerRunning)
        {
            RunEscapeTimer();
        }

        if (realGameHasStarted)
        {
            FindObjectOfType<RopeBridge>().scaleFactor = ropeBridge.scaleAmount;
            FindObjectOfType<CameraController>().EnableSpeedParticles(true);
        }
        else
        {
            FindObjectOfType<RopeBridge>().scaleFactor = 0;
            if (lineUp && controllable)
            {
                if (releasing)
                {
                    lineUp = false;
                }
            }

            if (!controllable)
            {
                if (releasing)
                {
                    controllable = true;
                    GameManager.instance.ResetFishOverlay();
                }
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
                ropeBridge.scaleFactor = ropeBridge.scaleAmount;
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
            reelerPosition = reeler.gameObject.GetComponent<Transform>();
            //FakeGameReeling(reeler);

            if (controllable)
            {
                if (!lineUp)
                {
                    if (reeling)
                    {
                        StopFishTimer();
                        StopEscapeTimer();
                        if (FindObjectOfType<PlayerBehavior>().GetSurprise() == true)
                        {
                            FindObjectOfType<PlayerBehavior>().TriggerSurprise(false);
                            exclamation.SetActive(false);
                        }
                        if (audioSource.clip != reelingIn)
                        {
                            audioSource.Stop();
                            audioSource.clip = reelingIn;
                            audioSource.Play();
                        }
                        reelerPosition.position = new Vector3(reelerPosition.position.x, reelerPosition.position.y + 0.1f, reelerPosition.position.z);
                        if (reelerPosition.position.y >= 0f)
                        {
                            if (fishOnTheLine)
                            {
                                FishReeledComplete();
                                fishOnTheLine = false;
                            }
                            reelerPosition.position = new Vector3(reelerPosition.position.x, 0f, reelerPosition.position.z);
                            audioSource.Stop();
                            if (currentState != State.MinReel)
                            {
                                SetState(State.MinReel);
                            }
                        }
                        else
                        {
                            SetState(State.ReelingIn);
                        }

                        if (bigOneOnTheLine && reelerPosition.position.y >= -5f)
                        {
                            StartRealGame();
                        }
                    }
                    else
                    {
                        if (audioSource.clip != reelingOut)
                        {
                            audioSource.Stop();
                            audioSource.clip = reelingOut;
                            audioSource.Play();
                        }
                        reelerPosition.position = new Vector3(reelerPosition.position.x, reelerPosition.position.y - 0.1f, reelerPosition.position.z);
                        if (reelerPosition.position.y < -20f)
                        {
                            reelerPosition.position = new Vector3(reelerPosition.position.x, -20f, reelerPosition.position.z);
                            audioSource.Stop();
                            if (currentState != State.MaxReel)
                            {
                                SetState(State.MaxReel);
                                if (!fishOnTheLine)
                                {

                                    actualFishTime = UnityEngine.Random.Range(minFishTimer, maxFishTimer);
                                    StartFishTimer();
                                }
                            }
                            if (fishOnTheLine && !escapeTimerRunning)
                            {
                                Debug.Log("The fish is trying to escape!");
                                StartEscapeTimer();
                            }
                        }
                        else
                        {
                            SetState(State.ReelingOut);
                        }
                    }
                }
                else
                {
                    reelerPosition.position = new Vector3(reelerPosition.position.x, 0f, reelerPosition.position.z);
                }
            }
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
            //LeanTween.moveLocalY(fakeReeler.gameObject, -20.0f, 1.0f).setOnComplete(StartFishTimer);
        }
        else
        {
            if (currentState == State.ReelingOutHoldFake)
            {
                audioSource.clip = reelingOut;
                audioSource.Play();
                //LeanTween.moveLocalY(fakeReeler.gameObject, 0.0f, 1.0f).setOnComplete(FishReeledComplete);
                fishTimerRunning = false;
                //reelerPosition.position = new Vector3(reelerPosition.position.x, reelerPosition.position.y + 0.1f, reelerPosition.position.z);
                if (reelerPosition.position.y >= 0)
                {
                    FishReeledComplete();
                }

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
                    //lineUp = false;
                    //actualFishTime = UnityEngine.Random.Range(minFishTimer, maxFishTimer);
                    //LeanTween.moveLocalY(fakeReeler.gameObject, -20.0f, 1.0f).setOnComplete(StartFishTimer);
                    //StartFishTimer();
                    //GameManager.instance.ResetFishOverlay();
                    //fishSprite.sprite = null;
                    //LeanTween.moveLocalY(fakeReeler.gameObject, 0.0f, 1.0f);
                }
                else
                {
                    audioSource.clip = reelingIn;
                    audioSource.Play();
                }
                break;
            case State.ReelingOutHoldFake:
                if (!realGameHasStarted)
                {

                }
                break;

            case State.ReelingIn:
                if (!realGameHasStarted)
                {

                }
                else
                {
                    audioSource.clip = reelingOut;
                    audioSource.Play();
                }
                break;
        }
    }

    void OnCompleteMoveBack()
    {
        fishReeled = fishes[UnityEngine.Random.Range(0, fishes.Length - 1)];

        fishSprite.sprite = fishReeled.fishSprite;
        GameManager.instance.SetFishSprite(fishSprite.sprite);

        fishOnTheLine = true;

        if (amountOfFakeFishReeled > 2)
        {
            // ToDo - add better transition
            if (UnityEngine.Random.Range(0, 1) == 0)
            {
                fishReeled = fishes[fishes.Length - 1];
                fishSprite.sprite = fishReeled.fishSprite;
                GameManager.instance.SetFishSprite(fishSprite.sprite);
                FindObjectOfType<PlayerBehavior>().TriggerSurprise(false);
                exclamation.SetActive(false);
                Debug.Log("Reeled in the big one!");
                StartCoroutine(FadeOut(music, 1.2f));
                bigOneOnTheLine = true;
                //StartRealGame();
            }
        }

    }

    private void StartRealGame()
    {
        music.clip = extremeTheme;
        music.Play();
        GameObject go = transform.parent.gameObject;
        transform.parent = null;
        Destroy(go);
        GameObject theBigOne = Instantiate(theBigOnePrefab, transform.position, Quaternion.identity);

        transform.parent = theBigOne.transform;
        timer.SetActive(true);

        realGameHasStarted = true;

        CheckerForHasStartedTheRealGameOnce.instance.hasStartedTheRealGameOnce = true;
    }

    void FishReeledComplete()
    {
        SetState(State.None);
        amountOfFakeFishReeled++;
        fishOnTheLine = false;
        lineUp = true;
        controllable = false;
        Debug.Log("Fish caught!");
        fishSprite.sprite = null;
        GameManager.instance.SetFishText(fishReeled.fishName);
    }

    private void OnDestroy()
    {
        // GameManager.instance.StartState();
        // SetState(State.None);
    }

    void StartFishTimer()
    {
        //SetState(State.ReelingOutHoldFake);
        //audioSource.Stop();
        fishTimerRunning = true;
    }

    void RunFishTimer()
    {
        fishTimer += Time.deltaTime;

        if (fishTimer > actualFishTime)
        {
            ShowSurprise();
            OnCompleteMoveBack();
            StopFishTimer();
            Debug.Log("Fish on the line!");
        }
    }

    void StopFishTimer()
    {
        fishTimerRunning = false;
        fishTimer = 0f;
    }

    void ShowSurprise()
    {
        playerSounds.clip = surprisedSound;
        playerSounds.Play();
        FindObjectOfType<PlayerBehavior>().TriggerSurprise(true);
        exclamation.SetActive(true);
    }

    void StartEscapeTimer()
    {
        escapeTimerRunning = true;
    }

    void RunEscapeTimer()
    {
        fishEscapeTimer -= Time.deltaTime;

        if (fishEscapeTimer <= 0)
        {
            FishEscapes();
            StopEscapeTimer();
        }
    }

    void StopEscapeTimer()
    {
        fishEscapeTimer = 3f;
        escapeTimerRunning = false;
    }

    void FishEscapes()
    {
        Debug.Log("The fish got away!");
        FindObjectOfType<PlayerBehavior>().TriggerSurprise(false);
        exclamation.SetActive(false);
        fishOnTheLine = false;
        fishSprite.sprite = null;
        StartFishTimer();
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

}
