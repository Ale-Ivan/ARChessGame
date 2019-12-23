using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Queen : Piece
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
        //bishop moves
        bool continueRightUp = true;
        bool continueRightDown = true;
        bool continueLeftUp = true;
        bool continueLeftDown = true;

        //rook moves
        bool continueLeft = true;
        bool continueRight = true;
        bool continueForward = true;
        bool continueBackward = true;

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

}
