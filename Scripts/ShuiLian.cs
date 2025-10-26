using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class ShuiLian : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkAcceleration = 20f;  
    public float walkSpeed = 15f;         
    public float runSpeed = 15f;         
    public float walkStopRate = 0.05f;

    [Header("Detection Zones")]
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    [Header("Jump Settings")]
    public float jumpForce = 13f;       
    public float jumpCooldown = 0.5f;     
    public float airHorizontalForce = 8f;
    public float jumpMinWait = 2f;     
    public float jumpMaxWait = 3f;

    private bool canJump = true;

    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Animator animator;
    private Damageable damageable;

    public enum WalkableDirection { Right, Left }
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    private bool isRunning = false;
    private float currentSpeed => isRunning ? runSpeed : walkSpeed;

    public WalkableDirection WalkDirection
    {
        get => _walkDirection;
        set
        {
            if (_walkDirection != value)
            {
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                walkDirectionVector = value == WalkableDirection.Right ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get => _hasTarget;
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);

    public float AttackCooldown
    {
        get => animator.GetFloat(AnimationStrings.attackCooldown);
        private set => animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        StartCoroutine(RandomRunRoutine());
        StartCoroutine(RandomJumpRoutine()); // Start jump routine
    }

    private void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        // Sync animator with vertical velocity
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        if (!damageable.LockVelocity)
        {
            if (CanMove && touchingDirections.IsGrounded)
            {
                float newVelocityX = Mathf.Clamp(
                    rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime),
                    -currentSpeed, currentSpeed);
                rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
            }
        }
    }

    private void FlipDirection()
    {
        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }

    private IEnumerator RandomRunRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f)); // shorter wait to run more often

            isRunning = true;
            animator.SetBool(AnimationStrings.isRunning, true);

            yield return new WaitForSeconds(Random.Range(3f, 5f)); // runs longer now

            isRunning = false;
            animator.SetBool(AnimationStrings.isRunning, false);
        }
    }

    // ---------------- Random Jump Logic ----------------
    private IEnumerator RandomJumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(jumpMinWait, jumpMaxWait));

            if (touchingDirections.IsGrounded && canJump)
            {
                StartCoroutine(JumpRoutine());
            }
        }
    }

    private IEnumerator JumpRoutine()
    {
        canJump = false;

        // Trigger jump animation
        animator.SetTrigger(AnimationStrings.jumpTrigger);

        // Apply upward force
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Optional attack in air
        if (HasTarget)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }

        // Add stronger horizontal movement while ascending
        while (rb.velocity.y > 0)
        {
            float randomDirection = Random.Range(-1f, 1f);
            rb.AddForce(new Vector2(randomDirection * airHorizontalForce, 0), ForceMode2D.Force);
            yield return null;
        }

        // Wait until grounded
        yield return new WaitUntil(() => touchingDirections.IsGrounded);

        // Reduced cooldown for more frequent jumps
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}
