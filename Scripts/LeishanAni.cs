using UnityEngine;

[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Animator))]
public class LeishanAni : MonoBehaviour
{
    [Header("Animator Parameters (must exist in this Animator)")]
    [SerializeField] private string hurtTrigger = "hurt";
    [SerializeField] private string dieTrigger  = "die";

    private Damageable damageable;
    private Animator animator;

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (damageable == null) return;
        damageable.damageableHit.AddListener(OnHit);
        damageable.damageableDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        if (damageable == null) return;
        damageable.damageableHit.RemoveListener(OnHit);
        damageable.damageableDeath.RemoveListener(OnDeath);
    }

    private void OnHit(int dmg, Vector2 kb)
    {
        // Only play HURT if still alive after this hit
        if (damageable != null && damageable.IsAlive)
            TrySetTrigger(hurtTrigger);
    }

    private void OnDeath()
    {
        TrySetTrigger(dieTrigger);
    }

    private void TrySetTrigger(string param)
    {
        if (!animator) return;

        // Safety: only trigger if this Animator actually has that trigger
        for (int i = 0; i < animator.parameterCount; i++)
        {
            var p = animator.GetParameter(i);
            if (p.name == param && p.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param); // clear just in case
                animator.SetTrigger(param);
                return;
            }
        }

        Debug.LogWarning($"LeishanAni: Trigger '{param}' not found on Animator of {name}.");
    }
}
