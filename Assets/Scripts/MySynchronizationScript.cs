using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class MySynchronizationScript : MonoBehaviour, IPunObservable
{
    PhotonView photonView;

    Vector3 networkedPosition;

    private GameObject chessBoard;

    private float distance;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();

        chessBoard = GameObject.Find("ChessBoard");
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            //rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance * (1.0f / PhotonNetwork.SerializationRate));
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, networkedPosition, distance * (1.0f / PhotonNetwork.SerializationRate));
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Debug.Log(gameObject.transform.position);
            //send data to the other player
            //stream.SendNext(rb.position - chessBoard.transform.position);
            stream.SendNext(gameObject.transform.position - chessBoard.transform.position);
        }
        else
        {
            //called on my object in the remote player's game
            networkedPosition = (Vector3)stream.ReceiveNext() + chessBoard.transform.position;
        }

        //distance = Vector3.Distance(rb.position, networkedPosition);
        distance = Vector3.Distance(gameObject.transform.position, networkedPosition);
    }
}
