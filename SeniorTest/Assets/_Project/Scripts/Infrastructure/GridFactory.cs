using UnityEngine;

public class GridFactory : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private BlockView _blockPrefab;
    [SerializeField] private Transform _gridRoot;

    public Transform GridRoot => _gridRoot;

    // Convierte fila/columna a posición world.
    // Fila 0 = arriba
    public Vector3 GridToWorld(int row, int col)
    {
        float x = col * _config.cellWidth - (_config.cols - 1) * _config.cellWidth / 2f;
        float y = -row * _config.cellHeight + (_config.rows - 1) * _config.cellHeight / 2f;

        return _gridRoot.position + new Vector3(x, y, 0f);
    }

    // Instancia un bloque en la posición indicada.
    public BlockView SpawnBlock(BlockColor color, int row, int col)
    {
        Vector3 worldPos = GridToWorld(row, col);

        BlockView block = Instantiate(_blockPrefab, worldPos, Quaternion.identity, _gridRoot);

        Sprite sprite = _config.blockSprites[(int)color];

        block.Initialize(color, new Vector2Int(row, col), sprite);

        // Orden de render Fila
        block.GetComponent<SpriteRenderer>().sortingOrder = _config.rows - row;

        return block;
    }

    // Genera el grid completo con colores aleatorios.
    // Retorna la matriz de vistas para que el controlador la maneje.
    public BlockView[,] BuildGrid(GridModel model)
    {
        var views = new BlockView[model.Rows, model.Cols];

        for (int row = 0; row < model.Rows; row++)
            for (int col = 0; col < model.Cols; col++)
            {
                var color = model.GetColor(row, col)!.Value;
                views[row, col] = SpawnBlock(color, row, col);
            }

        return views;
    }

    private BlockColor RandomColor()
    {
        int count = System.Enum.GetValues(typeof(BlockColor)).Length;
        return (BlockColor)Random.Range(0, count);
    }
}