using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { King, Queen, Bishop, Knight, Rook, Pawn };

public abstract class Piece : MonoBehaviour
{
    public PieceType type;
    public bool selected;
    protected PhotonView piecePhotonView;

    public abstract List<Vector2Int> MoveLocations(GameObject[,] gamePlan, Vector2Int piecePosition, bool isAI = false);

    public abstract List<Vector2Int> MoveLocationsForAI(GameObject[,] gamePlan, Vector2Int piecePosition);

    public abstract void GetAttackLocations(bool isForTemporaryCheck, GameObject[,] arrayWithPieces, Vector2Int currentPosition);

    public void OnMouseDown()
    {
        if (ARChessGameManager.ChosenGameMode == GameMode.SinglePlayer)
        {
            //check if it is my turn to move
            if (this.gameObject.tag.StartsWith(ARChessGameManager.currentPlayer))
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
        else
        {
            if (piecePhotonView.IsMine)
            {
                //check if it is my turn to move
                if (this.gameObject.tag.StartsWith(ARChessGameManager.currentPlayer))
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
        }
        
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public abstract PhotonView GetPhotonView();

}
