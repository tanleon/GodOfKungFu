using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    public float typingSpeed = 0.02f;
    public Animator animator;

    private Queue<DialogueLine> lines;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool inputLocked = false;

    public bool isDialogueActive = false;

    private PlayerController player;
    private DialogueLine currentLine;

    private System.Action onDialogueComplete; // NEW

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();

        if (animator != null)
        {
            animator.Play("hide");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!isDialogueActive || inputLocked) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputLocked = true;
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                DisplayNextDialogueLine();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue, System.Action onComplete = null) // MODIFIED
    {
        isDialogueActive = true;
        onDialogueComplete = onComplete; // NEW

        if (player != null)
            player.enabled = false;

        if (animator != null)
        {
            animator.Play("show");
        }
        else
        {
            gameObject.SetActive(true);
        }

        lines.Clear();
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        isTyping = true;

        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        inputLocked = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueArea.text = currentLine.line;
        isTyping = false;
        inputLocked = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        if (animator != null)
        {
            animator.Play("hide");
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (player != null)
            player.enabled = true;

        onDialogueComplete?.Invoke(); // Only call the one trigger that requested this dialogue
        onDialogueComplete = null;
    }
}
