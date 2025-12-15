using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [Header("Estado inicial")]
    public bool lockedAtStart = true;
    public bool unlockWhenAllEnemiesDead = true;

    [Header("Visuales")]
    public GameObject closedVisual;     // Door_Close
    public GameObject[] openVisuals;    // Door_Open_L, Door_Open_R

    [Header("Bloqueo físico")]
    public Collider2D solidBlocker;     // Blocker (collider grande)

    public bool IsLocked { get; private set; } = true;

    int _enemiesRemaining = 0;

    void Start()
    {
        SetLocked(lockedAtStart);
    }

    public void SetEnemiesToClear(int count)
    {
        _enemiesRemaining = Mathf.Max(0, count);

        if (!unlockWhenAllEnemiesDead)
            return;

        if (_enemiesRemaining == 0)
            SetLocked(false);
        else
            SetLocked(true);
    }

    public void NotifyEnemyKilled()
    {
        if (!unlockWhenAllEnemiesDead)
            return;

        _enemiesRemaining--;
        if (_enemiesRemaining <= 0)
        {
            SetLocked(false);
        }
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;

        // Collider que bloquea el paso
        if (solidBlocker != null)
            solidBlocker.enabled = locked;

        // Visual cerrada
        if (closedVisual != null)
            closedVisual.SetActive(locked);

        // Visuales abiertas
        if (openVisuals != null)
        {
            foreach (var vis in openVisuals)
            {
                if (vis != null)
                    vis.SetActive(!locked);
            }
        }
    }
}
