using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    Inventory inventoryScript;

    [SerializeField] bool isRedDoor;

    private void Start() {
        inventoryScript = FindAnyObjectByType<Inventory>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (inventoryScript.blueKeys > 0 && !isRedDoor) {
                inventoryScript.SpendKey(false);
                Destroy(gameObject);
            } else if (inventoryScript.redKeys > 0 && isRedDoor) {
                inventoryScript.SpendKey(true);
                Destroy(gameObject);
            }
        }
    }
}
