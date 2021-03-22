using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.scale(this.gameObject, new Vector3(1.3f, 1.3f, 1.3f), .3f).setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
