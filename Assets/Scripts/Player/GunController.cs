using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("Refs")]
    public Transform muzzle;
    public Bullet bulletPrefab;
    public PlayerInput playerInput;

    // ================= UI =================
    public AmmoIconsUI ammoUI; // asignable por inspector (preferido)
    // =====================================

    [Header("Fire Settings")]
    public float fireRate = 8f;

    [Header("Ammo")]
    public int magazineSize = 10;
    public float reloadTime = 1.2f;

    int currentAmmo;
    float nextFireTime;
    bool isReloading;

    void Awake()
    {
        currentAmmo = magazineSize; //  se mueve arriba SOLO para UI correcta

        // ================= UI =================
        if (!ammoUI)
            ammoUI = FindFirstObjectByType<AmmoIconsUI>(FindObjectsInactive.Include);

        ammoUI?.SetAmmo(currentAmmo, magazineSize);
        ammoUI?.SetReloading(false);
        // =====================================

        if (!playerInput)
            playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (isReloading)
            return;

        if (playerInput != null && playerInput.actions["Fire"].IsPressed())
        {
            TryShoot();
        }
    }

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

        // ================= UI =================
        ammoUI?.SetAmmo(currentAmmo, magazineSize);
        // =====================================
        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

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

        // ================= UI =================
        ammoUI?.SetReloading(true);
        // =====================================

        Debug.Log("Recargando...");
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Recarga completa");

        // ================= UI =================
        ammoUI?.SetReloading(false);
        ammoUI?.SetAmmo(currentAmmo, magazineSize);
        // =====================================
    }
}
