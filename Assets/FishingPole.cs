using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingPole : MonoBehaviour
{
    public PlayerBehavior player;
    public Transform lineAttachPoint;

    private void Update()
    {
        if (player.dead)
        {
            transform.parent = null;
            GetComponent<Rigidbody2D>().simulated = false;
        }

        GetComponent<Animator>().SetBool("Reeling", Input.GetKey(KeyCode.Space));
    }

}
