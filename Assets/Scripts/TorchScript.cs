using UnityEngine;

//TODO(@lazylllama): Implement dead-zone
public class TorchScript : MonoBehaviour {
	[Header("REFS")]
	[SerializeField] private GameObject playerObject;
	private PlayerController playerController;
	private Animator         animator;


	[Header("Torch Settings")]
	[SerializeField] private float smoothing;
	[SerializeField] private Vector2 offset;

	// Hashes
	private static readonly int IsLit = Animator.StringToHash("isLit");


	private void Start() {
		playerController = FindAnyObjectByType<PlayerController>();
		animator         = GetComponent<Animator>();

		// Light up the torch on start
		SetIsLit(true);
	}

	public void SetIsLit(bool isLit) {
		animator.SetBool(IsLit, isLit);
	}

	private void UpdatePosition() {
		var playerPosWOffset = playerObject.transform.position +
		                       new Vector3(playerController.isFacingRight ? offset.x : -offset.x, offset.y, 0);

		transform.position = Vector3.Lerp(transform.position, playerPosWOffset, smoothing * Time.deltaTime);
	}

	private void LateUpdate() {
		UpdatePosition();
	}
}