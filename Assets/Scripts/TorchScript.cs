using UnityEngine;

public class TorchScript : MonoBehaviour
{
    [Header("REFS")]
    [SerializeField] GameObject playerObject;
    PlayerController playerController;
    Animator animator;

    [Header("Torch Settings")]
    [SerializeField] float smoothing;
    [SerializeField] Vector2 offset;

    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        animator = GetComponent<Animator>();
        SetIsLit(true);
    }

    public void SetIsLit(bool isLit) {
        animator.SetBool("isLit", true);
    }

    public void UpdatePosition() {
        Vector3 playerPosWOffset;

        if (playerController.isFacingRight) {
            playerPosWOffset = playerObject.transform.position + new Vector3(offset.x, offset.y, 0);
        } else {
            playerPosWOffset = playerObject.transform.position + new Vector3(-offset.x, offset.y, 0);
        }

        transform.position = Vector3.Lerp(
                transform.position,
                playerPosWOffset,
                smoothing * Time.deltaTime
                );
    }

    void FixedUpdate() {
        UpdatePosition();
    }
}
