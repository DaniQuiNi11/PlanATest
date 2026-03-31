using UnityEngine;

public interface IBlock
{
    BlockColor Color { get; }
    Vector2Int GridPosition { get; }
    void Initialize(BlockColor color, Vector2Int gridPosition, Sprite sprite);
    void SetGridPosition(Vector2Int newPosition, Vector3 worldPosition);
}