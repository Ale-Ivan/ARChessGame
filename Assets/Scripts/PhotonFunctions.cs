using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PhotonFunctions : MonoBehaviourPunCallbacks
{
    public static void ConnectToPhoton(string username)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LocalPlayer.NickName = username;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public static void DisconnectFromPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    #region Photon Callback methods

    public override void OnConnected()
    {
        Debug.Log("We connected to Internet!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon!");
    }

    #endregion
}
