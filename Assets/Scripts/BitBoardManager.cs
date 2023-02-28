using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BitBoard;

/// <summary>
/// Manages bit-board representation of the game.
/// </summary>
public class BitBoardManager
{
    // Masks used to determine which pieces can move left or right.
    private const int MoveMask_DownLeft = unchecked((int)0b01110000011100000111000001110000),
        MoveMask_DownRight = unchecked((int)0b00001110000011100000111000000000),
        MoveMask_UpLeft = unchecked((int)0b00001110000011100000111000001110),
        MoveMask_UpRight = unchecked((int)0b00000000011100000111000001110000);
    public BitBoard bitBoard;

    public BitBoardManager(BitBoard newBoard)
    {
        bitBoard = newBoard;
    }

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

    public int GetMovable(PieceType color)
    {
        int freeSpaces = ~(bitBoard.pieces[0] | bitBoard.pieces[1]);
        int queens = (bitBoard.pieces[(int)color]) & bitBoard.pieces[2];
        int canMove;

        switch ((int)color)
        {
            case 0:
                canMove = GetMovableUp(freeSpaces, bitBoard.pieces[0]);

                if (queens != 0)
                {
                    canMove |= GetMovableDown(freeSpaces, queens);
                }
                return canMove;
            case 1:
                canMove = GetMovableDown(freeSpaces, bitBoard.pieces[1]);

                if (queens != 0)
                {
                    canMove |= GetMovableUp(freeSpaces, queens);
                }
                return canMove;
            default:
                Debug.LogError("Wrong color index!");
                return 0;
        }
    }

    private int GetMovableUp(int freeSpaces, int pieces)
    {
        int canMove = (freeSpaces << 4) & pieces;
        canMove |= ((freeSpaces & MoveMask_UpLeft) << 3) & pieces;
        canMove |= ((freeSpaces & MoveMask_UpRight) << 5) & pieces;

        return canMove;
    }

    private int GetMovableDown(int freeSpaces, int pieces)
    {
        int canMove = (freeSpaces >> 4) & pieces;
        canMove |= ((freeSpaces & MoveMask_DownLeft) >> 3) & pieces;
        canMove |= ((freeSpaces & MoveMask_DownRight) >> 5) & pieces;

        return canMove;
    }
}
