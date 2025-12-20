using System;
using UnityEngine;

public class StatsController : MonoBehaviour {
	#region Fields

	//* Instance
	public static StatsController Instance;

	//* Stats
	[SerializeField] private int  levelMax = 5;
	public                   bool levelPlaying;
	public                   int  level;

	public float health;
	public bool  LevelPause => (health <= 0) || !levelPlaying;

	//* Timers
	private float lowHpWarnTimer;

	#endregion

	#region Unity Functions

	private void Start() {
		health = 3;
		UIController.Instance.UpdateUI();
	}

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void FixedUpdate() {
		if (LevelPause || health > 1) return;
		lowHpWarnTimer -= Time.deltaTime;
		if (lowHpWarnTimer > 0) return;

		AudioManager.Instance.PlaySfx(AudioManager.AudioName.LowHealthWarn);

		lowHpWarnTimer = 2f;
	}

	#endregion

	#region Functions

	/// Go to the next level
	public void StartNextLevel() {
		if (level == levelMax) return;
		levelPlaying =  true;
		level        += 1;
	}

	/// <summary>
	/// Deal damage to the player
	/// </summary>
	/// <param name="damageAmount">HP to remove</param>
	/// <param name="sourcePosition">Position of the damage source</param>
	/// <param name="forceMult">1 by default, 0 to ignore knockback.</param>
	public void DealDamage(float damageAmount, Vector3 sourcePosition, float forceMult = 1f) {
		// If immune or level paused, do nothing
		if (PlayerController.Instance.isImmortal || LevelPause) {
			return;
		}

		// Deal damage and apply knockback
		health -= damageAmount;
		PlayerController.Instance.DamageKnockback(sourcePosition, forceMult);

		if (health <= 0) {
			TorchScript.Instance.SetIsLit(false);
			AudioManager.Instance.PlaySfx(sourcePosition.y < -10
				                              ? AudioManager.AudioName.PlayerFallDie
				                              : AudioManager.AudioName.PlayerDie);
		} else {
			// Apply immortality
			PlayerController.Instance.PlayerImmortal(1.25f);
			AudioManager.Instance.PlaySfx(AudioManager.AudioName.PlayerHurt);
		}

		// Update UI
		UIController.Instance.UpdateUI();
	}

	/// Register immunity for 0.2 seconds after a kill
	// To avoid bad problems with cramming or whatever might happen
	public void RegisterKill() => PlayerController.Instance.PlayerImmortal(0.2f);

	#endregion
}