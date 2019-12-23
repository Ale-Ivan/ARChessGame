using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bishop : Piece
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
        bool continueRightUp = true;
        bool continueRightDown = true;
        bool continueLeftUp = true;
        bool continueLeftDown = true;

        List<Vector2Int> locations = new List<Vector2Int>();

        for (int i = 1; i < 8; i++)
        {
            if (continueRightUp)
            {
                if (piecePosition.x + i < 8 && piecePosition.y + i < 8)
                {
                    Vector2Int diagonalRightUp = new Vector2Int(piecePosition.x + i, piecePosition.y + i);
                    locations.Add(diagonalRightUp);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(diagonalRightUp.x, diagonalRightUp.y) == false)
                    {
                        continueRightUp = false;
                    }
                }
            }

            if (continueLeftUp)
            {
                if (piecePosition.x + i < 8 && piecePosition.y - i >= 0)
                {
                    Vector2Int diagonalLeftUp = new Vector2Int(piecePosition.x + i, piecePosition.y - i);
                    locations.Add(diagonalLeftUp);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(diagonalLeftUp.x, diagonalLeftUp.y) == false)
                    {
                        continueLeftUp = false;
                    }
                }
            }
            
            if (continueRightDown)
            {
                if (piecePosition.x - i >= 0 && piecePosition.y + i < 8)
                {
                    Vector2Int diagonalRightDown = new Vector2Int(piecePosition.x - i, piecePosition.y + i);
                    locations.Add(diagonalRightDown);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(diagonalRightDown.x, diagonalRightDown.y) == false)
                    {
                        continueRightDown = false;
                    }
                }
            }

            if (continueLeftDown)
            {
                if (piecePosition.x - i >= 0 && piecePosition.y - i >= 0)
                {
                    Vector2Int diagonalLeftDown = new Vector2Int(piecePosition.x - i, piecePosition.y - i);
                    locations.Add(diagonalLeftDown);
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(diagonalLeftDown.x, diagonalLeftDown.y) == false)
                    {
                        continueLeftDown = false;
                    }
                }
            }
        }

        return locations;
    }

}
