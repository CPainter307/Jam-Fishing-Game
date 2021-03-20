using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpike : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D coll;

    public Vector2 forceToApply = new Vector2(-5f, 20f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();

        ShootSpike();
    }

    public void ShootSpike()
    {
        rb.AddForce(forceToApply, ForceMode2D.Impulse);
    }
}
