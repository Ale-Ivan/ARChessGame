﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using TMPro;
using ExitGames.Client.Photon;
using System;
using System.Linq;

public class ARChessGameManager : MonoBehaviourPunCallbacks
{
    public static GameObject[,] pieces;
    public static ARChessGameManager instance;

    [Header("UI")]
    public GameObject uI_InformPanelGameObject;
    public TextMeshProUGUI uI_InformText;
    public GameObject searchForGamesButtonGameObject;
    public GameObject adjustButton;
    public GameObject raycastCenterImage;
    public GameObject gameOverPanel;

    [Header("Materials")]
    public Material selectedMaterial;
    public Material defaultMaterialBlack;
    public Material defaultMaterialWhite;

    public GameObject chessBoard;
    public GameObject chatGameObject;

    private bool isPieceSelected;
    private Piece selectedPiece;

    private static int numberOfBlackQueens;
    private static int numberOfWhiteQueens;

    public static string currentPlayer;
    public static string otherPlayer;
    public static string colorOfLocalPlayer;
    public static string colorOfOpponent;
    public static string opponentName;

    public GameObject tileHighlightPrefab;
    public GameObject tileHighlightIllegal;
    public static List<GameObject> highlightedObjects;
    private static List<GameObject> tileHighlights;

    private List<GameObject> movedPieces;

    public int specialMove;

    private ulong attackedSquares; //store the squares that are attacked by the opponent's pieces; 2 hex digits/row;

    private GameObject[,] tempGamePlan;
    private ulong tempAttackedSquares;

    public static GameMode ChosenGameMode;

    /*private string path;
    private JSONObject userJSON;*/
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[8, 8];
        tempGamePlan = new GameObject[8, 8];
        uI_InformPanelGameObject.SetActive(true);
        gameOverPanel.SetActive(false);

        tileHighlights = new List<GameObject>();
        highlightedObjects = new List<GameObject>();

        movedPieces = new List<GameObject>();

        isPieceSelected = false;
        selectedPiece = null;

        numberOfBlackQueens = 1;
        numberOfWhiteQueens = 1;

        specialMove = 0;

