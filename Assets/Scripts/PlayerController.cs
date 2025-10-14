using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    InputAction jumpAction;
    InputAction moveAction;
    Vector2 moveInput;

    [Header("Player Stats")]
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpForce;

    [Header("Ground Check")]
    [SerializeField] bool isGrounded;
    [SerializeField] float groundCheckRadius;
    [SerializeField] Transform groundCheckPosition;
    [SerializeField] LayerMask groundLayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update() {
        GroundCheck();
        MoveCheck();
    }

    void FixedUpdate() {
        PerformMove();
    }

    void MoveCheck() {
        moveInput = moveAction.ReadValue<Vector2>();

        if (jumpAction.IsPressed() && isGrounded) {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    void PerformMove() {
        rb.linearVelocityX = moveInput.x * playerSpeed;
    }

    void GroundCheck() {
        Collider2D hit = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundLayer);

        if (hit != null) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }
    }

    void OnDrawGizmos() {
        if (isGrounded) { 
            Gizmos.color = Color.green;
        } else {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
    }
}
