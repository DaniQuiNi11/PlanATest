using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreValue;
    [SerializeField] private TMP_Text _movesValue;

    public void UpdateScore(int score) => _scoreValue.text = score.ToString();
    public void UpdateMoves(int moves) => _movesValue.text = moves.ToString();
}