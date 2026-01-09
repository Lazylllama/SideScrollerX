using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxShard : MonoBehaviour {
	#region Fields

	private Rigidbody2D rb;
	private Transform   parentPosition;

	private float aliveTimer;
	private float liveDuration;

	#endregion

	#region Unity Functions
	
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		parentPosition = transform.parent;
		liveDuration   = Random.Range(2f, 3f);
		
		Vector2 direction = (transform.position - parentPosition.position).normalized;
		rb.AddForce(new Vector2(direction.x + Random.Range(-0.3f, 0.3f), Mathf.Abs(direction.y) + Random.Range(0.5f, 1f)) * Random.Range(150f, 300f));
	}

	private void Update() {
		aliveTimer += Time.deltaTime;
		if (aliveTimer > liveDuration) Destroy(transform.parent.gameObject.transform.parent.gameObject);
	}

	#endregion
}