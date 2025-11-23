using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	// UI Elements
	[Header("UI Elements")]
	[SerializeField] private List<GameObject> uiHearts;
	[SerializeField] private GameObject uiKeys;
	[SerializeField] private GameObject uiCoins;

	// Ref
	private StatsController statsController;
	private Inventory       inventory;

	// Prefabs
	[Header("Prefabs")]
	[SerializeField] private GameObject blueKeyUIPrefab;
	[SerializeField] private GameObject redKeyUIPrefab;

	private void Start() {
		// Find the scripts in the scene
		statsController = FindAnyObjectByType<StatsController>();
		inventory       = FindAnyObjectByType<Inventory>();

		// Update the UI at start
		UpdateUI();
	}

	public void UpdateUI() {
		var health    = statsController.health;
		var totalKeys = 0;

		// Health
		// For every UI heart, set fill to health minus index, fill can be over 1 without breaking
		for (int i = 0; i < uiHearts.Count; i++) {
			uiHearts[i].GetComponent<Image>().fillAmount = health - i;
		}

		// Inventory
		uiCoins.GetComponentInChildren<TextMeshProUGUI>().text = inventory.coins.ToString() + "x";

		// TODO(@lazylllama): Optimize and add animations for adding/removing keys (fly in/out of view), i just have no clue how yet without making it even more stupid
		// Very inefficient to destroy and recreate every key every time, but works for now since the update frequency is low enough
		foreach (Transform child in uiKeys.transform) {
			Destroy(child.gameObject);
		}

		for (int i = 0; inventory.blueKeys > i; i++) {
			GameObject key = Instantiate(blueKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}

		for (int i = 0; inventory.redKeys > i; i++) {
			GameObject key = Instantiate(redKeyUIPrefab, uiKeys.transform);

			key.GetComponent<RectTransform>().anchoredPosition -= new Vector2(40 * totalKeys, 0);

			totalKeys++;
		}
	}
}