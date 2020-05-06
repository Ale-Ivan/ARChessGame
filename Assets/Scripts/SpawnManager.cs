using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPositions;

    public GameObject[] pieces;

    public GameObject chessBoardGameObject;

    public Material[] possibleMaterialColors;

    private string[] possibleColors = { "Black", "White" };
    private string pieceColor;

    public InputField messageInputField;
    public TextMeshProUGUI messageFromOpponent;

    public Mesh[] meshTypes;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #region Photon callback methods
    void OnEvent(EventData photonEvent)
    {
        /*if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            //Vector3 receivedPosition = (Vector3)data[0];
            Quaternion receivedRotation = (Quaternion)data[1];
            int receivedPlayerSelection = (int)data[3];
            int count = (int)data[4];
            string tag = (string)data[5];

            Material pieceMaterialColor = possibleMaterialColors[receivedPlayerSelection % 2];
            pieceColor = possibleColors[receivedPlayerSelection % 2];

            if (pieceColor.Equals("White"))
            {
                ARChessGameManager.instance.currentPlayer = "White";
            }
            else if (pieceColor.Equals("Black"))
            {
                ARChessGameManager.instance.otherPlayer = "Black";
            }

            ARChessGameManager.instance.colorOfOpponent = pieceColor;

            Vector3 instantiatePosition = spawnPositions[1].position;

            //Vector3 initialPositionFirstRow = instantiatePosition + new Vector3(1.05f, 0f, 0.15f);
            Vector3 initialPositionFirstRow = instantiatePosition + new Vector3(2.1f, 0f, 0.3f);
            //Vector3 initialPositionSecondRow = instantiatePosition + new Vector3(1.05f, 0f, -0.15f);
            Vector3 initialPositionSecondRow = instantiatePosition + new Vector3(2.1f, 0f, -0.3f);
            //Vector3 change = new Vector3(0.3f, 0f, 0f);
            Vector3 change = new Vector3(0.6f, 0f, 0f);

            GameObject playerPiece;
            if (count < 8)
            {
                if (count == 0 || count == 7) //Rook
                {
                    GameObject rook;
                    if (receivedPlayerSelection == 0 || receivedPlayerSelection == 1)
                    {
                        rook = pieces[0];
                    } 
                    else
                    {
                        rook = pieces[6];
                    }
                    playerPiece = Instantiate(rook, initialPositionFirstRow - count * change, receivedRotation);
                }
                else if (count == 1 || count == 6) //Knight
                {
                    GameObject knight;
                    if (receivedPlayerSelection == 0 || receivedPlayerSelection == 1)
                    {
                        knight = pieces[1];
                        playerPiece = Instantiate(knight, initialPositionFirstRow - count * change, Quaternion.Euler(-90, 0, 90));
                    }
                    else
                    {
                        knight = pieces[7];
                        playerPiece = Instantiate(knight, initialPositionFirstRow - count * change, Quaternion.Euler(0, 180, 0));
                    }
                }
                else if (count == 2 || count == 5) //Bishop
                {
                    GameObject bishop;
                    if (receivedPlayerSelection == 0 || receivedPlayerSelection == 1)
                    {
                        bishop = pieces[2];
                    }
                    else
                    {
                        bishop = pieces[8];
                    }
                    playerPiece = Instantiate(bishop, initialPositionFirstRow - count * change, receivedRotation);
                }
                else
                {
                    if (receivedPlayerSelection == 0) //simple black
                    {
                        playerPiece = Instantiate(pieces[count], initialPositionFirstRow - (count) * change, receivedRotation);
                    }
                    else if (receivedPlayerSelection == 2) //extra black
                    {
                        playerPiece = Instantiate(pieces[count + 6], initialPositionFirstRow - (count) * change, receivedRotation);
                    }
                    else if (receivedPlayerSelection == 1) //simple white
                    {
                        if (count == 3)
                        {
                            playerPiece = Instantiate(pieces[count + 1], initialPositionFirstRow - (count) * change, receivedRotation);
                        }
                        else
                        {
                            playerPiece = Instantiate(pieces[count - 1], initialPositionFirstRow - (count) * change, receivedRotation);
                        }
                    }
                    else
                    {
                        if (count == 3)
                        {
                            playerPiece = Instantiate(pieces[count + 7], initialPositionFirstRow - (count) * change, receivedRotation);
                        }
                        else
                        {
                            playerPiece = Instantiate(pieces[count + 5], initialPositionFirstRow - (count) * change, receivedRotation);
                        }
                    }
                }

                playerPiece.GetComponent<MeshRenderer>().material = pieceMaterialColor;

                PhotonView playerPiecePhotonView = playerPiece.GetComponent<PhotonView>();
                playerPiecePhotonView.ViewID = (int)data[2];
                ARChessGameManager.instance.AddPiece(playerPiece, 7, 7 - count);
            }
            else //Pawns
            {
                if (receivedPlayerSelection == 0 || receivedPlayerSelection == 1)
                {
                    playerPiece = Instantiate(pieces[5], initialPositionSecondRow - (count - 8) * change, receivedRotation);
                }
                else
                {
                    playerPiece = Instantiate(pieces[11], initialPositionSecondRow - (count - 8) * change, receivedRotation);
                }

                playerPiece.GetComponent<MeshRenderer>().material = pieceMaterialColor;

                PhotonView playerPiecePhotonView = playerPiece.GetComponent<PhotonView>();
                playerPiecePhotonView.ViewID = (int)data[2];
                ARChessGameManager.instance.AddPiece(playerPiece, 6, 15 - count);
            }
            playerPiece.tag = tag;
            //Debug.Log(playerPiece.tag);

        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.PlayerMessageEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            //Debug.Log("event" + data.ToString());
            string message = (string)data[0];
            //Debug.Log(message);
            messageFromOpponent.text = message;
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.PlayerQuitGameCode)
        {
            object[] data = (object[])photonEvent.CustomData;
        }*/
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

            //SpawnPlayer();
            //SpawnPieces();

            Spawn();
        }


    }

    #endregion


    #region Private methods
    private void Spawn()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
        {
            int selectionNumber = (int)playerSelectionNumber;
            //Debug.Log("Player Selection number is " + selectionNumber);

            Material pieceMaterialColor = possibleMaterialColors[selectionNumber % 2];
            pieceColor = possibleColors[selectionNumber % 2];

            if (pieceColor.Equals("White"))
            {
                ARChessGameManager.currentPlayer = "White";
            }
            else if (pieceColor.Equals("Black"))
            {
                ARChessGameManager.otherPlayer = "Black";
            }
            
            ARChessGameManager.colorOfLocalPlayer = pieceColor;

            Vector3 instantiatePosition = spawnPositions[0].position;

            //Vector3 initialPositionFirstRow = instantiatePosition - new Vector3(1.05f, 0f, 0.15f);
            Vector3 initialPositionFirstRow = instantiatePosition - new Vector3(2.1f, 0f, 0.3f);
            //Vector3 initialPositionSecondRow = instantiatePosition - new Vector3(1.05f, 0f, -0.15f);
            Vector3 initialPositionSecondRow = instantiatePosition - new Vector3(2.1f, 0f, -0.3f);
            //Vector3 change = new Vector3(0.3f, 0f, 0f);
            Vector3 change = new Vector3(0.6f, 0f, 0f);

            int numberOfRooks = 0;
            int numberOfKnights = 0;
            int numberOfBishops = 0;
            

            for (int i = 0; i < 8; i++)
            {
                //0 with 7 = Rook
                //1 with 6 = Knight
                //2 with 5 = Bishop
                //3 = King
                //4 = Queen
                GameObject instantiatedPiece;
                if (i == 0 || i == 7) //Rook
                {
                    if (selectionNumber == 0 || selectionNumber == 1)
                    {
                        instantiatedPiece = Instantiate(pieces[0], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                    }
                    else
                    {
                        instantiatedPiece = Instantiate(pieces[6], initialPositionFirstRow + i * change, Quaternion.identity);
                    }
                    numberOfRooks++;
                    instantiatedPiece.tag = pieceColor + "Rook" + numberOfRooks;
                }
                else if (i == 1 || i == 6) //Knight
                {
                    if (selectionNumber == 0 || selectionNumber == 1)
                    {
                        instantiatedPiece = Instantiate(pieces[1], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, -90));
                    }
                    else
                    {
                        instantiatedPiece = Instantiate(pieces[7], initialPositionFirstRow + i * change, Quaternion.identity);
                    }
                    numberOfKnights++;
                    instantiatedPiece.tag = pieceColor + "Knight" + numberOfKnights;
                }
                else if (i == 2 || i == 5) //Bishop
                {
                    if (selectionNumber == 0 || selectionNumber == 1)
                    {
                        instantiatedPiece = Instantiate(pieces[2], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                    }
                    else
                    {
                        instantiatedPiece = Instantiate(pieces[8], initialPositionFirstRow + i * change, Quaternion.identity);
                    }
                    numberOfBishops++;
                    instantiatedPiece.tag = pieceColor + "Bishop" + numberOfBishops;
                }
                else
                { 
                    if (selectionNumber == 0) //simple black
                    {
                        instantiatedPiece = Instantiate(pieces[i], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                        if (i == 3)
                        {
                            instantiatedPiece.tag = pieceColor + "King";
                        }
                        else
                        {
                            instantiatedPiece.tag = pieceColor + "Queen";
                        }
                    }
                    else if (selectionNumber == 2) //extra black
                    {
                        instantiatedPiece = Instantiate(pieces[i + 6], initialPositionFirstRow + i * change, Quaternion.identity);
                        if (i == 3)
                        {
                            instantiatedPiece.tag = pieceColor + "King";
                        }
                        else
                        {
                            instantiatedPiece.tag = pieceColor + "Queen";
                        }
                    }
                    else if (selectionNumber == 1) //simple white
                    {
                        if (i == 3)
                        {
                            instantiatedPiece = Instantiate(pieces[i + 1], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                            instantiatedPiece.tag = pieceColor + "Queen";
                        }
                        else
                        {
                            instantiatedPiece = Instantiate(pieces[i - 1], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                            instantiatedPiece.tag = pieceColor + "King";
                        }
                    }
                    else //extra white
                    {
                        if (i == 3)
                        {
                            instantiatedPiece = Instantiate(pieces[i + 6], initialPositionFirstRow + i * change, Quaternion.identity);
                            instantiatedPiece.tag = pieceColor + "Queen";
                        }
                        else
                        {
                            instantiatedPiece = Instantiate(pieces[i + 5], initialPositionFirstRow + i * change, Quaternion.identity);
                            instantiatedPiece.tag = pieceColor + "King";
                        }
                    }
                }

                instantiatedPiece.GetComponent<MeshRenderer>().material = pieceMaterialColor;

                PhotonView piecePhotonView = instantiatedPiece.GetComponent<PhotonView>();
                if (PhotonNetwork.AllocateViewID(piecePhotonView))
                {
                    object[] data = new object[]
                    {
                            instantiatedPiece.transform.position - chessBoardGameObject.transform.position, instantiatedPiece.transform.rotation, piecePhotonView.ViewID, selectionNumber, i, instantiatedPiece.tag
                    };
                    RaiseEvent(data);
                    ARChessGameManager.instance.AddPiece(instantiatedPiece, 0, i);
                }
                else
                {
                    Debug.Log("Failed to allocate a view ID");
                    Destroy(instantiatedPiece);
                }
            }

            for (int i = 8; i < 16; i++)
            {
                GameObject instantiatedPiece;
                if (selectionNumber == 0 || selectionNumber == 1)
                {
                   instantiatedPiece = Instantiate(pieces[5], initialPositionSecondRow + (i - 8) * change, Quaternion.Euler(-90, 0, 0));
                }
                else
                {
                    instantiatedPiece = Instantiate(pieces[11], initialPositionSecondRow + (i - 8) * change, Quaternion.identity);
                }
                instantiatedPiece.tag = pieceColor + "Pawn" + (i - 7);

                instantiatedPiece.GetComponent<MeshRenderer>().material = pieceMaterialColor;

                PhotonView piecePhotonView = instantiatedPiece.GetComponent<PhotonView>();
                if (PhotonNetwork.AllocateViewID(piecePhotonView))
                {
                    object[] data = new object[]
                    {
                            instantiatedPiece.transform.position - chessBoardGameObject.transform.position, instantiatedPiece.transform.rotation, piecePhotonView.ViewID, selectionNumber, i, instantiatedPiece.tag
                    };
                    RaiseEvent(data);

                    ARChessGameManager.instance.AddPiece(instantiatedPiece, 1, i - 8);
                }
                else
                {
                    Debug.Log("Failed to allocate a view ID");
                    Destroy(instantiatedPiece);
                }
            }
        }
    }


    /*private void SpawnPieces()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
        {
            Debug.Log("Player Selection number is " + (int)playerSelectionNumber);

            Vector3 instantiatePosition = spawnPositions[0].position;

            Vector3 initialPositionFirstRow = instantiatePosition - new Vector3(1.05f, 0f, 0.15f);
            //Vector3 initialPositionFirstRow = instantiatePosition - new Vector3(2.1f, 0f, 0.3f);
            Vector3 initialPositionSecondRow = instantiatePosition - new Vector3(1.05f, 0f, -0.15f);
            //Vector3 initialPositionSecondRow = instantiatePosition - new Vector3(2.1f, 0f, -0.3f);
            Vector3 change = new Vector3(0.3f, 0f, 0f);
            //Vector3 change = new Vector3(0.6f, 0f, 0f);
            int[] viewIDs = new int[16];

            int numberOfRooks = 0;
            int numberOfKnights = 0;
            int numberOfBishops = 0;

            if ((int)playerSelectionNumber == 0) //black pieces
            {
                for (int i = 0; i < 8; i++)
                {
                    //0 with 7 = Rook
                    //1 with 6 = Knight
                    //2 with 5 = Bishop
                    //3 = King
                    //4 = Queen
                    GameObject instantiatedBlackPiece;
                    if (i == 0 || i == 7) //Rook
                    {
                        instantiatedBlackPiece = Instantiate(blackPieces[0], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                        numberOfRooks++;
                        instantiatedBlackPiece.tag = "BlackRook" + numberOfRooks;
                    }
                    else if (i == 1 || i == 6) //Knight
                    {
                        instantiatedBlackPiece = Instantiate(blackPieces[1], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, -90));
                        numberOfKnights++;
                        instantiatedBlackPiece.tag = "BlackKnight" + numberOfKnights;
                    }
                    else if (i == 2 || i == 5) //Bishop
                    {
                        instantiatedBlackPiece = Instantiate(blackPieces[2], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                        numberOfBishops++;
                        instantiatedBlackPiece.tag = "BlackBishop" + numberOfBishops;
                    }
                    else
                    {
                        instantiatedBlackPiece = Instantiate(blackPieces[i], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                        if (i == 3)
                        {
                            instantiatedBlackPiece.tag = "BlackKing";
                        }
                        else
                        {
                            instantiatedBlackPiece.tag = "BlackQueen";
                        }
                    }

                    PhotonView piecePhotonView = instantiatedBlackPiece.GetComponent<PhotonView>();
                    if (PhotonNetwork.AllocateViewID(piecePhotonView))
                    {
                        object[] data = new object[]
                        {
                            instantiatedBlackPiece.transform.position - chessBoardGameObject.transform.position, instantiatedBlackPiece.transform.rotation, piecePhotonView.ViewID, playerSelectionNumber, i, instantiatedBlackPiece.tag
                        };
                        RaiseEvent(data);
                        viewIDs[i] = piecePhotonView.ViewID;

                        ARChessGameManager.instance.AddPiece(instantiatedBlackPiece, 0, i); 
                    }
                    else
                    {
                        Debug.Log("Failed to allocate a view ID");
                        Destroy(instantiatedBlackPiece);
                    }
                    //Debug.Log(instantiatedBlackPiece.tag);
                }
                for (int i = 8; i < 16; i++)
                {
                    GameObject instantiatedBlackPiece = Instantiate(blackPieces[5], initialPositionSecondRow + (i - 8) * change, Quaternion.Euler(-90, 0, 0));
                    instantiatedBlackPiece.tag = "BlackPawn" + (i - 7);
                    PhotonView piecePhotonView = instantiatedBlackPiece.GetComponent<PhotonView>();
                    if (PhotonNetwork.AllocateViewID(piecePhotonView))
                    {
                        object[] data = new object[]
                        {
                            instantiatedBlackPiece.transform.position - chessBoardGameObject.transform.position, instantiatedBlackPiece.transform.rotation, piecePhotonView.ViewID, playerSelectionNumber, i, instantiatedBlackPiece.tag
                        };
                        RaiseEvent(data);
                        viewIDs[i] = piecePhotonView.ViewID;

                        ARChessGameManager.instance.AddPiece(instantiatedBlackPiece, 1, i-8); 
                    }
                    else
                    {
                        Debug.Log("Failed to allocate a view ID");
                        Destroy(instantiatedBlackPiece);
                    }
                    //Debug.Log(instantiatedBlackPiece.tag);
                }
            }
            else //white pieces
            {
                for (int i = 0; i < 8; i++)
                {
                    //0 with 7 = Rook
                    //1 with 6 = Knight
                    //2 with 5 = Bishop
                    //3 = Queen
                    //4 = King
                    GameObject instantiatedWhitePiece;
                    if (i == 0 || i == 7) //Rook
                    {
                        instantiatedWhitePiece = Instantiate(whitePieces[0], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                        numberOfRooks++;
                        instantiatedWhitePiece.tag = "WhiteRook" + numberOfRooks;
                    }
                    else if (i == 1 || i == 6) //Knight
                    {
                        instantiatedWhitePiece = Instantiate(whitePieces[1], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, -90));
                        numberOfKnights++;
                        instantiatedWhitePiece.tag = "WhiteKnight" + numberOfKnights;
                    }
                    else if (i == 2 || i == 5) //Bishop
                    {
                        instantiatedWhitePiece = Instantiate(whitePieces[2], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                        numberOfBishops++;
                        instantiatedWhitePiece.tag = "WhiteBishop" + numberOfBishops;
                    }
                    else
                    {
                        instantiatedWhitePiece = Instantiate(whitePieces[i], initialPositionFirstRow + i * change, Quaternion.Euler(-90, 0, 0));
                        if (i == 3)
                        {
                            instantiatedWhitePiece.tag = "WhiteQueen";
                        }
                        else
                        {
                            instantiatedWhitePiece.tag = "WhiteKing";
                        }
                    }

                    PhotonView piecePhotonView = instantiatedWhitePiece.GetComponent<PhotonView>();
                    if (PhotonNetwork.AllocateViewID(piecePhotonView))
                    {
                        object[] data = new object[]
                        {
                            instantiatedWhitePiece.transform.position - chessBoardGameObject.transform.position, instantiatedWhitePiece.transform.rotation, piecePhotonView.ViewID, playerSelectionNumber, i, instantiatedWhitePiece.tag
                        };
                        RaiseEvent(data);
                        viewIDs[i] = piecePhotonView.ViewID;
                        ARChessGameManager.instance.AddPiece(instantiatedWhitePiece, 0, i );
                    }
                    //Debug.Log(instantiatedWhitePiece.tag);
                }
                for (int i = 8; i < 16; i++)
                {
                    GameObject instantiatedWhitePiece = Instantiate(whitePieces[5], initialPositionSecondRow + (i - 8) * change, Quaternion.Euler(-90, 0, 0));
                    instantiatedWhitePiece.tag = "WhitePawn" + (i - 7);
                    PhotonView piecePhotonView = instantiatedWhitePiece.GetComponent<PhotonView>();

                    if (PhotonNetwork.AllocateViewID(piecePhotonView))
                    {
                        object[] data = new object[]
                        {
                            instantiatedWhitePiece.transform.position - chessBoardGameObject.transform.position, instantiatedWhitePiece.transform.rotation, piecePhotonView.ViewID, playerSelectionNumber, i, instantiatedWhitePiece.tag
                        };
                        RaiseEvent(data);
                        viewIDs[i] = piecePhotonView.ViewID;
                        ARChessGameManager.instance.AddPiece(instantiatedWhitePiece, 1, i - 8);
                    }
                    //Debug.Log(instantiatedWhitePiece.tag);
                }
            }
            }
        }*/
    

    private void RaiseEvent(object[] data)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };
        PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raiseEventOptions, sendOptions);
    }

    private void RaiseMessageEvent(object[] data)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerMessageEventCode, data, raiseEventOptions, sendOptions);
    }

    public void OnSendButtonClicked()
    {
        string message = messageInputField.text;

        object[] data = new object[]
        {
            message
        };
        RaiseMessageEvent(data);

        messageInputField.text = "";
    }
    #endregion
}
