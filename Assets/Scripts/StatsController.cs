using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    // Health
    public float health;
    public float maxHealth = 3;

    // Ref
    UIController uiController;
    TorchScript torchScript;
    PlayerController playerController;

    // Timer
    float immunityTimer = 0f;

    void Start()
    {
        health = maxHealth;
        uiController = FindAnyObjectByType<UIController>();
        torchScript = FindAnyObjectByType<TorchScript>();
        playerController = FindAnyObjectByType<PlayerController>();

        uiController.UpdateUI();
    }

    public void DealDamage(float damageAmount, Vector3 sourcePosition) {
        // If immune, do nothing
        if (immunityTimer > 0f) {
            return;
        }

        // Deal damage and apply knockback
        health -= damageAmount;
        playerController.DamageKnockback(sourcePosition);

        // Update UI
        uiController.UpdateUI();
    }

    public void RegisterKill() {
        // Register immunity for 1.5 seconds after a kill
        StartCoroutine(ImmunityFrames(1.5f));
    }

    IEnumerator ImmunityFrames(float immunityDuration) {
        immunityTimer = 0f;
        while (immunityTimer < immunityDuration) {
            immunityTimer += Time.deltaTime;
            yield return null;
        }
        immunityTimer = 0f;
    }
}
