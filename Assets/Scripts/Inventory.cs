using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Stats")]
    public int blueKeys;
    public int redKeys;
    public int coins;

    // Ref
    private UIController uiController;

    private void Start() {
        uiController = FindAnyObjectByType<UIController>();
    }

    private void KeyCollect(bool isRed) {
        if (!isRed) {
            blueKeys++;
        } else {
            redKeys++;
        }

        uiController.UpdateUI();
    }

    private void CoinCollect(int amount) {
        coins += amount;
        uiController.UpdateUI();
    }

    public void SpendKey(bool isRed) {
        if (blueKeys > 0 && !isRed) {
            blueKeys--;
        } else if (redKeys > 0 && isRed) {
            redKeys--;
        }

        uiController.UpdateUI();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // TODO(@lazylllama): Combine key and coin collectors and incl destroy
        switch (collision.gameObject.tag) {
            case "BlueKey":
                KeyCollect(false);
                Destroy(collision.gameObject);
                break;
            case "RedKey":
                KeyCollect(true);
                Destroy(collision.gameObject);
                break;
            case "Coin":
                CoinCollect(1);
                Destroy(collision.gameObject);
                break;
        }
    }
}
    