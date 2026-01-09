using UnityEngine;

public class BoxShard : MonoBehaviour {
	#region Fields

	private Rigidbody2D rb;
	private Transform   parentPosition;

	#endregion

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		parentPosition = transform.parent;
		
		Vector2 direction = (transform.position - parentPosition.position).normalized;
		rb.AddForce(new Vector2(direction.x + Random.Range(-0.3f, 0.3f), Mathf.Abs(direction.y) + Random.Range(0.5f, 1f)) * Random.Range(150f, 300f));
		
		
	}
}