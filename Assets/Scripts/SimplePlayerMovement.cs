using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Get input for movement (left/right)
        float moveInput = Input.GetAxis("Horizontal");

        // Move the player horizontally
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
}
