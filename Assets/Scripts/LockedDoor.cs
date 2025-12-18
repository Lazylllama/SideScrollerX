using UnityEngine;

public class LockedDoor : MonoBehaviour {
	#region Fields

	//* Options
	[SerializeField] private bool isRedDoor;

	#endregion

	#region Unity Functions

	private void OnCollisionEnter2D(Collision2D collision) {
		//? Only respond to player collisions
		if (!collision.gameObject.CompareTag("Player")) return;

		switch (isRedDoor) {
			case true when Inventory.Instance.redKeys > 0:
				Inventory.Instance.SpendKey(true);
				break;
			case false when Inventory.Instance.blueKeys > 0:
				Inventory.Instance.SpendKey(false);
				break;
			default:
				//?	Do nothing if no key
				return;
		}

		Destroy(gameObject);
	}

	#endregion
}