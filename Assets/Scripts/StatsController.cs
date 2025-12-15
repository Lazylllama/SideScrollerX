using UnityEngine;

public class StatsController : MonoBehaviour {
	#region Fields

	//* Stats
	public  bool levelPlaying;
	public  int  level;
	private int  levelMax = 5;

	public float health;
	public float maxHealth = 3;
	public bool  LevelPause => (health <= 0) || (level >= levelMax) || !levelPlaying;

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

	public void StartNextLevel() {
		levelPlaying =  true;
		level        += 1;
	}

	public void DealDamage(float damageAmount, Vector3 sourcePosition, float forceMult = 1f) {
		// If immune or level paused, do nothing
		if (playerController.isImmortal || LevelPause) {
			return;
		}

		// Deal damage and apply knockback
		health -= damageAmount;
		playerController.DamageKnockback(sourcePosition, forceMult);

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