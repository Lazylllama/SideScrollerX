using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	// Hashes
	private static readonly int IsRunning  = Animator.StringToHash("isRunning");
	private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
	private static readonly int TakeDamage = Animator.StringToHash("takeDamage");

	// Refs
	private InputAction     jumpAction, moveAction;
	private Vector2         moveInput;
	private BoxCollider2D   playerEnemyCollider;
	private Rigidbody2D     playerRigidbody;
	private StatsController statsController;


	[Header("Player Stats")]
	[SerializeField] private float playerSpeed;
	[SerializeField] private float jumpForce;
	[SerializeField] private float extraJumpForce;
	[SerializeField] private float knockBackPower;
	[SerializeField] private float coyoteTime;
	public                   bool  isImmortal;

	[Header("Ground Check")]
	[SerializeField] private Transform groundCheckPosition;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float     groundCheckRadius;
	public                   bool      isGrounded;
	public                   bool      inKnockback;
	public                   bool      isCoyoteTimeActive;
	public                   bool      hasCoyoteJumped;

	[Header("Animation")]
	public bool isFacingRight;
	private Animator       playerAnimator;
	private SpriteRenderer playerSprite;

	// Timer
	private float jumpTimer;
	private float jumpingTimer;


	// Begin Functions
	private void Start() {
		playerRigidbody = GetComponent<Rigidbody2D>();
		moveAction      = InputSystem.actions.FindAction("Move"); // Primarily A & D and arrow keys left & right
		jumpAction      = InputSystem.actions.FindAction("Jump"); // Primarily Space & W and arrow keys up & down

		playerSprite        = GetComponentInChildren<SpriteRenderer>();
		playerAnimator      = GetComponentInChildren<Animator>();
		playerEnemyCollider = GetComponentInChildren<BoxCollider2D>();
		statsController     = FindAnyObjectByType<StatsController>();

		isFacingRight = true;
	}

	private void Update() {
		GroundCheck();
	}

	private void FixedUpdate() {
		MoveCheck();
		PerformMove();
	}

	private void MoveCheck() {
		moveInput = moveAction.ReadValue<Vector2>();

		if (inKnockback) return;

		jumpTimer += Time.deltaTime;


		switch (moveInput.x) {
			// If move input is negative, the player is looking left.
			case < 0:
				isFacingRight      = false;
				playerSprite.flipX = true;
				break;
			case > 0:
				isFacingRight      = true;
				playerSprite.flipX = false;
				break;
		}

		if (jumpAction.IsPressed()) {
			PerformJump(jumpAction.WasPressedThisFrame());
		}
	}

	private void PerformMove() {
		if (inKnockback) return;

		if (statsController.IsDead) {
		}

		playerAnimator.SetBool(IsRunning, moveInput.x != 0);

		playerRigidbody.linearVelocityX = moveInput.x * playerSpeed;
	}

	// If jumped in past 0.1 seconds => return
	// If grounded => Let player jump and reset timers
	// If not grounded, held space and jumped within 0.6 sec => add extra jump force
	// TODO(@lazylllama): Rework function to make it more optimized and simple
	private void PerformJump(bool thisFrame) {
		jumpingTimer += Time.deltaTime;
		if (jumpTimer < 0.1f) return;
		
		switch (isGrounded) {
			case false when !thisFrame && jumpingTimer < 0.3f:
				playerRigidbody.AddForce(Vector2.up * extraJumpForce, ForceMode2D.Impulse);
				break;
			case false when thisFrame && isCoyoteTimeActive && !hasCoyoteJumped:
			case true:
				playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
				ResetJump();
				break;
		}

		return;

		void ResetJump() {
			jumpingTimer    = 0;
			jumpTimer       = 0;
			hasCoyoteJumped = true;
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
			hasCoyoteJumped = false;
		} else {
			isGrounded = false;
			playerAnimator.SetBool(IsGrounded, false);
			if (!isCoyoteTimeActive && !hasCoyoteJumped) StartCoroutine(CoyoteTimeRoutine());
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = isGrounded ? Color.green : Color.yellow;
		Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
	}

	private IEnumerator KnockbackRoutine(Vector3 enemyPosition) {
		inKnockback = true;

		// Push the player in the opposite way of the enemy direction
		var knockBackDirection = (transform.position - enemyPosition).normalized;
		playerRigidbody.AddForce(knockBackDirection * knockBackPower, ForceMode2D.Impulse);

		// Wait until the player has lifted *before* waiting for touchdown
		yield return new WaitForSeconds(0.2f);
		yield return new WaitUntil(() => isGrounded);

		inKnockback = false;
	}

	private IEnumerator ImmortalityRoutine(float duration) {
		isImmortal = true;

		// Disable collision with enemies during immortality
		playerEnemyCollider.gameObject.SetActive(false);

		yield return new WaitForSeconds(duration);

		playerEnemyCollider.gameObject.SetActive(true);

		isImmortal = false;
	}

	private IEnumerator CoyoteTimeRoutine() {
		isCoyoteTimeActive = true;
		hasCoyoteJumped    = false;
		yield return new WaitForSeconds(coyoteTime);
		isCoyoteTimeActive = false;
	}
}