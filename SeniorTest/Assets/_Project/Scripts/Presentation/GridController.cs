using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private GridFactory _factory;
    [SerializeField] private GameUI _hud;
    [SerializeField] private GameOverScreen _gameOverScreen;

    private GridModel _model;
    private ScoreSystem _score;
    private BlockView[,] _views;
    private bool _inputEnabled;
    private bool _processing;

    private void Start()
    {
        InitSystems();
        StartGame();
    }

    private void InitSystems()
    {
        var palette = (BlockColor[])System.Enum.GetValues(typeof(BlockColor));

        _model = new GridModel(_config.rows, _config.cols, palette);
        _score = new ScoreSystem(_config.initialMoves);

        _model.OnBlocksCollected += HandleBlocksCollected;
        _model.OnBlocksFallen += HandleBlocksFallen;
        _model.OnBlocksSpawned += HandleBlocksSpawned;

        _score.OnScoreChanged += _hud.UpdateScore;
        _score.OnMovesChanged += _hud.UpdateMoves;
        _score.OnMovesExhausted += TriggerGameOver;
    }

    private void StartGame()
    {
        foreach (Transform child in _factory.GridRoot)
            if (child.GetComponent<BlockView>() != null)
                Destroy(child.gameObject);

        _model.Initialize();
        _views = _factory.BuildGrid(_model);

        _hud.UpdateScore(_score.Score);
        _hud.UpdateMoves(_score.MovesLeft);

        _inputEnabled = true;
        _processing = false;
    }

    private void Update()
    {
        if (!_inputEnabled || _processing) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            HandleInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            HandleInput(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position));
#endif
    }

    private void HandleInput(Vector3 worldPos)
    {
        worldPos.z = 0f;
        var hit = Physics2D.OverlapPoint(worldPos);
        if (hit == null) return;

        if (hit.TryGetComponent<BlockView>(out var block))
            OnBlockTapped(block.GridPosition);
    }

    private void OnBlockTapped(Vector2Int gridPos)
    {
        var group = _model.GetConnectedGroup(gridPos);
        if (group.Count == 0) return;

        _processing = true;
        _score.UseMove();
        _score.RegisterCollection(group.Count);
        _model.CollectBlocks(group);

        StartCoroutine(RefillSequence());
    }

    private IEnumerator RefillSequence()
    {
        yield return new WaitForSeconds(1f);
        _model.ApplyGravity();
        _model.Refill();
        _processing = false;
    }

    private void TriggerGameOver()
    {
        _inputEnabled = false;
        _gameOverScreen.Show();
    }

    public void Replay()
    {
        _gameOverScreen.Hide();
        _score.Reset();
        StartGame();
    }

    private void HandleBlocksCollected(List<Vector2Int> cells)
    {
        int pending = cells.Count;

        foreach (var cell in cells)
        {
            if (_views[cell.x, cell.y] == null)
            {
                pending--;
                continue;
            }

            var view = _views[cell.x, cell.y];
            _views[cell.x, cell.y] = null;

            // Destroy after animation completes
            view.PlayCollectAnimation(() =>
            {
                Destroy(view.gameObject);
                pending--;
            });
        }
    }

    private void HandleBlocksFallen(List<(Vector2Int from, Vector2Int to)> movements)
    {
        foreach (var (from, to) in movements)
        {
            var view = _views[from.x, from.y];
            if (view == null) continue;

            var targetPos = _factory.GridToWorld(to.x, to.y);

            view.SetGridPosition(to, view.transform.position);
            view.GetComponent<SpriteRenderer>().sortingOrder = _config.rows - to.x;

            _views[to.x, to.y] = view;
            _views[from.x, from.y] = null;

            // Animar
            view.PlayFallAnimation(targetPos);
        }
    }

    private void HandleBlocksSpawned(List<(Vector2Int pos, BlockColor color)> spawned)
    {
        foreach (var (pos, color) in spawned)
            _views[pos.x, pos.y] = _factory.SpawnBlock(color, pos.x, pos.y);
    }
}