using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class StatsController : MonoBehaviour {
	// Health
	public float health;
	public float maxHealth = 3;

	// Ref
	private UIController     uiController;
	private TorchScript      torchScript;
	private PlayerController playerController;

	// Timer
	private float immunityTimer = 0f;

	private void Start() {
		health = maxHealth;

		uiController     = FindAnyObjectByType<UIController>();
		torchScript      = FindAnyObjectByType<TorchScript>();
		playerController = FindAnyObjectByType<PlayerController>();

		uiController.UpdateUI();
	}

	public void DealDamage(float damageAmount, Vector3 sourcePosition) {
		// If immune, do nothing
		if (immunityTimer > 0f) {
			return;
		}

		// Deal damage and apply knockback
		health -= damageAmount;
		playerController.DamageKnockback(sourcePosition);

		// Update UI
		uiController.UpdateUI();
	}

	public void RegisterKill() {
		// Register immunity for 0.2 seconds after a kill
		StartCoroutine(ImmunityFrames(0.2f));
	}

	private IEnumerator ImmunityFrames(float immunityDuration) {
		immunityTimer = 0f;

		while (immunityTimer < immunityDuration) {
			immunityTimer += Time.deltaTime;
			yield return null;
		}

		immunityTimer = 0f;
	}
}