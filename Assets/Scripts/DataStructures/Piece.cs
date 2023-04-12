using System;
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
    public static Color LIGHT, LIGHT_MOVABLE, LIGHT_CAN_CAPTURE, DARK, DARK_MOVABLE, DARK_CAN_CAPTURE;
    public static float speed = 10f;
    public static Sprite PIECE, QUEEN;
    public int curBitBoardPos, legalPositions;
    private Vector3 curVelocity = Vector3.zero;
    private SpriteRenderer spriteRenderer;
    private Vector2Int originalPos;
    Vector2 pieceOffset = new Vector2(-0.5f, -0.5f);
    private PieceType pieceType, pieceColor;
    private bool isMoving = false, isMovable = false, canCapture = false;

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

    /// <summary>
    /// Sets the piece's queen status.
    /// </summary>
    /// <param name="queenStatus">Is the piece a queen?</param>
    public void SetQueen(bool queenStatus)
    {
        // If the piece is already a queen, do nothing.
        if ((queenStatus && pieceType == PieceType.queenPieces) || (!queenStatus && pieceType == pieceColor))
        {
            return;
        }

        if (queenStatus)
        {
            spriteRenderer.sprite = QUEEN;
            pieceType = PieceType.queenPieces;
        }
        else
        {
            spriteRenderer.sprite = PIECE;
            pieceType = pieceColor;
        }
    }

    /// <summary>
    /// Sets the piece's movable status.
    /// </summary>
    /// <param name="movableStatus">Is the piece movable?</param>
    public void SetMovable(bool movableStatus)
    {
        // If the value is the same, do nothing.
        if (movableStatus == isMovable)
        {
            return;
        }

        isMovable = movableStatus;
        UpdateColor();
    }

    /// <summary>
    /// Sets the piece's capture status.
    /// </summary>
    /// <param name="captureStatus">Can the piece capture?</param>
    public void SetCapture(bool captureStatus)
    {
        // If the value is the same, do nothing.
        if (captureStatus == canCapture)
        {
            return;
        }

        canCapture = captureStatus;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (canCapture)
        {
            spriteRenderer.color = pieceColor == PieceType.whitePieces ? LIGHT_CAN_CAPTURE : DARK_CAN_CAPTURE;
        }
        else if (isMovable)
        {
            spriteRenderer.color = pieceColor == PieceType.whitePieces ? LIGHT_MOVABLE : DARK_MOVABLE;
        }
        else
        {
            spriteRenderer.color = pieceColor == PieceType.whitePieces ? LIGHT : DARK;
        }
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
        if ((Main.isWhiteTurn && pieceColor == PieceType.blackPieces) || (!Main.isWhiteTurn && pieceColor == PieceType.whitePieces))
        {
            return;
        }

        // check if piece can be moved.
        originalPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (isMovable || canCapture)
        {
            isMoving = true;
            spriteRenderer.sortingOrder = 20;
            Tuple<bool, int> legalMoves = Main.bitBoardManager.GetLegalMoves(pieceColor);
            int moves = legalMoves.Item2;
            int[] mask;
            if (legalMoves.Item1)
            {
                BitBoardManager.CaptureMasks.TryGetValue(curBitBoardPos, out mask);
                moves &= mask[(int)pieceType];
            }
            else
            {
                BitBoardManager.MoveMasks.TryGetValue(curBitBoardPos, out mask);
                moves &= mask[(int)pieceType];
            }
            DisplayManager.DisplayMoveDots(moves);

            legalPositions = moves;
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
            if ((closestX % 2 == closestY % 2) &&
                    closestX >= -0.01f &&
                    closestX <= 7.01f &&
                    closestY >= -0.01f &&
                    closestY <= 7.01f &&
                    Vector2.Distance(newPos, transform.position) <= 0.5f)
            {
                int bitBoardPos = DisplayManager.PositionToInt(newPos);
                if ((legalPositions & bitBoardPos) != 0)
                {
                    MoveTo(newPos);

                    if (canCapture)
                    {
                        // Remove captured piece.
                        Vector2Int moveDirection = (newPos - originalPos);
                        moveDirection.Clamp(Vector2Int.one * -1, Vector2Int.one);
                        Vector2Int movedPos = originalPos + moveDirection;
                        while (movedPos != newPos)
                        {
                            int capturedPiecePos = DisplayManager.PositionToInt(movedPos);
                            Main.bitBoardManager.CapturePiece(capturedPiecePos, pieceColor == PieceType.whitePieces ? PieceType.blackPieces : PieceType.whitePieces);
                            movedPos += moveDirection;
                        }
                    }

                    int newBitBoardPos = bitBoardPos;
                    Main.bitBoardManager.MovePiece(pieceType, curBitBoardPos, newBitBoardPos);
                    curBitBoardPos = newBitBoardPos;

                    /*
                    if ((pieceType == PieceType.whitePieces && newPos.y == 7) || (pieceType == PieceType.blackPieces && newPos.y == 0))
                    {
                        SetQueen(true);
                        Main.bitBoardManager.PromotePiece(curBitBoardPos, pieceColor);
                    }
                    */

                    Main.isWhiteTurn = !Main.isWhiteTurn;
                    Main.displayManager.ClearPieceColors();
                }
                else
                {
                    MoveTo(originalPos);
                }
            }
            else
            {
                MoveTo(originalPos);
            }
            spriteRenderer.sortingOrder = 10;
            DisplayManager.DisplayMoveDots(0);

            isMoving = false;
        }
    }
}