using UnityEngine;

public class BoomController : MonoBehaviour {
	#region Fields

	[SerializeField] private float forceMult = 1;

	#endregion

	#region Unity Functions

	private void OnCollisionEnter2D(Collision2D other) {
		if (!other.gameObject.CompareTag("Player")) return;
		StatsController.Instance.DealDamage(1f, transform.position, forceMult);
	}

	#endregion
}