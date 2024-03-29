using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BitBoard;

/// <summary>
/// Converts bit-board state to checkers pieces and displays it on the screen using a standard checkers board.
/// </summary>
public class DisplayManager
{
    public static Camera mainCamera;
    private Piece[] pieces = new Piece[24];
    private static GameObject[] moveDots = new GameObject[32];
    private static Dictionary<int, Vector2Int> positionDictionary = new Dictionary<int, Vector2Int>();
    private static Dictionary<Vector2Int, int> reversePositionDictionary = new Dictionary<Vector2Int, int>();
    private static GameObject blankBoard, blankPiece, moveDot;
    private static Color boardColor;
    private static Sprite boardSprite;
    private SpriteRenderer board;

    // Initialize static values.
    static DisplayManager()
    {
        // Cash main camera.
        mainCamera = Camera.main;

        // Fill position dictionary with board positions that correspond to their bit-board representations.
        for (int i = 0; i < 32; i++)
        {
            positionDictionary.Add(Main.bytePositions[i], new Vector2Int((i % 4) * 2 + (i / 4) % 2, i / 4));
            reversePositionDictionary.Add(new Vector2Int((i % 4) * 2 + (i / 4) % 2, i / 4), Main.bytePositions[i]);
        }

        // Load necessary prefabs from file.
        blankBoard = Resources.Load("Prefabs/Board") as GameObject;
        blankPiece = Resources.Load("Prefabs/Piece") as GameObject;
        moveDot = Resources.Load("Prefabs/MoveDot") as GameObject;

        // Load player's visual preferences for board.
        Sprite blankBoardSprite = blankBoard.GetComponent<SpriteRenderer>().sprite;
        boardSprite = Sprite.Create(Resources.Load(PlayerPrefs.GetString("preferredBoardTexture", "Textures/Board")) as Texture2D, blankBoardSprite.rect, new Vector2(0f, 0f), blankBoardSprite.pixelsPerUnit);
        ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("preferredBoardColor", "#73B061"), out boardColor);

