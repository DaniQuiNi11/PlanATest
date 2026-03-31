using System;
using System.Collections.Generic;
using UnityEngine;

public class GridModel
{
    public readonly int Rows;
    public readonly int Cols;

    private readonly BlockColor?[,] _grid;
    private readonly BlockColor[] _palette;
    private readonly System.Random _rng;

    // El Presentador escucha esto para actualizar la vista
    public event Action<List<Vector2Int>> OnBlocksCollected;
    public event Action<List<(Vector2Int from, Vector2Int to)>> OnBlocksFallen;
    public event Action<List<(Vector2Int pos, BlockColor color)>> OnBlocksSpawned;

    public GridModel(int rows, int cols, BlockColor[] palette, int? seed = null)
    {
        Rows = rows;
        Cols = cols;
        _palette = palette;
        _grid = new BlockColor?[rows, cols];
        _rng = seed.HasValue ? new System.Random(seed.Value)
                                 : new System.Random();
    }

    public void Initialize()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                _grid[r, c] = RandomColor();
    }

    public BlockColor? GetColor(int row, int col) => _grid[row, col];

    // Encuentra las celdas conectadas del mimso color
    public List<Vector2Int> GetConnectedGroup(Vector2Int origin)
    {
        var target = _grid[origin.x, origin.y];
        if (target == null) return new List<Vector2Int>();

        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        var result = new List<Vector2Int>();

        queue.Enqueue(origin);
        visited.Add(origin);

        var directions = new[]
        {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var dir in directions)
            {
                var neighbor = current + dir;
                if (!InBounds(neighbor)) continue;
                if (visited.Contains(neighbor)) continue;

                var neighborColor = _grid[neighbor.x, neighbor.y];
                if (neighborColor == null) continue;
                if (neighborColor.Value != target.Value) continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        return result;
    }

    // Elimina los bloques seleccionados y notifica al listener
    public void CollectBlocks(List<Vector2Int> cells)
    {
        foreach (var cell in cells)
            _grid[cell.x, cell.y] = null;

        OnBlocksCollected?.Invoke(cells);
    }

    // Desplaza los bloques hacia abajo columna por columna para llenar las celdas vacías. (No Phisics)
    public List<(Vector2Int from, Vector2Int to)> ApplyGravity()
    {
        var movements = new List<(Vector2Int, Vector2Int)>();

        for (int c = 0; c < Cols; c++)
        {
            int writeRow = Rows - 1;
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (_grid[r, c] == null) continue;

                if (r != writeRow)
                {
                    movements.Add((new Vector2Int(r, c),
                                   new Vector2Int(writeRow, c)));
                    _grid[writeRow, c] = _grid[r, c];
                    _grid[r, c] = null;
                }
                writeRow--;
            }
        }

        OnBlocksFallen?.Invoke(movements);
        return movements;
    }

    // Llena cualquier celda vacia con un bloque nuevo
    public List<(Vector2Int pos, BlockColor color)> Refill()
    {
        var spawned = new List<(Vector2Int, BlockColor)>();

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (_grid[r, c] == null)
                {
                    var color = RandomColor();
                    _grid[r, c] = color;
                    spawned.Add((new Vector2Int(r, c), color));
                }

        OnBlocksSpawned?.Invoke(spawned);
        return spawned;
    }

    private bool InBounds(Vector2Int pos) =>
        pos.x >= 0 && pos.x < Rows &&
        pos.y >= 0 && pos.y < Cols;

    private BlockColor RandomColor() =>
        _palette[_rng.Next(_palette.Length)];
}