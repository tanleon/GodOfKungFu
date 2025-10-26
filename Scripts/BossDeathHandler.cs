using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDeathHandler : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "NextLevel";
    [SerializeField] private float delayBeforeSceneLoad = 0f;

    private Damageable damageable;

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnBossDeath);
    }

    private void OnDisable()
    {
        damageable.damageableDeath.RemoveListener(OnBossDeath);
    }

    private void OnBossDeath()
    {
        // Run the delay on a persistent runner so timescale and destruction won't break it
        SceneDelayRunner.Run(nextSceneName, delayBeforeSceneLoad);
    }
}

/// <summary>
/// Spawns a small persistent object to delay scene loads using unscaled time.
/// </summary>
public class SceneDelayRunner : MonoBehaviour
{
    public static void Run(string sceneName, float delay)
    {
        var go = new GameObject("SceneDelayRunner");
        DontDestroyOnLoad(go);
        var runner = go.AddComponent<SceneDelayRunner>();
        runner.StartCoroutine(runner.LoadRoutine(sceneName, delay));
    }

    private IEnumerator LoadRoutine(string sceneName, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay); // unaffected by Time.timeScale

        SceneManager.LoadScene(sceneName);
        Destroy(gameObject); // cleanup the runner
    }
}
