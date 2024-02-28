using System;
using System.Text;
using UnityEngine;

/* Responsible for aligning objects to round positions */
[ExecuteInEditMode]
public class GridObject : MonoBehaviour
{
    public Vector3 GridPosition { get; private set; }
    public const float Z_DEPTH = 0f;

    private StringBuilder _stringBuilder = new StringBuilder(8);

    private void Start()
    {
        AdjustPosition();
    }

    void Update()
    {
#if UNITY_EDITOR
        AdjustPosition();
        UpdateLabel();
#endif

    }

    private void UpdateLabel()
    {
        _stringBuilder.Clear();
        gameObject.name = _stringBuilder.Append($"T ({GridPosition.y},{GridPosition.x})").ToString();
    }

    private void AdjustPosition()
    {
        GridPosition = new(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y),
            Z_DEPTH);

        transform.position = GridPosition;
    }
}
