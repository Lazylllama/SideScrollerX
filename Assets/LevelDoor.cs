using System.Collections;
using UnityEngine;

public class LevelDoor : MonoBehaviour {
	#region Fields

	//* Hashes
	private static readonly int IsOpen = Animator.StringToHash("isOpen");

	//* Refs
	private Animator         animator;
	private Inventory        inventory;
	private UIController     uiController;

	#endregion

	#region Unity Functions

	private void Start() {
		animator         = GetComponent<Animator>();
		inventory        = FindAnyObjectByType<Inventory>();
		uiController     = FindAnyObjectByType<UIController>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (!other.gameObject.CompareTag("Player")) return;
		if (inventory.goldKeys > 1) return;

		StartCoroutine(OpenDoorRoutine());
	}

	#endregion

	#region Routines

	private IEnumerator OpenDoorRoutine() {
		StartCoroutine(uiController.NextLevel());
		
		// TODO(@lazylllama): play door open sound
		
		animator.SetBool(IsOpen, true);

		yield return null;
	}

	#endregion
}