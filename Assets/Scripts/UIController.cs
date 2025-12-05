using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	#region Fields

	// UI Elements
	[Header("UI Elements")]
	[SerializeField] private List<GameObject> uiHearts;
	[SerializeField] private GameObject uiKeys;
	[SerializeField] private GameObject uiCoins;

	// Ref
	[Header("Refs")]
	[SerializeField] private Animator hudAnimator;
	[SerializeField] private Animator        pauseMenuAnimator;
	[SerializeField] private Animator        cmAnimator;
	private                  StatsController statsController;
	private                  Inventory       inventory;
	private                  InputAction     pauseAction;

	// States
	public bool isPaused;

	// Prefabs
	[Header("Prefabs")]
	[SerializeField] private GameObject blueKeyUIPrefab;
	[SerializeField] private GameObject redKeyUIPrefab;
	[SerializeField] private GameObject goldKeyUIPrefab;

	// Hashes
	private static readonly int IsPauseMenuVisible = Animator.StringToHash("isPauseMenuVisible");
	private static readonly int IsHudVisible       = Animator.StringToHash("isHudVisible");
	private static readonly int IsZoomedOut        = Animator.StringToHash("isZoomedOut");

	#endregion

	#region Unity Functions

	private void Start() {
		// Find the scripts in the scene
		statsController = FindAnyObjectByType<StatsController>();
		inventory       = FindAnyObjectByType<Inventory>();

		pauseAction = InputSystem.actions.FindAction("Pause");

		hudAnimator.SetBool(IsHudVisible, true);
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
		uiCoins.GetComponentInChildren<TextMeshProUGUI>().text = inventory.coins.ToString() + "x";

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

	
	//* Pause Menu Functions
	public void ExitToMenu() {
		Debug.Log("sigma");
	}

	public void ResumeGame() {
		StartCoroutine(PauseRoutine(false));
	}

	#endregion

	#region Routines

	private IEnumerator PauseRoutine(bool pauseState) {
		isPaused = pauseState;

		cmAnimator.SetBool(IsZoomedOut, isPaused);
		hudAnimator.SetBool(IsHudVisible, !isPaused);
		pauseMenuAnimator.SetBool(IsPauseMenuVisible, isPaused);

		yield return new WaitForSeconds(1.25f);

		Time.timeScale = isPaused ? 0f : 1f;
	}

	#endregion
}