﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    private GameObject chessBoard;

    private void Awake()
    {
        piecePhotonView = GetComponent<PhotonView>();
        chessBoard = GameObject.FindGameObjectWithTag("ChessBoard");
    }

    void Start()
    {
        selected = false;
    }

    public override PhotonView GetPhotonView()
    {
        return piecePhotonView;
    }

    public override List<Vector2Int> MoveLocations(Vector2Int piecePosition)
    {
        bool continueLeft = true;
        bool continueRight = true;
        bool continueForward = true;
        bool continueBackward = true;

        List<Vector2Int> locations = new List<Vector2Int>();

        for (int i = 1; i < 8; i++)
        {
            if (continueForward)
            {
                if (piecePosition.x + i < 8)
                {
                    Vector2Int possibleLocationForward = new Vector2Int(piecePosition.x + i, piecePosition.y); //move forward
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, possibleLocationForward.x, possibleLocationForward.y))
                    {
                        locations.Add(possibleLocationForward);
                    }
                    else
                    {
                        if (ARChessGameManager.instance.GetPieceAtPosition(possibleLocationForward.x, possibleLocationForward.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                        {
                            locations.Add(possibleLocationForward);
                        }
                        continueForward = false;
                    }
                }
            }

            if (continueBackward)
            {
                if (piecePosition.x - i >= 0)
                {
                    Vector2Int possibleLocationBackward = new Vector2Int(piecePosition.x - i, piecePosition.y); //move backwards
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, possibleLocationBackward.x, possibleLocationBackward.y))
                    {
                        locations.Add(possibleLocationBackward);
                    }
                    else
                    {
                        if (ARChessGameManager.instance.GetPieceAtPosition(possibleLocationBackward.x, possibleLocationBackward.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                        {
                            locations.Add(possibleLocationBackward);
                        }
                        continueBackward = false;
                    }
                }
            }
            
            if (continueLeft)
            {
                if (piecePosition.y - i >= 0)
                {
                    Vector2Int possibleLocationLeft = new Vector2Int(piecePosition.x, piecePosition.y - i); //move left
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, possibleLocationLeft.x, possibleLocationLeft.y))
                    {
                        locations.Add(possibleLocationLeft);
                    }
                    else
                    {
                        if (ARChessGameManager.instance.GetPieceAtPosition(possibleLocationLeft.x, possibleLocationLeft.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                        {
                            locations.Add(possibleLocationLeft);
                        }
                        continueLeft = false;
                    }
                }
            }

            if (continueRight)
            {
                if (piecePosition.y + i < 8)
                {
                    Vector2Int possibleLocationRight = new Vector2Int(piecePosition.x, piecePosition.y + i); //move right
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, possibleLocationRight.x, possibleLocationRight.y))
                    {
                        locations.Add(possibleLocationRight);
                    }
                    else
                    {
                        if (ARChessGameManager.instance.GetPieceAtPosition(possibleLocationRight.x, possibleLocationRight.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                        {
                            locations.Add(possibleLocationRight);
                        }
                        continueRight = false;
                    }
                }
            }
        }

        return locations;
    }

    public override void GetAttackLocations(bool isForTemporaryCheck, GameObject[,] arrayWithPieces, Vector2Int currentPosition)
    {
        bool continueLeft = true;
        bool continueRight = true;
        bool continueForward = true;
        bool continueBackward = true;

        for (int i = 1; i < 8; i++)
        {
            if (continueForward) //backward in mirros
            {
                if (currentPosition.x - i >= 0)
                {
                    Vector2Int possibleLocationForward = new Vector2Int(currentPosition.x - i, currentPosition.y); //move forward

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - i, currentPosition.y);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, possibleLocationForward.x, possibleLocationForward.y) == false)
                    {
                        continueForward = false;
                    }
                }
            }

            if (continueBackward) //forward in mirror
            {
                if (currentPosition.x + i < 8)
                {
                    Vector2Int possibleLocationBackward = new Vector2Int(currentPosition.x + i, currentPosition.y); //move backwards

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + i, currentPosition.y);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, possibleLocationBackward.x, possibleLocationBackward.y) == false)
                    {
                        continueBackward = false;
                    }
                }
            }

            if (continueLeft) //right in mirror
            {
                if (currentPosition.y + i < 8)
                {
                    Vector2Int possibleLocationLeft = new Vector2Int(currentPosition.x, currentPosition.y + i); //move left

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x, currentPosition.y + i);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, possibleLocationLeft.x, possibleLocationLeft.y) == false)
                    {
                        continueLeft = false;
                    }
                }
            }

            if (continueRight) //left in mirror
            {
                if (currentPosition.y - i >= 0)
                {
                    Vector2Int possibleLocationRight = new Vector2Int(currentPosition.x, currentPosition.y - i); //move right

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x, currentPosition.y - i);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, possibleLocationRight.x, possibleLocationRight.y) == false)
                    {
                        continueRight = false;
                    }
                }
            }
        }
    }

    [PunRPC]
    private void MovePieceForOpponent(string tag, int x, int y)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(ARChessGameManager.pieces, tag);

        Vector3 initialPosition = myPiece.transform.position;
        Vector2Int oppositePosition = new Vector2Int(7 - x, 7 - y);
        Vector3 targetPosition = Geometry.PointFromGrid(oppositePosition);

        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
        ARChessGameManager.pieces[oppositePosition.x, oppositePosition.y] = myPiece; //the rook may have moved in any straight direction
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);

        ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
        ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);
    }

    [PunRPC]
    private void CapturePieceForOpponent(string tag, int x, int y)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(ARChessGameManager.pieces, tag);

        Vector2Int pos = new Vector2Int(7 - x, 7 - y);
        ARChessGameManager.instance.CapturePieceAt(pos);
        Vector3 targetPosition = Geometry.PointFromGrid(pos);
        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
        ARChessGameManager.pieces[pos.x, pos.y] = myPiece;
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);

        ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
        ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);
    }

    [PunRPC]
    private void SwitchPlayer()
    {
        ARChessGameManager.instance.ChangePlayer();
    }

}
