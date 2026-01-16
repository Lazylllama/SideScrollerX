using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsController : MonoBehaviour {
	#region Fields

	//* Instance
	public static StatsController Instance;

	//* Stats
	[SerializeField] private int  levelMax = 3;
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
	public void LogNextLevel() {
		Debug.Log("Next Level!");
		if (level == levelMax) return;
		if (level == 0) levelPlaying =  true;
		level        += 1;
	}

	/// <summary>
	/// Set the level playing boolean (enables/disables movement)
	/// </summary>
	/// <param name="value"></param>
	public void SetLevelPlaying(bool value) => levelPlaying = value;

	/// <summary>
	/// Check if the player is on the last level
	/// </summary>
	/// <returns>bool</returns>
	public bool IsLastLevel() => level == levelMax;

	/// <summary>
	/// Reset everything back to default pretty much
	/// </summary>
	public void ResetGame() {
		level        = 1;
		health       = 3;
		levelPlaying = true;

		UIController.Instance.UpdateUI();
		TorchScript.Instance.SetIsLit(true);
		Inventory.Instance.ResetInventory();
		SceneManager.LoadScene("Scenes/Level1");
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
			StartCoroutine(PlayerDieRoutine());
			if (sourcePosition.y > -10) AudioManager.Instance.PlaySfx(AudioManager.AudioName.PlayerDie);
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

	#region Routines

	private IEnumerator PlayerDieRoutine() {
		TorchScript.Instance.SetIsLit(false);
		yield return new WaitForSeconds(4f);
		ResetGame();
	}

	#endregion
}