using UnityEngine;

public class LockedDoor : MonoBehaviour {
	#region Fields

	[SerializeField] private bool      isRedDoor;
	private                  Inventory inventoryScript;

	#endregion

	#region Unity Functions

	private void Start() {
		inventoryScript = FindAnyObjectByType<Inventory>();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		//? Only respond to player collisions
		if (!collision.gameObject.CompareTag("Player")) return;

		switch (isRedDoor) {
			case true when inventoryScript.redKeys > 0:
				inventoryScript.SpendKey(true);
				break;
			case false when inventoryScript.blueKeys > 0:
				inventoryScript.SpendKey(false);
				break;
			default:
				//?	Do nothing if no key
				return;
		}

		Destroy(gameObject);
	}

	#endregion
}