using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float airControlFactor = 0.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 22f; // Increased for snappier jump
    [SerializeField] private float coyoteTime = 0.2f; // Time player can still jump after leaving ground
    [SerializeField] private float jumpBufferTime = 0.2f; // Time within which jump input is buffered
    [SerializeField] private float jumpCutMultiplier = 0.5f; // Reduces upward velocity when jump is released early
    [SerializeField] private float fallMultiplier = 3f; // Increases gravity when falling
    [SerializeField] private float maxFallSpeed = -20f; // Terminal velocity

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    [Header("Abilities")]
    [SerializeField] private bool doubleJumpUnlocked = true; // Double jump enabled by default
    [SerializeField] private int maxJumpCount = 2; // Maximum number of jumps allowed

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    private bool isGrounded;
    private float lastGroundedTime; // Tracks the last time player was on the ground
    private float lastJumpTime; // Tracks the last time jump was pressed
    private int currentJumpCount; // Tracks the number of jumps
    private bool jumpButtonHeld; // Whether the jump button is currently held
    private bool jumpRequested = false; // Tracks if jump input was requested

    private bool gameStarted = false; // Ensures no jump at the start of the game
    public bool isMoving = false;
    public bool canMove = true;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(0.1f); // Wait for input initialization
        gameStarted = true;
    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.started += OnJump;
        inputActions.Player.Jump.canceled += OnJumpRelease;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.started -= OnJump;
        inputActions.Player.Jump.canceled -= OnJumpRelease;
        inputActions.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput.x != 0;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && gameStarted)
        {
            jumpRequested = true; // Signal that a jump is requested
            lastJumpTime = Time.time;
            jumpButtonHeld = true;
        }
    }

    public void OnJumpRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            jumpButtonHeld = false;
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            }
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleJump();

        if (canMove)
        {
            MoveCharacter();
        }

        ApplyGravityModifiers();
        LimitUpwardSpeed(); // Ensure upward speed stays consistent
        LimitFallSpeed();
    }

    private void CheckGrounded()
    {
        Vector2 origin = new Vector2(transform.position.x, capsuleCollider.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            lastGroundedTime = Time.time; // Update for coyote time
            currentJumpCount = 0; // Reset jump count when grounded
        }
    }

    private void MoveCharacter()
    {
        float targetSpeed = moveInput.x * moveSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;
        float accelerationRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        // Apply air control factor if not grounded
        accelerationRate *= isGrounded ? 1f : airControlFactor;

        float movement = speedDifference * accelerationRate;
        rb.AddForce(new Vector2(movement, 0));
    }

    private void HandleJump()
    {
        if (jumpRequested)
        {
            if (currentJumpCount == 0 && (isGrounded || Time.time - lastGroundedTime <= coyoteTime))
            {
                Jump();
                currentJumpCount = 1;
            }
            else if (currentJumpCount < maxJumpCount && doubleJumpUnlocked)
            {
                Jump();
                currentJumpCount++;
            }

            jumpRequested = false; // Consume jump request
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        lastJumpTime = Time.time;
    }

    private void ApplyGravityModifiers()
    {
        if (rb.velocity.y < 0)
        {
            // Falling
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !jumpButtonHeld)
        {
            // Early jump release
            rb.gravityScale = fallMultiplier; // Higher gravity for cut short jump
        }
        else if (rb.velocity.y > 0 && jumpButtonHeld)
        {
            // Held jump
            rb.gravityScale = 1.2f; // Adjusted for faster upward motion
        }
        else
        {
            // Default upward movement
            rb.gravityScale = 1f;
        }
    }

    private void LimitUpwardSpeed()
    {
        if (rb.velocity.y > jumpForce)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void LimitFallSpeed()
    {
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }
}
