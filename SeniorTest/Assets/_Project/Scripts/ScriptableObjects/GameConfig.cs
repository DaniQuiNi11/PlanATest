using UnityEngine;

[CreateAssetMenu(menuName = "PuzzleGame/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Grid dimensions")]
    public int rows = 6;
    public int cols = 5;

    // 128px y 112px a 100ppu
    public float cellWidth = 1.28f;
    public float cellHeight = 1.12f;

    [Header("Gameplay")]
    public int initialMoves = 5;

    [Header("Block sprites")]
    public Sprite[] blockSprites;
}