        // Load player's preferred textures and colors for pieces.
        Sprite blankPieceSprite = blankPiece.GetComponentInChildren<SpriteRenderer>().sprite;
        Piece.PIECE = Sprite.Create(Resources.Load(PlayerPrefs.GetString("preferredPieceTexture", "Textures/Piece")) as Texture2D, blankPieceSprite.rect, new Vector2(0.5f, 0.5f), blankPieceSprite.pixelsPerUnit);
        Piece.QUEEN = Sprite.Create(Resources.Load(PlayerPrefs.GetString("preferredQueenTexture", "Textures/Queen")) as Texture2D, blankPieceSprite.rect, new Vector2(0.5f, 0.5f), blankPieceSprite.pixelsPerUnit);
        ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("preferredLightColor", "#FFD7B6"), out Piece.LIGHT);
        ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("preferredLightColor", "#FFF1E6"), out Piece.LIGHT_MOVABLE);
        ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("preferredLightColor", "#FF9369"), out Piece.LIGHT_CAN_CAPTURE);
        ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("preferredDarkColor", "#261107"), out Piece.DARK);
        ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("preferredDarkColor", "#4D2614"), out Piece.DARK_MOVABLE);
        ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("preferredDarkColor", "#873007"), out Piece.DARK_CAN_CAPTURE);
    }

    /// <summary>
    /// Initialize board and it's pieces.
    /// </summary>
    /// <param name="whitePieces">Integer representation of initial white piece layout.</param>
    /// <param name="blackPieces">Integer representation of initial black piece layout.</param>
    /// <param name="queenPieces">Integer representation of initial queen piece layout.</param>
    /// <param name="boardParent">Board prefab.</param>
    /// <param name="pieceParent">Parent to place pieces under.</param>
    public DisplayManager(int whitePieces, int blackPieces, int queenPieces, Transform boardParent, Transform pieceParent, Transform moveDotParent)
    {
        // Generate board.
        board = GameObject.Instantiate<GameObject>(blankBoard, boardParent).GetComponent<SpriteRenderer>();
        board.sprite = boardSprite;
        board.color = boardColor;

        // Place move dots.
        int moveDotIndex = 0;

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                moveDots[moveDotIndex] = GameObject.Instantiate<GameObject>(moveDot, new Vector2(i * 2 + (j % 2), j), Quaternion.identity, moveDotParent);
                moveDots[moveDotIndex].SetActive(false);
                moveDotIndex++;
            }
        }

        // Initialize pieces
        Vector2[] whitePositions = IntToPositions(whitePieces);
        for (int i = 0; i < whitePositions.Length; i++)
        {
            pieces[i] = GameObject.Instantiate<GameObject>(blankPiece, whitePositions[i], Quaternion.identity, pieceParent).GetComponent<Piece>();
            pieces[i].Initialize(PieceType.whitePieces, Main.bytePositions[i], ((queenPieces & whitePieces & Main.bytePositions[i]) != 0) ? true : false);
        }

        Vector2[] blackPositions = IntToPositions(blackPieces);
        for (int i = 0; i < blackPositions.Length; i++)
        {
            pieces[i + 12] = GameObject.Instantiate<GameObject>(blankPiece, blackPositions[i], Quaternion.identity, pieceParent).GetComponent<Piece>();
            pieces[i + 12].Initialize(PieceType.blackPieces, Main.bytePositions[i + 20], ((queenPieces & blackPieces & Main.bytePositions[i + 20]) != 0) ? true : false);
        }
    }

    /// <summary>
    /// Transforms int bit-board representation to positions on game board.
    /// </summary>
    /// <param name="pieceMap">Bit-board representation as int.</param>
    /// <returns>Array of piece positions on game board.</returns>
    public static Vector2[] IntToPositions(int pieceMap)
    {
        List<Vector2> piecePositions = new List<Vector2>();

        for (int i = 0; i < 32; i++)
        {
            int key = pieceMap & Main.bytePositions[i];
            Vector2Int value = positionDictionary.GetValueOrDefault(key, new Vector2Int(-1, 0));

            if (value.x != -1)
            {
                piecePositions.Add(value);
            }
        }

        return piecePositions.ToArray();
    }

    /// <summary>
    /// Transforms position on game board to bit-board int.
    /// </summary>
    /// <param name="position">Position on game board.</param>
    /// <returns>Int bit-board representation of single piece.</returns>
    public static int PositionToInt(Vector2Int position)
    {
        int result = reversePositionDictionary.GetValueOrDefault(position, -1);
        if (result == -1)
        {
            Debug.LogError("Tried to convert a board position, that doesn't exist!");
        }

        return result;
    }

    /// <summary>
    /// Displays dots on board to show possible moves.
    /// </summary>
    /// <param name="moves">Bit board representation of moves.</param>
    public static void DisplayMoveDots(int moves)
    {
        for (int i = 0; i < 32; i++)
        {
            moveDots[i].SetActive((moves & Main.bytePositions[i]) != 0);
        }
    }

    /// <summary>
    /// Updates piece visuals based on current game state.
    /// </summary>
    /// <param name="whitePieces">Integer representation of current white piece layout.</param>
    /// <param name="blackPieces">Integer representation of current black piece layout.</param>
    /// <param name="queenPieces">Integer representation of current queen piece layout.</param>
    public void UpdatePieces(int whitePieces, int blackPieces, int queenPieces)
    {
        if (Main.isWhiteTurn)
        {
            UpdatePiecesOfColor(PieceType.whitePieces, whitePieces, queenPieces);
        }
        else
        {
            UpdatePiecesOfColor(PieceType.blackPieces, blackPieces, queenPieces);
        }
    }

    public void ClearPieceColors()
    {
        for (int i = 0; i < 24; i++)
        {
            pieces[i].SetMovable(false);
            pieces[i].SetCapture(false);
        }
    }

    /// <summary>
    private void UpdatePiecesOfColor(PieceType pieceColor, int pieceMap, int queenMap)
    {
        int movablePieces = Main.bitBoardManager.GetMovable(pieceColor);
        int capturingPieces = Main.bitBoardManager.GetCapturing(pieceColor);

        for (int i = 12 * (int)pieceColor; i < 12 * ((int)pieceColor + 1); i++)
        {
            if ((pieceMap & pieces[i].curBitBoardPos) != 0)
            {
                pieces[i].SetQueen((queenMap & pieceMap & pieces[i].curBitBoardPos) != 0);

                if (capturingPieces == 0)
                {
                    // If there are no capturing moves, all movable pieces are also movable pieces.
                    pieces[i].SetCapture(false);
                    pieces[i].SetMovable((movablePieces & pieces[i].curBitBoardPos) != 0);
                }
                else
                {
                    // If there are capturing moves, only those pieces that can capture are movable.
                    pieces[i].SetMovable(false);
                    pieces[i].SetCapture((capturingPieces & pieces[i].curBitBoardPos) != 0);
                }
            }
            else
            {
                pieces[i].gameObject.SetActive(false);
            }
        }
    }
}
