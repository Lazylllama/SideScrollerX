using UnityEngine;

public class StatsController : MonoBehaviour {
	// Health
	public float health;
	public float maxHealth = 3;
	public bool  IsDead => health <= 0;

	// Ref
	private UIController     uiController;
	private TorchScript      torchScript;
	private PlayerController playerController;

	private void Start() {
		health = maxHealth;

		uiController     = FindAnyObjectByType<UIController>();
		torchScript      = FindAnyObjectByType<TorchScript>();
		playerController = FindAnyObjectByType<PlayerController>();

		uiController.UpdateUI();
	}

	public void DealDamage(float damageAmount, Vector3 sourcePosition) {
		// If immune or dead, do nothing
		if (playerController.isImmortal || IsDead) {
			return;
		}

		// Deal damage and apply knockback
		health -= damageAmount;
		playerController.DamageKnockback(sourcePosition);

		if (health <= 0) {
			torchScript.SetIsLit(false);
		} else {
			// Apply immortality
			playerController.PlayerImmortal(2f);
		}

		// Update UI
		uiController.UpdateUI();
	}

	public void RegisterKill() {
		// Register immunity for 2 seconds after a kill
		playerController.PlayerImmortal(2f);
	}
}