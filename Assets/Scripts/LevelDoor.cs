using System.Collections;
using UnityEngine;

public class LevelDoor : MonoBehaviour {
	#region Fields

	//* Hashes
	private static readonly int IsOpen = Animator.StringToHash("isOpen");

	//* Refs
	private Animator animator;

	#endregion

	#region Unity Functions

	private void Start() {
		animator = GetComponent<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (!other.gameObject.CompareTag("Player")) return;
		if (Inventory.Instance.goldKeys > 1) return;

		StartCoroutine(OpenDoorRoutine());
	}

	#endregion

	#region Routines

	private IEnumerator OpenDoorRoutine() {
		StartCoroutine(UIController.Instance.NextLevel());

		animator.SetBool(IsOpen, true);
		
		yield return new WaitForSeconds(1f);
		
		AudioManager.Instance.PlaySfx(AudioManager.AudioName.OpenDoor);
	}

	#endregion
}