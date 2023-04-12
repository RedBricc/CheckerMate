using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxNode : MonoBehaviour
{
    public int whitePieces, blackPieces, queenPieces;

    public MinMaxNode(int white, int black, int queens)
    {
        whitePieces = white;
        blackPieces = black;
        queenPieces = queens;
    }
}
