using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0f;
        currentTimer = nextAttackTime;
        timerID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseNextAttackTime();
        RunTimer();
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
                    currentTime = 0f;
                    currentTimer = nextAttackTime;
                    timerID = 0;
                    break;
            }
        }
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
        attackID = Random.Range(0, 3);
    }

    void DecreaseNextAttackTime()
    {
        /**
         * STUB
         * Here, have nextAttackTime get closer to minNextAttackTime
         * based on how much health the fish has left. I think a threshhold
         * system, where the number decriments based on percentage of the
         * fish's health (like 100%, 75%, 50%, 25%, etc.).
         **/
    }

    void FishAttack01()
    {
        Debug.Log("Fish attacking...");
    }

    void FishAttack02()
    {
        Debug.Log("Fish attacking...");
    }

    void FishAttack03()
    {
        Debug.Log("Fish attacking...");
    }
}
