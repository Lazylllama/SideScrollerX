using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    InputAction jumpAction;
    InputAction moveAction;
    Vector2 moveInput;

    float jumpTimer;

    [Header("Player Stats")]
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float knockBackPower;
    public bool isFacingRight;

    [Header("Ground Check")]
    [SerializeField] bool isGrounded;
    [SerializeField] float groundCheckRadius;
    [SerializeField] Transform groundCheckPosition;
    [SerializeField] LayerMask groundLayer;

    [Header("Animation")]
    SpriteRenderer playerSprite;
    Animator playerAnimator;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        playerSprite = GetComponentInChildren<SpriteRenderer>();
        playerAnimator = GetComponentInChildren<Animator>();

        isFacingRight = true;
    }

    void Update() {
        GroundCheck();
        MoveCheck();
    }

    void FixedUpdate() {
        PerformMove();
    }

    void MoveCheck() {
        jumpTimer += Time.deltaTime;

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput.x < 0) {
            isFacingRight = false;
            playerSprite.flipX = true;
        } else if (moveInput.x > 0) {
            isFacingRight = true;
            playerSprite.flipX = false;
        }

        if (jumpAction.IsPressed()) {
            PerformJump();
        }
    }
    
    void PerformMove() {
        if (moveInput.x != 0) {
            playerAnimator.SetBool("isRunning", true);
        } else {
            playerAnimator.SetBool("isRunning", false);
        }

        rb.linearVelocityX = moveInput.x * playerSpeed;
    }

    void PerformJump() {
        if (!isGrounded) return;
        if (jumpTimer < 0.1) return; //Avoid double jumps from a single press

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpTimer = 0;  
    }

    public void DamageKnockback(Vector3 enemyPosition) {
        Debug.Log("jävla skräp skit helvetes förbannade skit forces");
    }

    void GroundCheck() {
        Collider2D hit = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundLayer);

        if (hit != null) {
            isGrounded = true;
            playerAnimator.SetBool("isGrounded", true);
        } else { 
            isGrounded = false;
            playerAnimator.SetBool("isGrounded", false);
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
