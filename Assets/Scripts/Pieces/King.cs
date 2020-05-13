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
        type = PieceType.King;
    }

    void Start()
    {
        selected = false;
    }

    public override PhotonView GetPhotonView()
    {
        return piecePhotonView;
    }

    public override List<Vector2Int> MoveLocations(GameObject[,] gamePlan, Vector2Int piecePosition, bool isAI = false)
    {
        string otherPlayer = isAI ? ARChessGameManager.colorOfLocalPlayer : ARChessGameManager.colorOfOpponent;

        List<Vector2Int> locations = new List<Vector2Int>();

        if (piecePosition.x != 7)
        {
            Vector2Int forwardOne = new Vector2Int(piecePosition.x + 1, piecePosition.y);
            if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, forwardOne.x, forwardOne.y))
            {
                locations.Add(forwardOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, forwardOne.x, forwardOne.y).tag.StartsWith(otherPlayer))
                {
                    locations.Add(forwardOne);
                }
            }

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightUp = new Vector2Int(piecePosition.x + 1, piecePosition.y + 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, diagonalRightUp.x, diagonalRightUp.y))
                {
                    locations.Add(diagonalRightUp);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, diagonalRightUp.x, diagonalRightUp.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(diagonalRightUp);
                    }
                }
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftUp = new Vector2Int(piecePosition.x + 1, piecePosition.y - 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, diagonalLeftUp.x, diagonalLeftUp.y))
                {
                    locations.Add(diagonalLeftUp);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, diagonalLeftUp.x, diagonalLeftUp.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(diagonalLeftUp);
                    }
                }
            }
        }

        if (piecePosition.x != 0)
        {
            Vector2Int backwardsOne = new Vector2Int(piecePosition.x - 1, piecePosition.y);
            if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, backwardsOne.x, backwardsOne.y))
            {
                locations.Add(backwardsOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, backwardsOne.x, backwardsOne.y).tag.StartsWith(otherPlayer))
                {
                    locations.Add(backwardsOne);
                }
            }
   

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightDown = new Vector2Int(piecePosition.x - 1, piecePosition.y + 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, diagonalRightDown.x, diagonalRightDown.y))
                {
                    locations.Add(diagonalRightDown);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, diagonalRightDown.x, diagonalRightDown.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(diagonalRightDown);
                    }
                }
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftDown = new Vector2Int(piecePosition.x - 1, piecePosition.y - 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, diagonalLeftDown.x, diagonalLeftDown.y))
                {
                    locations.Add(diagonalLeftDown);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, diagonalLeftDown.x, diagonalLeftDown.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(diagonalLeftDown);
                    }
                }
            }
        }

        if (piecePosition.y != 0)
        {
            Vector2Int leftOne = new Vector2Int(piecePosition.x, piecePosition.y - 1);
            if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, leftOne.x, leftOne.y))
            {
                locations.Add(leftOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, leftOne.x, leftOne.y).tag.StartsWith(otherPlayer))
                {
                    locations.Add(leftOne);
                }
            }
        }

        if (piecePosition.y != 7)
        {
            Vector2Int rightOne = new Vector2Int(piecePosition.x, piecePosition.y + 1);
            if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, rightOne.x, rightOne.y))
            {
                locations.Add(rightOne);
            }
            else
            {
                if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, rightOne.x, rightOne.y).tag.StartsWith(otherPlayer))
                {
                    locations.Add(rightOne);
                }
            }
        }

        return locations;
    }

    public override List<Vector2Int> MoveLocationsForAI(GameObject[,] gamePlan, Vector2Int piecePosition)
    {
        return MoveLocations(gamePlan, piecePosition, true);
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
