using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHealthBar : MonoBehaviour
{

    void Update()
    {
        transform.localScale = new Vector3(GameObject.FindObjectOfType<FishBehavior>().currentHealth / 100f, transform.localScale.y);
    }
}
