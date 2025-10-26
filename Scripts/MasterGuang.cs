using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class MasterGuang : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    [Tooltip("Check this if Master Guang should start flipped (facing left).")]
    public bool startFlipped = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();

        if (startFlipped)
        {
            FlipCharacter();
        }
    }

    void Update()
    {
        // Master Guang does nothing but stay idle
        animator.SetBool(AnimationStrings.hasTarget, false);
        animator.SetBool(AnimationStrings.canMove, false);
    }

    private void FixedUpdate()
    {
        // Stop all movement
        if (!damageable.LockVelocity)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        // Optional: apply knockback if needed
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    private void FlipCharacter()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Flip horizontally
        transform.localScale = localScale;
    }
}
