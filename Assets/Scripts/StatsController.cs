using UnityEngine;

public class StatsController : MonoBehaviour {
	#region Fields

	//* Health
	public float health;
	public float maxHealth = 3;
	public bool  IsDead => health <= 0;

	//* Ref
	private UIController     uiController;
	private TorchScript      torchScript;
	private PlayerController playerController;

	#endregion

	#region Unity Functions

	private void Start() {
		health = maxHealth;

		uiController     = FindAnyObjectByType<UIController>();
		torchScript      = FindAnyObjectByType<TorchScript>();
		playerController = FindAnyObjectByType<PlayerController>();

		uiController.UpdateUI();
	}

	#endregion

	#region Functions

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
			playerController.PlayerImmortal(1.25f);
		}

		// Update UI
		uiController.UpdateUI();
	}

	public void RegisterKill() {
		// Register immunity for 0.2 seconds after a kill
		// To avoid bad problems with cramming or whatever might happen
		playerController.PlayerImmortal(0.2f);
	}

	#endregion
}