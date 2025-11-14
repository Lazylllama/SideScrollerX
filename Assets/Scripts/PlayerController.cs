using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	[Header("Refs")]
	private InputAction jumpAction, moveAction;
	private Vector2     moveInput;
	private Rigidbody2D rb;


	[Header("Player Stats"), SerializeField]
	private float playerSpeed,
	              jumpForce,
	              knockBackPower;
	public bool isFacingRight;

	[Header("Ground Check")]
	[SerializeField] private float groundCheckRadius;
	[SerializeField] private Transform groundCheckPosition;
	[SerializeField] private LayerMask groundLayer;
	private                  bool      isGrounded;


	[Header("Animation")]
	private SpriteRenderer playerSprite;
	private Animator playerAnimator;

	// Hashes
	private static readonly int IsRunning  = Animator.StringToHash("isRunning");
	private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

	// Timer
	private float jumpTimer;


	// Begin Functions
	private void Start() {
		rb         = GetComponent<Rigidbody2D>();
		moveAction = InputSystem.actions.FindAction("Move");
		jumpAction = InputSystem.actions.FindAction("Jump");

		playerSprite   = GetComponentInChildren<SpriteRenderer>();
		playerAnimator = GetComponentInChildren<Animator>();

		isFacingRight = true;
	}

	private void Update() {
		GroundCheck();
		MoveCheck();
	}

	private void FixedUpdate() {
		PerformMove();
	}

	private void MoveCheck() {
		jumpTimer += Time.deltaTime;

		moveInput = moveAction.ReadValue<Vector2>();

		if (moveInput.x < 0) {
			isFacingRight      = false;
			playerSprite.flipX = true;
		}
		else if (moveInput.x > 0) {
			isFacingRight      = true;
			playerSprite.flipX = false;
		}

		if (jumpAction.IsPressed()) {
			PerformJump();
		}
	}

	private void PerformMove() {
		if (moveInput.x != 0) {
			playerAnimator.SetBool(IsRunning, true);
		}
		else {
			playerAnimator.SetBool(IsRunning, false);
		}

		rb.linearVelocityX = moveInput.x * playerSpeed;
	}

	private void PerformJump() {
		if (!isGrounded) return;
		if (jumpTimer < 0.1) return; //Avoid double jumps from a single press

		rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

		jumpTimer = 0;
	}

	public void DamageKnockback(Vector3 enemyPosition) {
		var knockBackDirection = (enemyPosition - transform.position).normalized;
		rb.AddForce(knockBackDirection * knockBackPower, ForceMode2D.Impulse);
	}

	private void GroundCheck() {
		var hit = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundLayer);

		if (hit) {
			isGrounded = true;
			playerAnimator.SetBool(IsGrounded, true);
		}
		else {
			isGrounded = false;
			playerAnimator.SetBool(IsGrounded, false);
		}
	}

	private void OnDrawGizmos() {
		if (isGrounded) {
			Gizmos.color = Color.green;
		}
		else {
			Gizmos.color = Color.yellow;
		}

		Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
	}
}