using UnityEngine;

public class EnemyController : MonoBehaviour {
    Rigidbody2D rb;
    [SerializeField] float speed;

    bool lookingRight = true;
    bool isGrounded;

    [SerializeField] float checkDistance;
    [SerializeField] Transform lookCheck;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
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
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(groundCheck.position, checkDistance);
        Gizmos.DrawWireSphere(lookCheck.position, checkDistance);
    }
}
