using Photon.Pun;
using UnityEngine;

public class ScriptExample : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    [PunRPC]
    public void MovePieceForOpponent(Piece piece, Vector2Int finalPosition)
    {

    }
}


