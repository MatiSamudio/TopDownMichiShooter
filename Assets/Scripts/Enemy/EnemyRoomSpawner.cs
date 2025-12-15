using System.Collections.Generic;
using UnityEngine;

public class EnemyRoomSpawner : MonoBehaviour
{
    [Header("Enemigos")]
    public GameObject enemyPrefab;
    public int minEnemiesPerRoom = 5;
    public int maxEnemiesPerRoom = 7;

    [Header("Modo single-room (escenas NO procedurales)")]
    public bool autoSpawnOnStartSingleRoom = false;

    public BoxCollider2D singleRoomSpawnArea;   // área donde se spawnean enemigos
    public DoorLock singleRoomDoorLock;         // puerta que se abrirá cuando mueran todos

    void Start()
    {
        // Este Start solo se usa en escenas sin MultiRoomGenerator / sin rooms procedurales
        if (!autoSpawnOnStartSingleRoom)
            return;

        if (singleRoomSpawnArea == null || singleRoomDoorLock == null)
        {
            Debug.LogWarning("EnemyRoomSpawner: modo single-room activado pero faltan singleRoomSpawnArea o singleRoomDoorLock.");
            return;
        }

        SpawnInSingleRoom();
    }

    /// <summary>
    /// Llamado por MultiRoomGenerator cuando ya generó TODOS los rooms.
    /// </summary>
    public void SpawnEnemiesInRooms(IEnumerable<ProceduralRoomGenerator> rooms)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyRoomSpawner: asigná enemyPrefab.");
            return;
        }

        foreach (var room in rooms)
        {
            if (room == null) continue;

            DoorLock doorLock = room.generatedDoorLock;
            if (doorLock == null)
            {
                Debug.LogWarning($"EnemyRoomSpawner: room {room.name} no tiene DoorLock generado.");
                continue;
            }

            int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
            doorLock.SetEnemiesToClear(enemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 spawnPos = room.GetRandomFloorWorldPosition();
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    DoorLock capturedDoor = doorLock;
                    health.OnDeath += () =>
                    {
                        capturedDoor.NotifyEnemyKilled();
                    };
                }
                else
                {
                    Debug.LogWarning("EnemyRoomSpawner: enemyPrefab no tiene EnemyHealth.");
                }
            }
        }
    }

    void SpawnInSingleRoom()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyRoomSpawner: asigná enemyPrefab.");
            return;
        }

        if (singleRoomSpawnArea == null || singleRoomDoorLock == null)
        {
            Debug.LogWarning("EnemyRoomSpawner: singleRoomSpawnArea o singleRoomDoorLock no asignados.");
            return;
        }

        int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
        singleRoomDoorLock.SetEnemiesToClear(enemyCount);

        Bounds b = singleRoomSpawnArea.bounds;

        for (int i = 0; i < enemyCount; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            Vector3 spawnPos = new Vector3(x, y, 0f);

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                DoorLock capturedDoor = singleRoomDoorLock;
                health.OnDeath += () =>
                {
                    capturedDoor.NotifyEnemyKilled();
                };
            }
        }
    }
}
