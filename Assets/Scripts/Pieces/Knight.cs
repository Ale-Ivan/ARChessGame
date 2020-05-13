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
        type = PieceType.Knight;
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

        if (piecePosition.y <= 5)
        {
            if (piecePosition.x <= 6)
            {
                Vector2Int possibleLocationRightUp_OneRow = new Vector2Int(piecePosition.x + 1, piecePosition.y + 2);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationRightUp_OneRow.x, possibleLocationRightUp_OneRow.y))
                {
                    locations.Add(possibleLocationRightUp_OneRow);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationRightUp_OneRow.x, possibleLocationRightUp_OneRow.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationRightUp_OneRow);
                    }
                }
            }
            if (piecePosition.x >= 1)
            {
                Vector2Int possibleLocationRightDown_OneRow = new Vector2Int(piecePosition.x - 1, piecePosition.y + 2);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationRightDown_OneRow.x, possibleLocationRightDown_OneRow.y))
                {
                    locations.Add(possibleLocationRightDown_OneRow);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationRightDown_OneRow.x, possibleLocationRightDown_OneRow.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationRightDown_OneRow);
                    }
                }
            }
        }

        if (piecePosition.y <= 6)
        {
            if (piecePosition.x <= 5)
            {
                Vector2Int possibleLocationRightUp_TwoRows = new Vector2Int(piecePosition.x + 2, piecePosition.y + 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationRightUp_TwoRows.x, possibleLocationRightUp_TwoRows.y))
                {
                    locations.Add(possibleLocationRightUp_TwoRows);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationRightUp_TwoRows.x, possibleLocationRightUp_TwoRows.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationRightUp_TwoRows);
                    }
                }
            }
            if (piecePosition.x >= 2)
            {
                Vector2Int possibleLocationRightDown_TwoRows = new Vector2Int(piecePosition.x - 2, piecePosition.y + 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationRightDown_TwoRows.x, possibleLocationRightDown_TwoRows.y))
                {
                    locations.Add(possibleLocationRightDown_TwoRows);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationRightDown_TwoRows.x, possibleLocationRightDown_TwoRows.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationRightDown_TwoRows);
                    }
                }
            }
        }

        if (piecePosition.y >= 2)
        {
            if (piecePosition.x <= 6)
            {
                Vector2Int possibleLocationLeftUp_OneRow = new Vector2Int(piecePosition.x + 1, piecePosition.y - 2);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationLeftUp_OneRow.x, possibleLocationLeftUp_OneRow.y))
                {
                    locations.Add(possibleLocationLeftUp_OneRow);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationLeftUp_OneRow.x, possibleLocationLeftUp_OneRow.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationLeftUp_OneRow);
                    }
                }
            }
            if (piecePosition.x >= 1)
            {
                Vector2Int possibleLocationLeftDown_OneRow = new Vector2Int(piecePosition.x - 1, piecePosition.y - 2);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationLeftDown_OneRow.x, possibleLocationLeftDown_OneRow.y))
                {
                    locations.Add(possibleLocationLeftDown_OneRow);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationLeftDown_OneRow.x, possibleLocationLeftDown_OneRow.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationLeftDown_OneRow);
                    }
                }
            }
        }
        
        if (piecePosition.y >= 1)
        {
            if (piecePosition.x <= 5)
            {
                Vector2Int possibleLocationLeftUp_TwoRows = new Vector2Int(piecePosition.x + 2, piecePosition.y - 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationLeftUp_TwoRows.x, possibleLocationLeftUp_TwoRows.y))
                {
                    locations.Add(possibleLocationLeftUp_TwoRows);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationLeftUp_TwoRows.x, possibleLocationLeftUp_TwoRows.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationLeftUp_TwoRows);
                    }
                }
            }
            if (piecePosition.x >= 2)
            {
                Vector2Int possibleLocationLeftDown_TwoRows = new Vector2Int(piecePosition.x - 2, piecePosition.y - 1);
                if (ARChessGameManager.instance.CheckIfPositionIsFree(gamePlan, possibleLocationLeftDown_TwoRows.x, possibleLocationLeftDown_TwoRows.y))
                {
                    locations.Add(possibleLocationLeftDown_TwoRows);
                }
                else
                {
                    if (ARChessGameManager.instance.GetPieceAtPosition(gamePlan, possibleLocationLeftDown_TwoRows.x, possibleLocationLeftDown_TwoRows.y).tag.StartsWith(otherPlayer))
                    {
                        locations.Add(possibleLocationLeftDown_TwoRows);
                    }
                }
            }
        }

        return locations;
    }

    public override List<Vector2Int> MoveLocationsForAI(GameObject[,] gamePlan, Vector2Int piecePosition)
    {
        return MoveLocations(gamePlan, piecePosition, true); //bishop's moves are the same in mirror
    }

    public override void GetAttackLocations(bool isForTemporaryCheck, GameObject[,] arrayWithPieces, Vector2Int currentPosition)
    {
        if (currentPosition.y >= 2)
        {
            if (currentPosition.x >= 1)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - 1, currentPosition.y - 2);
            }
            if (currentPosition.x <= 6)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + 1, currentPosition.y - 2);
            }
        }

        if (currentPosition.y >= 1)
        {
            if (currentPosition.x >= 2)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - 2, currentPosition.y - 1);
            }
            if (currentPosition.x <= 5)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + 2, currentPosition.y - 1);
            }
        }

        if (currentPosition.y <= 5)
        {
            if (currentPosition.x >= 1)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - 1, currentPosition.y + 2);
            }
            if (currentPosition.x <= 6)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + 1, currentPosition.y + 2);
            }
        }

        if (currentPosition.y <= 6)
        {
            if (currentPosition.x >= 2)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - 2, currentPosition.y + 1);
            }
            if (currentPosition.x <= 5)
            {
                ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + 2, currentPosition.y + 1);
            }
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
