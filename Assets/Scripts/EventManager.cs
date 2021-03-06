﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RaiseEventCodes
{
    PlayerSpawnEventCode = 0,
    PlayerMessageEventCode = 1,
    PlayerQuitGameCode = 2,
    PlayerCheckMate = 3,
    PlayerRequestedDraw = 4,
    OpponentAcceptedDraw = 5,
    OpponentDeniedDraw = 6
}
public class EventManager : MonoBehaviour
{
    [Header("Spawn event")]
    public Transform[] spawnPositions;
    public GameObject chessBoardGameObject;
    public Material[] possibleMaterialColors;
    public GameObject[] pieces;
    private string pieceColor;
    private string[] possibleColors = { "Black", "White" };

    [Header("Message event")]
    public InputField messageInputField;
    public TextMeshProUGUI messageFromOpponent;

    public GameObject uI_InformPanelGameObject;
    public TextMeshProUGUI uI_InformText;

    [Header("Game over")]
    public GameObject gameOverPanel;

    [Header("Draw")]
    public GameObject opponentRequestedDrawOptions;

    private int numberOfWins;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        numberOfWins = FileManager.instance.ReadIntFromFile("NumberOfWins");
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            //Debug.Log("spawn");
            object[] data = (object[])photonEvent.CustomData;
            //Vector3 receivedPosition = (Vector3)data[0];
            Quaternion receivedRotation = (Quaternion)data[1];
            int receivedPlayerSelection = (int)data[3];
            int count = (int)data[4];
            string tag = (string)data[5];

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, out object playerSelectionNumber))
            {
                if (receivedPlayerSelection == (int)playerSelectionNumber)
                {
                    //inform the players that they cannot play together
                    uI_InformText.text = "Both players have chosen the same color to play with! You cannot play together";
                    uI_InformPanelGameObject.SetActive(true);

                    gameOverPanel.SetActive(true);
                    ARChessGameManager.instance.EndGame();
                }
                else
                {
                    Material pieceMaterialColor = possibleMaterialColors[receivedPlayerSelection % 2];
                    pieceColor = possibleColors[receivedPlayerSelection % 2];

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
                        playerPiece.tag = tag;
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
                        playerPiece.tag = tag;
                        ARChessGameManager.instance.AddPiece(playerPiece, 6, 15 - count);
                    }
                    //playerPiece.tag = tag;
                    //Debug.Log(playerPiece.tag);
                }
            }
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.PlayerMessageEventCode)
        {
            //Debug.Log("chat");
            object[] data = (object[])photonEvent.CustomData;
            //Debug.Log("event" + data.ToString());
            string message = (string)data[0];
            //Debug.Log(message);
            messageFromOpponent.text = message;
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.PlayerQuitGameCode)
        {
            //only for singleplayer
            if (ARChessGameManager.ChosenGameMode == GameMode.SinglePlayer)
            {
                FileManager.instance.DeleteEntriesThatStartWith(ARChessGameManager.colorOfLocalPlayer);
                FileManager.instance.DeleteEntriesThatStartWith(ARChessGameManager.colorOfOpponent);
            }

            FileManager.instance.ChangeNumericPropertyValue("NumberOfWins", numberOfWins + 1);

            uI_InformText.text = ARChessGameManager.opponentName + " has left the room! You won!";
            uI_InformPanelGameObject.SetActive(true);

            gameOverPanel.SetActive(true);
            ARChessGameManager.instance.EndGame();
            //SceneLoader.Instance.LoadScene("Scene_Start");
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.PlayerCheckMate)
        {
            //only for singleplayer
            if (ARChessGameManager.ChosenGameMode == GameMode.SinglePlayer)
            {
                FileManager.instance.DeleteEntriesThatStartWith(ARChessGameManager.colorOfLocalPlayer);
                FileManager.instance.DeleteEntriesThatStartWith(ARChessGameManager.colorOfOpponent);
            }
                
            FileManager.instance.ChangeNumericPropertyValue("NumberOfWins", numberOfWins + 1);

            uI_InformText.text = "Game Over! You won against " + ARChessGameManager.opponentName;
            uI_InformPanelGameObject.SetActive(true);

            gameOverPanel.SetActive(true);
            ARChessGameManager.instance.EndGame();
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.PlayerRequestedDraw)
        {
            opponentRequestedDrawOptions.SetActive(true);
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.OpponentAcceptedDraw)
        {
            int numberOfLosses = FileManager.instance.ReadIntFromFile("NumberOfWins");
            FileManager.instance.ChangeNumericPropertyValue("NumberOfWins", numberOfLosses + 0.5);
            
            opponentRequestedDrawOptions.SetActive(false);

            uI_InformText.text = "Opponent accepted your draw request!";
            uI_InformPanelGameObject.SetActive(true);

            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));

            gameOverPanel.SetActive(true);
            ARChessGameManager.instance.EndGame();
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.OpponentDeniedDraw)
        {
            opponentRequestedDrawOptions.SetActive(false);

            uI_InformText.text = "Opponent denied your draw request!";
            uI_InformPanelGameObject.SetActive(true);
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
        }
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _gameObject.SetActive(false);
    }
}
