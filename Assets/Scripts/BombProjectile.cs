using System;
using System.Collections;
using System.Globalization;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class Bomb : MonoBehaviour {
	#region Fields

	//* Refs
	[SerializeField] private GameObject boomGameObject;

	private CinemachineImpulseSource impulseSource;
	private TMP_Text                 countText;
	private Rigidbody2D              bombRb;
	private Animator                 animator;

	//* Constants
	[SerializeField] private float throwForce = 3f;
	[SerializeField] private float bombTimer  = 3f;

	//* Variables
	private float countTimer = 3f;

	#endregion

	#region Unity Functions

	private void Start() {
		impulseSource = GetComponent<CinemachineImpulseSource>();
		bombRb        = GetComponentInChildren<Rigidbody2D>();
		countText     = GetComponentInChildren<TMP_Text>();

		bombRb.AddForce((transform.right + transform.up) * throwForce);

		StartCoroutine(BombTimer());
	}


	private void Update() {
		countTimer     -= Time.deltaTime;
		countText.text =  Math.Ceiling(countTimer).ToString(CultureInfo.CurrentCulture);
	}

	#endregion

	#region Routines

	private IEnumerator BombTimer() {
		yield return new WaitForSeconds(bombTimer);

		boomGameObject.SetActive(true);

		bombRb.bodyType = RigidbodyType2D.Static;

		impulseSource.GenerateImpulse();
		
		yield return new WaitForSeconds(0.25f);
		
		Destroy(gameObject);
	}

	#endregion
}