using System;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	[Header("States")]
	public bool lookingRight = true;
	public bool isDead;

	[Header("Settings")]
	[SerializeField] float checkDistance;
	[SerializeField] float speed;
	[SerializeField] float damageAmount;

	[Header("REFS")]
	[SerializeField] private Transform lookCheck;
	[SerializeField] private Transform  groundCheck;
	[SerializeField] private LayerMask  groundLayer;
	[SerializeField] private GameObject pointL, pointR;

	private Rigidbody2D     rb;
	private StatsController statsController;
	private Animator        animator;

	// Hashes
	private static readonly int IsWalking = Animator.StringToHash("isWalking");

	private void Start() {
		rb              = GetComponent<Rigidbody2D>();
		statsController = FindAnyObjectByType<StatsController>();
		animator        = GetComponent<Animator>();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player") && !isDead) {
			statsController.DealDamage(damageAmount, transform.position);
		}
	}

	private void Update() {
		var xPos      = gameObject.transform.position.x;
		var lookHit   = Physics2D.Raycast(lookCheck.position,   Vector2.down, checkDistance, groundLayer);
		var groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);

		if (!lookHit.collider && groundHit.collider && !isDead) {
			lookingRight       =  !lookingRight;
			transform.rotation *= Quaternion.Euler(0, 180f, 0);
		} else if (
			(xPos > pointR.transform.position.x && lookingRight) ||
			xPos < pointL.transform.position.x && !lookingRight) {
			lookingRight       =  !lookingRight;
			transform.rotation *= Quaternion.Euler(0, 180f, 0);
		}
	}

	private void FixedUpdate() {
		MoveEnemy();
	}

	private void MoveEnemy() {
		// Move enemy no matter dead or alive to create better death animation :)
		rb.linearVelocityX = transform.right.x * speed;

		if (isDead) return;
		if (rb.linearVelocityX == 0) {
			animator.SetBool(IsWalking, false);
		} else {
			animator.SetBool(IsWalking, true);
		}
	}

	public void EnemyDie() {
		isDead = true;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(groundCheck.position, checkDistance);
		Gizmos.DrawWireSphere(lookCheck.position,   checkDistance);
	}
}