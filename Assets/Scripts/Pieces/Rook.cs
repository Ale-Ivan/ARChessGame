using Photon.Pun;
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
                    locations.Add(possibleLocationForward);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(possibleLocationForward.x, possibleLocationForward.y) == false)
                    {
                        continueForward = false;
                    }
                }
            }

            if (continueBackward)
            {
                if (piecePosition.x - i >= 0)
                {
                    Vector2Int possibleLocationBackward = new Vector2Int(piecePosition.x - i, piecePosition.y); //move backwards
                    locations.Add(possibleLocationBackward);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(possibleLocationBackward.x, possibleLocationBackward.y) == false)
                    {
                        continueBackward = false;
                    }
                }
            }
            
            if (continueLeft)
            {
                if (piecePosition.y - i >= 0)
                {
                    Vector2Int possibleLocationLeft = new Vector2Int(piecePosition.x, piecePosition.y - i); //move left
                    locations.Add(possibleLocationLeft);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(possibleLocationLeft.x, possibleLocationLeft.y) == false)
                    {
                        continueLeft = false;
                    }
                }
            }

            if (continueRight)
            {
                if (piecePosition.y + i < 8)
                {
                    Vector2Int possibleLocationRight = new Vector2Int(piecePosition.x, piecePosition.y + i); //move right
                    locations.Add(possibleLocationRight);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(possibleLocationRight.x, possibleLocationRight.y) == false)
                    {
                        continueRight = false;
                    }
                }
            }
        }

        return locations;
    }


    [PunRPC]
    private void MovePieceForOpponent(string tag, int x, int y)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(tag);

        Vector3 initialPosition = myPiece.transform.position;
        Vector2Int oppositePosition = new Vector2Int(7 - x, 7 - y);
        Vector3 targetPosition = Geometry.PointFromGrid(oppositePosition);

        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
        ARChessGameManager.pieces[oppositePosition.x, oppositePosition.y] = myPiece; //the rook may have moved in any straight direction
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);
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
