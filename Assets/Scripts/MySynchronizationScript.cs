using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class MySynchronizationScript : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;

    Vector3 networkedPosition;

    private GameObject chessBoard;

    private float distance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();

        chessBoard = GameObject.Find("ChessBoard");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance * (1.0f / PhotonNetwork.SerializationRate));
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //send data to the other player
            stream.SendNext(rb.position - chessBoard.transform.position); 
        }
        else
        {
            //called on my object in the remote player's game
            networkedPosition = (Vector3)stream.ReceiveNext() + chessBoard.transform.position;
        }

        distance = Vector3.Distance(rb.position, networkedPosition);
    }
}
