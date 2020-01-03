using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Knight : Piece
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

        if (piecePosition.y <= 5)
        {
            if (piecePosition.x <= 6)
            {
                Vector2Int possibleLocationRightUp_OneRow = new Vector2Int(piecePosition.x + 1, piecePosition.y + 2);
                locations.Add(possibleLocationRightUp_OneRow);
            }
            if (piecePosition.x >= 1)
            {
                Vector2Int possibleLocationRightDown_OneRow = new Vector2Int(piecePosition.x - 1, piecePosition.y + 2);
                locations.Add(possibleLocationRightDown_OneRow);
            }
        }

        if (piecePosition.y <= 6)
        {
            if (piecePosition.x <= 5)
            {
                Vector2Int possibleLocationRightUp_TwoRows = new Vector2Int(piecePosition.x + 2, piecePosition.y + 1);
                locations.Add(possibleLocationRightUp_TwoRows);
            }
            if (piecePosition.x >= 2)
            {
                Vector2Int possibleLocationRightDown_TwoRows = new Vector2Int(piecePosition.x - 2, piecePosition.y + 1);
                locations.Add(possibleLocationRightDown_TwoRows);
            }
        }

        if (piecePosition.y >= 2)
        {
            if (piecePosition.x <= 6)
            {
                Vector2Int possibleLocationLeftUp_OneRow = new Vector2Int(piecePosition.x + 1, piecePosition.y - 2);
                locations.Add(possibleLocationLeftUp_OneRow);
            }
            if (piecePosition.x >= 1)
            {
                Vector2Int possibleLocationLeftDown_OneRow = new Vector2Int(piecePosition.x - 1, piecePosition.y - 2);
                locations.Add(possibleLocationLeftDown_OneRow);
            }
        }
        
        if (piecePosition.y >= 1)
        {
            if (piecePosition.x <= 5)
            {
                Vector2Int possibleLocationLeftUp_TwoRows = new Vector2Int(piecePosition.x + 2, piecePosition.y - 1);
                locations.Add(possibleLocationLeftUp_TwoRows);
            }
            if (piecePosition.x >= 2)
            {
                Vector2Int possibleLocationLeftDown_TwoRows = new Vector2Int(piecePosition.x - 2, piecePosition.y - 1);
                locations.Add(possibleLocationLeftDown_TwoRows);
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

        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
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
        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
        ARChessGameManager.pieces[pos.x, pos.y] = myPiece;
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);
    }

    [PunRPC]
    private void SwitchPlayer()
    {
        ARChessGameManager.instance.ChangePlayer();
    }
}
