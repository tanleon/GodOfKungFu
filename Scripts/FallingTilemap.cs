using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D), typeof(TilemapCollider2D))]
public class FallingTilemap : MonoBehaviour
{
    public enum FallMode { MakeRigidbodyDynamic, DisableCollider }
    [Header("Behavior")]
    public FallMode fallMode = FallMode.MakeRigidbodyDynamic;
    public float delayBeforeFall = 0.5f;
    public float destroyAfter = 3f; // 0 = never destroy

    private Rigidbody2D rb;
    private TilemapCollider2D col;
    private bool triggered;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<TilemapCollider2D>();

        // Start solid & supported
        rb.bodyType = RigidbodyType2D.Static;

        // Helpful log on setup
        Debug.Log($"[FallingTilemap] Ready. IsTrigger={col.isTrigger}, Mode={fallMode}", this);
    }

    // --- Use this when TilemapCollider2D.IsTrigger == false
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!col || col.isTrigger) return;       // not our mode
        if (triggered) return;
        if (!collision.collider.CompareTag("Player")) return;

        Debug.Log("[FallingTilemap] Collision with Player → will fall", this);
        StartCoroutine(FallRoutine());
    }

    // --- Use this when TilemapCollider2D.IsTrigger == true
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!col || !col.isTrigger) return;      // not our mode
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("[FallingTilemap] Trigger with Player → will fall", this);
        StartCoroutine(FallRoutine());
    }

    private System.Collections.IEnumerator FallRoutine()
    {
        triggered = true;
        yield return new WaitForSeconds(delayBeforeFall);

        switch (fallMode)
        {
            case FallMode.MakeRigidbodyDynamic:
                rb.bodyType = RigidbodyType2D.Dynamic; // platform drops with gravity
                break;

            case FallMode.DisableCollider:
                col.enabled = false;                   // player falls through
                break;
        }

        if (destroyAfter > 0f) Destroy(gameObject, destroyAfter);
    }
}