        attackedSquares = 0x0000000000000000;
        tempAttackedSquares = 0x0000000000000000;
    }

    #region UI callback methods

    public void JoinRoom()
    {
        if (ChosenGameMode == GameMode.MultiplayerAtRandom)
        {
            JoinRandomRoom();
        }
        else if (ChosenGameMode == GameMode.MultiplayerWithFriend)
        {
            JoinWantedRoom();
        }
    }

    public void JoinWantedRoom()
    {
        uI_InformText.text = "Searching for available rooms";

        string roomName = GameModesManager.Instance.GetRoomName();

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = false,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);

        searchForGamesButtonGameObject.SetActive(false);
        raycastCenterImage.SetActive(false);
    }

    public void JoinRandomRoom()
    {
        uI_InformText.text = "Searching for available rooms";
        PhotonNetwork.JoinRandomRoom();
        searchForGamesButtonGameObject.SetActive(false);
    }

    #endregion

    #region Photon callback methods

    /*public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("failed");
        uI_InformText.text = message;
        CreateAndJoinRoom();
    }*/

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        uI_InformText.text = message;
        CreateAndJoinRoom();
    }

    public override void OnCreatedRoom()
    {
        uI_InformText.text = "created room " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public override void OnJoinedRoom()
    {
        adjustButton.SetActive(false);
        raycastCenterImage.SetActive(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "Joined room " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";
        }
        else
        {
            opponentName = PhotonNetwork.PlayerListOthers[0].NickName;
            //Debug.Log(opponentName);

            uI_InformText.text = "Joined room " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
            chatGameObject.SetActive(true);
        }

        //Debug.Log("joined " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InformText.text = newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            opponentName = newPlayer.NickName;
            //Debug.Log(opponentName);
        }

        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
        chatGameObject.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Start");
    }

    #endregion

    #region Private methods

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + UnityEngine.Random.Range(0, 10000);
        uI_InformText.text = "Created room " + randomRoomName;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2
        };

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _gameObject.SetActive(false);
    }

    public void SetAttackSquare(bool isForTemporaryCheck, int row, int column)
    {
        int square = 63 - (row * 8 + column);
        if (isForTemporaryCheck)
        {
            tempAttackedSquares |= (1UL << square);
        }
        else
        {
            attackedSquares |= (1UL << square);
        }
    }

    public bool VerifyForCheck(GameObject[,] wantedPieces, bool isForTemporaryCheck)
    {
        ulong squares;
        if (isForTemporaryCheck)
        {
            squares = tempAttackedSquares;
        }
        else
        {
            squares = attackedSquares;
        }
        
        Vector2Int myKingPosition = GetRowAndColumn(wantedPieces, colorOfLocalPlayer + "King");

        int myKingSquare = 63 - (myKingPosition.x * 8 + myKingPosition.y);
        if ((squares & (1UL << myKingSquare)) == 0) //the bit is not set to 1
        {
            uI_InformPanelGameObject.SetActive(false);
            return false;
        }
        else //I am in check
        {
            if (!isForTemporaryCheck)
            {
                if (IsCheckMate())
                {
                    /*string path = Application.persistentDataPath + "/ARChessGameUserSave.json";
                    if (File.Exists(path))
                    {
                        string jsonString = File.ReadAllText(path);
                        JSONObject userJSON = (JSONObject)JSON.Parse(jsonString);
                        int numberOfLosses = userJSON["NumberOfLosses"];
                        userJSON["NumberOfLosses"] = numberOfLosses + 1;
                        File.WriteAllText(path, userJSON.ToString()); //also closes the file after writing
                    }*/

                    int numberOfLosses = FileManager.instance.ReadIntFromFile("NumberOfLosses");
                    FileManager.instance.ChangePropertyIntValue("NumberOfLosses", numberOfLosses + 1);

                    uI_InformText.text = "Game Over! Your opponent, " + opponentName + ", wins!";
                    uI_InformPanelGameObject.SetActive(true);

                    gameOverPanel.SetActive(true);
                    EndGame();

                    object[] data = new object[]
                    {
                        PhotonNetwork.LocalPlayer.NickName
                    };
                    RaiseEvent(data);

                }
                else
                {
                    uI_InformText.text = "You are in CHECK! Save your King!";
                    uI_InformPanelGameObject.SetActive(true);
                    StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
                }
            }
            return true;
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
        PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerCheckMate, data, raiseEventOptions, sendOptions);
    }


    private bool IsCheckMate()
    {
        bool isCheckMate = true;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    if (pieces[i, j].gameObject.tag.StartsWith(colorOfLocalPlayer))
                    {
                        Vector2Int currentPosition = GetRowAndColumn(pieces, pieces[i, j].gameObject.tag);
                        Piece myPiece = pieces[i, j].GetComponent<Piece>();
                        List<Vector2Int> possibleMoves = myPiece.MoveLocations(currentPosition);
                        List<bool> legalMoves = GetArrayOfLegalMoves(possibleMoves, myPiece, currentPosition);

                     
                        foreach (bool legalMove in legalMoves)
                        {
                            if (legalMove)
                            {
                                isCheckMate = false;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return isCheckMate;
    }

    public void RefreshAttackedSquares(GameObject[,] wantedPieces, bool isForTemporaryCheck)
    {
        if (isForTemporaryCheck)
        {
            tempAttackedSquares = 0x0000000000000000;
        }
        else
        {
            attackedSquares = 0x0000000000000000;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (wantedPieces[i, j] != null)
                {
                    if (wantedPieces[i, j].tag.StartsWith(colorOfOpponent))
                    {
                        Vector2Int currentPosition = new Vector2Int(i, j);

                        Piece piece = wantedPieces[i, j].GetComponent<Piece>();
                        piece.GetAttackLocations(isForTemporaryCheck, wantedPieces, currentPosition);
                    }
                }
            }
        }

        //Debug.Log(Convert.ToString((long)attackedSquares, 2).PadLeft(64, '0'));
    }

    public void AddPiece(GameObject piece, int row, int col)
    {
        pieces[row, col] = piece;

        if (ChosenGameMode == GameMode.SinglePlayer)
        {
            string position = row.ToString() + col.ToString();
            FileManager.instance.AddNewProperty(piece.tag, position);
        }
    }

    public static void PrintPieces()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    Debug.Log(pieces[i, j].tag + " " + i + " " + j);
                }
            }
        }
    }

    public bool CheckIfPositionIsFree(GameObject[,] arrayWithPieces, int i, int j)
    {
        if (arrayWithPieces[i, j] == null)
            return true;
        else
            return false;
    }

    public void SetPositionToNull(int i, int j)
    {
        pieces[i, j] = null;
    }

    public void SetPositionToObject(int i, int j, GameObject piece)
    {
        pieces[i, j] = piece;

        if (ChosenGameMode == GameMode.SinglePlayer)
        {
            string position = i.ToString() + j.ToString();
            FileManager.instance.ChangePropertyStringValue(piece.tag, position);
        }
    }

    public GameObject FindGameObjectWithTag(string tag)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    if (pieces[i, j].tag == tag)
                    {
                        return pieces[i, j];
                    }
                }
            }
        }
        return null;
    }

    public Vector2Int GetRowAndColumn(GameObject[,] wantedPieces, string wantedTag)
    {
        Vector2Int objectCoordinates = new Vector2Int();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (wantedPieces[i, j] != null)
                {
                    if (wantedPieces[i, j].CompareTag(wantedTag))
                    {
                        objectCoordinates.Set(i, j);
                        break;
                    }
                }
            }
        }
        return objectCoordinates;
    }

    private void AddToMovedPieces(GameObject piece)
    {
        movedPieces.Add(piece);
    }

    public GameObject GetPieceAtPosition(int x, int y)
    {
        return pieces[x, y];
    }
    public void MovePiece(GameObject piece, Vector3 finalPosition)
    {
        StartCoroutine(MoveObject(piece, piece.transform.position, finalPosition));
        AddToMovedPieces(piece);
    }

    public IEnumerator MoveObject(GameObject piece, Vector3 initial, Vector3 final)
    {
        float totalMovementTime = 0.5f; //the amount of time you want the movement to take
        float currentMovementTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(piece.transform.position, final) > 0)
        {
            currentMovementTime += Time.deltaTime;
            piece.transform.position = Vector3.Lerp(initial, final, currentMovementTime / totalMovementTime);
            yield return null;
        }
    }

    public void ChangePlayer()
    {
        string tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;

        //Debug.Log(currentPlayer + " " + otherPlayer);
    }

    public static void IncrementNumberOfBlackQueens()
    {
        numberOfBlackQueens++;
    }

    public static int GetNumberOfBlackQueens()
    {
        return numberOfBlackQueens;
    }

    public static void IncrementNumberOfWhiteQueens()
    {
        numberOfWhiteQueens++;
    }

    public static int GetNumberOfWhiteQueens()
    {
        return numberOfWhiteQueens;
    }

    public void SelectPiece(Piece myPiece)
    {
        if (isPieceSelected)
        {
            DeselectPiece(selectedPiece);
        }

        MeshRenderer renderer = myPiece.GetComponent<MeshRenderer>();
        renderer.material = selectedMaterial;
        myPiece.selected = true;

        selectedPiece = myPiece;
        isPieceSelected = true;

        ShowPossibleMoves(myPiece);
        MoveSelector.instance.EnterState(myPiece);
    }

    public void DeselectPiece(Piece myPiece)
    {
        MeshRenderer renderers = myPiece.GetComponent<MeshRenderer>();
        if (myPiece.gameObject.tag.StartsWith("Black"))
        {
            renderers.material = defaultMaterialBlack;
        }
        else if (myPiece.gameObject.tag.StartsWith("White"))
        {
            renderers.material = defaultMaterialWhite;
        }

        foreach (GameObject highlight in tileHighlights)
        {
            highlight.SetActive(false);
        }

        foreach (GameObject highlightedObject in highlightedObjects)
        {
            MeshRenderer renderer = highlightedObject.GetComponent<MeshRenderer>();
            if (highlightedObject.gameObject.tag.StartsWith("Black"))
            {
                renderer.material = defaultMaterialBlack;
            }
            else if (highlightedObject.gameObject.tag.StartsWith("White"))
            {
                renderer.material = defaultMaterialWhite;
            }
        }

        tileHighlights = new List<GameObject>();
        highlightedObjects = new List<GameObject>();

        myPiece.selected = false;

        selectedPiece = null;
        isPieceSelected = false;
    }

    private List<bool> GetArrayOfLegalMoves(List<Vector2Int> possibleMoves, Piece piece, Vector2Int currentPosition)
    {
        List<bool> legalMoves = new List<bool>();

        foreach(Vector2Int possibleMove in possibleMoves)
        {
            Array.Copy(pieces, tempGamePlan, pieces.Length);

            if (tempGamePlan[possibleMove.x, possibleMove.y] != null)
            {
                tempGamePlan[possibleMove.x, possibleMove.y] = null;
            }
            //move piece in each possible location
            tempGamePlan[possibleMove.x, possibleMove.y] = piece.gameObject;
            tempGamePlan[currentPosition.x, currentPosition.y] = null;

            //refresh attacked squares
            RefreshAttackedSquares(tempGamePlan, true);

            //check if moving the piece to that position puts you in check
            bool legal = !VerifyForCheck(tempGamePlan, true);
            legalMoves.Add(legal);
        }

        return legalMoves;
    }

    private void ShowPossibleMoves(Piece myPiece)
    {
        Vector2Int currentPosition = GetRowAndColumn(pieces, myPiece.gameObject.tag);

        //Debug.Log(currentPosition);
        List<Vector2Int> possibleMoves = myPiece.MoveLocations(currentPosition);
        List<bool> legalMoves = GetArrayOfLegalMoves(possibleMoves, myPiece, currentPosition);

        int index = 0;

        if (possibleMoves.Count > 0)
        {
            foreach (Vector2Int possibleMove in possibleMoves)
            {
                if (CheckIfPositionIsFree(pieces, possibleMove.x, possibleMove.y) == false) //check if position is non-empty
                {
                    GameObject objectOverHighlight = GetPieceAtPosition(possibleMove.x, possibleMove.y);
                    //Debug.Log(currentPlayer + " " + otherPlayer);
                    if (!string.IsNullOrEmpty(otherPlayer)) {
                        bool isThisMoveLegal = legalMoves.ElementAt(index);
                        if (isThisMoveLegal)
                        {
                            if ((myPiece.gameObject.tag.StartsWith(currentPlayer) && objectOverHighlight.tag.StartsWith(otherPlayer)) || (myPiece.gameObject.tag.StartsWith(otherPlayer) && objectOverHighlight.tag.StartsWith(currentPlayer)))
                            {
                                MeshRenderer renderers = objectOverHighlight.GetComponent<MeshRenderer>();
                                renderers.material = selectedMaterial;

                                highlightedObjects.Add(objectOverHighlight);

                                Vector3 tileHighlightPosition = Geometry.PointFromGrid(possibleMove);
                                //Debug.Log(tileHighlightPosition);
                                GameObject tileHighlight = Instantiate(tileHighlightPrefab, tileHighlightPosition + chessBoard.transform.position, Quaternion.identity); // + chessBoard.transform.position
                                tileHighlight.tag = "Highlight";
                                tileHighlights.Add(tileHighlight);
                            }
                        } 
                    }
                    //if ((myPiece.gameObject.tag.StartsWith("White") && objectOverHighlight.tag.StartsWith("Black")) || (myPiece.gameObject.tag.StartsWith("Black") && objectOverHighlight.tag.StartsWith("White")))    
                }
                else
                { 
                    Vector3 tileHighlightPosition = Geometry.PointFromGrid(possibleMove);
                    bool isThisMoveLegal = legalMoves.ElementAt(index);
                    GameObject tileHighlight;
                    if (isThisMoveLegal)
                    {
                        tileHighlight = Instantiate(tileHighlightPrefab, tileHighlightPosition + chessBoard.transform.position, Quaternion.identity);
                        tileHighlight.tag = "Highlight";
                        tileHighlights.Add(tileHighlight);
                    }
                    /*else
                    {
                        tileHighlight = Instantiate(tileHighlightIllegal, tileHighlightPosition + chessBoard.transform.position, Quaternion.identity);
                        tileHighlight.tag = "IllegalHighlight";
                    }*/
                    //Debug.Log(tileHighlightPosition);
                }
                index++;
            }
        }

        //castling 
        //not allowed when the king is in check
        if (myPiece.gameObject.CompareTag("BlackKing"))
        {
            if (!VerifyForCheck(pieces, false))
            {
                if (!movedPieces.Contains(myPiece.gameObject)) //the king is not moved
                {
                    //check in the left of the currentPosition
                    //x - row, y - column
                    if (CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y - 1) && CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y - 2))
                    {
                        GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y - 3);
                        if (otherPiece.CompareTag("BlackRook1") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int();
                            grid.Set(currentPosition.x, currentPosition.y - 2);
                            Vector3 anotherPossibleMove = Geometry.PointFromGrid(grid);

                            GameObject tileHighlight = Instantiate(tileHighlightPrefab, anotherPossibleMove + chessBoard.transform.position, Quaternion.identity);
                            tileHighlight.tag = "Highlight";
                            tileHighlights.Add(tileHighlight);

                            specialMove = 1;
                        }
                    }

                    //check in the right of the currentPosition
                    if (CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y + 1) && CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y + 2) && CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y + 3))
                    {
                        GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y + 4);
                        if (otherPiece.CompareTag("BlackRook2") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int();
                            grid.Set(currentPosition.x, currentPosition.y + 2);
                            Vector3 anotherPossibleMove = Geometry.PointFromGrid(grid);

                            GameObject tileHighlight = Instantiate(tileHighlightPrefab, anotherPossibleMove + chessBoard.transform.position, Quaternion.identity);
                            tileHighlight.tag = "Highlight";
                            tileHighlights.Add(tileHighlight);

                            specialMove = 2;
                        }
                    }
                }
            }  
        }

        if (myPiece.gameObject.CompareTag("WhiteKing"))
        {
            if (!VerifyForCheck(pieces, false))
            {
                if (!movedPieces.Contains(myPiece.gameObject)) //the king is not moved
                {
                    //check in the left of the currentPosition
                    if (CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y - 1) && CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y - 2) && CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y - 3))
                    {
                        GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y - 4);
                        if (otherPiece.CompareTag("WhiteRook1") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int();
                            grid.Set(currentPosition.x, currentPosition.y - 2);
                            Vector3 anotherPossibleMove = Geometry.PointFromGrid(grid);

                            GameObject tileHighlight = Instantiate(tileHighlightPrefab, anotherPossibleMove + chessBoard.transform.position, Quaternion.identity);
                            tileHighlight.tag = "Highlight";
                            tileHighlights.Add(tileHighlight);

                            specialMove = 1;
                        }
                    }

                    //check in the right of the currentPosition
                    if (CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y + 1) && CheckIfPositionIsFree(pieces, currentPosition.x, currentPosition.y + 2))
                    {
                        GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y + 3);
                        if (otherPiece.CompareTag("WhiteRook2") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int();
                            grid.Set(currentPosition.x, currentPosition.y + 2);
                            Vector3 anotherPossibleMove = Geometry.PointFromGrid(grid);

                            GameObject tileHighlight = Instantiate(tileHighlightPrefab, anotherPossibleMove + chessBoard.transform.position, Quaternion.identity);
                            tileHighlight.tag = "Highlight";
                            tileHighlights.Add(tileHighlight);

                            specialMove = 2;
                        }
                    }
                }
            }
        }
    }

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject capturedPiece = GetPieceAtPosition(gridPoint.x, gridPoint.y);

        if (ChosenGameMode == GameMode.SinglePlayer)
        {
            FileManager.instance.ChangePropertyStringValue(capturedPiece.tag, "X");
        }

        if (capturedPiece != null)
        {
            Destroy(capturedPiece);
        }
        
    }

    public void EndGame()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    Destroy(pieces[i, j]);;
                }
            }
        }
    }

    public void OnBackToStartButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Start");
    }

    #endregion
}
