using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways] // opcional: para probar también en editor
public class RandomFloorTiles : MonoBehaviour
{
    [Header("Tilemap de suelo")]
    public Tilemap floorTilemap;

    [Header("Tiles posibles para el suelo (variantes)")]
    public TileBase[] floorVariants;

    [Header("Semilla (opcional)")]
    public int seed = 0;
    public bool useRandomSeed = true;

    [Header("Ejecutar automáticamente")]
    public bool runOnStart = true;

    void Start()
    {
        if (!Application.isPlaying)
            return;

        if (runOnStart)
        {
            ApplyRandomTiles();
        }
    }

    [ContextMenu("Aplicar random tiles ahora")]
    public void ApplyRandomTiles()
    {
        if (floorTilemap == null)
        {
            Debug.LogWarning("RandomFloorTiles: floorTilemap no asignado.");
            return;
        }

        if (floorVariants == null || floorVariants.Length == 0)
        {
            Debug.LogWarning("RandomFloorTiles: no hay floorVariants asignados.");
            return;
        }

        // Configurar semilla
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        System.Random prng = new System.Random(seed);

        // Recorremos todas las celdas del Tilemap
        BoundsInt bounds = floorTilemap.cellBounds;

        floorTilemap.CompressBounds(); // opcional para achicar el área

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                // Solo tocamos las celdas que YA tengan un tile de suelo
                if (floorTilemap.HasTile(cellPos))
                {
                    int index = prng.Next(0, floorVariants.Length);
                    TileBase randomTile = floorVariants[index];
                    floorTilemap.SetTile(cellPos, randomTile);
                }
            }
        }

        Debug.Log($"RandomFloorTiles: aplicado random con semilla {seed}.");
    }
}
