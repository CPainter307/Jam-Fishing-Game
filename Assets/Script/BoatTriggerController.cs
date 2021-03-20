using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTriggerController : MonoBehaviour
{
    public GameObject player;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // player.GetComponent<PlayerBehavior>().attached = true;
            print("Attaching");
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // player.GetComponent<PlayerBehavior>().attached = false;
            print("Dettaching");
        }
    }
}
