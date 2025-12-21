using UnityEngine;

public class Elevator : MonoBehaviour {
	//* Hashes
	//private static readonly int IsActive = Animator.StringToHash("isActive");

	//* Options
	[SerializeField] private float speed;

	//* Refs
	[SerializeField] private GameObject    pointA, pointB;
	private                  Animator      animator;

	private void Start() {
		animator = GetComponent<Animator>();

		//animator.SetBool(IsActive, true);
	}

	//public void ElevatorSetActive(bool isActive) => animator.SetBool(IsActive, isActive);
}