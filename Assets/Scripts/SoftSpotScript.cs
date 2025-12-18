using System.Collections;
using UnityEngine;

public class SoftSpotScript : MonoBehaviour {
	#region Fields

	//* Hashes
	private static readonly int EnemyDie = Animator.StringToHash("enemyDie");

	//* Refs
	private EnemyController  enemyController;
	private StatsController  statsController;
	private PlayerController playerController;
	private Animator         animator;
	private BoxCollider2D    softSpotCollider;

	#endregion

	#region Unity Functions

	private void Start() {
		enemyController  = GetComponentInParent<EnemyController>();
		animator         = GetComponentInParent<Animator>();
		statsController  = FindAnyObjectByType<StatsController>();
		playerController = FindAnyObjectByType<PlayerController>();
		softSpotCollider = GetComponent<BoxCollider2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (!collision.gameObject.CompareTag("Player")) return;
		if (playerController.isImmortal) return;

		StartCoroutine(EnemyDieRoutine());
		softSpotCollider.enabled = false;
		statsController.RegisterKill();
	}

	#endregion

	#region Functions

	private IEnumerator EnemyDieRoutine() {
		enemyController.EnemyDie();
		animator.SetTrigger(EnemyDie);
		AudioManager.Instance.PlaySfx(enemyController.isFlyingEnemy
			                              ? AudioManager.AudioName.BeeDie
			                              : AudioManager.AudioName.BlobDie);
		yield return new WaitForSeconds(1.5f);
		Destroy(enemyController.gameObject);
	}

	#endregion
}