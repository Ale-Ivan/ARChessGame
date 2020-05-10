using Photon.Pun;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SinglePlayerGameManager : MonoBehaviour
{
    private string path;
    private string jsonString;
    private JSONObject userJSON = new JSONObject();

    public Transform[] spawnPositions;
    public GameObject[] pieces;
    public GameObject chessBoardGameObject;
    public Material[] possibleMaterialColors;

    private string[] possibleColors = { "Black", "White" };
    private string pieceColor;
    private string colorOfAI;

    private GameObject[,] boardPieces;

    private void Awake()
    {
        path = Application.persistentDataPath + "/ARChessGameUserSave.json";
        if (File.Exists(path))
        {
            jsonString = File.ReadAllText(path);
            userJSON = (JSONObject)JSON.Parse(jsonString);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        boardPieces = new GameObject[8, 8];
    }

    public void OnPlayButtonClicked()
    {
        Spawn();
    }

    private void Spawn()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
        {
            int selectionNumber = (int)playerSelectionNumber;
            //Debug.Log("Player Selection number is " + selectionNumber);

            Material pieceMaterialColor = possibleMaterialColors[selectionNumber % 2];
            pieceColor = possibleColors[selectionNumber % 2];

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
                PhotonNetwork.AllocateViewID(piecePhotonView);

                SetPositionToPiece(instantiatedPiece, 0, i);
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
                PhotonView piecePhotonView = instantiatedPiece.GetComponent<PhotonView>();
                PhotonNetwork.AllocateViewID(piecePhotonView);

                SetPositionToPiece(instantiatedPiece, 1, i);
            }
        }
    }

    private void SetPositionToPiece(GameObject piece, int row, int column)
    {
        boardPieces[row, column] = piece;
    }
}

