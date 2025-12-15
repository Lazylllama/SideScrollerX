using NUnit.Framework;
using UnityEngine;

public class Inventory : MonoBehaviour {
	#region Fields

	[Header("Inventory Stats")]
	public int blueKeys;
	public int  redKeys;
	public int  goldKeys;
	public int  coins;
	public bool hasBomb;

	// Ref
	private UIController uiController;

	#endregion

	#region Unity Functions

	private void Start() {
		uiController = FindAnyObjectByType<UIController>();
	}

	#endregion

	// Handle all pickups
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag.Contains("Key")) {
			KeyCollect(collision.gameObject.tag);
			Destroy(collision.gameObject);
		} else {
			switch (collision.gameObject.tag) {
				case "Coin":
					CoinCollect(1);
					Destroy(collision.gameObject);
					break;
				case "BombPickup":
					BombCollect();
					Destroy(collision.gameObject);
					break;
			}
		}
	}

	#region Functions

	private void KeyCollect(string type) {
		switch (type) {
			case "BlueKey":
				blueKeys++;
				break;
			case "RedKey":
				redKeys++;
				break;
			case "GoldKey":
				goldKeys++;
				break;
		}

		uiController.UpdateUI();
	}

	private void CoinCollect(int amount) {
		coins += amount;
		uiController.UpdateUI();
	}

	private void BombCollect() {
		hasBomb = true;
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

	public void UseBomb() {
		hasBomb = false;
		uiController.UpdateUI();
	}

	#endregion
}