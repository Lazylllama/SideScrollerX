using UnityEngine;

public class SoftSpotScript : MonoBehaviour {
	private EnemyController enemyController;
	private StatsController statsController;

	private void Start() {
		enemyController = GetComponentInParent<EnemyController>();
		statsController = FindAnyObjectByType<StatsController>();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (!collision.gameObject.CompareTag("Player")) return;

		Destroy(enemyController.gameObject);
		statsController.RegisterKill();
	}
}