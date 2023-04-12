using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BitBoard;

/// <summary>
/// Manages large scale behavior for game loop.
/// </summary>
public class Main : MonoBehaviour
{
    public static readonly int[] bytePositions = new int[32] { -2147483648, 1073741824, 536870912, 268435456, 134217728, 67108864, 33554432, 16777216, 8388608, 4194304, 2097152, 1048576, 524288, 262144, 131072, 65536, 32768, 16384, 8192, 4096, 2048, 1024, 512, 256, 128, 64, 32, 16, 8, 4, 2, 1 };
    public static DisplayManager displayManager;
    public static BitBoardManager bitBoardManager;
    public static MinMax minMax;
    public static bool isWhiteTurn = true;
    private BitBoard bitBoard;

    void Start()
    {
        // Load past game if one exists, otherwise set up a new game.
        bitBoard = new BitBoard(
            PlayerPrefs.GetInt("savedWhitePieces", unchecked((int)0b11111111111100000000000000000000)),
            PlayerPrefs.GetInt("savedBlackPieces", unchecked((int)0b00000000000000000000111111111111)),
            PlayerPrefs.GetInt("savedQueenPieces", 0));
        bitBoardManager = new BitBoardManager(bitBoard);

        displayManager = new DisplayManager(
            bitBoard.pieces[(int)PieceType.whitePieces],
            bitBoard.pieces[(int)PieceType.blackPieces],
            bitBoard.pieces[(int)PieceType.queenPieces],
            transform,
            GameObject.FindWithTag("Pieces").transform,
            GameObject.FindWithTag("Move Dots").transform);

        minMax = new MinMax(bitBoard.pieces[(int)PieceType.whitePieces],
            bitBoard.pieces[(int)PieceType.blackPieces],
            bitBoard.pieces[(int)PieceType.queenPieces]);
    }

    void Update()
    {
        // Save game if player quits.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.SetInt("savedWhitePieces", bitBoard.pieces[(int)PieceType.whitePieces]);
            PlayerPrefs.SetInt("savedBlackPieces", bitBoard.pieces[(int)PieceType.blackPieces]);
            PlayerPrefs.SetInt("savedQueenPieces", bitBoard.pieces[(int)PieceType.queenPieces]);
            PlayerPrefs.Save();
            Debug.Log("Game Saved");
            Application.Quit();
            return;
        }


        // Check for game over.
        if (bitBoardManager.GetMovable(PieceType.whitePieces) == 0 || bitBoardManager.GetMovable(PieceType.blackPieces) == 0)
        {
            Debug.Log("Game Over");
            PlayerPrefs.DeleteKey("savedWhitePieces");
            PlayerPrefs.DeleteKey("savedBlackPieces");
            PlayerPrefs.DeleteKey("savedQueenPieces");
            PlayerPrefs.Save();
            return;
        }

        // Update piece colors.
        displayManager.UpdatePieces(bitBoard.pieces[(int)PieceType.whitePieces], bitBoard.pieces[(int)PieceType.blackPieces], bitBoard.pieces[(int)PieceType.queenPieces]);
    }
}