using UnityEngine;

//TODO(@lazylllama): Implement dead-zone to prevent jittery movement on slight player movements (use same dead zone as cam isch)
public class TorchScript : MonoBehaviour {
	#region Fields

	[Header("Refs")]
	[SerializeField] private GameObject playerObject;
	private PlayerController playerController;
	private Animator         animator;


	[Header("Torch Settings")]
	[SerializeField] private float smoothing;
	[SerializeField] private Vector2 offset;

	//* Hashes
	private static readonly int IsLit = Animator.StringToHash("isLit");

	#endregion

	#region Unity Functions

	//? Set refs and default values
	private void Start() {
		playerController = FindAnyObjectByType<PlayerController>();
		animator         = GetComponent<Animator>();

		//? Light up the torch on start
		SetIsLit(true);
	}

	//? Utilizes LateUpdate to ensure the player has moved first (looks very shit otherwise)
	private void LateUpdate() {
		UpdatePosition();
	}

	#endregion

	#region Functions

	//? Set the torchs lit state, used in other scripts to turn it on/off
	public void SetIsLit(bool isLit) {
		animator.SetBool(IsLit, isLit);
	}

	//? Make a lerp to the players position plus the offset
	private void UpdatePosition() {
		var playerPosWOffset = playerObject.transform.position +
		                       new Vector3(playerController.isFacingRight ? offset.x : -offset.x, offset.y, 0);

		transform.position = Vector3.Lerp(transform.position, playerPosWOffset, smoothing * Time.deltaTime);
	}

	#endregion
}