using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class King : Piece
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
            Vector2Int forwardOne = new Vector2Int(piecePosition.x + 1, piecePosition.y);
            locations.Add(forwardOne);

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightUp = new Vector2Int(piecePosition.x + 1, piecePosition.y + 1);
                locations.Add(diagonalRightUp);
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftUp = new Vector2Int(piecePosition.x + 1, piecePosition.y - 1);
                locations.Add(diagonalLeftUp);
            }
        }

        if (piecePosition.x != 0)
        {
            Vector2Int backwardsOne = new Vector2Int(piecePosition.x - 1, piecePosition.y);
            locations.Add(backwardsOne);

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightDown = new Vector2Int(piecePosition.x - 1, piecePosition.y + 1);
                locations.Add(diagonalRightDown);
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftDown = new Vector2Int(piecePosition.x - 1, piecePosition.y - 1);
                locations.Add(diagonalLeftDown);
            }
        }

        if (piecePosition.y != 0)
        {
            Vector2Int leftOne = new Vector2Int(piecePosition.x, piecePosition.y - 1);
            locations.Add(leftOne);
        }

        if (piecePosition.y != 7)
        {
            Vector2Int rightOne = new Vector2Int(piecePosition.x, piecePosition.y + 1);
            locations.Add(rightOne);
        }

        return locations;
    }

    public override void GetAttackLocations(Vector2Int currentPosition)
    {
        if (currentPosition.x != 0) //backward
        {
            ARChessGameManager.instance.SetAttackSquare(currentPosition.x - 1, currentPosition.y);

            if (currentPosition.y != 0) //diagonalLeftDown
            {
                ARChessGameManager.instance.SetAttackSquare(currentPosition.x - 1, currentPosition.y - 1);
            }

            if (currentPosition.y != 7) //diagonalRightDown
            {
                ARChessGameManager.instance.SetAttackSquare(currentPosition.x - 1, currentPosition.y + 1);
            }
        }

        if (currentPosition.x != 7) //forward
        {
            ARChessGameManager.instance.SetAttackSquare(currentPosition.x + 1, currentPosition.y);

            if (currentPosition.y != 0) //diagonalLeftUp
            {
                ARChessGameManager.instance.SetAttackSquare(currentPosition.x + 1, currentPosition.y - 1);
            }

            if (currentPosition.y != 7) //diagonalRightUp
            {
                ARChessGameManager.instance.SetAttackSquare(currentPosition.x + 1, currentPosition.y + 1);
            }
        }

        if (currentPosition.y != 7) //right
        {
            ARChessGameManager.instance.SetAttackSquare(currentPosition.x, currentPosition.y + 1);
        }

        if (currentPosition.y != 0) //left
        {
            ARChessGameManager.instance.SetAttackSquare(currentPosition.x, currentPosition.y - 1);
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

    
}
