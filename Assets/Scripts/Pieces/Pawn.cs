﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private void Awake()
    {
        piecePhotonView = GetComponent<PhotonView>();
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

        Vector2Int forwardOne = new Vector2Int(piecePosition.x + 1, piecePosition.y); //move one square
        if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 1, piecePosition.y))
        {
            Debug.Log("one");
            locations.Add(forwardOne);
        }

        if (piecePosition.x == 1)
        {
            Vector2Int forwardTwo = new Vector2Int(piecePosition.x + 2, piecePosition.y); //move two squares
            if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 2, piecePosition.y))
            {
                Debug.Log("two");
                locations.Add(forwardTwo);
            }
        }

        if (piecePosition.y != 7 && piecePosition.x != 7)
        {
            Vector2Int diagonalRight = new Vector2Int(piecePosition.x + 1, piecePosition.y + 1); //attack
            if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 1, piecePosition.y + 1) == false)
            {
                Debug.Log("right");
                locations.Add(diagonalRight);
            }
        }
        if (piecePosition.y != 0 && piecePosition.x != 7)
        {
            Vector2Int diagonalLeft = new Vector2Int(piecePosition.x + 1, piecePosition.y - 1); //attack
            if (ARChessGameManager.instance.CheckIfPositionIsFree(piecePosition.x + 1, piecePosition.y - 1) == false)
            {
                Debug.Log("left");
                locations.Add(diagonalLeft);
            }
        }
            
        return locations;
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

        ARChessGameManager.instance.MovePiece(myPiece, targetPosition);
        ARChessGameManager.pieces[oppositePosition.x, oppositePosition.y] = myPiece; //the pawn may have moved one square or two squares
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);
        //ARChessGameManager.PrintPieces();
    }

    [PunRPC]
    private void CapturePieceForOpponent(string tag, int x, int y)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(tag);

        Vector2Int pos = new Vector2Int(7 - x, 7 - y);
        ARChessGameManager.instance.CapturePieceAt(pos);
        Vector3 targetPosition = Geometry.PointFromGrid(pos);
        ARChessGameManager.instance.MovePiece(myPiece, targetPosition);
        ARChessGameManager.pieces[pos.x, pos.y] = myPiece;
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);
    }
}
