using UnityEngine;

public class Elevator : MonoBehaviour {
	//* Options
	[SerializeField] private float speed;

	//* Refs
	[SerializeField] private GameObject    platform;
	[SerializeField] private GameObject    pointA, pointB;
	private                  ElevatorLever lever;

	private void Start() {
		lever = GetComponentInChildren<ElevatorLever>();
	}

	private void FixedUpdate() {
		platform.transform.position = Vector2.MoveTowards(platform.transform.position,
		                                                  lever.leverState
			                                                  ? pointB.transform.position
			                                                  : pointA.transform.position, speed * Time.deltaTime);
	}
}