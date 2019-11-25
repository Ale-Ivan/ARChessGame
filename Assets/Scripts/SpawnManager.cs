using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPositions;

    public GameObject chessBoardGameObject;

    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #region Photon callback methods
    //This method will be called at other players, when I raise an event => my battleArenaGameObject != their battleArenaGameObject
    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 receivedPosition = (Vector3)data[0];
            Quaternion receivedRotation = (Quaternion)data[1];
            int receivedPlayerSelection = (int)data[3];

            GameObject playerGameObject = Instantiate(playerPrefabs[receivedPlayerSelection], spawnPositions[1].position + chessBoardGameObject.transform.position, Quaternion.identity);

            for (int i = 0; i < 8; i++)
            {
                GameObject firstRowPiece = playerGameObject.transform.GetChild(i).gameObject;
                ARChessGameManager.AddPiece(firstRowPiece, 7, i);
                GameObject secondRowiece = playerGameObject.transform.GetChild(i + 8).gameObject;
                ARChessGameManager.AddPiece(secondRowiece, 6, i);
            }

            ARChessGameManager.PrintPieces();

            PhotonView _photonView = playerGameObject.GetComponent<PhotonView>();
            _photonView.ViewID = (int)data[2];
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            /*object playerSelectionNumber; 
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log("Player Selection number is " + (int)playerSelectionNumber);

                int randomSpawnPoint = Random.Range(0, spawnPositions.Length - 1);
                Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;

                PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }*/

            SpawnPlayer();
        }


    }

    #endregion


    #region Private methods

    private void SpawnPlayer()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
        {
            Debug.Log("Player Selection number is " + (int)playerSelectionNumber);

           // int randomSpawnPoint = Random.Range(0, spawnPositions.Length - 1);
            Vector3 instantiatePosition = spawnPositions[0].position;
            
            GameObject playerGameObject = Instantiate(playerPrefabs[(int)playerSelectionNumber], instantiatePosition, Quaternion.Euler(0, 180, 0));

            for (int i = 0; i < 8; i++)
            {
                GameObject firstRowPiece = playerGameObject.transform.GetChild(i).gameObject;
                ARChessGameManager.AddPiece(firstRowPiece, 0, i);
                GameObject secondRowiece = playerGameObject.transform.GetChild(i + 8).gameObject;
                ARChessGameManager.AddPiece(secondRowiece, 1, i);
            }

            //ARChessGameManager.PrintPieces();

            PhotonView _photonView = playerGameObject.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(_photonView))
            {
                object[] data = new object[]
                {
                    playerGameObject.transform.position - chessBoardGameObject.transform.position, playerGameObject.transform.rotation, _photonView.ViewID, playerSelectionNumber
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others, 
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                //raise events
                PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raiseEventOptions, sendOptions);

            }
            else
            {
                Debug.Log("Failed to allocate a view ID");
                Destroy(playerGameObject);
            }
        }
    }

    #endregion
}
