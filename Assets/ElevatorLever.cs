using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorLever : MonoBehaviour {
	private Animator    animator;
	private InputAction secondaryInteract;


	private void Start() {
		secondaryInteract = InputSystem.actions.FindAction("SecondaryInteract"); // Primarily Q key
	}

	private void FixedUpdate() {
	}
}