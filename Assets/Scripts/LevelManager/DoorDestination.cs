using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class DoorDestination : MonoBehaviour
{
    public enum DoorMode
    {
        Scene,
        Room
    }

    [Header("Dependencias")]
    public DoorLock doorLock;    // referencia al DoorLock de esta puerta (puede ser null si no querés lock)

    [Header("Modo")]
    public DoorMode mode = DoorMode.Scene;

    [Header("Scene mode")]
    public string sceneToLoad;

    [Header("Room mode")]
    public Transform roomTargetPosition;   // a dónde teletransportar al player
    public Collider2D roomCameraBounds;    // bounds para CinemachineConfiner2D
    public GameObject spawner;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

        doorLock = GetComponent<DoorLock>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("P_Player"))
            return;

        if (doorLock != null && doorLock.IsLocked)
            return;

        switch (mode)
        {
            case DoorMode.Scene:
                LoadScene();
                break;

            case DoorMode.Room:
                TeleportToRoom(other.transform);
                spawner.SetActive(true);
                break;
        }
    }

    void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("DoorDestination: sceneToLoad no asignada.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError($"DoorDestination: la escena '{sceneToLoad}' no está en el Build Profile.");
            return;
        }

        SceneLoader.Load(sceneToLoad);

    }

    void TeleportToRoom(Transform player)
    {
        // 1) Teleport instantáneo del player
        if (roomTargetPosition != null)
        {
            player.position = roomTargetPosition.position;
        }
        else
        {
            Debug.LogWarning("DoorDestination: roomTargetPosition no asignado.");
        }

        // 2) Actualizar inmediatamente los límites de la cámara
        if (roomCameraBounds != null)
        {
            var vcam = FindFirstObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                var confiner = vcam.GetComponent<CinemachineConfiner2D>();
                if (confiner != null)
                {
                    confiner.BoundingShape2D = roomCameraBounds;
                    confiner.InvalidateBoundingShapeCache();
                }
            }
        }

        // 3) Spawn enemigos del room destino (cuando entrás)
        if (roomCameraBounds != null)
        {
            var gen = roomCameraBounds.GetComponentInParent<ProceduralRoomGenerator>();
            if (gen != null)
            {
                var spawner = FindFirstObjectByType<EnemyRoomSpawner>();
                if (spawner != null)
                {
                    spawner.SpawnEnemiesForRoom(gen);
                }
            }
        }
    }
}
