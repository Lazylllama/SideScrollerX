using System.ComponentModel;
using UnityEngine;

public class ParallaxController : MonoBehaviour {
	private float   lengthX;
	private Vector3 startingPos;

	[Header("Camera")]
	[SerializeField] GameObject cam;

	[Header("Parallax Settings")]
	// 0: Move with camera
	// 1: Stay in place.
	// Values in between create a parallax effect.
	[Range(0, 1)] [SerializeField] private float parallaxEffect;
	[Range(0, 1)] [SerializeField] private float parallaxEffectY;

	// Ignore infinite scrolling for the moon
	[SerializeField] private bool isMoon;
	[SerializeField] private bool ignoreY;

	private void Start() {
		startingPos = transform.position;
		lengthX     = GetComponent<SpriteRenderer>().bounds.size.x;
	}

	// Update is called once per frame
	private void LateUpdate() {
		var distanceX = cam.transform.position.x * parallaxEffect;
		var movementX = cam.transform.position.x * (1 - parallaxEffect);
		var distanceY = cam.transform.position.y * parallaxEffectY;
		var pos       = transform.position;

		transform.position = new Vector3(startingPos.x + distanceX, ignoreY ? pos.y : startingPos.y + distanceY, pos.z);

		if (isMoon) return;

		if (movementX > startingPos.x + lengthX) {
			startingPos.x += lengthX;
		} else if (movementX < startingPos.x - lengthX) {
			startingPos.x -= lengthX;
		}
	}
};