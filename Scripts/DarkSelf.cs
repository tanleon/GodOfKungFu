using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class DarkSelf : MonoBehaviour
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

    [Header("Chi Blast Settings")]
    public GameObject chiBlastPrefab;
    public Transform chiBlastSpawnPoint;
    public float chiBlastCooldown = 5f;

    private bool canJump = true;
    private bool canChiBlast = true;

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
        animator.SetBool(AnimationStrings.canMove, true);  // Force enable movement
        StartCoroutine(RandomRunRoutine());
        StartCoroutine(RandomJumpRoutine());
        StartCoroutine(RandomChiBlastRoutine());
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
        Debug.Log($"DarkSelf IsGrounded: {touchingDirections.IsGrounded}, yVelocity: {rb.velocity.y}");

        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        // Force yVelocity to zero when grounded
        if (touchingDirections.IsGrounded)
        {
            animator.SetFloat(AnimationStrings.yVelocity, 0f);
            Debug.Log("DarkSelf landed: yVelocity forced to 0");
        }
        else
        {
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
        }

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
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            isRunning = true;
            animator.SetBool(AnimationStrings.isRunning, true);

            yield return new WaitForSeconds(Random.Range(3f, 5f));

            isRunning = false;
            animator.SetBool(AnimationStrings.isRunning, false);
        }
    }

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

        animator.SetTrigger(AnimationStrings.jumpTrigger);

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (HasTarget)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }

        while (rb.velocity.y > 0)
        {
            float randomDirection = Random.Range(-1f, 1f);
            rb.AddForce(new Vector2(randomDirection * airHorizontalForce, 0), ForceMode2D.Force);
            yield return null;
        }

        yield return new WaitUntil(() => touchingDirections.IsGrounded);
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private IEnumerator RandomChiBlastRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));

            if (canChiBlast && HasTarget && CanMove)
            {
                StartCoroutine(ChiBlastRoutine());
            }
        }
    }

    private IEnumerator ChiBlastRoutine()
    {
        canChiBlast = false;

        animator.SetTrigger(AnimationStrings.rangedAttackTrigger);

        yield return new WaitForSeconds(0.5f); // Sync with animation

        SpawnChiBlast();

        yield return new WaitForSeconds(chiBlastCooldown);
        canChiBlast = true;
    }

    private void SpawnChiBlast()
    {
        if (chiBlastPrefab != null && chiBlastSpawnPoint != null)
        {
            GameObject chiBlast = Instantiate(chiBlastPrefab, chiBlastSpawnPoint.position, Quaternion.identity);

            // Flip projectile based on facing direction
            chiBlast.transform.localScale = new Vector3(IsFacingRight ? 1 : -1, 1, 1);

            Projectile projectile = chiBlast.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.moveSpeed = new Vector2(Mathf.Abs(projectile.moveSpeed.x) * (IsFacingRight ? 1 : -1), projectile.moveSpeed.y);
            }
        }
    }

    // Manage facing direction based on walk direction
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get => _isFacingRight;
        private set
        {
            if (_isFacingRight != value)
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);

            _isFacingRight = value;
        }
    }
}

