using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class MoveSelector : MonoBehaviour
{
    private Piece piece;
    public static MoveSelector instance;

    public GameObject chessBoard;

    public Text debugText;

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
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    //debugText.text = "Touch";
                    Ray screenRay = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(screenRay, out RaycastHit hit))
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        Vector3 hitPosition = hitObject.transform.position - chessBoard.transform.position;
                        Vector2Int gridPoint = Geometry.GridFromPoint(hitPosition);

                        debugText.text = hitPosition.ToString("f2") + " " + gridPoint.x + " " + gridPoint.y;

                        if (hitObject.tag == "Highlight")
                        {
                            //debugText.text = "can move " + gridPoint.x + " " + gridPoint.y;
                            if (ARChessGameManager.instance.CheckIfPositionIsFree(gridPoint.x, gridPoint.y)) //empty location => can move
                            {
                                ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                                Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(piece.tag);
                                ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                                ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);
                                debugText.text += " " + initialPosition.x + " " + initialPosition.y;
                                piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                            }

                            ExitState();
                        }
                        else if ((piece.tag.StartsWith("White") && hitObject.tag.StartsWith("Black")) || (piece.tag.StartsWith("Black") && hitObject.tag.StartsWith("White"))) //full location => capture piece
                        {
                            ARChessGameManager.instance.CapturePieceAt(gridPoint);
                            ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition + chessBoard.transform.position);
                            Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(piece.tag);
                            ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                            ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);

                            piece.GetPhotonView().RPC("CapturePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                            ExitState();
                        }
                    }
                }
            }
        }
    }

    public void EnterState(Piece movingPiece)
    {
        debugText.text = "enter state " + movingPiece.gameObject.tag;
        piece = movingPiece;
        this.enabled = true;
    }

    public void ExitState()
    {
        //debugText.text = "exit state";
        ARChessGameManager.instance.DeselectPiece(piece);
        this.enabled = false;
    }

    
}
