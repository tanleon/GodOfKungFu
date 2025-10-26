using System.Collections;
using UnityEngine;
using TMPro;

public class FlashTextAtStart : MonoBehaviour
{
    public TextMeshProUGUI flashText;
    public float fadeDuration = 0.5f;
    public float visibleDuration = 2.0f;

    void Start()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        flashText.gameObject.SetActive(true);
        flashText.alpha = 0f;

        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            flashText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        flashText.alpha = 1f;

        // Hold visible
        yield return new WaitForSeconds(visibleDuration);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            flashText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        flashText.alpha = 0f;
        flashText.gameObject.SetActive(false);
    }
}
