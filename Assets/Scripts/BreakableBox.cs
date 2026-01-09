using System;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBox : MonoBehaviour {
	# region Fields

	[SerializeField] private GameObject     shards;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private GameObject     spawnPrefab;
	private                  Collider2D     boxCollider; // Hehe, literally

	// Higher = More tough?
	[SerializeField] private float fragility;

	#endregion

	#region Unity Functions

	private void Start() {
		boxCollider = GetComponent<Collider2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (!(collision.relativeVelocity.magnitude >= fragility)) return;

		shards.SetActive(true);

		spriteRenderer.enabled = false;
		boxCollider.enabled    = false;

		if (spawnPrefab) Instantiate(spawnPrefab, transform.position, Quaternion.identity);
	}

	#endregion
}