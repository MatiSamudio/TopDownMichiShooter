using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("Refs")]
    public Transform muzzle;
    public Bullet bulletPrefab;
    public PlayerInput playerInput;   // referencia al PlayerInput

    [Header("Fire Settings")]
    public float fireRate = 8f;        // balas por segundo

    [Header("Ammo")]
    public int magazineSize = 10;      // balas por cargador
    public float reloadTime = 1.2f;    // segundos de recarga

    int currentAmmo;
    float nextFireTime;
    bool isReloading;

    void Awake()
    {
        currentAmmo = magazineSize;
        if (!playerInput)
            playerInput = GetComponent<PlayerInput>(); // toma el del mismo Player
    }

    void Update()
    {
        if (isReloading)
            return;

        // Leemos directamente el estado de la acción "Fire"
        if (playerInput != null && playerInput.actions["Fire"].IsPressed())
        {
            TryShoot();
        }
    }

    // Recarga: seguimos usando SendMessages con InputValue (esto sí funciona)
    private void OnReload(InputValue value)
    {
        if (value.isPressed)
        {
            StartReload();
        }
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        nextFireTime = Time.time + 1f / fireRate;

        if (!bulletPrefab || !muzzle)
        {
            Debug.LogWarning("GunController sin prefab o muzzle asignado");
            return;
        }

        Vector2 dir = muzzle.right;
        Bullet b = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        b.Fire(dir);

        currentAmmo--;
        Debug.Log($"Disparo: {currentAmmo}/{magazineSize}");
    }

    void StartReload()
    {
        if (isReloading) return;
        if (currentAmmo == magazineSize) return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Recargando...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log("Recarga completa");
    }
}
