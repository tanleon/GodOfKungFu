using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthSlider;
    public TMP_Text healthBarText;

    [Header("Settings")]
    [Tooltip("Tag of the boss character to track")]
    [SerializeField] private string bossTag = "Boss";

    private Damageable bossDamageable;

    private void Awake()
    {
        GameObject boss = GameObject.FindGameObjectWithTag(bossTag);

        if (boss == null)
        {
            Debug.LogError($"No GameObject with tag '{bossTag}' found!");
            return;
        }

        bossDamageable = boss.GetComponent<Damageable>();
        if (bossDamageable == null)
        {
            Debug.LogError($"No Damageable component found on GameObject with tag '{bossTag}'!");
        }
    }

    private void Start()
    {
        if (bossDamageable != null)
        {
            healthSlider.value = CalculateSliderPercentage(bossDamageable.Health, bossDamageable.MaxHealth);
            healthBarText.text = $"HP {bossDamageable.Health} / {bossDamageable.MaxHealth}";
        }
    }

    private void OnEnable()
    {
        if (bossDamageable != null)
            bossDamageable.healthChanged.AddListener(OnHealthChanged);
    }

    private void OnDisable()
    {
        if (bossDamageable != null)
            bossDamageable.healthChanged.RemoveListener(OnHealthChanged);
    }

    private float CalculateSliderPercentage(float current, float max)
    {
        return current / max;
    }

    private void OnHealthChanged(int newHealth, int maxHealth)
    {
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = $"HP {newHealth} / {maxHealth}";
    }
}
