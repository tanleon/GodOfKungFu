using UnityEngine;

public class LeiShan : MonoBehaviour
{
    [Header("Refs")]
    public Projectile2D projectilePrefab;   // FireBall prefab 
    public Transform firePoint;

    [Header("Firing (Ranged)")]
    public float rangeCooldown = 2f;        // cooldown between fireballs
    public float projectileSpeed = 8f;

    [Header("Melee Attack")]
    [Tooltip("How much damage LeiShan deals with melee attack")]
    public int meleeDamage = 15;
    public Vector2 meleeKnockback = new Vector2(5f, 6f);
    public float meleeRange = 2f;
    public float meleeCooldown = 1.5f;
    public LayerMask meleeHitLayers;        // Should include Player layer

    [Header("Targeting")]
    public float detectionRadius = 8f;
    public LayerMask targetLayers;
    public LayerMask obstacleLayers;

    private static readonly int RangeAttackHash = Animator.StringToHash("rangeattack");
    private static readonly int MeleeAttackHash = Animator.StringToHash("attack");

    private float nextRangeTime;
    private float nextMeleeTime;
    private Transform target;
    private Collider2D myCollider;
    private Animator animator;

    private Vector2 pendingShotDir = Vector2.right;
    private bool hasPendingShot = false;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Detect player
        var hit = Physics2D.OverlapCircle(transform.position, detectionRadius, targetLayers);
        target = hit ? hit.transform : null;
        if (target == null) return;

        // Calculate direction + distance
        Vector2 dir = (target.position - firePoint.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        // Flip to face player
        if (dir.x != 0f)
        {
            var s = transform.localScale;
            s.x = Mathf.Sign(dir.x) * Mathf.Abs(s.x);
            transform.localScale = s;
        }

        // ✅ Decide attack type
        if (distance <= meleeRange && Time.time >= nextMeleeTime)
        {
            nextMeleeTime = Time.time + meleeCooldown;
            animator.SetTrigger(MeleeAttackHash);
        }
        else if (distance > meleeRange && Time.time >= nextRangeTime)
        {
            if (obstacleLayers.value == 0 ||
                !Physics2D.Raycast(firePoint.position, dir, detectionRadius, obstacleLayers))
            {
                pendingShotDir = dir;
                hasPendingShot = true;
                nextRangeTime = Time.time + rangeCooldown;

                animator.SetTrigger(RangeAttackHash);
            }
        }
    }

    // CALLED by animation event during melee attack
    public void Anim_MeleeHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meleeRange, meleeHitLayers);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Damageable>(out var dmg) && dmg.IsAlive)
            {
                Vector2 dir = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
                Vector2 deliveredKnockback = new Vector2(dir.x * Mathf.Abs(meleeKnockback.x), meleeKnockback.y);

                dmg.Hit(meleeDamage, deliveredKnockback);
            }
        }
    }

    // CALLED by animation event on the rangeattack clip
    public void Anim_Shoot()
    {
        if (!projectilePrefab || !firePoint) return;
        if (!hasPendingShot) return;

        var proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.speed = projectileSpeed;
        proj.gameObject.layer = LayerMask.NameToLayer("Default");
        proj.Launch(pendingShotDir, myCollider);

        hasPendingShot = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        if (firePoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)(pendingShotDir * 2f));
        }
    }
}
