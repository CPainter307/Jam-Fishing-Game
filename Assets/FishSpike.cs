using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpike : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D coll;

    private float angle;

    public Vector2 forceToApply = new Vector2(-5f, 20f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();

        ShootSpike();
    }

    public void ShootSpike()
    {
        rb.AddForce(forceToApply + new Vector2(0, Random.Range(-30, 50)), ForceMode2D.Impulse);
    }

    private void FixedUpdate() {
        Vector2 v = rb.velocity;
        angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
