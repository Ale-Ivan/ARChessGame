﻿using Photon.Pun;
using UnityEngine;

using UnityEngine.UI;
public class MoveSelector : MonoBehaviour
{
    private Piece piece;
    public static MoveSelector instance;

    public GameObject chessBoard;

    public Mesh queenMesh;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenRay, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                Vector3 hitPosition = hitObject.transform.position - chessBoard.transform.position;
                Vector2Int gridPoint = Geometry.GridFromPoint(hitPosition);
                if (hitObject.CompareTag("Highlight"))
                {
                    if (ARChessGameManager.instance.specialMove == 1) //switch king with rook1
                    {
                        GameObject rook1 = ARChessGameManager.instance.GetPieceAtPosition(ARChessGameManager.pieces, 0, 0);

                        if (piece.gameObject.CompareTag("BlackKing"))
                        {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 1, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 3);

                            Vector2Int specialMove = new Vector2Int(0, 2);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook1, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 2, rook1);
                            ARChessGameManager.instance.SetPositionToNull(0, 0);

                            ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
                            ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);

                            ARChessGameManager.instance.ChangePlayer();

                            if (ARChessGameManager.ChosenGameMode != GameMode.SinglePlayer)
                            {
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 1);
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "BlackRook1", 0, 2);
                                piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                            }
                        }
                        else if (piece.gameObject.CompareTag("WhiteKing"))
                        {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 2, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 4);

                            Vector2Int specialMove = new Vector2Int(0, 3);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook1, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 3, rook1);
                            ARChessGameManager.instance.SetPositionToNull(0, 0);

                            ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
                            ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);

                            ARChessGameManager.instance.ChangePlayer();

                            if (ARChessGameManager.ChosenGameMode != GameMode.SinglePlayer)
                            {
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 2);
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "WhiteRook1", 0, 3);
                                piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                            }

                        }
                        ARChessGameManager.instance.specialMove = 0;
                    }
                    else if (ARChessGameManager.instance.specialMove == 2) //switch king with rook2
                    {
                        GameObject rook2 = ARChessGameManager.instance.GetPieceAtPosition(ARChessGameManager.pieces, 0, 7);

                        if (piece.gameObject.CompareTag("BlackKing"))
                        {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 5, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 3);

                            Vector2Int specialMove = new Vector2Int(0, 4);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook2, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 4, rook2);
                            ARChessGameManager.instance.SetPositionToNull(0, 7);

                            ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
                            ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);

                            ARChessGameManager.instance.ChangePlayer();

                            if (ARChessGameManager.ChosenGameMode != GameMode.SinglePlayer)
                            {
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 5);
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "BlackRook2", 0, 4);
                                piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                            }
                        }
                        else if (piece.gameObject.CompareTag("WhiteKing"))
                        {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 6, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 4);

                            Vector2Int specialMove = new Vector2Int(0, 5);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook2, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 5, rook2);
                            ARChessGameManager.instance.SetPositionToNull(0, 7);

                            ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
                            ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);

                            ARChessGameManager.instance.ChangePlayer();

                            if (ARChessGameManager.ChosenGameMode != GameMode.SinglePlayer)
                            {
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 6);
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "WhiteRook2", 0, 5);
                                piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                            }
                        }

                        ARChessGameManager.instance.specialMove = 0;
                    }

                    if (ARChessGameManager.instance.CheckIfPositionIsFree(ARChessGameManager.pieces, gridPoint.x, gridPoint.y)) //empty location => can move
                    {
                        ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                        Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(ARChessGameManager.pieces, piece.tag);
                        ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                        ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);

                        ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
                        ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);

                        //debugText.text += " " + initialPosition.x + " " + initialPosition.y;
                        ARChessGameManager.instance.ChangePlayer();

                        if (ARChessGameManager.ChosenGameMode != GameMode.SinglePlayer)
                        {
                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                            piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                        }
                    }

                    if (piece.tag.Contains("Pawn") && gridPoint.x == 7) //transform the black pawn into a black queen
                    {
                        Mesh meshInstance = Instantiate(queenMesh);
                        piece.GetComponent<MeshFilter>().sharedMesh = meshInstance;
                        Destroy(piece.GetComponent<Pawn>());
                        piece.gameObject.AddComponent<Queen>();

                        string oldTag = piece.tag; //store the old tag

                        if (piece.tag.Contains("Black"))
                        {
                            ARChessGameManager.IncrementNumberOfBlackQueens();
                            piece.tag = "BlackQueen" + ARChessGameManager.GetNumberOfBlackQueens(); //new tag
                        }
                        else if (piece.tag.Contains("White"))
                        {
                            ARChessGameManager.IncrementNumberOfWhiteQueens();
                            piece.tag = "WhiteQueen" + ARChessGameManager.GetNumberOfWhiteQueens(); //new tag
                        }

                        if (ARChessGameManager.ChosenGameMode != GameMode.SinglePlayer)
                        {
                            //tell the opponent to also instantiate the queen
                            piece.GetPhotonView().RPC("TransformPawnInQueen", RpcTarget.OthersBuffered, oldTag, piece.tag);
                        }
                    }

                    ExitState();
                }
                //else if (ARChessGameManager.instance.highlightedObjects.Contains(hitObject) && ((piece.tag.StartsWith("White") && hitObject.tag.StartsWith("Black")) || (piece.tag.StartsWith("Black") && hitObject.tag.StartsWith("White")))) //full location => capture piece
                else if (ARChessGameManager.highlightedObjects.Contains(hitObject) && (piece.tag.StartsWith(ARChessGameManager.currentPlayer) && hitObject.tag.StartsWith(ARChessGameManager.otherPlayer)))
        
                 {
                    ARChessGameManager.instance.CapturePieceAt(gridPoint);
                    ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);

                    Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(ARChessGameManager.pieces, piece.tag);
                    
                    ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                    ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);

                    ARChessGameManager.instance.RefreshAttackedSquares(ARChessGameManager.pieces, false);
                    ARChessGameManager.instance.VerifyForCheck(ARChessGameManager.pieces, false);

                    ARChessGameManager.instance.ChangePlayer();

                    if (ARChessGameManager.ChosenGameMode != GameMode.SinglePlayer)
                    {
                        piece.GetPhotonView().RPC("CapturePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                        piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                    }

                    ExitState();
                }
            }
        }

        /*if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Ray screenRay = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(screenRay, out RaycastHit hit))
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        Vector3 hitPosition = hitObject.transform.position - chessBoard.transform.position;
                        Vector2Int gridPoint = Geometry.GridFromPoint(hitPosition);

                        if (hitObject.tag == "Highlight")
                        {
                            if (ARChessGameManager.instance.specialMove == 1) //switch king with rook1
                    {
                        GameObject rook1 = ARChessGameManager.instance.GetPieceAtPosition(0, 0);

                        if (piece.gameObject.tag.Equals("BlackKing") || piece.gameObject.tag.Equals("BlueKing"))
                        {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 1, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 3);
                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 1);

                            Vector2Int specialMove = new Vector2Int();
                            specialMove.Set(0, 2);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook1, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 2, rook1);
                            ARChessGameManager.instance.SetPositionToNull(0, 0);

                            //ARChessGameManager.instance.RefreshAttackedSquares();

                            //ARChessGameManager.instance.VerifyForCheck();

                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "BlackRook1", 0, 2);

                            ARChessGameManager.instance.ChangePlayer();
                            piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                        }
                        else if (piece.gameObject.tag.Equals("WhiteKing") || piece.gameObject.tag.Equals("RedKing"))
                                {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 2, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 4);
                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 2);

                            Vector2Int specialMove = new Vector2Int();
                            specialMove.Set(0, 2);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook1, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 3, rook1);
                            ARChessGameManager.instance.SetPositionToNull(0, 0);

                            //ARChessGameManager.instance.RefreshAttackedSquares();

                            //ARChessGameManager.instance.VerifyForCheck();

                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "WhiteRook1", 0, 3);

                            ARChessGameManager.instance.ChangePlayer();
                            piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                        }
                        ARChessGameManager.instance.specialMove = 0;
                    }
                    else if (ARChessGameManager.instance.specialMove == 2) //switch king with rook2
                    {
                        GameObject rook2 = ARChessGameManager.instance.GetPieceAtPosition(0, 7);

                        if (piece.gameObject.tag.Equals("BlackKing") || piece.gameObject.tag.Equals("BlueKing"))
                        {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 5, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 3);

                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 5);

                            Vector2Int specialMove = new Vector2Int();
                            specialMove.Set(0, 4);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook2, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 4, rook2);
                            ARChessGameManager.instance.SetPositionToNull(0, 7);

                            //ARChessGameManager.instance.RefreshAttackedSquares();

                            //ARChessGameManager.instance.VerifyForCheck();

                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "BlackRook2", 0, 4);

                            ARChessGameManager.instance.ChangePlayer();
                            piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                        }
                        else if (piece.gameObject.tag.Equals("WhiteKing") || piece.gameObject.tag.Equals("RedKing"))
                        {
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 6, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(0, 4);
                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, 0, 6);

                            Vector2Int specialMove = new Vector2Int();
                            specialMove.Set(0, 5);
                            Vector3 specialMovePosition = Geometry.PointFromGrid(specialMove);
                            ARChessGameManager.instance.MovePiece(rook2, specialMovePosition + chessBoard.transform.position);
                            ARChessGameManager.instance.SetPositionToObject(0, 5, rook2);
                            ARChessGameManager.instance.SetPositionToNull(0, 7);

                            //ARChessGameManager.instance.RefreshAttackedSquares();

                            //ARChessGameManager.instance.VerifyForCheck();

                            piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, "WhiteRook2", 0, 5);

                            ARChessGameManager.instance.ChangePlayer();
                            piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                        }

                        ARChessGameManager.instance.specialMove = 0;
                    }

                            //debugText.text = "can move " + gridPoint.x + " " + gridPoint.y;
                            if (ARChessGameManager.instance.CheckIfPositionIsFree(gridPoint.x, gridPoint.y)) //empty location => can move
                            {
                                ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);

                                //ARChessGameManager.instance.MovePiece(piece.gameObject); //add the piece to movedPieces list

                                Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(piece.tag);
                                ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                                ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);

                                //ARChessGameManager.instance.RefreshAttackedSquares();

                                //ARChessGameManager.instance.VerifyForCheck();

                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                                ARChessGameManager.instance.ChangePlayer();
                                piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                            }

                            if (piece.tag.Contains("Pawn") && gridPoint.x == 7) //transform the black pawn into a black queen
                            {
                                Mesh meshInstance = Instantiate(queenMesh);
                                piece.GetComponent<MeshFilter>().sharedMesh = meshInstance;
                                Destroy(piece.GetComponent<Pawn>());
                                piece.gameObject.AddComponent<Queen>();

                                string oldTag = piece.tag; //store the old tag

                                if (piece.tag.Contains("Black"))
                                {
                                    ARChessGameManager.IncrementNumberOfBlackQueens();
                                    piece.tag = "BlackQueen" + ARChessGameManager.GetNumberOfBlackQueens();
                                }
                                else if (piece.tag.Contains("White"))
                                {
                                    ARChessGameManager.IncrementNumberOfWhiteQueens();
                                    piece.tag = "WhiteQueen" + ARChessGameManager.GetNumberOfWhiteQueens();
                                }
                                else if (piece.tag.Contains("Blue"))
                                {
                                    ARChessGameManager.IncrementNumberOfBlueQueens();
                                    piece.tag = "BlueQueen" + ARChessGameManager.GetNumberOfBlueQueens();
                                }
                                else if (piece.tag.Contains("Red"))
                                {
                                    ARChessGameManager.IncrementNumberOfRedQueens();
                                    piece.tag = "RedQueen" + ARChessGameManager.GetNumberOfRedQueens();
                                }

                                //tell the opponent to also instantiate the queen
                                piece.GetPhotonView().RPC("TransformPawnInQueen", RpcTarget.OthersBuffered, oldTag, piece.tag);
                            }

                            ExitState();
                        }
                        //else if (ARChessGameManager.instance.highlightedObjects.Contains(hitObject) && ((piece.tag.StartsWith("White") && hitObject.tag.StartsWith("Black")) || (piece.tag.StartsWith("Black") && hitObject.tag.StartsWith("White")))) //full location => capture piece
                        else if (ARChessGameManager.instance.highlightedObjects.Contains(hitObject) && (piece.tag.StartsWith(ARChessGameManager.instance.currentPlayer) && hitObject.tag.StartsWith(ARChessGameManager.instance.otherPlayer)))
                        {
                            ARChessGameManager.instance.CapturePieceAt(gridPoint);
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);

                            //ARChessGameManager.instance.MovePiece(piece.gameObject); //add the piece to movedPieces list

                            Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(piece.tag);
                            ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);

                            //ARChessGameManager.instance.RefreshAttackedSquares();

                            //ARChessGameManager.instance.VerifyForCheck();

                            piece.GetPhotonView().RPC("CapturePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                            ARChessGameManager.instance.ChangePlayer();
                            piece.GetPhotonView().RPC("SwitchPlayer", RpcTarget.OthersBuffered);
                            ExitState();
                        }
                        
                        
                    }
                }
            }
        }*/
    }

    public void EnterState(Piece movingPiece)
    {
        piece = movingPiece;
        this.enabled = true;
    }

    public void ExitState()
    {
        ARChessGameManager.instance.DeselectPiece(piece);
        this.enabled = false;
    }

    
}
