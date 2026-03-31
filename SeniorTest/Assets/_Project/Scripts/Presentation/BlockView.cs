using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlockView : MonoBehaviour
{
    public BlockColor Color { get; private set; }
    public Vector2Int GridPosition { get; private set; }

    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(BlockColor color, Vector2Int gridPosition, Sprite sprite)
    {
        Color = color;
        GridPosition = gridPosition;
        _renderer.sprite = sprite;
        
        gameObject.name = $"Block [{gridPosition.x},{gridPosition.y}] {color}";
    }

    public void SetGridPosition(Vector2Int newPosition, Vector3 worldPosition)
    {
        GridPosition = newPosition;
        transform.position = worldPosition;
        gameObject.name = $"Block [{newPosition.x},{newPosition.y}] {Color}";
    }
}