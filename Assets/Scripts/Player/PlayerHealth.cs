using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;

    public float CurrentHealth => currentHealth;

    [Header("Refs")]
    public GameManager gameManager;   // ← referencia directa

    float currentHealth;
    bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;

        // por si te olvidás de asignar en el inspector
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    public void TakeDamage(float amount, Vector2 hitPoint)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        var controller = GetComponent<TopDownPlayerController2D>();
        if (controller) controller.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        if (gameManager != null)
            gameManager.PlayerDied();
        else
            Debug.LogWarning("PlayerHealth: no hay GameManager asignado");
    }
}
