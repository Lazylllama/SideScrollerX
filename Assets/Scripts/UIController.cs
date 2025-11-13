using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // UI Elements
    [Header("UI Elements")]
    [SerializeField] List<GameObject> uiHearts;
    [SerializeField] GameObject uiKeys;
    [SerializeField] GameObject uiCoins;

    // Ref
    StatsController statsController;
    Inventory inventory;

    // Prefabs
    [Header("Prefabs")]
    [SerializeField] GameObject blueKeyUIPrefab;
    [SerializeField] GameObject redKeyUIPrefab;

    void Start()
    {
        // Find the scripts in the scene
        statsController = FindAnyObjectByType<StatsController>();
        inventory = FindAnyObjectByType<Inventory>();

        // Update the UI at start
        UpdateUI();
    }

    public void UpdateUI() {
        float health = statsController.health;
        int totalKeys = 0;
        
        // Health
        // Jag har ingen aning hur jag lyckades med detta men wow det funkar
        for (int i = 0; i < uiHearts.Count; i++) {
           uiHearts[i].GetComponent<Image>().fillAmount = health - i;
        }

        // Inventory
        uiCoins.GetComponentInChildren<TextMeshProUGUI>().text = inventory.coins.ToString() + "x";

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
