using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlockView : MonoBehaviour, IBlock
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

    //ANIMACIONES****

    //Nacen

    public void PlaySpawnAnimation()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        transform.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float ease = 1f + 2.5f * Mathf.Pow(t - 1f, 3f)
                            + 1.5f * Mathf.Pow(t - 1f, 2f);
            transform.localScale = Vector3.one * Mathf.Max(0f, ease);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    //Desaparece
    public void PlayCollectAnimation(Action onComplete)
    {
        StartCoroutine(CollectRoutine(onComplete));
    }

    private IEnumerator CollectRoutine(Action onComplete)
    {
        float duration = 0.25f;
        float punchTime = 0.08f;

        float elapsed = 0f;
        while (elapsed < punchTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / punchTime;
            transform.localScale = Vector3.LerpUnclamped(Vector3.one,
                                   Vector3.one * 1.2f, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float ease = t * t;
            transform.localScale = Vector3.LerpUnclamped(Vector3.one * 1.2f,
                                   Vector3.zero, ease);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        onComplete?.Invoke();
    }

    //Cae

    public void PlayFallAnimation(Vector3 targetPosition, Action onComplete = null)
    {
        StartCoroutine(FallRoutine(targetPosition, onComplete));
    }

    private IEnumerator FallRoutine(Vector3 targetPosition, Action onComplete)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease out — starts fast, slows at the end (gravity feel)
            float ease = 1f - (1f - t) * (1f - t);
            transform.position = Vector3.Lerp(startPos, targetPosition, ease);
            yield return null;
        }

        transform.position = targetPosition;
        onComplete?.Invoke();
    }
}