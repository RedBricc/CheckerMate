using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages large scale behavior for game loop.
/// </summary>
public class Main : MonoBehaviour
{
    public static readonly int[] bytePositions = new int[32] { -2147483648, 1073741824, 536870912, 268435456, 134217728, 67108864, 33554432, 16777216, 8388608, 4194304, 2097152, 1048576, 524288, 262144, 131072, 65536, 32768, 16384, 8192, 4096, 2048, 1024, 512, 256, 128, 64, 32, 16, 8, 4, 2, 1 };
    private DisplayManager displayManager;
    public int whitePieces;
    private int blackPieces;
    private int queenPieces;

    void Awake()
    {
        // Load past game if one exists, otherwise set up a new game.
        whitePieces = PlayerPrefs.GetInt("savedWhitePieces", -1048576);
        blackPieces = PlayerPrefs.GetInt("savedBlackPieces", 4095);
        queenPieces = PlayerPrefs.GetInt("savedQueenPieces", 0);
    }

    void Start()
    {
        displayManager = new DisplayManager(whitePieces, blackPieces, queenPieces, transform, GameObject.FindWithTag("Pieces").transform);
    }

    void Update()
    {

    }
}