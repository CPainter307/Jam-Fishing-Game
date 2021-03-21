using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    // Stores the fish's maximum health.
    public float maxFishHealth;

    // Stores the fish's current health.
    public float currentHealth;
    
    /**
     * Stores the current timer being checked by the fish's behavior. Once the timer
     * reaches this number, the next phase starts and the currentTimer changes.
     **/
    float currentTimer;

    // Stores the value that the timer is currently at.
    float currentTime;

    /** 
     * This float is the time between attacks for the fish, starting at the end of
     * the previous attack time and then going until the pre-attack delay. This value
     * is decreased to a limit of minNextAttackTime as fish health decreases.
     **/
    public float nextAttackTime;

    /**
     * The smallest amount of time the fish can take before attacking. As the fish's health
     * gets lower, nextAttackTime gets closer and closer to this (threshhold based).
     **/
    public float minNextAttackTime;

    // A value representing a third of the difference between nextAttackTime and minNextAttackTime.
    float attackTimeThird;

    // Booleans that allow nextAttackTime to only change once per threshhold.
    bool changeTime1 = false;
    bool changeTime2 = false;
    bool changeTime3 = false;

    // Stores the current attack time.
    float attackTime;

    // Stores the attack time for the fish's attack 1.
    public float attack01Time;

    // Stores the attack time for the fish's attack 2.
    public float attack02Time;

    // Stores the attack time for the fish's attack 3.
    public float attack03Time;

    // Stores the lower range of the pre attack delay (ideally, this should be 0).
    public float preAttackRangeLow;

    // Stores the upper range of thet pre attack delay (shouldn't be very large).
    public float preAttackRangeHigh;

    // Stores the pre attack delay after selected from the range.
    float preAttackDelayTime;

    // Stores the id of the fish's attack it's going to use.
    int attackID;

    /**
     * Stores the id of the current timer.
     * 0 = nextAttackTime
     * 1 = preAttackDelayTime
     * 2 = attackTime
     **/
    int timerID;

    /*movement*/
    private Rigidbody2D rigidbody;
    public float moveSpeed = 5.0f;
    public float bobAmount = 20.0f;
    public float jumpForce = 90.0f;

    public Transform tailPosition;
    public float splashStrength;

    public float maxXVelocity = 50f;

    public float maxDepth = -5f;

    bool canMove = true;

    public PlayerBehavior player;

    public GameObject spikePrefab;
    public Transform[] spikeSpawnPositions;

    public AudioClip landInWater;
    public AudioClip jumpSound;
    public AudioClip growlSound;

    private AudioSource audioSource;

    public float audioPlayDiveSoundDepth = -2f;

    private float angle;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxFishHealth;
        currentTime = 0f;
        currentTimer = nextAttackTime;
        timerID = 0;
        attackTimeThird = ((nextAttackTime - minNextAttackTime) / 3f);

        rigidbody = GetComponent<Rigidbody2D>();

        rigidbody.position = new Vector2(rigidbody.position.x + 9.0f, 0.0f);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseNextAttackTime();
        RunTimer();
        // DecreaseHealth();
        if (currentTime >= currentTimer)
        {
            switch (timerID)
            {
                case 0:
                    Debug.Log("nextAttackTime complete at " + currentTime + " seconds. Moving to preAttackDelay.");
                    currentTime = 0f;
                    preAttackDelayTime = Random.Range(preAttackRangeLow, preAttackRangeHigh + 1);
                    currentTimer = preAttackDelayTime;
                    timerID = 1;
                    break;

                case 1:
                    Debug.Log("preAttackDelay complete at " + currentTime + " seconds. Moving to attackTime.");
                    SelectFishAttack();
                    FishAttack(attackID);
                    currentTime = 0f;
                    currentTimer = attackTime;
                    timerID = 2;
                    break;

                case 2:
                    Debug.Log("attackTime complete at " + currentTime + " seconds. Moving to nextAttackTime.");
                    canMove = true;
                    currentTime = 0f;
                    currentTimer = nextAttackTime;
                    timerID = 0;
                    break;
            }
        }

        UpdateCamera();

        Vector2 v = rigidbody.velocity;
        angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void UpdateCamera()
    {
        // Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        // pos.x = Mathf.Clamp01(pos.x);
        // pos.y = Mathf.Clamp01(pos.y);
        // transform.position = Camera.main.ViewportToWorldPoint(pos);
        // Vector3 xyz = transform.position;
        // float distance = Mathf.Max(xyz.x, xyz.y, xyz.z);
        // distance /= (2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad));
        // // Move camera in -z-direction; change '2.0f' to your needs
        // Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -distance * 2.0f);
    }

    private void FixedUpdate()
    {
        Move();

        if (maxDepth >= rigidbody.position.y)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);

            
        }

        if (audioPlayDiveSoundDepth >= rigidbody.position.y)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = landInWater;
                audioSource.pitch = UnityEngine.Random.Range(.90f, 1.1f);
                audioSource.volume = .5f;
                audioSource.Play();
            }
        }
    }

    private void Move()
    {
        if (canMove)
        {
            // rigidbody.MovePosition(rigidbody.position + new Vector2(moveSpeed * Time.fixedDeltaTime, Time.fixedDeltaTime * bobAmount * Mathf.Sin(Time.time * 20.0f)));
            rigidbody.AddForce(new Vector2(moveSpeed * Time.fixedDeltaTime, 0f/*Time.fixedDeltaTime * bobAmount * Mathf.Sin(Time.time * 20.0f)*/));
            if (rigidbody.velocity.x > maxXVelocity)
            {
                rigidbody.velocity = new Vector2(maxXVelocity, rigidbody.velocity.y);
            }
        }
        // transform.position = new Vector2(moveSpeed * Time.fixedDeltaTime, bobAmount * Mathf.Sin(Time.time * 20.0f));
    }

    void RunTimer()
    {
        currentTime += Time.deltaTime;
    }

    void FishAttack(int id)
    {
        string attackString;
        switch (id)
        {
            case 0:
                attackString = "Tail Splash Attack";
                attackTime = attack01Time;
                FishAttack01();
                break;

            case 1:
                attackString = "Giant Leap Attack";
                attackTime = attack02Time;
                FishAttack02();
                break;

            case 2:
                attackString = "Falling Spikes Attack";
                attackTime = attack03Time;
                FishAttack03();
                break;

            default:
                attackString = "INVALID ATTACK - random failed";
                attackTime = 0f;
                break;
        }
        Debug.Log("Fish attacks with " + attackString + ".");
    }

    void SelectFishAttack()
    {
        //attackID = Random.Range(1, 2);
        attackID = Random.Range(0, 3);
    }

    /**
     * Here, have nextAttackTime get closer to minNextAttackTime
     * based on how much health the fish has left. I think a threshhold
     * system, where the number decriments based on percentage of the
     * fish's health (like 100%, 75%, 50%, 25%, etc.).
     **/
    void DecreaseNextAttackTime()
    {
        // Fish health between 75% and 51%.
        if (currentHealth <= (maxFishHealth * 0.75)
            && currentHealth > (maxFishHealth * 0.50))
        {
            if (!changeTime1)
            {
                nextAttackTime = nextAttackTime - attackTimeThird;
                changeTime1 = true;
                Debug.Log("nextAttackTime updated to " + nextAttackTime + " seconds.");
            }
        }

        // Fish health between 50% and 26%.
        if (currentHealth <= (maxFishHealth * 0.50)
            && currentHealth > (maxFishHealth * 0.25))
        {
            if (!changeTime2)
            {
                nextAttackTime = nextAttackTime - attackTimeThird;
                changeTime2 = true;
                Debug.Log("nextAttackTime updated to " + nextAttackTime + " seconds.");
            }
        }

        // Fish health between 25% and 0%.
        if (currentHealth <= (maxFishHealth * 0.25)
            && currentHealth > (maxFishHealth * 0.00))
        {
            if (!changeTime3)
            {
                nextAttackTime = minNextAttackTime;
                changeTime3 = true;
                Debug.Log("nextAttackTime updated to " + nextAttackTime + " seconds.");
            }
        }
    }

    void FishAttack01()
    {
        Debug.Log("Fish attacking...");

        StartCoroutine(SplashTimed());
    }

    IEnumerator SplashTimed()
    {
        DoSplash();
        yield return new WaitForSeconds(1f);
        DoSplash();
        yield return new WaitForSeconds(1f);
        DoSplash();
        yield return new WaitForSeconds(1f);
        DoSplash();
    }

    private void DoSplash()
    {
        WaterManager.instance.Splash(tailPosition.position.x, splashStrength);

        GameObject holder = new GameObject();
        AudioSource audi = holder.AddComponent(typeof(AudioSource)) as AudioSource;

        audi.clip = jumpSound;
        audi.pitch = UnityEngine.Random.Range(.90f, 1.1f);
        audi.volume = .5f;
        audi.Play();
        Destroy(holder, 2.0f);
    }

    void FishAttack02()
    {
        Debug.Log("Fish attacking...");

        if (canMove)
        {
            rigidbody.AddForce(new Vector3(0, jumpForce), ForceMode2D.Impulse);

            GameObject holder = new GameObject();
            AudioSource audi = holder.AddComponent(typeof(AudioSource)) as AudioSource;

            audi.clip = jumpSound;
            audi.pitch = UnityEngine.Random.Range(.90f, 1.1f);
            audi.volume = .5f;
            audi.Play();
            Destroy(holder, 2.0f);
            // player.attached = false;
        }
        canMove = false;
    }

    void FishAttack03()
    {
        Debug.Log("Fish attacking...");

        GameObject holder = new GameObject();
        AudioSource audi = holder.AddComponent(typeof(AudioSource)) as AudioSource;

        audi.clip = growlSound;
        audi.pitch = UnityEngine.Random.Range(.90f, 1.1f);
        audi.volume = .5f;
        audi.Play();
        Destroy(holder, 5.0f);

        StartCoroutine(SpikeSpawn());
    }

    public IEnumerator SpikeSpawn()
    {
        if (rigidbody.position.y > 1.0f)
            Instantiate(spikePrefab, spikeSpawnPositions[0]);
        yield return new WaitForSeconds(.5f);
        if (rigidbody.position.y > 1.0f)
            Instantiate(spikePrefab, spikeSpawnPositions[1]);
        yield return new WaitForSeconds(.5f);
        if (rigidbody.position.y > 1.0f)
            Instantiate(spikePrefab, spikeSpawnPositions[2]);
        yield return new WaitForSeconds(.5f);
        if (rigidbody.position.y > 1.0f)
            Instantiate(spikePrefab, spikeSpawnPositions[0]);
    }

    public void DecreaseHealth(float damage)
    {
        currentHealth -= Time.deltaTime * damage;

        if (currentHealth <= 0)
        {
            GameManager.instance.TriggerWin();
        }
        /**
        if (Input.GetKey(KeyCode.Q))
        {
        }
        **/
    }
}
