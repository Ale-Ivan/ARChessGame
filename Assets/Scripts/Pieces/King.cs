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
            if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, forwardOne.x, forwardOne.y))
            {
                locations.Add(forwardOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(forwardOne.x, forwardOne.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                {
                    locations.Add(forwardOne);
                }
            }

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightUp = new Vector2Int(piecePosition.x + 1, piecePosition.y + 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, diagonalRightUp.x, diagonalRightUp.y))
                {
                    locations.Add(diagonalRightUp);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(diagonalRightUp.x, diagonalRightUp.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                    {
                        locations.Add(diagonalRightUp);
                    }
                }
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftUp = new Vector2Int(piecePosition.x + 1, piecePosition.y - 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, diagonalLeftUp.x, diagonalLeftUp.y))
                {
                    locations.Add(diagonalLeftUp);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(diagonalLeftUp.x, diagonalLeftUp.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                    {
                        locations.Add(diagonalLeftUp);
                    }
                }
            }
        }

        if (piecePosition.x != 0)
        {
            Vector2Int backwardsOne = new Vector2Int(piecePosition.x - 1, piecePosition.y);
            if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, backwardsOne.x, backwardsOne.y))
            {
                locations.Add(backwardsOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(backwardsOne.x, backwardsOne.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                {
                    locations.Add(backwardsOne);
                }
            }
   

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightDown = new Vector2Int(piecePosition.x - 1, piecePosition.y + 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, diagonalRightDown.x, diagonalRightDown.y))
                {
                    locations.Add(diagonalRightDown);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(diagonalRightDown.x, diagonalRightDown.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                    {
                        locations.Add(diagonalRightDown);
                    }
                }
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftDown = new Vector2Int(piecePosition.x - 1, piecePosition.y - 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, diagonalLeftDown.x, diagonalLeftDown.y))
                {
                    locations.Add(diagonalLeftDown);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(diagonalLeftDown.x, diagonalLeftDown.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                    {
                        locations.Add(diagonalLeftDown);
                    }
                }
            }
        }

        if (piecePosition.y != 0)
        {
            Vector2Int leftOne = new Vector2Int(piecePosition.x, piecePosition.y - 1);
            if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, leftOne.x, leftOne.y))
            {
                locations.Add(leftOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(leftOne.x, leftOne.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                {
                    locations.Add(leftOne);
                }
            }
        }

        if (piecePosition.y != 7)
        {
            Vector2Int rightOne = new Vector2Int(piecePosition.x, piecePosition.y + 1);
            if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, rightOne.x, rightOne.y))
            {
                locations.Add(rightOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(rightOne.x, rightOne.y).tag.StartsWith(ARChessGameManager.colorOfOpponent))
                {
                    locations.Add(rightOne);
                }
            }
        }

        return locations;
    }

    public override void GetAttackLocations(bool isForTemporaryCheck, GameObject[,] arrayWithPieces, Vector2Int currentPosition)
    {
        if (currentPosition.x != 0) //backward
        {
            ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - 1, currentPosition.y);

            if (currentPosition.y != 0) //diagonalLeftDown
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - 1, currentPosition.y - 1);
            }

            if (currentPosition.y != 7) //diagonalRightDown
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - 1, currentPosition.y + 1);
            }
        }

        if (currentPosition.x != 7) //forward
        {
            ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + 1, currentPosition.y);

            if (currentPosition.y != 0) //diagonalLeftUp
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + 1, currentPosition.y - 1);
            }

            if (currentPosition.y != 7) //diagonalRightUp
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + 1, currentPosition.y + 1);
            }
        }

        if (currentPosition.y != 7) //right
        {
            ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x, currentPosition.y + 1);
        }

        if (currentPosition.y != 0) //left
        {
            ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x, currentPosition.y - 1);
        }
    }

    [PunRPC]
    private void MovePieceForOpponent(string tag, int x, int y)
    {
        GameObject myPiece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(ARChessGameManager.pieces, tag);
        //Debug.Log(coordinates);

        Vector3 initialPosition = myPiece.transform.position;
        Vector2Int oppositePosition = new Vector2Int(7 - x, 7 - y);
        Vector3 targetPosition = Geometry.PointFromGrid(oppositePosition);

        ARChessGameManager.instance.MovePiece(myPiece, targetPosition + chessBoard.transform.position);
        ARChessGameManager.pieces[oppositePosition.x, oppositePosition.y] = myPiece; //the pawn may have moved one square or two squares
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);
        //ARChessGameManager.PrintPieces();

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
