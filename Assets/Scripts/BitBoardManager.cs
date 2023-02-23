using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BitBoard;

/// <summary>
/// Manages bit-board representation of the game.
/// </summary>
public class BitBoardManager
{
    public BitBoard bitBoard;

    /// <summary>
    /// Move one piece in bit-board.
    /// </summary>
    /// <param name="type">Type of piece to be moved.</param>
    /// <param name="oldPos">Bit-board position of the piece.</param>
    /// <param name="newPos">Bit-board position to be moved to.</param>
    public void MovePiece(PieceType type, int oldPos, int newPos)
    {
        bitBoard.pieces[(int)type] = (bitBoard.pieces[(int)type] ^ oldPos) | newPos;
    }
}
