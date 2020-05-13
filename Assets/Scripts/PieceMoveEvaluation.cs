using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PieceValues
{
    Pawn = 100,
    Knight = 320,
    Bishop = 330,
    Rook = 500,
    Queen = 900,
    King = 20000
}

public class PieceMoveEvaluation : MonoBehaviour
{
    public static PieceMoveEvaluation instance;

    private void Awake()
    {
        instance = this;
    }

    public static int[,] PieceSquareTablePawnWhite = new int[8, 8] 
    { 
        {  0,  0,  0,  0,  0,  0,  0,  0 },
        { 50, 50, 50, 50, 50, 50, 50, 50 },
        { 10, 10, 20, 30, 30, 20, 10, 10 },
        {  5,  5, 10, 25, 25, 10,  5,  5 },
        {  0,  0,  0, 20, 20,  0,  0,  0 },
        {  5, -5,-10,  0,  0,-10, -5,  5 },
        {  5, 10, 10,-20,-20, 10, 10,  5 },
        {  0,  0,  0,  0,  0,  0,  0,  0 }
    };

    public static int[,] PieceSquareTablePawnWhite1 = new int[8, 8]
    {
        {  0,  0,  0,  0,  0,  0,  0,  0 },
        {  5, 10, 10,-20,-20, 10, 10,  5 },
        {  5, -5,-10,  0,  0,-10, -5,  5 },
        {  0,  0,  0, 20, 20,  0,  0,  0 },
        {  5,  5, 10, 25, 25, 10,  5,  5 },
        { 10, 10, 20, 30, 30, 20, 10, 10 },
        { 50, 50, 50, 50, 50, 50, 50, 50 },
        {  0,  0,  0,  0,  0,  0,  0,  0 }
    };

    public static int[,] PieceSquareTablePawnBlack = ReverseArray(PieceSquareTablePawnWhite);
    public static int[,] PieceSquareTablePawnBlack1 = ReverseArray(PieceSquareTablePawnWhite1);

    public static int[,] PieceSquareTableKnightWhite = new int[8, 8]
    {
        { -50,-40,-30,-30,-30,-30,-40,-50 },
        { -40,-20,  0,  0,  0,  0,-20,-40 },
        { -30,  0, 10, 15, 15, 10,  0,-30 },
        { -30,  5, 15, 20, 20, 15,  5,-30 },
        { -30,  0, 15, 20, 20, 15,  0,-30 },
        { -30,  5, 10, 15, 15, 10,  5,-30 },
        { -40,-20,  0,  5,  5,  0,-20,-40 },
        { -50,-40,-30,-30,-30,-30,-40,-50 }
    };

    public static int[,] PieceSquareTableKnightWhite1 = new int[8, 8]
    {
        { -50,-40,-30,-30,-30,-30,-40,-50 },
        { -40,-20,  0,  5,  5,  0,-20,-40 },
        { -30,  0, 10, 15, 15, 10,  0,-30 },
        { -30,  5, 15, 20, 20, 15,  5,-30 },
        { -30,  0, 15, 20, 20, 15,  0,-30 },
        { -30,  5, 10, 15, 15, 10,  5,-30 },
        { -40,-20,  0,  0,  0,  0,-20,-40 },
        { -50,-40,-30,-30,-30,-30,-40,-50 }
    };

    public static int[,] PieceSquareTableKnightBlack = ReverseArray(PieceSquareTableKnightWhite);
    public static int[,] PieceSquareTableKnightBlack1 = ReverseArray(PieceSquareTableKnightWhite1);

    public static int[,] PieceSquareTableBishopWhite = new int[8, 8]
    {
        { -20,-10,-10,-10,-10,-10,-10,-20 },
        { -10,  0,  0,  0,  0,  0,  0,-10 },
        { -10,  0,  5, 10, 10,  5,  0,-10 },
        { -10,  5,  5, 10, 10,  5,  5,-10 },
        { -10,  0, 10, 10, 10, 10,  0,-10 },
        { -10, 10, 10, 10, 10, 10, 10,-10 },
        { -10,  5,  0,  0,  0,  0,  5,-10 },
        { -20,-10,-10,-10,-10,-10,-10,-20 }
    };

    public static int[,] PieceSquareTableBishopWhite1 = new int[8, 8]
    {
        { -20,-10,-10,-10,-10,-10,-10,-20 },
        { -10,  5,  0,  0,  0,  0,  5,-10 },
        { -10, 10, 10, 10, 10, 10, 10,-10 },
        { -10,  0, 10, 10, 10, 10,  0,-10 },
        { -10,  5,  5, 10, 10,  5,  5,-10 },
        { -10,  0,  5, 10, 10,  5,  0,-10 },
        { -10,  0,  0,  0,  0,  0,  0,-10 },
        { -20,-10,-10,-10,-10,-10,-10,-20 }
    };

