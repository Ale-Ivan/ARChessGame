using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelector : MonoBehaviour
{
    private Piece piece;
    public static MoveSelector instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject hitObject = hit.collider.gameObject;
                Vector3 hitPosition = hitObject.transform.position;
                Vector2Int gridPoint = Geometry.GridFromPoint(hitPosition);

                if (hitObject.tag == "Highlight")
                {
                    if (ARChessGameManager.instance.CheckIfPositionIsFree(gridPoint.x, gridPoint.y)) //empty location => can move
                    {
                        ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition);
                        Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(piece.tag);
                        ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                        ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);

                        piece.GetPhotonView().RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                    }      
                    
                    ExitState();
                }
                else if ((piece.tag.StartsWith("White") && hitObject.tag.StartsWith("Black")) || (piece.tag.StartsWith("Black") && hitObject.tag.StartsWith("White")))
                {
                   ARChessGameManager.instance.CapturePieceAt(gridPoint);
                   ARChessGameManager.instance.MovePiece(piece.GetGameObject(), hitPosition);
                   Vector2Int initialPosition = ARChessGameManager.instance.GetRowAndColumn(piece.tag);
                   ARChessGameManager.instance.SetPositionToObject(gridPoint.x, gridPoint.y, piece.GetGameObject());
                   ARChessGameManager.instance.SetPositionToNull(initialPosition.x, initialPosition.y);

                   piece.GetPhotonView().RPC("CapturePieceForOpponent", RpcTarget.OthersBuffered, piece.gameObject.tag, gridPoint.x, gridPoint.y);
                    ExitState();
                }
            }
        }
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
