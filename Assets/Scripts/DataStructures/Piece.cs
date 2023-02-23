using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checkers piece structure used for display purposes.
/// </summary>
public class Piece : MonoBehaviour
{
    private Vector3 curVelocity = Vector3.zero;
    private SpriteRenderer spriteRenderer;
    public static Sprite PIECE, QUEEN;
    public static Color LIGHT, DARK;
    public static float speed = 5f;
    public bool isQueen;

    void Awake()
    {
        // Find renderer for piece.
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Initializer for basic piece.
    /// </summary>
    /// <param name="sprite">Sprite that will be used for new piece</param>
    /// <param name="color">Color hue to be applied to new piece.</param>
    /// <param name="position">Initial position of piece.</param>
    /// <param name="queenStatus">Is the piece a queen?</param>
    public void Initialize(Sprite sprite, Color color, bool queenStatus)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        isQueen = queenStatus;
    }

    /// <summary>
    /// Smoothly moves the piece from one location to another.
    /// </summary>
    /// <param name="newPosition">New position of piece.</param>
    /// <param name="speed">Speed multiplier, by default the animation completes in one second.</param>
    public void MoveTo(Vector2Int newPosition)
    {
        StopCoroutine("SmoothMove");
        StartCoroutine("SmoothMove", newPosition);
    }

    // Septate function for moving is required as it is called recursively each frame until movement is completed.
    IEnumerator SmoothMove(Vector2Int newPosition)
    {
        while (Vector2.Distance(transform.position, newPosition) > 0.001f)
        {
            // Move a fraction of the distance towards target.
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime * speed);

            // Wait for one frame, then continue until target is reached.
            yield return null;
        }
    }
}