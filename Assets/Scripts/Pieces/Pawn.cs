﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
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
        List<Vector2Int> locations = new List<Vector2Int>();

        if (piecePosition.x != 7)
        {
            Vector2Int forwardOne = new Vector2Int(piecePosition.x + 1, piecePosition.y); //move one square
            if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 1, piecePosition.y))
            {
                //Debug.Log("one");
                locations.Add(forwardOne);

                if (piecePosition.x == 1)
                {
                    Vector2Int forwardTwo = new Vector2Int(piecePosition.x + 2, piecePosition.y); //move two squares
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 2, piecePosition.y))
                    {
                        //Debug.Log("two");
                        locations.Add(forwardTwo);
                    }
                }
            }
        }

        if (piecePosition.y != 7 && piecePosition.x != 7)
        {
            Vector2Int diagonalRight = new Vector2Int(piecePosition.x + 1, piecePosition.y + 1); //attack
            if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 1, piecePosition.y + 1) == false)
            {
                //Debug.Log("right");
                locations.Add(diagonalRight);

            }
        }
        if (piecePosition.y != 0 && piecePosition.x != 7)
        {
            Vector2Int diagonalLeft = new Vector2Int(piecePosition.x + 1, piecePosition.y - 1); //attack
            if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 1, piecePosition.y - 1) == false)
            {
                //Debug.Log("left");
                locations.Add(diagonalLeft);

            }
        }
            
        return locations;
    }

    public override void GetAttackLocations(Vector2Int currentPosition)
    {
        if (currentPosition.x != 0 && currentPosition.y != 7) //diagonal left in mirror
        {
            if (ARChessGameManager.instance.CheckIfPositionIsFree(currentPosition.x - 1, currentPosition.y + 1) == false)
            {
                ARChessGameManager.instance.SetAttackSquare(currentPosition.x - 1, currentPosition.y + 1);
            }
        }

        if (currentPosition.x != 0 && currentPosition.y != 0) //diagonal right in mirror
        {
            if (ARChessGameManager.instance.CheckIfPositionIsFree(currentPosition.x - 1, currentPosition.y - 1) == false)
            {
                ARChessGameManager.instance.SetAttackSquare(currentPosition.x - 1, currentPosition.y - 1);
            }
        }
    }

    [PunRPC]
    private void MovePieceForOpponent(string tag, int x, int y)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(tag);
        //Debug.Log(coordinates);

        Vector3 initialPosition = myPiece.transform.position;
        Vector2Int oppositePosition = new Vector2Int(7 - x, 7 - y);
        Vector3 targetPosition = Geometry.PointFromGrid(oppositePosition);

        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
        ARChessGameManager.pieces[oppositePosition.x, oppositePosition.y] = myPiece; //the pawn may have moved one square or two squares
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);
        //ARChessGameManager.PrintPieces();

        ARChessGameManager.instance.RefreshAttackedSquares();
        ARChessGameManager.instance.VerifyForCheck();
    }

    [PunRPC]
    private void CapturePieceForOpponent(string tag, int x, int y)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(tag);

        Vector2Int pos = new Vector2Int(7 - x, 7 - y);
        ARChessGameManager.instance.CapturePieceAt(pos);
        Vector3 targetPosition = Geometry.PointFromGrid(pos);
        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
        ARChessGameManager.pieces[pos.x, pos.y] = myPiece;
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);

        ARChessGameManager.instance.RefreshAttackedSquares();
        ARChessGameManager.instance.VerifyForCheck();
    }

    [PunRPC]
    private void SwitchPlayer()
    {
        ARChessGameManager.instance.ChangePlayer();
    }

    [PunRPC]
    private void TransformPawnInQueen(string tag, string newTag)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Mesh meshInstance = Instantiate(MoveSelector.instance.queenMesh);
        myPiece.GetComponent<MeshFilter>().sharedMesh = meshInstance;
        Destroy(myPiece.GetComponent<Pawn>());
        myPiece.gameObject.AddComponent<Queen>();

        myPiece.tag = newTag;
    }

    
}
