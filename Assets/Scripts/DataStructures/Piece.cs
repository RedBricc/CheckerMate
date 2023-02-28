using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BitBoard;

/// <summary>
/// Checkers piece structure used for display purposes.
/// </summary>
public class Piece : MonoBehaviour
{
    private static readonly Vector3 xy = new Vector3(1, 1, 0);
    public static Main mainController;
    public static Color LIGHT, DARK;
    public static float speed = 10f;
    public static Sprite PIECE, QUEEN;
    private Vector3 curVelocity = Vector3.zero;
    private SpriteRenderer spriteRenderer;
    private Vector2Int originalPos;
    Vector2 pieceOffset = new Vector2(-0.5f, -0.5f);
    private int curBitBoardPos;
    private PieceType pieceType, pieceColor;
    private bool isMoving = false;

    void Awake()
    {
        // Find renderer for piece.
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Initializer for basic piece.
    /// </summary>
    /// <param name="color">Color type to be applied to new piece.</param>
    /// <param name="bitBoardPos">Position on bit board.</param>
    /// <param name="queenStatus">Is the piece a queen?</param>
    public void Initialize(PieceType color, int bitBoardPos, bool queenStatus)
    {
        curBitBoardPos = bitBoardPos;
        pieceColor = color;

        if (color == PieceType.whitePieces)
        {
            spriteRenderer.color = LIGHT;
        }
        else
        {
            spriteRenderer.color = DARK;
        }

        if (queenStatus)
        {
            spriteRenderer.sprite = QUEEN;
            pieceType = PieceType.queenPieces;
        }
        else
        {
            spriteRenderer.sprite = PIECE;
            pieceType = color;
        }
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

    private void OnMouseDown()
    {
        // check if piece can be moved.
        originalPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        Vector2[] allMovablePieces = DisplayManager.IntToPositions(mainController.bitBoardManager.GetMovable(pieceColor));
        for (int i = 0; i < allMovablePieces.Length; i++)
        {
            if (originalPos == allMovablePieces[i])
            {
                isMoving = true;
            }
        }

        // Display legal moves.
        if (isMoving)
        {

        }
    }

    // Move piece to mouse position while being dragged.
    private void OnMouseDrag()
    {
        if (isMoving)
        {
            Ray cameraRaycast = DisplayManager.mainCamera.ScreenPointToRay(Input.mousePosition);
            transform.position = new Vector2(cameraRaycast.origin.x, cameraRaycast.origin.y) + pieceOffset;
        }
    }

    // Place piece on closest board position if available.
    void OnMouseUp()
    {
        if (isMoving)
        {
            int closestX = Mathf.RoundToInt(transform.position.x);
            int closestY = Mathf.RoundToInt(transform.position.y);
            Vector2Int newPos = new Vector2Int(closestX, closestY);

            // Check if new position is an allowed position on the board. If not, return to original position.
            if ((closestX % 2 == closestY % 2) && closestX >= -0.01f && closestX <= 7.01f && closestY >= -0.01f && closestY <= 7.01f && Vector2.Distance(newPos, transform.position) <= 0.5f)
            {
                MoveTo(newPos);
                int newBitBoardPos = DisplayManager.PositionToInt(newPos);
                mainController.bitBoardManager.MovePiece(pieceType, curBitBoardPos, newBitBoardPos);
                curBitBoardPos = newBitBoardPos;
            }
            else
            {
                MoveTo(originalPos);
            }
        }
    }
}