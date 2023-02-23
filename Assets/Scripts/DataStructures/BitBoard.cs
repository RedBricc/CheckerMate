using System.Collections;

/// <summary>
/// Stores information about the game board in bit-board representation.
/// </summary>
public struct BitBoard
{
    public int[] pieces;
    public enum PieceType { whitePieces, blackPieces, queenPieces };

    /// <summary>
    /// Initialize new bit-board.
    /// </summary>
    /// <param name="newWhite">Integer representing white piece locations on board.</param>
    /// <param name="newBlack">Integer representing black piece locations on board.</param>
    /// <param name="newQueens">Integer representing queen piece locations on board.</param>
    public BitBoard(int newWhite, int newBlack, int newQueens)
    {
        pieces = new int[3] { newWhite, newBlack, newQueens };
    }
}