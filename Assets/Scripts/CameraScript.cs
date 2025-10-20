using Unity.Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Vector3 offsetLookatPosition;

    CinemachinePositionComposer positionComposer;

    PlayerController playerController;
    GameObject player;

    void Start() {
        playerController = FindAnyObjectByType<PlayerController>();
        positionComposer = FindAnyObjectByType<CinemachinePositionComposer>();
        player = GameObject.FindWithTag("Player");
    }

    void Update() {


    }
}
