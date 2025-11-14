using System.ComponentModel;
using UnityEngine;

public class ParallaxController : MonoBehaviour {
	float startingPos, length;

	[Header("Refs")]
	[SerializeField] GameObject cam;

	[Header("Parallax Settings")]
	// 0: Move with camera
	// 1: Stay in place.
	// Values in between create parallax effect.
	[Range(0, 1)] [SerializeField] private float parallaxEffect;

	// Ignore infinite scrolling for moon
	[SerializeField] private bool isMoon;

	void Start() {
		startingPos = transform.position.x;
		length      = GetComponent<SpriteRenderer>().bounds.size.x;
	}

	// Update is called once per frame
	void LateUpdate() {
		float distance = cam.transform.position.x * parallaxEffect;

		transform.position = new Vector3(
		                                 startingPos + distance,
		                                 transform.position.y,
		                                 transform.position.z
		                                );

		// Proceed only if not the moon
		if (!isMoon) {
			float movement = cam.transform.position.x * (1 - parallaxEffect);

			if (movement > startingPos + length) {
				startingPos += length;
			}
			else if (movement < startingPos - length) {
				startingPos -= length;
			}
		}
	}
}