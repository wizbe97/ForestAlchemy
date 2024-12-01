using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;


    public bool isMoving = false;
    public bool canMove = true;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;


    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (canMove)
            MoveCharacter();

    }

    private void MoveCharacter()
    {
        if (isGrounded)
        {
            // On the ground, set velocity directly based on input
            if (moveInput.x != 0)
            {
                rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
            }
            else
            {
                // Decelerate smoothly to stop when no input
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, 0.1f), rb.velocity.y);
            }
        }
        else
        {
            // In the air, allow momentum to persist unless there's input
            if (moveInput.x != 0)
            {
                rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
            }
            // No deceleration while airborne; let the horizontal velocity persist
        }
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }



    private void CheckGrounded()
    {
        isGrounded = capsuleCollider.IsTouchingLayers(groundLayer);
    }

}