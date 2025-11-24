using System;
using Unity.VisualScripting;
using UnityEngine;

public class LevelDoor : MonoBehaviour {
	#region Fields

	//* Hashes
	private static readonly int IsOpen = Animator.StringToHash("isOpen");

	private Animator  animator;
	private Inventory inventory;

	#endregion

	#region Unity Functions

	private void Start() {
		animator  = GetComponent<Animator>();
		inventory = FindAnyObjectByType<Inventory>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (!other.gameObject.CompareTag("Player")) return;
		if (inventory.goldKeys > 1) return;

		animator.SetBool(IsOpen, true);
	}

	#endregion
}