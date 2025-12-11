using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyContactDamage : MonoBehaviour
{
    public float damage = 10f;
    public float damageCooldown = 0.5f;

    private float nextDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime)
            return;

        IDamageable damageable = collision.collider.GetComponentInParent<IDamageable>();
        if (damageable != null && collision.collider.CompareTag("P_Player"))
        {
            Vector2 hitPoint = collision.GetContact(0).point;
            damageable.TakeDamage(damage, hitPoint);
            nextDamageTime = Time.time + damageCooldown;
        }
    }
}
