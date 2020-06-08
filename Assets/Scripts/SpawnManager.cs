using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public static SpawnManager instance;

    private void Awake()
    {
        instance = this;
    }

    #region Photon callback methods

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnMultiplayer();
        }
    }

    #endregion


    #region Private methods
    private void SpawnMultiplayer()
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

    public void SpawnSingleplayer()
    {
        SpawnPlayer();
        SpawnAI();

       
    }

    private void SpawnPlayer()
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
                ARChessGameManager.instance.AddPiece(instantiatedPiece, 0, i);
            }

            for (int i = 0; i < 8; i++)
            {
                GameObject instantiatedPiece;
                if (selectionNumber == 0 || selectionNumber == 1)
                {
                    instantiatedPiece = Instantiate(pieces[5], initialPositionSecondRow + i * change, Quaternion.Euler(-90, 0, 0));
                }
                else
                {
                    instantiatedPiece = Instantiate(pieces[11], initialPositionSecondRow + i * change, Quaternion.identity);
                }
                instantiatedPiece.tag = pieceColor + "Pawn" + (i + 1);
                instantiatedPiece.GetComponent<MeshRenderer>().material = pieceMaterialColor;
                ARChessGameManager.instance.AddPiece(instantiatedPiece, 1, i);
            }
        }
    }

    private void SpawnAI()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
        {
            int selectionNumber = (int)playerSelectionNumber;
            int selectionNumberAI; ;
            if (selectionNumber == 0 || selectionNumber == 2) //black or extra black
            {
                selectionNumberAI = 1;
            }
            else
            {
                selectionNumberAI = 0;
            }

            Material pieceMaterialColor = possibleMaterialColors[selectionNumberAI % 2];
            pieceColor = possibleColors[selectionNumberAI % 2];

            if (pieceColor.Equals("White"))
            {
                ARChessGameManager.currentPlayer = "White";
            }
            else if (pieceColor.Equals("Black"))
            {
                ARChessGameManager.otherPlayer = "Black";
            }

            ARChessGameManager.colorOfOpponent = pieceColor;

            Vector3 instantiatePosition = spawnPositions[1].position;

            //Vector3 initialPositionFirstRow = instantiatePosition + new Vector3(1.05f, 0f, 0.15f);
            Vector3 initialPositionFirstRow = instantiatePosition + new Vector3(2.1f, 0f, 0.3f);
            //Vector3 initialPositionSecondRow = instantiatePosition + new Vector3(1.05f, 0f, -0.15f);
            Vector3 initialPositionSecondRow = instantiatePosition + new Vector3(2.1f, 0f, -0.3f);
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
                    instantiatedPiece = Instantiate(pieces[0], initialPositionFirstRow - i * change, Quaternion.Euler(-90, 0, 0));
                    numberOfRooks++;
                    instantiatedPiece.tag = pieceColor + "Rook" + numberOfRooks;
                }
                else if (i == 1 || i == 6) //Knight
                {
                    instantiatedPiece = Instantiate(pieces[1], initialPositionFirstRow - i * change, Quaternion.Euler(-90, 0, 90));
                    numberOfKnights++;
                    instantiatedPiece.tag = pieceColor + "Knight" + numberOfKnights;
                }
                else if (i == 2 || i == 5) //Bishop
                {
                    instantiatedPiece = Instantiate(pieces[2], initialPositionFirstRow - i * change, Quaternion.Euler(-90, 0, 0));
                    numberOfBishops++;
                    instantiatedPiece.tag = pieceColor + "Bishop" + numberOfBishops;
                }
                else
                {
                    if (selectionNumberAI == 0) //simple black
                    {
                        instantiatedPiece = Instantiate(pieces[i], initialPositionFirstRow - i * change, Quaternion.Euler(-90, 0, 0));
                        if (i == 3)
                        {
                            instantiatedPiece.tag = pieceColor + "King";
                        }
                        else
                        {
                            instantiatedPiece.tag = pieceColor + "Queen";
                        }
                    }
                    else //simple white
                    {
                        if (i == 3)
                        {
                            instantiatedPiece = Instantiate(pieces[i + 1], initialPositionFirstRow - i * change, Quaternion.Euler(-90, 0, 0));
                            instantiatedPiece.tag = pieceColor + "Queen";
                        }
                        else
                        {
                            instantiatedPiece = Instantiate(pieces[i - 1], initialPositionFirstRow - i * change, Quaternion.Euler(-90, 0, 0));
                            instantiatedPiece.tag = pieceColor + "King";
                        }
                    }
                }

                instantiatedPiece.GetComponent<MeshRenderer>().material = pieceMaterialColor;
                ARChessGameManager.instance.AddPiece(instantiatedPiece, 7, 7 - i);
            }

            for (int i = 0; i < 8; i++)
            {
                GameObject instantiatedPiece;
                instantiatedPiece = Instantiate(pieces[5], initialPositionSecondRow - i * change, Quaternion.Euler(-90, 0, 0));
                instantiatedPiece.tag = pieceColor + "Pawn" + (i + 1);
                instantiatedPiece.GetComponent<MeshRenderer>().material = pieceMaterialColor;
                ARChessGameManager.instance.AddPiece(instantiatedPiece, 6, 7 - i);
            }
        }

        StartCoroutine(MakeFirstMove());
    }

    public void SpawnFromFile()
    {
        List<Tuple<string, string>> blackPieces = FileManager.instance.GetPositionsOfPieces("Black");
        List<Tuple<string, string>> whitePieces = FileManager.instance.GetPositionsOfPieces("White");

        //0 = rook
        //1 = knight
        //2 = bishop
        //3 = king
        //4 = queen
        //5 = pawn

        int numberOfBlackRooks = 0;
        int numberOfBlackKnights = 0;
        int numberOfBlackBishops = 0;
        int numberOfBlackPawns = 0;

        int numberOfWhiteRooks = 0;
        int numberOfWhiteKnights = 0;
        int numberOfWhiteBishops = 0;
        int numberOfWhitePawns = 0;

        Material blackPieceMaterial = possibleMaterialColors[0];
        Material whitePieceMaterial = possibleMaterialColors[1];

        string blackColor = possibleColors[0];
        string whiteColor = possibleColors[1];

        Quaternion blackKnightRotation;
        if (ARChessGameManager.colorOfLocalPlayer.Equals("Black"))
        {
            blackKnightRotation = Quaternion.Euler(-90, 0, -90);
        }
        else
        {
            blackKnightRotation = Quaternion.Euler(-90, 0, 90);
        }

        Quaternion whiteKnightRotation;
        if (ARChessGameManager.colorOfLocalPlayer.Equals("White"))
        {
            whiteKnightRotation = Quaternion.Euler(-90, 0, -90);
        }
        else
        {
            whiteKnightRotation = Quaternion.Euler(-90, 0, 90);
        }

        foreach (Tuple<string, string> blackPiece in blackPieces)
        {
            if (!blackPiece.Item2.Equals("X"))
            {
                GameObject instantiatedBlackPiece;
                int row = (int) char.GetNumericValue(blackPiece.Item2[0]);
                int column = (int) char.GetNumericValue(blackPiece.Item2[1]);
                if (blackPiece.Item1.Contains("Rook"))
                {
                    instantiatedBlackPiece = Instantiate(pieces[0], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    numberOfBlackRooks++;
                    instantiatedBlackPiece.tag = blackColor + "Rook" + numberOfBlackRooks;
                }
                else if (blackPiece.Item1.Contains("Knight"))
                {
                    instantiatedBlackPiece = Instantiate(pieces[1], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), blackKnightRotation);
                    numberOfBlackKnights++;
                    instantiatedBlackPiece.tag = blackColor + "Knight" + numberOfBlackKnights;
                }
                else if (blackPiece.Item1.Contains("Bishop"))
                {
                    instantiatedBlackPiece = Instantiate(pieces[2], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    numberOfBlackBishops++;
                    instantiatedBlackPiece.tag = blackColor + "Bishop" + numberOfBlackBishops;
                }
                else if (blackPiece.Item1.Contains("King"))
                {
                    instantiatedBlackPiece = Instantiate(pieces[3], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    instantiatedBlackPiece.tag = blackColor + "King";
                }
                else if (blackPiece.Item1.Contains("Queen"))
                {
                    instantiatedBlackPiece = Instantiate(pieces[4], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    instantiatedBlackPiece.tag = blackColor + "Queen";
                } 
                else
                {
                    instantiatedBlackPiece = Instantiate(pieces[5], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    numberOfBlackPawns++;
                    instantiatedBlackPiece.tag = blackColor + "Pawn" + numberOfBlackPawns;
                }

                instantiatedBlackPiece.GetComponent<MeshRenderer>().material = blackPieceMaterial;
                ARChessGameManager.pieces[row, column] = instantiatedBlackPiece;
            }
        }

        foreach (Tuple<string, string> whitePiece in whitePieces)
        {
            if (!whitePiece.Item2.Equals("X"))
            {
                GameObject instantiatedWhitePiece;
                int row = (int)char.GetNumericValue(whitePiece.Item2[0]);
                int column = (int)char.GetNumericValue(whitePiece.Item2[1]);
                if (whitePiece.Item1.Contains("Rook"))
                {
                    instantiatedWhitePiece = Instantiate(pieces[0], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    numberOfWhiteRooks++;
                    instantiatedWhitePiece.tag = whiteColor + "Rook" + numberOfWhiteRooks;
                }
                else if (whitePiece.Item1.Contains("Knight"))
                {
                    instantiatedWhitePiece = Instantiate(pieces[1], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), whiteKnightRotation);
                    numberOfWhiteKnights++;
                    instantiatedWhitePiece.tag = whiteColor + "Knight" + numberOfWhiteKnights;
                }
                else if (whitePiece.Item1.Contains("Bishop"))
                {
                    instantiatedWhitePiece = Instantiate(pieces[2], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    numberOfWhiteBishops++;
                    instantiatedWhitePiece.tag = whiteColor + "Bishop" + numberOfWhiteBishops;
                }
                else if (whitePiece.Item1.Contains("King"))
                {
                    instantiatedWhitePiece = Instantiate(pieces[3], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    instantiatedWhitePiece.tag = whiteColor + "King";
                }
                else if (whitePiece.Item1.Contains("Queen"))
                {
                    instantiatedWhitePiece = Instantiate(pieces[4], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    instantiatedWhitePiece.tag = whiteColor + "Queen";
                }
                else
                {
                    instantiatedWhitePiece = Instantiate(pieces[5], new Vector3(-2.1f + column * 0.6f, 0, -2.1f + row * 0.6f), Quaternion.Euler(-90, 0, 0));
                    numberOfWhitePawns++;
                    instantiatedWhitePiece.tag = whiteColor + "Pawn" + numberOfWhitePawns;
                }

                instantiatedWhitePiece.GetComponent<MeshRenderer>().material = whitePieceMaterial;
                ARChessGameManager.pieces[row, column] = instantiatedWhitePiece;
            }
        }
    }

    private IEnumerator MakeFirstMove()
    {
        yield return null;
        if (ARChessGameManager.colorOfLocalPlayer.Equals("Black"))
        {
            ARChessGameManager.instance.MovePieceAI();
        }
    }

    #endregion
}
