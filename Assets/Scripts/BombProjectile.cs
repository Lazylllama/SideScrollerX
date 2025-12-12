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
	private float countTimer;

	//* Hashes
	private static readonly int BombBoom = Animator.StringToHash("bombBoom");

	#endregion

	#region Unity Functions

	private void Start() {
		impulseSource = GetComponent<CinemachineImpulseSource>();
		bombRb        = GetComponentInChildren<Rigidbody2D>();
		countText     = GetComponentInChildren<TMP_Text>();
		animator      = GetComponentInChildren<Animator>();
		bombRb.AddForce((transform.right + transform.up) * throwForce);

		StartCoroutine(BombTimer());

		countTimer = bombTimer;
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

		animator.SetTrigger(BombBoom);
		impulseSource.GenerateImpulse();

		yield return new WaitForSeconds(0.25f);

		Destroy(gameObject);
	}

	#endregion
}