using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private bool hasTriggered = false;

    [Header("End Level After This Dialogue?")]
    public bool endsLevel = false;
    public string nextSceneName; // Optional

    public void TriggerDialogue()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            DialogueManager.Instance.StartDialogue(dialogue, HandleDialogueEnded); // Pass callback
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }

    private void HandleDialogueEnded()
    {
        if (!endsLevel) return;

        Debug.Log("Dialogue finished. Loading next level...");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
