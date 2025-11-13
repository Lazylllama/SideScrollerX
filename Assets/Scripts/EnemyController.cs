using UnityEngine;

public class EnemyController : MonoBehaviour {
    Rigidbody2D rb;

    public bool lookingRight = true;
    public bool isGrounded;

    [Header("Settings")]
    [SerializeField] float checkDistance;
    [SerializeField] float speed;
    [SerializeField] float damageAmount;

    [Header("REFS")]
    [SerializeField] Transform lookCheck;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    StatsController statsController;
    Animator animator;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        statsController = FindAnyObjectByType<StatsController>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            statsController.DealDamage(damageAmount, transform.position);
        }    
    }

    void Update() {
        RaycastHit2D lookHit = Physics2D.Raycast(lookCheck.position, Vector2.down, checkDistance, groundLayer);
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);

        if (lookHit.collider == null && groundHit.collider != null) {
            lookingRight = !lookingRight;
            transform.rotation *= Quaternion.Euler(0, 180f, 0);
        }
    }

    void FixedUpdate() {
        MoveEnemy();
    }

    void MoveEnemy() {
        rb.linearVelocityX = transform.right.x * speed;

        if (rb.linearVelocityX == 0) {
            animator.SetBool("isWalking", false);
        } else {
            animator.SetBool("isWalking", true);
        }
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(groundCheck.position, checkDistance);
        Gizmos.DrawWireSphere(lookCheck.position, checkDistance);
    }
}
