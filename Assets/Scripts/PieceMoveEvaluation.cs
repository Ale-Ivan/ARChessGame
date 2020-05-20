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

    List<Tuple<Piece, double,  Vector2Int>> pieceValues = new List<Tuple<Piece, double, Vector2Int>>();

    public Tuple<Piece, double, Vector2Int> EvaluateBoard(Tuple<Piece, Vector2Int, GameObject[,]> boardGamePlan, bool isMax)
    {
        //Debug.Log('a');
        double value = 0;
        double maxValue = 0.0;
        double minValue = 0.0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (boardGamePlan.Item3[i, j] != null)
                {
                    if (isMax)
                    {
                        if (boardGamePlan.Item3[i, j].tag.StartsWith("White"))
                        {
                            value += GetPieceValue(boardGamePlan.Item3[i, j], i, j);
                        }
                    }
                    else
                    {
                        if (boardGamePlan.Item3[i, j].tag.StartsWith("Black"))
                        {
                            value += GetPieceValue(boardGamePlan.Item3[i, j], i, j);
                        }
                    }
                }
            }
        }

        //Debug.Log(boardGamePlan.Item1.gameObject.tag + " " + value + " " + boardGamePlan.Item2);

        if (pieceValues.Count == 0)
        {
            maxValue = value;
            pieceValues.Add(new Tuple<Piece, double, Vector2Int>(boardGamePlan.Item1, value, boardGamePlan.Item2));
            //Debug.Log("0: " + boardGamePlan.Item1.gameObject.tag + " " + value + " " + boardGamePlan.Item2);
        }
        else
        {
            if (isMax)
            {
                if (value > maxValue)
                {
                    maxValue = value;
                    pieceValues.Add(new Tuple<Piece, double, Vector2Int>(boardGamePlan.Item1, value, boardGamePlan.Item2));
                    //Debug.Log("max: " + maxValue + " " + boardGamePlan.Item1.gameObject.tag + " " + value + " " + boardGamePlan.Item2);
                }
            }
            else
            {
                if (value < minValue)
                {
                    minValue = value;
                    pieceValues.Add(new Tuple<Piece, double, Vector2Int>(boardGamePlan.Item1, value, boardGamePlan.Item2));
                    //Debug.Log("min: " + boardGamePlan.Item1.gameObject.tag + " " + value + " " + boardGamePlan.Item2);
                }
            }
        }

        pieceValues.Sort((x, y) => y.Item2.CompareTo(x.Item2));
        return pieceValues.ElementAt(0);


        /*Tuple<Piece, double, Vector2Int> bestElement;
        List<Tuple<Piece, double, Vector2Int>> equalValues;
        if (isMax)
        {
            //pieceValues.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            Debug.Log("max: " + maxValue);
            equalValues = pieceValues.FindAll(x => x.Item2.Equals(maxValue));
            if (equalValues.Count > 1)
            {
                Random r = new Random();
                int rInt = r.Next(0, equalValues.Count);
                bestElement = equalValues.ElementAt(rInt);
                Debug.Log("random: " + rInt);
            }
            else
            {
                bestElement = equalValues.ElementAt(0);
            }
        }
        else
        {
            //pieceValues.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            Debug.Log("min: " + minValue);

            equalValues = pieceValues.FindAll(x => x.Item2.Equals(minValue));
            if (equalValues.Count > 1)
            {
                Random r = new Random();
                int rInt = r.Next(0, equalValues.Count - 1);
                bestElement = equalValues.ElementAt(rInt);
                Debug.Log("random: " + rInt);
            }
            else
            {
                bestElement = equalValues.ElementAt(0);
            }
        }

        //Tuple<Piece, double, Vector2Int> firstElement = pieceValues.ElementAt(0);
        //Debug.Log(firstElement.Item1.gameObject.tag + " " + firstElement.Item2  + " " +firstElement.Item3);

        return bestElement;*/
    }

    Tuple<Piece, Vector2Int, GameObject[,]> evaluateTupleMin = new Tuple<Piece, Vector2Int, GameObject[,]>(null, Vector2Int.zero, null);
    Tuple<Piece, Vector2Int, GameObject[,]> evaluateTupleMax = new Tuple<Piece, Vector2Int, GameObject[,]>(null, Vector2Int.zero, null);

    public Tuple<Piece, double, Vector2Int> AlphaBetaMax(int depth, double alpha, double beta, GameObject[,] board)
    {
        if (depth == 0)
        {
            return EvaluateBoard(evaluateTupleMax, true);
        }

        List<Tuple<Piece, Vector2Int, GameObject[,]>> gamePlans;
        gamePlans = ARChessGameManager.instance.FindAllPossibleMovesForPiecesOfColor(board, ARChessGameManager.colorOfOpponent);
        //Debug.Log("max: " + gamePlans.Count);

        Tuple<Piece, double, Vector2Int> returnValue = new Tuple<Piece, double, Vector2Int>(null, alpha, Vector2Int.zero);

        foreach (Tuple<Piece, Vector2Int, GameObject[,]> tuple in gamePlans)
        {
            evaluateTupleMax = tuple;
            //Debug.Log("max: " + tuple.Item1.gameObject.tag + " " + tuple.Item2);
            GameObject[,] tempGamePlan = tuple.Item3;
            Tuple<Piece, double, Vector2Int> minimaxValue = AlphaBetaMin(depth - 1, alpha, beta, tempGamePlan);
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
            return EvaluateBoard(evaluateTupleMin, false);
        }

        List<Tuple<Piece, Vector2Int, GameObject[,]>> gamePlans;
        gamePlans = ARChessGameManager.instance.FindAllPossibleMovesForPiecesOfColor(board, ARChessGameManager.colorOfLocalPlayer);
        //Debug.Log("min: " + gamePlans.Count);

        Tuple<Piece, double, Vector2Int> returnValue = new Tuple<Piece, double, Vector2Int>(null, beta, Vector2Int.zero);

        foreach (Tuple<Piece, Vector2Int, GameObject[,]> tuple in gamePlans)
        {
            evaluateTupleMin = tuple;
            //Debug.Log("min: " + tuple.Item1.gameObject.tag + " " + tuple.Item2);

            GameObject[,] tempGamePlan = tuple.Item3;
            Tuple<Piece, double, Vector2Int> minimaxValue = AlphaBetaMax(depth - 1, alpha, beta, tempGamePlan);
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

    /*public Tuple<Piece, double, Vector2Int> minimax(int depth, int alpha, int beta, bool isMaximizingPlayer, GameObject[,] board)
    {
        if (depth == 0)
        {
            //Debug.Log(evaluateTuple.Item1.tag);
            return EvaluateBoard(evaluateTuple, isMaximizingPlayer);
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
                if (minimaxValue.Item2 >= beta)
                {
                    //bestMove = minimaxValue.Item2;
                    return new Tuple<Piece, int, Vector2Int>(tuple.Item1, beta, tuple.Item2);
                }
                //returnValue = new Tuple<Piece, int, Vector2Int>(tuple.Item1, bestMove, tuple.Item2);
                //Debug.Log(returnValue);
                if (minimaxValue.Item2 > alpha)
                {
                    alpha = minimaxValue.Item2;
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
    }*/

    private double GetPieceValue(GameObject pieceGameObject, int row, int column)
    {
        Piece piece = pieceGameObject.GetComponent<Piece>();
        string tag = pieceGameObject.tag;

        double value;

        switch (piece.type)
        {
            case PieceType.Pawn: value = 100 + (tag.StartsWith("White") ? PieceSquareTablePawnWhite1[row, column] : PieceSquareTablePawnBlack1[row, column]); break;
            case PieceType.Knight: value = 300 + (tag.StartsWith("White") ? PieceSquareTableKnightWhite1[row, column] : PieceSquareTableKnightBlack1[row, column]); break;
            case PieceType.Bishop: value = 300 + (tag.StartsWith("White") ? PieceSquareTableBishopWhite1[row, column] : PieceSquareTableBishopBlack1[row, column]); break;
            case PieceType.Rook: value = 500 + (tag.StartsWith("White") ? PieceSquareTableRookWhite1[row, column] : PieceSquareTableRookBlack1[row, column]); break;
            case PieceType.Queen: value = 900 + (tag.StartsWith("White") ? PieceSquareTableQueenWhite1[row, column] : PieceSquareTableQueenBlack1[row, column]); break;
            case PieceType.King: value = 9000 + (tag.StartsWith("White") ? PieceSquareTableKingWhite1[row, column] : PieceSquareTableKingBlack1[row, column]); break;
            default: return 0;
        }

        return tag.StartsWith(ARChessGameManager.colorOfOpponent) ? value : -value;
        //Debug.Log(piece.type);
    }
}