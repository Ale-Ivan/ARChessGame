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
                        continueRightUp = false;
                    }
                }
            }

            if (continueLeftUp)
            {
                if (piecePosition.x + i < 8 && piecePosition.y - i >= 0)
                {
                    Vector2Int diagonalLeftUp = new Vector2Int(piecePosition.x + i, piecePosition.y - i);
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
                        continueLeftUp = false;
                    }
                }
            }
            
            if (continueRightDown)
            {
                if (piecePosition.x - i >= 0 && piecePosition.y + i < 8)
                {
                    Vector2Int diagonalRightDown = new Vector2Int(piecePosition.x - i, piecePosition.y + i);
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
                        continueRightDown = false;
                    }
                }
            }

            if (continueLeftDown)
            {
                if (piecePosition.x - i >= 0 && piecePosition.y - i >= 0)
                {
                    Vector2Int diagonalLeftDown = new Vector2Int(piecePosition.x - i, piecePosition.y - i);
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
                        continueLeftDown = false;
                    }
                }
            }
        }

        return locations;
    }

    public override void GetAttackLocations(bool isForTemporaryCheck, GameObject[,] arrayWithPieces, Vector2Int currentPosition)
    {
        bool continueRightUp = true;
        bool continueRightDown = true;
        bool continueLeftUp = true;
        bool continueLeftDown = true;

        for (int i = 1; i < 8; i++) //all movements are in mirror
        {         
            if (continueRightUp) //leftDown in mirror
            {
                if (currentPosition.x - i >= 0 && currentPosition.y - i >= 0)
                {
                    Vector2Int diagonalRightUp = new Vector2Int(currentPosition.x - i, currentPosition.y - i);

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - i, currentPosition.y - i);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, diagonalRightUp.x, diagonalRightUp.y) == false)
                    {
                        continueRightUp = false;
                    }

                }
            }

            if (continueLeftUp) //rightDown in mirror
            {
                if (currentPosition.x - i >= 0 && currentPosition.y + i < 8)
                {
                    Vector2Int diagonalLeftUp = new Vector2Int(currentPosition.x - i, currentPosition.y + i);

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x - i, currentPosition.y + i);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, diagonalLeftUp.x, diagonalLeftUp.y) == false)
                    {
                        continueLeftUp = false;
                    }
                }
            }

            if (continueRightDown) //leftUp in mirror
            {
                if (currentPosition.x + i < 8 && currentPosition.y - i >= 0)
                {
                    Vector2Int diagonalRightDown = new Vector2Int(currentPosition.x + i, currentPosition.y - i);

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + i, currentPosition.y - i);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, diagonalRightDown.x, diagonalRightDown.y) == false)
                    {
                        continueRightDown = false;
                    }
                }
            }

            if (continueLeftDown) //rightUp in mirror
            {
                if (currentPosition.x + i < 8 && currentPosition.y + i < 8)
                {
                    Vector2Int diagonalLeftDown = new Vector2Int(currentPosition.x + i, currentPosition.y + i);

                    ARChessGameManager.instance.SetAttackSquare(isForTemporaryCheck, currentPosition.x + i, currentPosition.y + i);

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(arrayWithPieces, diagonalLeftDown.x, diagonalLeftDown.y) == false)
                    {
                        continueLeftDown = false;
                    }
                }
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
