using System.ComponentModel;
using UnityEngine;

public class ParallaxController : MonoBehaviour {
	private float startingPos, length;

	[Header("Camera")]
	[SerializeField] GameObject cam;

	[Header("Parallax Settings")]
	// 0: Move with camera
	// 1: Stay in place.
	// Values in between create a parallax effect.
	[Range(0, 1)] [SerializeField] private float parallaxEffect;

	// Ignore infinite scrolling for the moon
	[SerializeField] private bool isMoon;

	private void Start() {
		startingPos = transform.position.x;
		length      = GetComponent<SpriteRenderer>().bounds.size.x;
	}

	// Update is called once per frame
	private void LateUpdate() {
		var distance = cam.transform.position.x * parallaxEffect;
		var movement = cam.transform.position.x * (1 - parallaxEffect);
		var pos      = transform.position;

		transform.position = new Vector3(startingPos + distance, pos.y, pos.z);

		if (isMoon) return;

		if (movement > startingPos + length) {
			startingPos += length;
		} else if (movement < startingPos - length) {
			startingPos -= length;
		}
	}
}