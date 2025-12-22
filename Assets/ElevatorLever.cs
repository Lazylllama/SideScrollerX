using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorLever : MonoBehaviour {
	#region Fields

	//* Hashes
	private static readonly int LeverState = Animator.StringToHash("leverState");

	//* Options
	[SerializeField] private float radius;

	//* Refs
	[SerializeField] private SpriteRenderer keyUiRenderer;
	private                  Animator       animator;
	private                  InputAction    interactAction;

	//* States
	public bool leverState;

	#endregion

	#region Unity Functions

	private void Start() {
		interactAction = InputSystem.actions.FindAction("Interact"); // Primarily E key

		animator = GetComponent<Animator>();
	}

	private void Update() {
		var distance = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
		var opacity = Mathf.Clamp01(1 - distance / radius);

		keyUiRenderer.color = new Color(1f, 1f, 1f, opacity);

		if (!interactAction.WasPressedThisFrame() || distance > radius) return;

		leverState = !leverState;
		animator.SetBool(LeverState, leverState);
	}

	#endregion
}