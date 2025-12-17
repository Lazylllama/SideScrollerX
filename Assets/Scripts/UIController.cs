using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	#region Fields
	
	[Header("UI Elements")]
	[SerializeField] private List<GameObject> uiHearts;
	[SerializeField] private GameObject uiKeys;
	[SerializeField] private GameObject uiCoins;
	[SerializeField] private GameObject uiBomb;
	
	[Header("Refs")]
	[SerializeField] private Animator startMenuAnimator;
	[SerializeField] private Animator hudAnimator;
	[SerializeField] private Animator pauseMenuAnimator;
	[SerializeField] private Animator cmAnimator;

	private StatsController statsController;
	private Inventory       inventory;
	private InputAction     pauseAction;
	private TextMeshProUGUI uiCoinsText;

	// States
	public bool isPaused;

	[Header("Prefabs")]
	[SerializeField] private GameObject blueKeyUIPrefab;
	[SerializeField] private GameObject redKeyUIPrefab;
	[SerializeField] private GameObject goldKeyUIPrefab;

	// Hashes
	private static readonly int IsPauseMenuVisible = Animator.StringToHash("isPauseMenuVisible");
	private static readonly int IsHudVisible       = Animator.StringToHash("isHudVisible");
	private static readonly int IsZoomedOut        = Animator.StringToHash("isZoomedOut");
	private static readonly int HasMenuOffset      = Animator.StringToHash("hasMenuOffset");
	private static readonly int MenuFlyAway        = Animator.StringToHash("menuFlyAway");

	#endregion

	#region Unity Functions

	private void Start() {
		// Find the scripts in the scene
		statsController = FindAnyObjectByType<StatsController>();
		inventory       = FindAnyObjectByType<Inventory>();
		uiCoinsText     = uiCoins.GetComponentInChildren<TextMeshProUGUI>();

		pauseAction = InputSystem.actions.FindAction("Pause");

		hudAnimator.SetBool(IsHudVisible, false);
		pauseMenuAnimator.SetBool(IsPauseMenuVisible, false);
		cmAnimator.SetBool(IsZoomedOut, false);

		// Update the UI at the start
		UpdateUI();
	}

	private void FixedUpdate() {
		if (pauseAction.IsPressed()) {
			StartCoroutine(PauseRoutine(true));
		}
	}

	#endregion

	#region Functions

	public void UpdateUI() {
		var health    = statsController.health;
		var totalKeys = 0;

		// Health
		// For every UI heart, set fill to health minus index, fill can be over 1 without breaking
		for (var i = 0; i < uiHearts.Count; i++) {
			uiHearts[i].GetComponent<Image>().fillAmount = health - i;
		}

		// Inventory
		uiCoinsText.text = inventory.coins.ToString() + "x";
		uiBomb.SetActive(inventory.hasBomb);

		// TODO(@lazylllama): Optimize and add animations for adding/removing keys (fly in/out of view), i just have no clue how yet without making it even more stupid
		// Very inefficient to destroy and recreate every key every time, but works for now since the update frequency is low enough
		foreach (Transform child in uiKeys.transform) {
			Destroy(child.gameObject);
		}

		for (var i = 0; inventory.blueKeys > i; i++) {
			var key = Instantiate(blueKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}

		for (var i = 0; inventory.redKeys > i; i++) {
			var key = Instantiate(redKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}

		for (var i = 0; inventory.goldKeys > i; i++) {
			var key = Instantiate(goldKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}
	}

	public void StartLevel() {
		StartCoroutine(StartLevelRoutine());
	}

	public void ExitGame() {
		Application.Quit();
	}


	//* Pause Menu Functions
	public void ExitToMenu() {
		cmAnimator.SetBool(IsZoomedOut, isPaused);
	}

	public void ResumeGame() {
		StartCoroutine(PauseRoutine(false));
	}

	public void RestartLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	#endregion

	#region Routines

	private IEnumerator PauseRoutine(bool pauseState) {
		isPaused = pauseState;

		hudAnimator.SetBool(IsHudVisible, !isPaused);
		pauseMenuAnimator.SetBool(IsPauseMenuVisible, isPaused);

		// Time no worky when paused
		if (isPaused) yield return new WaitForSeconds(0.5f);

		Time.timeScale = isPaused ? 0f : 1f;
	}

	public IEnumerator NextLevel() {
		yield return new WaitForSeconds(0.2f);
		Time.timeScale = 0f;

		cmAnimator.SetBool(IsZoomedOut, true);
	}

	private IEnumerator StartLevelRoutine() {
		if (!statsController.LevelPause) yield break;

		cmAnimator.SetBool(HasMenuOffset, false);
		hudAnimator.SetBool(IsHudVisible, true);
		startMenuAnimator.SetTrigger(MenuFlyAway);
		
		Cursor.visible = false;
		
		yield return new WaitForSeconds(1.5f);

		statsController.StartNextLevel();
	}

	#endregion
}