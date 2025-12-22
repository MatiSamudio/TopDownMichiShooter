using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MultiRoomGenerator : MonoBehaviour
{
    [Header("Configuración general")]
    public GameObject roomPrefab;          // Prefab que contiene: Grid, Tilemaps, CameraBounds, ProceduralRoomGenerator
    public int roomsToGenerate = 5;
    public float verticalRoomGap = 4f;     // espacio entre rooms

    [Header("Jugador & Cámara compartida")]
    public Transform playerTransform;                   // Player global
    public CinemachineConfiner2D sharedCameraConfiner;  // Confiner usado por la vCam principal

    [Header("Escena de salida")]
    public string exitSceneName = "NextLevel";

    // Internos
    readonly List<ProceduralRoomGenerator> _roomGenerators = new();

    IEnumerator Start()
    {
        if (roomPrefab == null)
        {
            Debug.LogError("MultiRoomGenerator: asigná roomPrefab.");
            yield break;
        }

        if (sharedCameraConfiner == null)
        {
            sharedCameraConfiner = FindFirstObjectByType<CinemachineConfiner2D>();
        }

        float currentYOffset = 0f;

        for (int i = 0; i < roomsToGenerate; i++)
        {
            // 1) Instanciamos el room en la posición actual
            Vector3 roomPos = new Vector3(0f, currentYOffset, 0f);
            GameObject roomInstance = Instantiate(roomPrefab, roomPos, Quaternion.identity, transform);

            // 2) Buscamos el ProceduralRoomGenerator dentro del prefab
            var gen = roomInstance.GetComponentInChildren<ProceduralRoomGenerator>();
            if (gen == null)
            {
                Debug.LogError("MultiRoomGenerator: el prefab no tiene ProceduralRoomGenerator.");
                yield break;
            }

            // 3) Asignamos confiner compartido
            gen.cameraConfiner = sharedCameraConfiner;

            // 4) Solo el primer room mueve al jugador real
            gen.playerTransform = (i == 0) ? playerTransform : null;

            _roomGenerators.Add(gen);

            // Dejamos que su Start() corra y genere el room antes de usar sus datos
            yield return null;

            // 5) Calculamos cuánto subir el siguiente room
            float roomHeightWorld = gen.cameraBounds != null ? gen.cameraBounds.size.y : (gen.maxHeight + 2);
            currentYOffset += roomHeightWorld + verticalRoomGap;
        }

        // Esperamos un frame extra por seguridad
        yield return null;

        // 6) Configuramos las puertas entre rooms
        SetupDoors();
    }

    void SetupDoors()
    {
        for (int i = 0; i < _roomGenerators.Count; i++)
        {
            var gen = _roomGenerators[i];

            if (gen.generatedDoorTransform == null)
            {
                Debug.LogWarning($"MultiRoomGenerator: room {i} no tiene puerta generada.");
                continue;
            }

            var doorDest = gen.generatedDoorTransform.GetComponent<DoorDestination>();
            if (doorDest == null)
            {
                Debug.LogWarning($"MultiRoomGenerator: la puerta del room {i} no tiene DoorDestination.");
                continue;
            }

            bool isLast = (i == _roomGenerators.Count - 1);

            if (isLast)
            {
                // Última puerta, salida de escena
                doorDest.mode = DoorDestination.DoorMode.Scene;
                doorDest.sceneToLoad = exitSceneName;
            }
            else
            {
                // Creamos un punto de spawn para el siguiente room
                var nextGen = _roomGenerators[i + 1];

                GameObject spawnGO = new GameObject($"Room{i + 2}_Spawn");
                spawnGO.transform.position = nextGen.generatedPlayerSpawnPosition;
                spawnGO.transform.SetParent(nextGen.transform, true);

                doorDest.mode = DoorDestination.DoorMode.Room;
                doorDest.roomTargetPosition = spawnGO.transform;
                doorDest.roomCameraBounds = nextGen.cameraBounds;
            }
        }

        // Al final, configuramos el confiner en la primer habitación
        if (sharedCameraConfiner != null && _roomGenerators.Count > 0)
        {
            var firstGen = _roomGenerators[0];

            if (firstGen.cameraBounds != null)
            {
                sharedCameraConfiner.BoundingShape2D = firstGen.cameraBounds;
                sharedCameraConfiner.InvalidateBoundingShapeCache();
            }
        }
        // llamar al EnemyRoomSpawner
        var spawner = FindFirstObjectByType<EnemyRoomSpawner>();
        if (spawner != null)
        {
            spawner.RegisterRooms(_roomGenerators);
            spawner.SpawnEnemiesForRoom(_roomGenerators[0]);
        }
    }
}
