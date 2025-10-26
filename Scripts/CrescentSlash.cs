using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrescentSlash : MonoBehaviour
{
    public int damage = 15;             // how much damage it deals
    public float lifetime = 3f;        // destroy after X seconds
    public GameObject hitEffect;       // optional effect when it hits something
    public float knockbackStrength = 5f; // how hard it pushes enemies

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // destroy automatically after lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable target = collision.GetComponent<Damageable>();
        if (target != null)
        {
            // ✅ Knockback direction is based on current velocity
            Vector2 knockback = rb.velocity.normalized * knockbackStrength;

            target.Hit(damage, knockback);
        }

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
