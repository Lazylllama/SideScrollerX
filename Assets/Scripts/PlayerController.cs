using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	#region Fields

	//* Instance
	public static PlayerController Instance;

	//* Hashes
	private static readonly int IsRunning  = Animator.StringToHash("isRunning");
	private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
	private static readonly int TakeDamage = Animator.StringToHash("takeDamage");

	//* Refs
	private InputAction   jumpAction, moveAction, secondaryInteractAction;
	private BoxCollider2D playerEnemyCollider;
	private Rigidbody2D   playerRigidbody;

	[Header("Player Stats")]
	public bool isImmortal;
	[SerializeField] private float jumpForce, extraJumpForce, extraJumpTime, knockBackPower, coyoteTime, playerSpeed;

	[Header("Ground Check")]
	[SerializeField] private Transform groundCheckPosition;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float     groundCheckRadius;
	private                  bool      isGrounded, inKnockback, isCoyoteTimeActive, hasCoyoteJumped;

	[Header("Animation")]
	public bool isFacingRight;
	private Animator                 playerSpriteAnimator, playerAnimator;
	private SpriteRenderer           playerSprite;
	private CinemachineImpulseSource playerImpulseSource;

	[Header("Prefabs")]
	[SerializeField] private GameObject bombPrefab;

	//* Timer
	private float jumpCooldown;
	private float jumpPowerTimer;

	//* States
	private Vector2 moveInput;

	#endregion

	#region Unity Functions

	private void Start() {
		playerRigidbody = GetComponent<Rigidbody2D>();
		moveAction = InputSystem.actions.FindAction("Move"); // Primarily A & D and arrow keys left & right
		jumpAction = InputSystem.actions.FindAction("Jump"); // Primarily Space & W and arrow keys up & down
		secondaryInteractAction = InputSystem.actions.FindAction("SecondaryInteract"); // Primarily Q key

		var playerAnimators = GetComponentsInChildren<Animator>();

		playerAnimator      = GetComponent<Animator>();
		playerImpulseSource = GetComponent<CinemachineImpulseSource>();
		playerSprite        = GetComponentInChildren<SpriteRenderer>();
		playerEnemyCollider = GetComponentInChildren<BoxCollider2D>();

		// TODO(@lazylllama): Clean this up later
		//? Find the other animator and set it as the player sprite animator
		foreach (var animator in playerAnimators) {
			if (animator == playerAnimator) continue;
			playerSpriteAnimator = animator;
			break;
		}

		isFacingRight = true;
	}

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void Update() {
		GroundCheck();

		jumpCooldown += Time.deltaTime;
	}

	private void FixedUpdate() {
		MoveCheck();
		PerformMove();

		ActionCheck();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (!other.gameObject.CompareTag("PlayerDeathBorder")) return;
		StatsController.Instance.DealDamage(1f, transform.position);
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.PlayerFallDie);
		ResetPlayerPosition();
	}

	#endregion

	#region Movement Functions

	private void MoveCheck() {
		if (StatsController.Instance.LevelPause) return;
		moveInput = moveAction.ReadValue<Vector2>();

		if (inKnockback) return;

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

		if (jumpAction.IsPressed()) PerformJump();
	}

	private void PerformMove() {
		if (inKnockback) return;

		if (StatsController.Instance.LevelPause) {
			playerSpriteAnimator.SetBool(IsRunning, false);
			playerRigidbody.linearVelocityX = 0;
			return;
		}

		playerSpriteAnimator.SetBool(IsRunning, moveInput.x != 0);

		playerRigidbody.linearVelocityX = moveInput.x * playerSpeed;
	}

	// TODO(@lazylllama): Rework function to make it more optimized and simple
	private void PerformJump() {
		switch (isGrounded) {
			case false when hasCoyoteJumped && jumpPowerTimer > 0f:
				playerRigidbody.AddForce(Vector2.up * (jumpForce * Time.fixedDeltaTime * extraJumpForce),
				                         ForceMode2D.Impulse);
				jumpPowerTimer -= Time.fixedDeltaTime;
				break;
			case false when !hasCoyoteJumped && isCoyoteTimeActive:
			case true:
				if (jumpCooldown < 0.3f) return;
				playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
				hasCoyoteJumped = true;
				ResetJump();
				break;
		}

		return;

		void ResetJump() {
			// Only reset the jump if the player is grounded
			if (!isGrounded) return;
			jumpCooldown   = 0f;
			jumpPowerTimer = extraJumpTime;
		}
	}

	private void GroundCheck() {
		if (jumpCooldown < 0.1f) return;
		var hit = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundLayer);

		if (hit) {
			isGrounded = true;
			playerSpriteAnimator.SetBool(IsGrounded, true);
			hasCoyoteJumped = false;
		} else {
			isGrounded = false;
			playerSpriteAnimator.SetBool(IsGrounded, false);
			if (!isCoyoteTimeActive && !hasCoyoteJumped) StartCoroutine(CoyoteTimeRoutine());
		}
	}

	private void ActionCheck() {
		if (!secondaryInteractAction.IsPressed() ||
		    !Inventory.Instance.hasBomb                    ||
		    StatsController.Instance.LevelPause) return;

		Instantiate(bombPrefab, transform.position, Quaternion.identity);
		Inventory.Instance.UseBomb();
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.ThrowBomb);
	}

	#endregion

	#region Coroutines

	/// <summary>
	/// Makes the player immortal for a certain duration
	/// </summary>
	/// <param name="duration">Immortality duration in seconds</param>
	public void PlayerImmortal(float duration) {
		StartCoroutine((ImmortalityRoutine(duration)));
	}

	/// <summary>
	/// Damages the player and applies knockback from an enemy position
	/// </summary>
	/// <param name="enemyPosition">Vector3 position of the damage source</param>
	/// <param name="forceMult">Intensiveness of the knockback.</param>
	public void DamageKnockback(Vector3 enemyPosition, float forceMult = 1f) {
		playerSpriteAnimator.SetTrigger(TakeDamage);
		playerAnimator.SetTrigger(TakeDamage);
		StartCoroutine(KnockbackRoutine(enemyPosition, forceMult));
	}

	private IEnumerator KnockbackRoutine(Vector3 enemyPosition, float forceMult = 1f) {
		inKnockback = true;

		//? Camera shake
		//* Only shake the camera if the forceMult > 0
		playerImpulseSource.GenerateImpulseWithForce(forceMult > 0 ? 0.6f : 0f);

		//? Push the player in the opposite way of the enemy direction
		var knockBackDirection = (transform.position - enemyPosition).normalized;
		playerRigidbody.AddForce(knockBackDirection * (knockBackPower * forceMult), ForceMode2D.Impulse);

		//? Wait until the player has lifted *before* waiting for touchdown
		yield return new WaitForSeconds(0.2f);
		yield return new WaitUntil(() => isGrounded);

		inKnockback = false;
	}

	private IEnumerator ImmortalityRoutine(float duration) {
		isImmortal = true;

		//? Disable collision with enemies during immortality
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

	#endregion

	#region Functions

	/// <summary>
	/// Sets the player to be back at Vector3.Zero
	/// </summary>
	public void ResetPlayerPosition() {
		transform.position = Vector3.zero;
	}

	#endregion

	#region Other

	private void OnDrawGizmos() {
		Gizmos.color = isGrounded ? Color.green : Color.yellow;
		Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
	}

	#endregion
}