using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA;
    public Transform pointB;
    public float walkAcceleration = 3f;
    public float maxSpeed = 2f;
    public float walkStopRate = 0.05f;

    [Header("Damage Settings")]
    public int damage = 10;

    private Vector3 target;
    private Vector3 lastPosition;
    private Vector3 moveDirection;
    private float fixedY;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    private int originalLayer;
    private Coroutine ignorePlayerCoroutine;

    public enum WalkableDirection { Right, Left }
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        originalLayer = gameObject.layer;
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("⚠️ MovingObstacle needs both PointA and PointB assigned!");
            enabled = false;
            return;
        }
        target = pointB.position;
        lastPosition = transform.position;
        fixedY = transform.position.y;
        WalkDirection = WalkableDirection.Right;
    }

    void Update()
    {
        // Automated movement between pointA and pointB
        if (pointA == null || pointB == null) return;

        Vector3 targetPos = new Vector3(target.x, fixedY, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, maxSpeed * Time.deltaTime);

        moveDirection = (transform.position - lastPosition).normalized;
        lastPosition = transform.position;

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
            FlipDirection();
        }

        // Wall detection and direction flip (like Knight)
        if (touchingDirections != null && touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }
    }

    private void FixedUpdate()
    {
        // Prevent movement through walls
        if (touchingDirections != null && touchingDirections.IsOnWall)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        // Knight-like velocity logic (but automated, not player/AI driven)
        if (!damageable.LockVelocity)
        {
            rb.velocity = new Vector2(
                Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed),
                rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
            WalkDirection = WalkableDirection.Left;
        else
            WalkDirection = WalkableDirection.Right;
    }


    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    // Add this to deliver damage/knockback on contact with player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamagePlayerOnContact(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayerOnContact(collision.collider);
    }

    private void TryDamagePlayerOnContact(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            float playerSpeed = Mathf.Abs(playerRb.velocity.x);
            float passThroughSpeed = 5f;

            if (playerSpeed >= passThroughSpeed)
            {
                // Temporarily move slime to IgnorePlayer layer
                if (ignorePlayerCoroutine == null)
                    ignorePlayerCoroutine = StartCoroutine(TemporarilyIgnorePlayer());

                return;
            }
        }

        Damageable dmg = collision.GetComponent<Damageable>();
        if (dmg != null)
        {
            // Knockback direction based on movement
            float movementX = moveDirection.x;
            if (Mathf.Abs(movementX) < 0.01f)
            {
                float towardsTarget = target.x - transform.position.x;
                movementX = Mathf.Abs(towardsTarget) > 0.01f ? Mathf.Sign(towardsTarget) : 1f;
            }
            float signX = Mathf.Sign(movementX);

            Vector2 finalKnockback = new Vector2(
                3f * signX,
                2f
            );

            dmg.Hit(damage, finalKnockback);
        }
    }

    private IEnumerator TemporarilyIgnorePlayer()
    {
        // Change to IgnorePlayer layer (make sure this layer exists and is set up in Physics2D settings)
        gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
        yield return new WaitForSeconds(5f);
        gameObject.layer = originalLayer;
        ignorePlayerCoroutine = null;
    }
}