    public static int[,] PieceSquareTableBishopBlack = ReverseArray(PieceSquareTableBishopWhite);
    public static int[,] PieceSquareTableBishopBlack1 = ReverseArray(PieceSquareTableBishopWhite1);

    public static int[,] PieceSquareTableRookWhite = new int[8, 8]
    {
        {  0,  0,  0,  0,  0,  0,  0,  0 },
        {  5, 10, 10, 10, 10, 10, 10,  5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        {  0,  0,  0,  5,  5,  0,  0,  0 }
    };

    public static int[,] PieceSquareTableRookWhite1 = new int[8, 8]
    {
        {  0,  0,  0,  5,  5,  0,  0,  0 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        { -5,  0,  0,  0,  0,  0,  0, -5 },
        {  5, 10, 10, 10, 10, 10, 10,  5 },
        {  0,  0,  0,  0,  0,  0,  0,  0 }
    };

    public static int[,] PieceSquareTableRookBlack = ReverseArray(PieceSquareTableRookWhite);
    public static int[,] PieceSquareTableRookBlack1 = ReverseArray(PieceSquareTableRookWhite1);

    public static int[,] PieceSquareTableQueenWhite = new int[8, 8]
    {
         { -20,-10,-10, -5, -5,-10,-10,-20 },
         { -10,  0,  0,  0,  0,  0,  0,-10 },
         { -10,  0,  5,  5,  5,  5,  0,-10 },
         {  -5,  0,  5,  5,  5,  5,  0, -5 },
         {   0,  0,  5,  5,  5,  5,  0, -5 },
         { -10,  5,  5,  5,  5,  5,  0,-10 },
         { -10,  0,  5,  0,  0,  0,  0,-10 },
         { -20,-10,-10, -5, -5,-10,-10,-20 }
    };

    public static int[,] PieceSquareTableQueenWhite1 = new int[8, 8]
    {
         { -20,-10,-10, -5, -5,-10,-10,-20 },
         { -10,  0,  5,  0,  0,  0,  0,-10 },
         { -10,  5,  5,  5,  5,  5,  0,-10 },
         {   0,  0,  5,  5,  5,  5,  0, -5 },
         {  -5,  0,  5,  5,  5,  5,  0, -5 },
         { -10,  0,  5,  5,  5,  5,  0,-10 },
         { -10,  0,  0,  0,  0,  0,  0,-10 },
         { -20,-10,-10, -5, -5,-10,-10,-20 }
    };

    public static int[,] PieceSquareTableQueenBlack = ReverseArray(PieceSquareTableQueenWhite);
    public static int[,] PieceSquareTableQueenBlack1 = ReverseArray(PieceSquareTableQueenWhite1);

    public static int[,] PieceSquareTableKingWhite = new int[8, 8]
    {
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -20,-30,-30,-40,-40,-30,-30,-20 },
        { -10,-20,-20,-20,-20,-20,-20,-10 },
        {  20, 20,  0,  0,  0,  0, 20, 20 },
        {  20, 30, 10,  0,  0, 10, 30, 20 }
    };

    public static int[,] PieceSquareTableKingWhite1 = new int[8, 8]
    {
        {  20, 30, 10,  0,  0, 10, 30, 20 },
        {  20, 20,  0,  0,  0,  0, 20, 20 },
        { -10,-20,-20,-20,-20,-20,-20,-10 },
        { -20,-30,-30,-40,-40,-30,-30,-20 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 }
    };

    public static int[,] PieceSquareTableKingBlack = ReverseArray(PieceSquareTableKingWhite);
    public static int[,] PieceSquareTableKingBlack1 = ReverseArray(PieceSquareTableKingWhite1);

    private static int[,] ReverseArray(int[,] array)
    {
        int[,] reversedArray = new int[8, 8];
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                reversedArray[i, j] = array[i, 7 - i];
            }
        }
        return reversedArray;
    }

    List<Tuple<Piece, int,  Vector2Int>> pieceValues = new List<Tuple<Piece, int, Vector2Int>>();

    public Tuple<Piece, int, Vector2Int> EvaluateBoard(Tuple<Piece, Vector2Int, GameObject[,]> boardGamePlan)
    {
        //Debug.Log('a');
        int value = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (boardGamePlan.Item3[i, j] != null)
                {
                    value += GetPieceValue(boardGamePlan.Item3[i, j], i, j);
                }
            }
        }

        pieceValues.Add(new Tuple<Piece, int, Vector2Int>(boardGamePlan.Item1, value, boardGamePlan.Item2));
        pieceValues.Sort((x, y) => y.Item2.CompareTo(x.Item2));

        //Debug.Log(pieceValues.ElementAt(0));

        return pieceValues.ElementAt(0);
    }

    Tuple<Piece, Vector2Int, GameObject[,]> evaluateTuple = new Tuple<Piece, Vector2Int, GameObject[,]>(null, Vector2Int.zero, null);

    public Tuple<Piece, int, Vector2Int> minimax(int depth, double alpha, double beta, bool isMaximizingPlayer, GameObject[,] board)
    {
        if (depth == 0)
        {
            //Debug.Log(evaluateTuple.Item1.tag);
            return EvaluateBoard(evaluateTuple);
        }

        //Debug.Log("minimax " + isMaximizingPlayer + " " + depth);

        List<Tuple<Piece, Vector2Int, GameObject[,]>> gamePlans;
        if (isMaximizingPlayer) //white player
        {
            gamePlans = ARChessGameManager.instance.FindAllPossibleMovesForPiecesOfColor(board, "White");
        }
        else //black player
        {
            gamePlans = ARChessGameManager.instance.FindAllPossibleMovesForPiecesOfColor(board, "Black");
        }

        if (isMaximizingPlayer)
        {
            int bestMove = -99999;
            Tuple<Piece, int, Vector2Int> returnValue = new Tuple<Piece, int, Vector2Int>(null, bestMove, Vector2Int.zero);
            foreach (Tuple<Piece, Vector2Int, GameObject[,]> tuple in gamePlans)
            {
                evaluateTuple = tuple;
                GameObject[,] tempGamePlan = tuple.Item3;
                Tuple<Piece, int, Vector2Int> minimaxValue = minimax(depth - 1, alpha, beta, !isMaximizingPlayer, tempGamePlan);
                if (minimaxValue.Item2 > bestMove)
                {
                    bestMove = minimaxValue.Item2;
                    returnValue = new Tuple<Piece, int, Vector2Int>(tuple.Item1, bestMove, tuple.Item2);
                }
                //returnValue = new Tuple<Piece, int, Vector2Int>(tuple.Item1, bestMove, tuple.Item2);
                //Debug.Log(returnValue);
                alpha = Math.Max(alpha, bestMove);
                if (beta <= alpha)
                {
                    return returnValue;
                }
            }
            return returnValue;
        } 
        else
        {
            int bestMove = 99999;
            Tuple<Piece, int, Vector2Int> returnValue = new Tuple<Piece, int, Vector2Int>(null, bestMove, Vector2Int.zero);
            foreach (Tuple<Piece, Vector2Int, GameObject[,]> tuple in gamePlans)
            {
                evaluateTuple = tuple;
                GameObject[,] tempGamePlan = tuple.Item3;
                Tuple<Piece, int, Vector2Int> minimaxValue = minimax(depth - 1, alpha, beta, !isMaximizingPlayer, tempGamePlan);
                if (minimaxValue.Item2 < bestMove)
                {
                    bestMove = minimaxValue.Item2;
                    returnValue = new Tuple<Piece, int, Vector2Int>(tuple.Item1, bestMove, tuple.Item2);
                }
                //returnValue = new Tuple<Piece, int, Vector2Int>(tuple.Item1, bestMove, tuple.Item2);
                //Debug.Log("ret: " + returnValue);
                beta = Math.Min(beta, bestMove);
                if (beta <= alpha)
                {
                    return returnValue;
                }
            }
            return returnValue;
        }
    }

    private int GetPieceValue(GameObject pieceGameObject, int row, int column)
    {
        Piece piece = pieceGameObject.GetComponent<Piece>();
        string tag = pieceGameObject.tag;

        switch (piece.type)
        {
            case PieceType.Pawn: return tag.StartsWith("White") ? (int)PieceValues.Pawn + PieceSquareTablePawnWhite[column, row] : -(int)PieceValues.Pawn + PieceSquareTablePawnBlack[column, row];
            case PieceType.Knight: return tag.StartsWith("White") ? (int)PieceValues.Knight + PieceSquareTableKnightWhite[column, row] : -(int)PieceValues.Knight + PieceSquareTableKnightBlack[column, row];
            case PieceType.Bishop: return tag.StartsWith("White") ? (int)PieceValues.Bishop + PieceSquareTableBishopWhite[column, row] : -(int)PieceValues.Bishop + PieceSquareTableBishopBlack[column, row];
            case PieceType.Rook: return tag.StartsWith("White") ? (int)PieceValues.Rook + PieceSquareTableRookWhite[column, row] : -(int)PieceValues.Rook + PieceSquareTableRookBlack[column, row];
            case PieceType.Queen: return tag.StartsWith("White") ? (int)PieceValues.Queen + PieceSquareTableQueenWhite[column, row] : -(int)PieceValues.Queen + PieceSquareTableQueenBlack[column, row];
            case PieceType.King: return tag.StartsWith("White") ? (int)PieceValues.King + PieceSquareTableKingWhite[column, row] : -(int)PieceValues.King + PieceSquareTableKingBlack[column, row];
            default: return 0;
        }
        //Debug.Log(piece.type);
    }
}