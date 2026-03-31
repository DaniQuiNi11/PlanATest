using System;

public class ScoreSystem
{
    public int Score { get; private set; }
    public int MovesLeft { get; private set; }

    //El Presentador se suscribe para actualizar la vista
    public event Action<int> OnScoreChanged;
    public event Action<int> OnMovesChanged;
    public event Action OnMovesExhausted;

    private readonly int _initialMoves;

    public ScoreSystem(int initialMoves)
    {
        _initialMoves = initialMoves;
        MovesLeft = initialMoves;
    }

    // Puntos dependen de bloques recolectados
    public void RegisterCollection(int blockCount)
    {
        Score += blockCount;
        OnScoreChanged?.Invoke(Score);
    }

    public void UseMove()
    {
        if (MovesLeft <= 0) return;

        MovesLeft--;
        OnMovesChanged?.Invoke(MovesLeft);

        if (MovesLeft == 0)
            OnMovesExhausted?.Invoke();
    }

    public void Reset()
    {
        Score = 0;
        MovesLeft = _initialMoves;
        OnScoreChanged?.Invoke(Score);
        OnMovesChanged?.Invoke(MovesLeft);
    }
}