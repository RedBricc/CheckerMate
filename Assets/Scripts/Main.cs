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
    public DisplayManager displayManager;
    public BitBoardManager bitBoardManager;
    private BitBoard bitBoard;

    void Awake()
    {
        Piece.mainController = this;
    }

    void Start()
    {
        // Load past game if one exists, otherwise set up a new game.
        bitBoard = new BitBoard(PlayerPrefs.GetInt("savedWhitePieces", unchecked((int)0b11111111111100000000000000000000)), PlayerPrefs.GetInt("savedBlackPieces", unchecked((int)0b00000000000000000000111111111111)), PlayerPrefs.GetInt("savedQueenPieces", 0));
        bitBoardManager = new BitBoardManager(bitBoard);

        displayManager = new DisplayManager(
            bitBoard.pieces[(int)PieceType.whitePieces],
            bitBoard.pieces[(int)PieceType.blackPieces],
            bitBoard.pieces[(int)PieceType.queenPieces],
            transform,
            GameObject.FindWithTag("Pieces").transform);
    }
}