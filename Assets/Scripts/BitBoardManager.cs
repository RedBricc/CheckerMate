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
    public static Dictionary<int, int[]> MoveMasks = new Dictionary<int, int[]>(),
        CaptureMasks = new Dictionary<int, int[]>();
    private static int[] diagonals = new int[15];
    // Masks used to determine which pieces can move based on position.
    private const int MoveMask_Up = unchecked((int)0b11111111111111111111111111110000),
        MoveMask_Down = unchecked((int)0b00001111111111111111111111111111),
        MoveMask_UpLeft = unchecked((int)0b00001110000011100000111000001110),
        MoveMask_UpRight = unchecked((int)0b00000000011100000111000001110000),
        MoveMask_DownLeft = unchecked((int)0b01110000011100000111000001110000),
        MoveMask_DownRight = unchecked((int)0b00001110000011100000111000000000);
    public BitBoard bitBoard;

    static BitBoardManager()
    {
        // Create diagonals.
        diagonals[0] = unchecked((int)0b00000000000000000000000010001000);
        diagonals[1] = unchecked((int)0b00000000000000001000100001000100);
        diagonals[2] = unchecked((int)0b00000000100010000100010000100010);
        diagonals[3] = unchecked((int)0b10001000010001000010001000010001);
        diagonals[4] = unchecked((int)0b01000100001000100001000100000000);
        diagonals[5] = unchecked((int)0b00100010000100010000000000000000);
        diagonals[6] = unchecked((int)0b00010001000000000000000000000000);
        diagonals[7] = unchecked((int)0b10000000000000000000000000000000);
        diagonals[8] = unchecked((int)0b01001000100000000000000000000000);
        diagonals[9] = unchecked((int)0b00100100010010001000000000000000);
        diagonals[10] = unchecked((int)0b00010010001001000100100010000000);
        diagonals[11] = unchecked((int)0b00000001000100100010010001001000);
        diagonals[12] = unchecked((int)0b00000000000000010001001000100100);
        diagonals[13] = unchecked((int)0b00000000000000000000000100010010);
        diagonals[14] = unchecked((int)0b00000000000000000000000000000001);

        // Create masks for each position on the board.
        for (int i = 0; i < 32; i++)
        {
            // Create move masks for each position on the board.
            int bytePosition = Main.bytePositions[i];
            int[] moveMasks = new int[3];

            // Move masks for white pieces.
            moveMasks[0] = (bytePosition >> 4) & MoveMask_Up;
            moveMasks[0] |= (bytePosition >> 5) & MoveMask_DownLeft;
            moveMasks[0] |= (bytePosition >> 3) & MoveMask_DownRight;

            // Move masks for black pieces.
            moveMasks[1] = (bytePosition << 4) & MoveMask_Down;
            moveMasks[1] |= (bytePosition << 5) & MoveMask_UpLeft;
            moveMasks[1] |= (bytePosition << 3) & MoveMask_UpRight;

            // Move masks for queen pieces.
            moveMasks[2] = 0;
            for (int j = 0; j < 15; j++)
            {
                int isOverlapping = bytePosition & diagonals[j];
                if (isOverlapping != 0)
                {
                    moveMasks[2] |= diagonals[j];
                }
            }
            moveMasks[2] ^= bytePosition;

            MoveMasks.Add(bytePosition, moveMasks);

            // Create capture masks for each position on the board.
            int[] captureMasks = new int[3];

            // Capture masks for white pieces.
            captureMasks[0] = (bytePosition >> 8);
            captureMasks[0] |= (bytePosition >> 7);
            captureMasks[0] |= (bytePosition >> 9);

            // Capture masks for black pieces.
            captureMasks[1] = (bytePosition << 8);
            captureMasks[1] |= (bytePosition << 7);
            captureMasks[1] |= (bytePosition << 9);

            // Capture masks for queen pieces.
            captureMasks[2] = 0;
            for (int j = 0; j < 15; j++)
            {
                int isOverlapping = bytePosition & diagonals[j];
                if (isOverlapping != 0)
                {
                    captureMasks[2] |= diagonals[j];
                }
            }
            captureMasks[2] ^= bytePosition | captureMasks[0] | captureMasks[1];

            CaptureMasks.Add(bytePosition, captureMasks);
        }
    }

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

    /// <summary>
    /// Find movable pieces of one color.
    /// </summary>
    /// <param name="color">Type of piece.</param>
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

    /// <summary>
    /// Find pieces of one color that can capture the openent's pieces.
    /// </summary>
    /// <param name="color">Type of piece.</param>
    public int GetCapturing(PieceType color)
    {
        int freeSpaces = ~(bitBoard.pieces[0] | bitBoard.pieces[1]);
        int queens = (bitBoard.pieces[(int)color]) & bitBoard.pieces[2];
        int canCapture;

        switch ((int)color)
        {
            case 0:
                canCapture = GetCapturersUp(freeSpaces, bitBoard.pieces[0], bitBoard.pieces[1]);

                if (queens != 0)
                {
                    canCapture |= GetCapturersDown(freeSpaces, queens, bitBoard.pieces[1]);
                }
                return canCapture;
            case 1:
                canCapture = GetCapturersDown(freeSpaces, bitBoard.pieces[1], bitBoard.pieces[0]);

                if (queens != 0)
                {
                    canCapture |= GetCapturersUp(freeSpaces, queens, bitBoard.pieces[0]);
                }
                return canCapture;
            default:
                Debug.LogError("Wrong color index!");
                return 0;
        }
    }

    /// <summary>
    /// Capture one piece in bit-board.
    /// </summary>
    /// <param name="piecePosition">Piece position on bit-board.</param>
    /// <param name="color">Piece color.</param>
    public void CapturePiece(int piecePosition, PieceType color)
    {
        bitBoard.pieces[(int)color] ^= piecePosition;
    }

    /// <summary>
    /// Promote one piece in bit-board.
    /// </summary>
    /// <param name="piecePosition">Piece position on bit-board.</param>
    /// <param name="color">Piece color.</param>
    public void PromotePiece(int piecePosition, PieceType color)
    {
        bitBoard.pieces[2] |= piecePosition;
    }

    /// <summary>
    /// Find legal moves for one color.
    /// </summary>
    /// <param name="color">Type of piece.</param>
    /// <returns>Tuple consisting of a bool value if there are capturing moves as well as the moves themselves.</returns>
    public Tuple<bool, int> GetLegalMoves(PieceType color)
    {
        int freeSpaces = ~(bitBoard.pieces[0] | bitBoard.pieces[1]);
        int capturing = GetCapturing(color);
        int queens = (bitBoard.pieces[(int)color]) & bitBoard.pieces[2];
        int moves = 0;

        if (capturing != 0)
        {
            // If there are capturing moves, only those are legal.
            switch ((int)color)
            {
                case 0:
                    moves = GetCapturerMovesUp(freeSpaces, capturing, bitBoard.pieces[1]);

                    return new Tuple<bool, int>(true, moves);
                case 1:
                    moves = GetCapturerMovesDown(freeSpaces, capturing, bitBoard.pieces[0]);

                    return new Tuple<bool, int>(true, moves);
                default:
                    Debug.LogError("Wrong color index!");
                    return new Tuple<bool, int>(true, moves);
            }
        }

        // If there are no capturing moves, all movable pieces are legal.
        int movable = GetMovable(color);

        switch ((int)color)
        {
            case 0:
                moves = GetMovesUp(freeSpaces, movable);
                return new Tuple<bool, int>(false, moves);
            case 1:
                moves = GetMovesDown(freeSpaces, movable);
                return new Tuple<bool, int>(false, moves);
            default:
                Debug.LogError("Wrong color index!");
                return new Tuple<bool, int>(false, 0);
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
        int canMove = ((freeSpaces >> 4) & MoveMask_Down) & pieces;
        canMove |= ((freeSpaces & MoveMask_DownLeft) >> 3) & pieces;
        canMove |= ((freeSpaces & MoveMask_DownRight) >> 5) & pieces;

        return canMove;
    }

    private int GetMovesUp(int freeSpaces, int pieces)
    {
        int moves = ((pieces & MoveMask_Up) >> 4) & freeSpaces;
        moves |= ((pieces & MoveMask_DownLeft) >> 3) & freeSpaces;
        moves |= ((pieces & MoveMask_DownRight) >> 5) & freeSpaces;

        return moves;
    }

    private int GetMovesDown(int freeSpaces, int pieces)
    {
        int moves = ((pieces & MoveMask_Down) << 4) & freeSpaces;
        moves |= ((pieces & MoveMask_UpLeft) << 3) & freeSpaces;
        moves |= ((pieces & MoveMask_UpRight) << 5) & freeSpaces;

        return moves;
    }

    private int GetCapturersUp(int freeSpaces, int ownPieces, int opponentPieces)
    {
        int canCapture = 0;
        // Find pieces that have a free space above them.
        int freeOpponentPieces = (freeSpaces << 4) & opponentPieces;

        if (freeOpponentPieces != 0)
        {
            // Find pieces that can capture them from below.
            canCapture |= ((freeOpponentPieces & MoveMask_UpLeft) << 3) & ownPieces;
            canCapture |= ((freeOpponentPieces & MoveMask_UpRight) << 5) & ownPieces;
        }

        // Find pieces that have a free space above them.
        freeOpponentPieces = ((freeSpaces & MoveMask_UpLeft) << 3) & opponentPieces;
        freeOpponentPieces |= ((freeSpaces & MoveMask_UpRight) << 5) & opponentPieces;

        // Find pieces that can capture them from below.
        canCapture |= (freeOpponentPieces << 4) & ownPieces;

        return canCapture;
    }

    private int GetCapturersDown(int freeSpaces, int ownPieces, int opponentPieces)
    {
        int canCapture = 0;
        int freeOpponentPieces = ((freeSpaces >> 4) & MoveMask_Down) & opponentPieces;

        if (freeOpponentPieces != 0)
        {
            canCapture |= ((freeOpponentPieces & MoveMask_DownLeft) >> 3) & ownPieces;
            canCapture |= ((freeOpponentPieces & MoveMask_DownRight) >> 5) & ownPieces;
        }

        freeOpponentPieces = ((freeSpaces & MoveMask_DownLeft) >> 3) & opponentPieces;
        freeOpponentPieces |= ((freeSpaces & MoveMask_DownRight) >> 5) & opponentPieces;

        canCapture |= (freeOpponentPieces >> 4) & ownPieces;

        return canCapture;
    }

    private int GetCapturerMovesUp(int freeSpaces, int capturingPieces, int opponentPieces)
    {
        // Find pieces that have a free space above them.
        int capturerMoves = 0;
        int freeOpponentPieces = (freeSpaces << 4) & opponentPieces;

        if (freeOpponentPieces != 0)
        {
            // Find the directions that the piece can capture them from below.
            capturerMoves |= (((freeOpponentPieces & MoveMask_UpLeft) << 3) & capturingPieces) >> 7;
            capturerMoves |= (((freeOpponentPieces & MoveMask_UpRight) << 5) & capturingPieces) >> 9;
        }

        // Find pieces that have a free space above them.
        int freeOpponentPiecesLeft = ((freeSpaces & MoveMask_UpLeft) << 3) & opponentPieces;
        int freeOpponentPiecesRight = ((freeSpaces & MoveMask_UpRight) << 5) & opponentPieces;

        // Find the directions that the piece can capture them from below.
        capturerMoves |= ((freeOpponentPiecesLeft << 4) & capturingPieces) >> 7;
        capturerMoves |= ((freeOpponentPiecesRight << 4) & capturingPieces) >> 9;

        return capturerMoves;
    }

    private int GetCapturerMovesDown(int freeSpaces, int capturingPieces, int opponentPieces)
    {
        // Find pieces that have a free space above them.
        int capturerMoves = 0;
        int freeOpponentPieces = ((freeSpaces >> 4) & MoveMask_Down) & opponentPieces;

        if (freeOpponentPieces != 0)
        {
            // Find the directions that the piece can capture them from below.
            capturerMoves |= (((freeOpponentPieces & MoveMask_DownLeft) >> 3) & capturingPieces) << 7;
            capturerMoves |= (((freeOpponentPieces & MoveMask_DownRight) >> 5) & capturingPieces) << 9;
        }

        // Find pieces that have a free space above them.
        int freeOpponentPiecesLeft = ((freeSpaces & MoveMask_DownLeft) >> 3) & opponentPieces;
        int freeOpponentPiecesRight = ((freeSpaces & MoveMask_DownRight) >> 5) & opponentPieces;

        // Find the directions that the piece can capture them from below.
        capturerMoves |= ((freeOpponentPiecesLeft >> 4) & capturingPieces) << 7;
        capturerMoves |= ((freeOpponentPiecesRight >> 4) & capturingPieces) << 9;

        return capturerMoves;
    }
}
