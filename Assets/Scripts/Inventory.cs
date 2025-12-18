using System.Collections;
using UnityEngine;

public class Inventory : MonoBehaviour {
	#region Fields

	// Instance
	public static Inventory Instance;

	[Header("Inventory Stats")]
	public int blueKeys;
	public int  redKeys;
	public int  goldKeys;
	public int  coins;
	public bool hasBomb;

	#endregion

	#region Unity Functions

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
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
					StartCoroutine(CoinCollectRoutine(1, collision.gameObject));
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
				AudioManager.Instance.PlaySfx(AudioManager.AudioName.CollectItem);
				break;
			case "RedKey":
				redKeys++;
				AudioManager.Instance.PlaySfx(AudioManager.AudioName.CollectItem);
				break;
			case "GoldKey":
				goldKeys++;
				AudioManager.Instance.PlaySfx(AudioManager.AudioName.GoldenKeyCollect);
				break;
		}

		UIController.Instance.UpdateUI();
	}

	private void BombCollect() {
		hasBomb = true;
		UIController.Instance.UpdateUI();
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.CollectItem);
	}

	public void SpendKey(bool isRed) {
		if (blueKeys > 0 && !isRed) {
			blueKeys--;
		} else if (redKeys > 0 && isRed) {
			redKeys--;
		} else return;

		AudioManager.Instance.PlaySfx(AudioManager.AudioName.UseKey);
		UIController.Instance.UpdateUI();
	}

	public void UseBomb() {
		hasBomb = false;
		UIController.Instance.UpdateUI();
	}

	#endregion

	#region Routines

	private IEnumerator CoinCollectRoutine(int amount, GameObject coin) {
		coins += amount;
		UIController.Instance.UpdateUI();
		coin.GetComponentInChildren<Collider2D>().enabled     = false;
		coin.GetComponentInChildren<SpriteRenderer>().enabled = false;
		coin.GetComponentInChildren<ParticleSystem>().Play();
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.CoinCollect);
		yield return new WaitForSeconds(3f);
		Destroy(coin);
	}

	#endregion
}