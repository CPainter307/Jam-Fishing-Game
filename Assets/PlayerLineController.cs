using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineController : MonoBehaviour
{
    public static PlayerLineController instance;
    [HideInInspector] public CircleCollider2D reelCircle;
    public FishingPole pole;
    [HideInInspector] public RopeBridge ropeBridge;

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
        if (Vector3.Distance(pole.transform.position, reelCircle.transform.position) > reelCircle.radius)
        {
            ropeBridge.scaleFactor = -1;
            
        }
    }

    private void LateUpdate()
    {

    }
}
