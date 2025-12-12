using System;
using UnityEngine;

public class BoomController : MonoBehaviour {
	#region Fields

	private StatsController statsController;

	[SerializeField] private float forceMult = 1;

	#endregion

	#region Unity Functions

	private void Start() {
		statsController = FindAnyObjectByType<StatsController>();
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (!other.gameObject.CompareTag("Player")) return;
		statsController.DealDamage(1f, transform.position, forceMult);
	}

	#endregion
}