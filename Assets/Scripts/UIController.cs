using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	#region Fields

	//* Instance
	public static UIController Instance;

	[Header("UI Elements")]
	[SerializeField] private List<GameObject> uiHearts;
	[SerializeField] private TextMeshProUGUI uiCoins;
	[SerializeField] private TextMeshProUGUI uiScore;
	[SerializeField] private GameObject      uiKeys;
	[SerializeField] private GameObject      uiBomb;

	[Header("Refs")]
	[SerializeField] private Animator startMenuAnimator;
	[SerializeField] private Animator hudAnimator;
	[SerializeField] private Animator pauseMenuAnimator;
	[SerializeField] private Animator winScreenAnimator;
	[SerializeField] private Animator cmAnimator;

	private InputAction pauseAction;

	//* States
	public bool isPaused;

	[Header("Prefabs")]
	[SerializeField] private GameObject blueKeyUIPrefab;
	[SerializeField] private GameObject redKeyUIPrefab;
	[SerializeField] private GameObject goldKeyUIPrefab;

	//* Hashes
	private static readonly int IsPauseMenuVisible = Animator.StringToHash("isPauseMenuVisible");
	private static readonly int IsHudVisible       = Animator.StringToHash("isHudVisible");
	private static readonly int IsZoomedOut        = Animator.StringToHash("isZoomedOut");
	private static readonly int HasMenuOffset      = Animator.StringToHash("hasMenuOffset");
	private static readonly int MenuFlyAway        = Animator.StringToHash("menuFlyAway");
	private static readonly int HasWon             = Animator.StringToHash("hasWon");

	#endregion

	#region Unity Functions

	private void Start() {
		// Ensure time is normal and the cursor is visible at the start
		Time.timeScale = 1f;
		Cursor.visible = true;

		// Set Refs
		pauseAction = InputSystem.actions.FindAction("Pause");

		hudAnimator.SetBool(IsHudVisible, false);
		pauseMenuAnimator.SetBool(IsPauseMenuVisible, false);
		cmAnimator.SetBool(IsZoomedOut, false);
		winScreenAnimator.SetBool(HasWon, false);

		// Update the UI at the start
		UpdateUI();
	}

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void FixedUpdate() {
		if (pauseAction.IsPressed()) {
			StartCoroutine(PauseRoutine(true));
		}
	}

	#endregion

	#region Functions

	/// Refreshes the UI with the current stats
	public void UpdateUI() {
		var health    = StatsController.Instance.health;
		var totalKeys = 0;

		// Health
		// For every UI heart, set fill to health minus index. fill can be over 1 without breaking
		for (var i = 0; i < uiHearts.Count; i++) {
			uiHearts[i].GetComponent<Image>().fillAmount = health - i;
		}

		// Inventory
		uiCoins.text = Inventory.Instance.coins + "x";
		uiScore.text = "SCORE: "                + Inventory.Instance.coins;
		uiBomb.SetActive(Inventory.Instance.hasBomb);

		// TODO(@lazylllama): Optimize and add animations for adding/removing keys (fly in/out of view), i just have no clue how yet without making it even more stupid
		// Very inefficient to destroy and recreate every key every time, but works for now since the update frequency is low enough
		foreach (Transform child in uiKeys.transform) {
			Destroy(child.gameObject);
		}

		for (var i = 0; Inventory.Instance.blueKeys > i; i++) {
			var key = Instantiate(blueKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}

		for (var i = 0; Inventory.Instance.redKeys > i; i++) {
			var key = Instantiate(redKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}

		for (var i = 0; Inventory.Instance.goldKeys > i; i++) {
			var key = Instantiate(goldKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}
	}

	/// Starts the level
	public void StartLevel() {
		StartCoroutine(StartLevelRoutine());
	}

	/// Exits the game completely
	public void ExitGame() {
		Application.Quit();
	}


	//* Pause Menu Functions
	/// Exits to the main menu
	public void ExitToMenu() {
		cmAnimator.SetBool(IsZoomedOut, isPaused);
	}

	/// Resumes the game from the paused state
	public void ResumeGame() {
		StartCoroutine(PauseRoutine(false));
	}

	/// Restarts the game
	public void RestartGame() {
		StartCoroutine(PauseRoutine(false));
		PlayerController.Instance.ResetPlayerPosition();
		StatsController.Instance.ResetGame();
		SceneManager.LoadScene("Scenes/Level1");
		UpdateUI();
	}

	/// Plays a sound effect for clicking on UI elements
	public void ClickSound() {
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.CursorPress);
	}

	#endregion

	#region Routines

	private IEnumerator PauseRoutine(bool pauseState) {
		isPaused = pauseState;

		AudioManager.Instance.PlaySfx(pauseState
			                              ? AudioManager.AudioName.PauseOpen
			                              : AudioManager.AudioName.PauseClose);
		hudAnimator.SetBool(IsHudVisible, !isPaused);
		pauseMenuAnimator.SetBool(IsPauseMenuVisible, isPaused);

		// Time no worky when paused
		if (isPaused) yield return new WaitForSeconds(0.5f);

		Cursor.visible = isPaused;
		Time.timeScale = isPaused ? 0f : 1f;
	}

	public IEnumerator NextLevel() {
		// Stop Character Movement
		StatsController.Instance.SetLevelPlaying(false);

		// If its the last level do sum else
		Debug.Log(StatsController.Instance.level);
		if (StatsController.Instance.IsLastLevel()) {
			hudAnimator.SetBool(IsHudVisible, false);
			yield return new WaitForSeconds(2f);
			winScreenAnimator.SetBool(HasWon, true);
			Cursor.visible = true;
			yield break;
		}

		// Log the next level
		StatsController.Instance.LogNextLevel();

		// Zoom out the camera
		cmAnimator.SetBool(IsZoomedOut, true);

		// Wait for the zoom out animation to finish to avoid snappy movement
		yield return new WaitForSeconds(3f);
		PlayerController.Instance.ResetPlayerPosition();
		yield return new WaitForSeconds(2f);
		cmAnimator.SetBool(IsZoomedOut, false);

		// Allow character movement again and load the next level
		// (must finish routine before loading a new scene)
		StatsController.Instance.SetLevelPlaying(true);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	private IEnumerator StartLevelRoutine() {
		if (!StatsController.Instance.LevelPause) yield break;

		cmAnimator.SetBool(HasMenuOffset, false);
		hudAnimator.SetBool(IsHudVisible, true);
		startMenuAnimator.SetTrigger(MenuFlyAway);

		Cursor.visible = false;

		yield return new WaitForSeconds(1.5f);

		StatsController.Instance.LogNextLevel();
	}

	#endregion
}