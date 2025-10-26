using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    public float smallJumpImpulse = 5f;

    private Vector2 moveInput;
    private TouchingDirections touchingDirections;
    private Damageable damageable;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isRunningInputHeld = false;

    private enum AbilityLevel { Level1, Level2, Level3, Level4 }
    private AbilityLevel abilityLevel = AbilityLevel.Level1;

    public float CurrentMoveSpeed
    {
        get
        {
            if (!CanMove || !IsMoving || touchingDirections.IsOnWall)
                return 0f;

            float baseSpeed = touchingDirections.IsGrounded
                ? (isRunningInputHeld ? runSpeed : walkSpeed)
                : airWalkSpeed;

            return baseSpeed;
        }
    }

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving
    {
        get => _isMoving;
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField] private bool _isRunning = false;
    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get => _isFacingRight;
        private set
        {
            if (_isFacingRight != value)
                transform.localScale *= new Vector2(-1, 1);

            _isFacingRight = value;
        }
    }

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);
    public bool IsAlive => animator.GetBool(AnimationStrings.isAlive);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        SetAbilityLevelFromScene();
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (DialogueManager.Instance.isDialogueActive)
        {
            moveInput = Vector2.zero;
            IsMoving = false;
            IsRunning = false;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;

        if (IsAlive)
        {
            IsRunning = isRunningInputHeld && IsMoving;
            SetFacingDirection(moveInput);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (abilityLevel < AbilityLevel.Level2) return;

        if (context.started)
        {
            // only start running if player is grounded
            if (touchingDirections != null && touchingDirections.IsGrounded)
                isRunningInputHeld = true;
        }
        else if (context.canceled)
        {
            isRunningInputHeld = false;
        }

        // ensure running is disabled while airborne
        if (touchingDirections != null && !touchingDirections.IsGrounded)
        {
            isRunningInputHeld = false;
        }

        // Running only when input held, moving, and grounded
        IsRunning = isRunningInputHeld && IsMoving && (touchingDirections != null && touchingDirections.IsGrounded);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (abilityLevel < AbilityLevel.Level3) return;
        if (DialogueManager.Instance.isDialogueActive) return;

        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnSmallJump(InputAction.CallbackContext context)
    {
        if (DialogueManager.Instance.isDialogueActive) return;

        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.smallJumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, smallJumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (DialogueManager.Instance.isDialogueActive) return;

        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (abilityLevel < AbilityLevel.Level4) return;
        if (DialogueManager.Instance.isDialogueActive) return;

        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    // Determines which ability level to allow based on scene name (e.g. "Level 2", "Boss 3 Lesson", etc.)
    private void SetAbilityLevelFromScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int levelNum = ExtractLevelNumber(sceneName);

        switch (levelNum)
        {
            case 1:
                abilityLevel = AbilityLevel.Level1;
                break;
            case 2:
                abilityLevel = AbilityLevel.Level2;
                break;
            case 3:
                abilityLevel = AbilityLevel.Level3;
                break;
            case 4:
                abilityLevel = AbilityLevel.Level4;
                break;
            default:
                abilityLevel = AbilityLevel.Level1;
                break;
        }
    }

    // Extracts the first number found in the scene name
    private int ExtractLevelNumber(string sceneName)
    {
        foreach (var word in sceneName.Split(' '))
        {
            if (int.TryParse(word, out int number))
            {
                return number;
            }
        }

        return 1; // Default fallback
    }
}
