using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { King, Queen, Bishop, Knight, Rook, Pawn };

public abstract class Piece : MonoBehaviour
{
    public PieceType type;
    public bool selected;
    protected PhotonView piecePhotonView;

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);

    public void OnMouseDown()
    {
        if (piecePhotonView.IsMine)
        {
            if (!selected)
            {
                ARChessGameManager.instance.SelectPiece(this);
            }
            else
            {
                ARChessGameManager.instance.DeselectPiece(this);
            }
        }
        
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public abstract PhotonView GetPhotonView();


    
}
