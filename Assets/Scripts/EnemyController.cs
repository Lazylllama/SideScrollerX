using System;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [Header("States")]
    public bool lookingRight = true;
    public bool isGrounded;

    [Header("Settings")]
    [SerializeField] float checkDistance;
    [SerializeField] float speed;
    [SerializeField] float damageAmount;

    [Header("REFS")]
    [SerializeField] private Transform lookCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private StatsController statsController;
    private Animator animator;
    
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        statsController = FindAnyObjectByType<StatsController>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            statsController.DealDamage(damageAmount, transform.position);
        }    
    }

    private void Update() {
        var lookHit = Physics2D.Raycast(lookCheck.position, Vector2.down, checkDistance, groundLayer);
        var groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);

        if (!lookHit.collider && groundHit.collider) {
            lookingRight = !lookingRight;
            transform.rotation *= Quaternion.Euler(0, 180f, 0);
        }
    }

    private void FixedUpdate() {
        MoveEnemy();
    }

    private void MoveEnemy() {
        rb.linearVelocityX = transform.right.x * speed;

        if (rb.linearVelocityX == 0) {
            animator.SetBool(IsWalking, false);
        } else {
            animator.SetBool(IsWalking, true);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(groundCheck.position, checkDistance);
        Gizmos.DrawWireSphere(lookCheck.position, checkDistance);
    }
}
