using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	[Header("Refs")]
	private InputAction jumpAction, moveAction;
	private Vector2       moveInput;
	private BoxCollider2D playerEnemyCollider;
	private Rigidbody2D   rb;


	[Header("Player Stats")]
	[SerializeField] private float playerSpeed;
	[SerializeField] private float jumpForce;
	[SerializeField] private float extraJumpForce;
	[SerializeField] private float knockBackPower;
	public                   bool  isImmortal;

	[Header("Ground Check")]
	[SerializeField] private float groundCheckRadius;
	[SerializeField] private Transform groundCheckPosition;
	[SerializeField] private LayerMask groundLayer;
	private                  bool      isGrounded;
	private                  bool      inKnockback;


	[Header("Animation")]
	private SpriteRenderer playerSprite;
	private Animator playerAnimator;
	public  bool     isFacingRight;

	// Hashes
	private static readonly int IsRunning  = Animator.StringToHash("isRunning");
	private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
	private static readonly int TakeDamage = Animator.StringToHash("takeDamage");

	// Timer
	private float jumpTimer;
	private float jumpingTimer;


	// Begin Functions
	private void Start() {
		rb         = GetComponent<Rigidbody2D>();
		moveAction = InputSystem.actions.FindAction("Move");
		jumpAction = InputSystem.actions.FindAction("Jump");

		playerSprite        = GetComponentInChildren<SpriteRenderer>();
		playerAnimator      = GetComponentInChildren<Animator>();
		playerEnemyCollider = GetComponentInChildren<BoxCollider2D>();

		isFacingRight = true;
	}

	private void Update() {
		GroundCheck();
		MoveCheck();
	}

	private void FixedUpdate() {
		PerformMove();
	}

	void MoveCheck() {
		moveInput = moveAction.ReadValue<Vector2>();

		if (inKnockback) return;

		jumpTimer += Time.deltaTime;

		if (moveInput.x < 0) {
			isFacingRight      = false;
			playerSprite.flipX = true;
		}
		else if (moveInput.x > 0) {
			isFacingRight      = true;
			playerSprite.flipX = false;
		}

		if (jumpAction.IsPressed()) {
			PerformJump(jumpAction.WasPressedThisFrame());
		}
	}

	private void PerformMove() {
		if (inKnockback) return;

		playerAnimator.SetBool(IsRunning, moveInput.x != 0);

		rb.linearVelocityX = moveInput.x * playerSpeed;
	}

	private void PerformJump(bool thisFrame) {
		jumpingTimer += Time.deltaTime;
		if (jumpTimer < 0.1f) return;

		switch (isGrounded) {
			case false when !thisFrame && jumpingTimer < 0.6:
				rb.AddForce(Vector2.up * extraJumpForce, ForceMode2D.Impulse);
				break;
			case true:
				jumpingTimer = 0;

				rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

				jumpTimer = 0;
				break;
		}
	}

	public void PlayerImmortal(float duration) {
		StartCoroutine((ImmortalityRoutine(duration)));
	}

	public void DamageKnockback(Vector3 enemyPosition) {
		playerAnimator.SetTrigger(TakeDamage);
		StartCoroutine(KnockbackRoutine(enemyPosition));
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
		Gizmos.color = isGrounded ? Color.green : Color.yellow;
		Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
	}

	private IEnumerator KnockbackRoutine(Vector3 enemyPosition) {
		inKnockback = true;
		var knockBackDirection = (transform.position - enemyPosition).normalized;

		rb.AddForce(knockBackDirection * knockBackPower, ForceMode2D.Impulse);

		yield return new WaitForSeconds(0.2f);
		yield return new WaitUntil(() => isGrounded);

		inKnockback = false;

		yield return null;
	}

	private IEnumerator ImmortalityRoutine(float duration) {
		isImmortal = true;
		playerEnemyCollider.gameObject.SetActive(false);

		yield return new WaitForSeconds(duration);

		playerEnemyCollider.gameObject.SetActive(false);
		isImmortal = false;
	}
}