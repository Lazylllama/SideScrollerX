using System.Collections;
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

		uiController.UpdateUI();
	}

	private void BombCollect() {
		hasBomb = true;
		uiController.UpdateUI();
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.CollectItem);
	}

	public void SpendKey(bool isRed) {
		if (blueKeys > 0 && !isRed) {
			blueKeys--;
		} else if (redKeys > 0 && isRed) {
			redKeys--;
		} else return;

		AudioManager.Instance.PlaySfx(AudioManager.AudioName.UseKey);

		uiController.UpdateUI();
	}

	public void UseBomb() {
		hasBomb = false;
		uiController.UpdateUI();
	}

	#endregion

	#region Routines

	private IEnumerator CoinCollectRoutine(int amount, GameObject coin) {
		coins += amount;
		uiController.UpdateUI();
		coin.GetComponentInChildren<Collider2D>().enabled     = false;
		coin.GetComponentInChildren<SpriteRenderer>().enabled = false;
		coin.GetComponentInChildren<ParticleSystem>().Play();
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.CoinCollect);
		yield return new WaitForSeconds(3f);
		Destroy(coin);
	}

	#endregion
}