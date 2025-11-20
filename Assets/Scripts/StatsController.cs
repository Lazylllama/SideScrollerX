using UnityEngine;

public class StatsController : MonoBehaviour {
	// Health
	public float health;
	public float maxHealth = 3;

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
		// If immune, do nothing
		if (playerController.isImmortal) {
			return;
		}

		// Deal damage and apply knockback
		health -= damageAmount;
		playerController.DamageKnockback(sourcePosition);

		if (health <= 0) {
			torchScript.SetIsLit(false);
		}
		else {
			// Apply immortality
			playerController.PlayerImmortal(3f);
		}

		// Update UI
		uiController.UpdateUI();
	}

	public void RegisterKill() {
		// Register immunity for 5 seconds after a kill
		playerController.PlayerImmortal(2f);
	}
}