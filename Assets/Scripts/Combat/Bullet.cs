using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 2f;
    public float damage = 20f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Tiempo de vida limitado
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Llamar cuando se dispara la bala.
    /// </summary>
    public void Fire(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // No queremos pegarle al Player
        if (other.CompareTag("P_Player"))
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            damageable.TakeDamage(damage, hitPoint);
        }

        Destroy(gameObject);
    }
}
