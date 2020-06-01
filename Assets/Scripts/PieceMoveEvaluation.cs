using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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

    public static double[,] PieceSquareTablePawnWhite2 = new double[8, 8]
    {
        { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
        { 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0 },
        { 1.0, 1.0, 2.0, 3.0, 3.0, 2.0, 1.0, 1.0 },
        { 0.5, 0.5, 1.0, 2.5, 2.5, 1.0, 0.5, 0.5 },
        { 0.0, 0.0, 0.0, 2.0, 2.0, 0.0, 0.0, 0.0 },
        { 0.5, -0.5, -1.0, 0.0, 0.0, -1.0, -0.5, 0.5 },
        { 0.5, 1.0, 1.0, -2.0, -2.0, 1.0, 1.0, 0.5 },
        { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 }
    };

    public static double[,] PieceSquareTableKnightWhite2 = new double[8, 8]
    {
        { -5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 },
        { -4.0, -2.0, 0.0, 0.0, 0.0, 0.0, -2.0, -4.0 },
        { -3.0, 0.0, 1.0, 1.5, 1.5, 1.0, 0.0, -3.0 },
        { -3.0, 0.5, 1.5, 2.0, 2.0, 1.5, 0.5, -3.0 },
        { -3.0, 0.0, 1.5, 2.0, 2.0, 1.5, 0.0, -3.0 },
        { -3.0, 0.5, 1.0, 1.5, 1.5, 1.0, 0.5, -3.0 },
        { -4.0, -2.0, 0.0, 0.5, 0.5, 0.0, -2.0, -4.0 },
        { -5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 }
    };

    public static double[,] PieceSquareTableBishopWhite2 = new double[8, 8]
    {
        { -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0 },
        { -1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -1.0 },
        { -1.0, 0.0, 0.5, 1.0, 1.0, 0.5, 0.0, -1.0 },
        { -1.0, 0.5, 0.5, 1.0, 1.0, 0.5, 0.5, -1.0 },
        { -1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, -1.0 },
        { -1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0 },
        { -1.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.5, -1.0 },
        { -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0 }
    };

    public static double[,] PieceSquareTableRookWhite2 = new double[8, 8]
    {
        { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
        { 0.5, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.5 },
        { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
        { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
        { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
        { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
        { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
        { 0.0, 0.0, 0.0, 0.5, 0.5, 0.0, 0.0, 0.0 }
    };

    public static double[,] PieceSquareTableQueenWhite2 = new double[8, 8]
    {
        { -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0 },
        { -1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -1.0 },
        { -1.0, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -1.0 },
        { -0.5, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -0.5 },
        { 0.0, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -0.5 },
        { -1.0, 0.5, 0.5, 0.5, 0.5, 0.5, 0.0, -1.0 },
        { -1.0, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, -1.0 },
        { -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0 }
    };

    public static double[,] PieceSquareTableKingWhite2 = new double[8, 8]
    {
        { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
        { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
        { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
        { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
        { -2.0, -3.0, -3.0, -4.0, -4.0, -3.0, -3.0, -2.0 },
        { -1.0, -2.0, -2.0, -2.0, -2.0, -2.0, -2.0, -1.0 },
        { 2.0, 2.0, 0.0, 0.0, 0.0, 0.0, 2.0, 2.0 },
        { 2.0, 3.0, 1.0, 0.0, 0.0, 1.0, 3.0, 2.0 }
    };


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

    //public static int[,] PieceSquareTablePawnBlack = ReverseArray(PieceSquareTablePawnWhite);
    public static int[,] PieceSquareTablePawnBlack1;
    //public static double[,] PieceSquareTablePawnBlack2 = ReverseDoubleArray(PieceSquareTablePawnWhite2);

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

    //public static int[,] PieceSquareTableKnightBlack = ReverseArray(PieceSquareTableKnightWhite);
    public static int[,] PieceSquareTableKnightBlack1;
    //public static double[,] PieceSquareTableKnightBlack2 = ReverseDoubleArray(PieceSquareTableKnightWhite2);


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

    //public static int[,] PieceSquareTableBishopBlack = ReverseArray(PieceSquareTableBishopWhite);
    public static int[,] PieceSquareTableBishopBlack1;
    //public static double[,] PieceSquareTableBishopBlack2 = ReverseDoubleArray(PieceSquareTableBishopWhite2);


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

    //public static int[,] PieceSquareTableRookBlack = ReverseArray(PieceSquareTableRookWhite);
    public static int[,] PieceSquareTableRookBlack1;
    //public static double[,] PieceSquareTableRookBlack2 = ReverseDoubleArray(PieceSquareTableRookWhite2);


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

    //public static int[,] PieceSquareTableQueenBlack = ReverseArray(PieceSquareTableQueenWhite);
    public static int[,] PieceSquareTableQueenBlack1;
    //public static double[,] PieceSquareTableQueenBlack2 = ReverseDoubleArray(PieceSquareTableQueenWhite2);


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

    //public static int[,] PieceSquareTableKingBlack = ReverseArray(PieceSquareTableKingWhite);
    public static int[,] PieceSquareTableKingBlack1;
    //public static double[,] PieceSquareTableKingBlack2 = ReverseDoubleArray(PieceSquareTableKingWhite2);

    private void Start()
    {
        PieceSquareTablePawnBlack1 = ReverseArray(PieceSquareTablePawnWhite1);
        PieceSquareTableKnightBlack1 = ReverseArray(PieceSquareTableKnightWhite1);
        PieceSquareTableBishopBlack1 = ReverseArray(PieceSquareTableBishopWhite1);
        PieceSquareTableRookBlack1 = ReverseArray(PieceSquareTableRookWhite1);
        PieceSquareTableQueenBlack1 = ReverseArray(PieceSquareTableQueenWhite1);
        PieceSquareTableKingBlack1 = ReverseArray(PieceSquareTableKingWhite1);
    }
    private static int[,] ReverseArray(int[,] array)
    {
        int[,] reversedArray = new int[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                reversedArray[i, j] = array[i, 7 - j];
            }
        }
        return reversedArray;
    }

    private static double[,] ReverseDoubleArray(double[,] array)
    {
        double[,] reversedArray = new double[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                reversedArray[i, j] = array[i, 7 - i];
            }
        }
        return reversedArray;
    }

    public Tuple<Piece, double, Vector2Int> EvaluateBoard(Tuple<Piece, Vector2Int, GameObject[,]> boardGamePlan, bool isMax)
    {
        double value = 0;
        double bestValue = -99999;

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

        if (value > bestValue)
        {
            bestValue = value;
            return new Tuple<Piece, double, Vector2Int>(boardGamePlan.Item1, bestValue, boardGamePlan.Item2);
        }

        return null;
    }

    Tuple<Piece, Vector2Int, GameObject[,]> evaluateTupleMin = new Tuple<Piece, Vector2Int, GameObject[,]>(null, Vector2Int.zero, null);
    Tuple<Piece, Vector2Int, GameObject[,]> evaluateTupleMax = new Tuple<Piece, Vector2Int, GameObject[,]>(null, Vector2Int.zero, null);

    public Tuple<Piece, double, Vector2Int> AlphaBetaMax(int depth, double alpha, double beta, GameObject[,] board)
    {
        if (depth == 0)
        {
            return EvaluateBoard(evaluateTupleMin, true);
        }

        List<Tuple<Piece, Vector2Int, GameObject[,]>> gamePlans;
        gamePlans = ARChessGameManager.instance.FindAllPossibleMovesForPiecesOfColor(board, ARChessGameManager.colorOfOpponent);

        Tuple<Piece, double, Vector2Int> returnValue = new Tuple<Piece, double, Vector2Int>(null, alpha, Vector2Int.zero);

        foreach (Tuple<Piece, Vector2Int, GameObject[,]> tuple in gamePlans)
        {
            evaluateTupleMax = tuple;

            Tuple<Piece, double, Vector2Int> minimaxValue = AlphaBetaMin(depth - 1, alpha, beta, tuple.Item3);
            if (minimaxValue.Item2 >= beta)
            {
                evaluateTupleMax = tuple;
                return new Tuple<Piece, double, Vector2Int>(tuple.Item1, beta, tuple.Item2);
            }
            if (minimaxValue.Item2 > alpha)
            {
                alpha = minimaxValue.Item2;
                evaluateTupleMax = tuple;
                returnValue = new Tuple<Piece, double, Vector2Int>(tuple.Item1, alpha, tuple.Item2);
            }
        }
        return returnValue;
    }

    public Tuple<Piece, double, Vector2Int> AlphaBetaMin(int depth, double alpha, double beta, GameObject[,] board)
    {
        if (depth == 0)
        {
            return EvaluateBoard(evaluateTupleMax, false);
        }

        List<Tuple<Piece, Vector2Int, GameObject[,]>> gamePlans;
        gamePlans = ARChessGameManager.instance.FindAllPossibleMovesForPiecesOfColor(board, ARChessGameManager.colorOfLocalPlayer);

        Tuple<Piece, double, Vector2Int> returnValue = new Tuple<Piece, double, Vector2Int>(null, beta, Vector2Int.zero);

        foreach (Tuple<Piece, Vector2Int, GameObject[,]> tuple in gamePlans)
        {
            evaluateTupleMin = tuple;

            Tuple<Piece, double, Vector2Int> minimaxValue = AlphaBetaMax(depth - 1, alpha, beta, tuple.Item3);
            if (minimaxValue.Item2 <= alpha)
            {
                evaluateTupleMin = tuple;
                return new Tuple<Piece, double, Vector2Int>(tuple.Item1, alpha, tuple.Item2);
            }
            if (minimaxValue.Item2 < beta)
            {
                beta = minimaxValue.Item2;
                evaluateTupleMin = tuple;
                returnValue = new Tuple<Piece, double, Vector2Int>(tuple.Item1, beta, tuple.Item2);
            }
        }

        return returnValue;
    }

    private double GetPieceValue(GameObject pieceGameObject, int row, int column)
    {
        double value;

        switch (pieceGameObject.GetComponent<Piece>().type)
        {
            case PieceType.Pawn: value = (int)PieceValues.Pawn + (tag.StartsWith("White") ? PieceSquareTablePawnWhite1[row, column] : PieceSquareTablePawnBlack1[row, column]); break;
            case PieceType.Knight: value = (int)PieceValues.Knight + (tag.StartsWith("White") ? PieceSquareTableKnightWhite1[row, column] : PieceSquareTableKnightBlack1[row, column]); break;
            case PieceType.Bishop: value = (int)PieceValues.Bishop + (tag.StartsWith("White") ? PieceSquareTableBishopWhite1[row, column] : PieceSquareTableBishopBlack1[row, column]); break;
            case PieceType.Rook: value = (int)PieceValues.Rook + (tag.StartsWith("White") ? PieceSquareTableRookWhite1[row, column] : PieceSquareTableRookBlack1[row, column]); break;
            case PieceType.Queen: value = (int)PieceValues.Queen + (tag.StartsWith("White") ? PieceSquareTableQueenWhite1[row, column] : PieceSquareTableQueenBlack1[row, column]); break;
            case PieceType.King: value = (int)PieceValues.King + (tag.StartsWith("White") ? PieceSquareTableKingWhite1[row, column] : PieceSquareTableKingBlack1[row, column]); break;
            default: return 0;
        }

        return pieceGameObject.tag.StartsWith(ARChessGameManager.colorOfOpponent) ? value : -value;
    }

}