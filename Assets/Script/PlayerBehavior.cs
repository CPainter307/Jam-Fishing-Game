using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D playerBody;
    public GameObject boat;
    // The attached variable is for when you want the player to be flung into the air
    public bool attached;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is attached to the boat and not being flung into the air
        if (attached)
        {
            // Parent the position and rotation of the player to the boat
            player.transform.parent = boat.transform;
            // This needs to be done, otherwise, the player starts doing some FUNKY things
            playerBody.simulated = false;
        }
        // Player is being flung into the air
        else
        {   // Player now moves independently of the boat
            player.transform.parent = null;
            // Simulated player physics are on for the time being
            playerBody.simulated = true;
        }
    }
}
