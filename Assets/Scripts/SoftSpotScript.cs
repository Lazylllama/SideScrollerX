using UnityEngine;

public class SoftSpotScript : MonoBehaviour
{
    EnemyController enemyController;
    StatsController statsController;

    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        statsController = FindAnyObjectByType<StatsController>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Destroy(enemyController.gameObject);
            statsController.RegisterKill();
        }
    }
}
