using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralRoomGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [Header("Tamaño de la habitación (en celdas)")]
    public int minWidth = 12;
    public int maxWidth = 20;
    public int minHeight = 8;
    public int maxHeight = 14;

    [Header("Random Walk (forma de la habitación)")]
    [Range(0.1f, 1f)]
    public float fillRatio = 0.45f; // qué fracción aprox. de la grilla se llenará de piso

    [Header("Tiles de suelo")]
    [Tooltip("Podés repetir tiles para darles más peso.")]
    public TileBase[] floorVariants;

    [Header("Tiles de pared")]
    public TileBase wallTop;
    public TileBase wallTopCapTile;
    public TileBase wallBottom;
    public TileBase wallLeft;
    public TileBase wallRight;
    public TileBase wallCornerTopLeft;
    public TileBase wallCornerTopLeftCapTile;
    public TileBase wallCornerTopRight;
    public TileBase wallCornerTopRightCapTile;
    public TileBase wallCornerBottomLeft;
    public TileBase wallCornerBottomRight;

    [Header("Puerta")]
    public GameObject doorPrefab;
    public Vector3 doorWorldOffset = Vector3.zero;

    [Header("Jugador")]
    public Transform playerTransform; // Player en escena
    public Vector3 playerWorldOffset = new Vector3(0.5f, 0.5f, 0f);

    [Header("Bounds de cámara")]
    public BoxCollider2D cameraBounds;              // Lo usa el Confiner
    public CinemachineConfiner2D cameraConfiner;    // vCam confiner

    [Header("Semilla")]
    public bool useRandomSeed = true;
    public int seed = 0;

    // Datos generados (para ser usados por otros scripts)
    [HideInInspector] public Transform generatedDoorTransform;
    [HideInInspector] public DoorLock generatedDoorLock;
    [HideInInspector] public Vector3 generatedPlayerSpawnPosition;

    // Internos
    int _roomWidth;
    int _roomHeight;
    int _minX;
    int _maxX;
    int _minY;
    int _maxY;
    System.Random _prng;

    void Start()
    {
        if (floorTilemap == null || wallTilemap == null)
        {
            Debug.LogError("ProceduralRoomGenerator: asigná floorTilemap y wallTilemap.");
            return;
        }

        if (floorVariants == null || floorVariants.Length == 0)
        {
            Debug.LogError("ProceduralRoomGenerator: asigná al menos un tile de floorVariants.");
            return;
        }

        InitRandom();
        GenerateRoom();
    }

    void InitRandom()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        _prng = new System.Random(seed);
    }

    public void GenerateRoom()
    {
        // 1) Elegir tamaño de la habitación
        _roomWidth = _prng.Next(minWidth, maxWidth + 1);
        _roomHeight = _prng.Next(minHeight, maxHeight + 1);

        // La centramos alrededor del origen (0,0) en coordenadas de Tilemap
        _minX = -_roomWidth / 2;
        _maxX = _minX + _roomWidth - 1;

        _minY = -_roomHeight / 2;
        _maxY = _minY + _roomHeight - 1;

        // Limpiar tilemaps por si acaso
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // 2) Generar suelo interior
        FillFloor();

        // 3) Generar paredes alrededor (un marco de 1 celda)
        GenerateWalls();

        // 4) Colocar puerta en la pared superior
        PlaceDoorOnTopWall();

        // 5) Colocar player en una casilla interior
        PlacePlayer();

        // 6) Ajustar bounds de cámara
        UpdateCameraBounds();
    }

    void FillFloor()
    {
        for (int x = _minX; x <= _maxX; x++)
        {
            for (int y = _minY; y <= _maxY; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                // Elegimos un tile random del arreglo
                int index = _prng.Next(0, floorVariants.Length);
                TileBase tile = floorVariants[index];

                floorTilemap.SetTile(cellPos, tile);
            }
        }
    }

    void GenerateWalls()
    {
        // Vamos a hacer un marco de una celda alrededor de la habitación
        int outerMinX = _minX - 1;
        int outerMaxX = _maxX + 1;
        int outerMinY = _minY - 1;
        int outerMaxY = _maxY + 1;

        for (int x = outerMinX; x <= outerMaxX; x++)
        {
            for (int y = outerMinY; y <= outerMaxY; y++)
            {
                bool isInnerX = (x >= _minX && x <= _maxX);
                bool isInnerY = (y >= _minY && y <= _maxY);

                // Si está dentro del rectángulo interior, es suelo, no pared
                if (isInnerX && isInnerY)
                    continue;

                // Solo queremos el borde exacto (no queremos llenar todo el exterior)
                bool isBorderX = (x == outerMinX || x == outerMaxX);
                bool isBorderY = (y == outerMinY || y == outerMaxY);

                if (!(isBorderX || isBorderY))
                    continue;

                Vector3Int cellPos = new Vector3Int(x, y, 0);

                bool left = (x == outerMinX);
                bool right = (x == outerMaxX);
                bool bottom = (y == outerMinY);
                bool top = (y == outerMaxY);

                // Selección de tile según borde
                TileBase tile = null;
                TileBase capTile = null;

                // ESQUINAS SUPERIORES
                if (top && left)
                {
                    tile = wallCornerTopLeft;
                    capTile = wallCornerTopLeftCapTile;
                }
                else if (top && right)
                {
                    tile = wallCornerTopRight;
                    capTile = wallCornerTopRightCapTile;
                }
                // ESQUINAS INFERIORES
                else if (bottom && left)
                    tile = wallCornerBottomLeft;
                else if (bottom && right)
                    tile = wallCornerBottomRight;

                // PAREDES SUPERIORES
                else if (top)
                {
                    tile = wallTop;
                    capTile = wallTopCapTile;
                }
                // PAREDES INFERIORES
                else if (bottom)
                    tile = wallBottom;
                
                // LATERALES
                else if (left)
                    tile = wallLeft;
                else if (right)
                    tile = wallRight;

                // Pintar pared base
                if (tile != null)
                    wallTilemap.SetTile(cellPos, tile);

                // Si estamos en el borde superior, pintamos el segundo tile encima
                if (top && capTile != null)
                {
                    Vector3Int posCap = new Vector3Int(x, y + 1, 0);
                    wallTilemap.SetTile(posCap, capTile);
                }
            }
        }
    }

    void PlaceDoorOnTopWall()
    {
        if (doorPrefab == null) return;

        int outerMinX = _minX;
        int outerMaxX = _maxX + 1;
        int topY = _maxY + 1;

        // Escogemos una X random en la parte superior
        int doorX = _prng.Next(_minX, _maxX + 1);
        Vector3Int doorCell = new Vector3Int(doorX, topY, 0);
        Vector3Int nextdoorCell = new Vector3Int(doorX-1, topY, 0);

        // Eliminar pared base
        wallTilemap.SetTile(doorCell, null);
        wallTilemap.SetTile(nextdoorCell, null);

        // Eliminar pared cap (segundo tile)
        Vector3Int capCell = new Vector3Int(doorX, topY + 1, 0);
        Vector3Int nextcapCell = new Vector3Int(doorX-1, topY + 1, 0);
        wallTilemap.SetTile(capCell, null);
        wallTilemap.SetTile(nextcapCell, null);


        // Instanciar puerta
        Vector3 worldPos = wallTilemap.CellToWorld(doorCell) + doorWorldOffset;
        GameObject doorInstance = Instantiate(doorPrefab, worldPos, Quaternion.identity);

        // guardamos referencia
        generatedDoorTransform = doorInstance.transform; 
        generatedDoorLock = doorInstance.GetComponent<DoorLock>();
    }

    void PlacePlayer()
    {
        // Siempre calculamos dónde estaría el spawn
        int spawnX = _prng.Next(_minX + 1, _maxX);
        int spawnY = _minY + 1;

        Vector3Int spawnCell = new Vector3Int(spawnX, spawnY, 0);
        Vector3 worldPos = floorTilemap.CellToWorld(spawnCell) + playerWorldOffset;

        generatedPlayerSpawnPosition = worldPos; // guardamos el spawn

        // Y solo movemos al player real si tenemos referencia
        if (playerTransform != null)
        {
            playerTransform.position = worldPos;
        }
    }

    void UpdateCameraBounds()
    {
        if (cameraBounds == null)
            return;

        // Asumimos cell size = 1. Si no, podríamos multiplicar.
        float width = (_roomWidth + 2);   // +2 para incluir paredes
        float height = (_roomHeight + 2);

        // Centro del rectángulo incluyendo paredes
        float centerX = (_minX + _maxX) / 2f;
        float centerY = (_minY + _maxY) / 2f;

        cameraBounds.size = new Vector2(width, height+1);
        cameraBounds.offset = new Vector2(centerX + 0.5f, centerY + 1.3f);

        // Actualizar Confiner
        if (cameraConfiner != null)
        {
            cameraConfiner.BoundingShape2D = cameraBounds;
            cameraConfiner.InvalidateBoundingShapeCache();
        }
    }
    public Vector3 GetRandomFloorWorldPosition()
    {
        int x = Random.Range(_minX + 1, _maxX);
        int y = Random.Range(_minY + 1, _maxY);

        Vector3Int cell = new Vector3Int(x, y, 0);
        return floorTilemap.CellToWorld(cell) + playerWorldOffset;
    }
}
