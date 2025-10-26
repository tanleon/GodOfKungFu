using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    public int damage = 10;
    public Vector2 knockback = new Vector2(4f, 6f);
    public float speed = 8f;
    public float lifeTime = 5f;
    public LayerMask hitLayers;

    private Vector2 dir = Vector2.right;
    private float t;
    private Rigidbody2D rb;
    private Collider2D col;

    
    private Animator anim;
    private bool exploding;

    private Collider2D owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();                 

        if (rb) { rb.isKinematic = true; rb.gravityScale = 0f; }
        if (col) col.isTrigger = true;
    }

    public void Launch(Vector2 direction, Collider2D ownerCollider = null)
    {
        dir = direction.normalized;
        owner = ownerCollider;
        t = 0f;
        exploding = false;

        if (owner && col) Physics2D.IgnoreCollision(col, owner, true);
        if (rb) rb.velocity = dir * speed;
        else transform.right = dir;
    }

    private void Update()
    {
        if (exploding) return;                            

        t += Time.deltaTime;
        if (!rb) transform.Translate(dir * speed * Time.deltaTime, Space.World);
        if (t >= lifeTime) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == owner || other.isTrigger || exploding) return;
        if (hitLayers.value != 0 && (hitLayers & (1 << other.gameObject.layer)) == 0) return;

        if (other.TryGetComponent<Damageable>(out var dmg) && dmg.IsAlive)
        {
            Vector2 delivered = new Vector2(Mathf.Sign(dir.x) * Mathf.Abs(knockback.x), knockback.y);
            dmg.Hit(damage, delivered);
        }

        // play explode anim if available, otherwise just destroy
        if (anim != null)
        {
            exploding = true;
            if (rb) rb.velocity = Vector2.zero;
            if (col) col.enabled = false;
            anim.SetTrigger("explode");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // CALLED BY FB_Explode clip event on last frame
    public void Anim_Despawn()                            
    {
        Destroy(gameObject);
    }
}
