using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHealthBar : MonoBehaviour
{

    void Update()
    {
        FishBehavior fish = GameObject.FindObjectOfType<FishBehavior>();
        if (fish != null)
            transform.localScale = new Vector3(fish.currentHealth / 100f, transform.localScale.y);
    }
}
