using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using TMPro;
using ExitGames.Client.Photon;
using System;
using System.Linq;
using UnityEngine.UI;

public class ARChessGameManager : MonoBehaviourPunCallbacks
{
    public static GameObject[,] pieces;
    public static ARChessGameManager instance;

    [Header("UI")]
    public GameObject uI_InformPanelGameObject;
    public TextMeshProUGUI uI_InformText;
    public GameObject searchForGamesButtonGameObject;
    public GameObject playSingleplayerButtonGameObject;
    public GameObject adjustButton;
    public GameObject raycastCenterImage;
    public GameObject gameOverPanel;
    public GameObject pauseButtonGameObject;
    public GameObject startNewGameObject;

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

    public GameObject drawButton;
    public GameObject quitButton;
    public GameObject backButton;

    private List<GameObject> movedPieces;

    public int specialMove;

    private ulong attackedSquares; //store the squares that are attacked by the opponent's pieces; 2 hex digits/row;

    private GameObject[,] tempGamePlan;
    private ulong tempAttackedSquares;

    public static GameMode ChosenGameMode;

    public GameObject playerTurnGameObject;
    public Text playerTurnText;
    public static string roomName;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[8, 8];
        tempGamePlan = new GameObject[8, 8];
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

        uI_InformPanelGameObject.SetActive(true);

        currentPlayer = "White";
        otherPlayer = "Black";
    }

    #region UI callback methods
    public void OnPlayButtonClicked()
    {
        adjustButton.SetActive(false);
        raycastCenterImage.SetActive(false);
        uI_InformPanelGameObject.SetActive(false);

        playSingleplayerButtonGameObject.SetActive(false);

        if (FileManager.instance.ExistsFile() && !FileManager.instance.ReadBoolFromFile("PlayWithoutUser"))
        {
            pauseButtonGameObject.SetActive(true);
            backButton.SetActive(false);
            quitButton.SetActive(true);
        }

        if (FileManager.instance.ReadBoolFromFile("GamePaused"))
        {
            //do you want to start a new game or continue from where you have left?
            startNewGameObject.SetActive(true);
        }
        else
        {
            SpawnManager.instance.SpawnSingleplayer();
            playerTurnText.text = currentPlayer + "'s turn";
            playerTurnGameObject.SetActive(true);
        }
    }

    public void OnYesButtonClicked()
    {
        colorOfLocalPlayer = FileManager.instance.ReadStringFromFile("ColorOfLocalPlayer");
        colorOfOpponent = FileManager.instance.ReadStringFromFile("ColorOfOpponent");

        currentPlayer = FileManager.instance.ReadStringFromFile("CurrentPlayer");
        otherPlayer = FileManager.instance.ReadStringFromFile("OtherPlayer");

        startNewGameObject.SetActive(false);

        SpawnManager.instance.SpawnFromFile();
        playerTurnText.text = currentPlayer + "'s turn";
        playerTurnGameObject.SetActive(true);
        backButton.SetActive(false);
    }

    public void OnNoButtonClicked()
    {
        FileManager.instance.DeleteEntriesRelatedToLastGame();

        startNewGameObject.SetActive(false);

        SpawnManager.instance.SpawnSingleplayer();
        playerTurnText.text = currentPlayer + "'s turn";
        playerTurnGameObject.SetActive(true);
        backButton.SetActive(false);
    }

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

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("failed");
        uI_InformText.text = message;
        CreateAndJoinRoom(roomName);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        uI_InformText.text = message;
        CreateAndJoinRoom("");
    }

    public override void OnCreatedRoom()
    {
        uI_InformText.text = "created room " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    IEnumerator NoPlayerEnteredRoom()
    {
        yield return new WaitForSeconds(10);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "We could not find anyone for you to play with...";

            EndGame();
            //PhotonNetwork.LeaveRoom();
            gameOverPanel.SetActive(true);

            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 3.0f)); 
        }
    }

    public override void OnJoinedRoom()
    {
        adjustButton.SetActive(false);
        raycastCenterImage.SetActive(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "Joined room " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";

            if (ChosenGameMode == GameMode.MultiplayerAtRandom)
            {
                StartCoroutine(NoPlayerEnteredRoom());
            }
        }
        else
        {
            opponentName = PhotonNetwork.PlayerListOthers[0].NickName;
            //Debug.Log(opponentName);

            uI_InformText.text = "Joined room " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
            chatGameObject.SetActive(true);

            drawButton.SetActive(true);
            quitButton.SetActive(true);
            backButton.SetActive(false);

            playerTurnText.text = currentPlayer + "'s turn";
            playerTurnGameObject.SetActive(true);
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

            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
            chatGameObject.SetActive(true);

            drawButton.SetActive(true);
            quitButton.SetActive(true);
            backButton.SetActive(false);

            playerTurnText.text = currentPlayer + "'s turn";
            playerTurnGameObject.SetActive(true);
        }
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Start");
    }

    #endregion

    #region Private methods

    private void CreateAndJoinRoom(string roomName)
    {
        string randomRoomName;
        if (string.IsNullOrEmpty(roomName))
        {
            randomRoomName = "Room " + UnityEngine.Random.Range(0, 10000);
            uI_InformText.text = "Created room " + randomRoomName;
        }
        else
        {
            randomRoomName = roomName;
            uI_InformText.text = "Created room " + randomRoomName;
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2
        };

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        uI_InformText.text = "Created room failed" + message;
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
                    int numberOfLosses = FileManager.instance.ReadIntFromFile("NumberOfLosses");
                    FileManager.instance.ChangeNumericPropertyValue("NumberOfLosses", numberOfLosses + 1);

                    uI_InformText.text = "Game Over! Your opponent, " + opponentName + ", wins!";
                    uI_InformPanelGameObject.SetActive(true);

                    gameOverPanel.SetActive(true);
                    EndGame();

                    object[] data = new object[]
                    {
                        PhotonNetwork.LocalPlayer.NickName
                    };
                    RaiseEventCheckMate(data);

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

    private void RaiseEventCheckMate(object[] data)
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
                        List<Vector2Int> possibleMoves = myPiece.MoveLocations(pieces, currentPosition);
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
                    if (wantedPieces[i, j].tag.StartsWith("colorOfOpponent"))
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
                    if (pieces[i, j].CompareTag(tag))
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

    public GameObject GetPieceAtPosition(GameObject[,] gamePlan, int x, int y)
    {
        return gamePlan[x, y];
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

        StartCoroutine(ChangePlayerText());

        if (ChosenGameMode == GameMode.SinglePlayer)
        {
            StartCoroutine(ChangePlayerWait());
        }

        //Debug.Log(currentPlayer + " " + otherPlayer);
    }

    public IEnumerator ChangePlayerWait()
    {
        yield return new WaitForSeconds(2);
        if (currentPlayer == colorOfOpponent)
        {
            MovePieceAI();
        }
    }

    public IEnumerator ChangePlayerText()
    {
        yield return new WaitForSeconds(1);
        playerTurnText.text = currentPlayer + "'s turn";
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
        if (ChosenGameMode == GameMode.MultiplayerAtRandom || ChosenGameMode == GameMode.MultiplayerWithFriend)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount != 2)
            {
                return;
            }
        }

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

    public void MovePieceAI()
    {
        Tuple<Piece, double, Vector2Int> value;

        value = PieceMoveEvaluation.instance.AlphaBetaMax(2, int.MinValue, int.MaxValue, pieces);

        /*if (colorOfOpponent.Equals("White"))
        {
            value = PieceMoveEvaluation.instance.AlphaBetaMax(2, int.MinValue, int.MaxValue, pieces);
        }
        else
        {
            value = PieceMoveEvaluation.instance.AlphaBetaMin(2, int.MinValue, int.MaxValue, pieces);
        }*/

        // Debug.Log(value.Item2);
        //Debug.Log(value.Item1.gameObject.tag);

        Piece piece = value.Item1;
        Vector2Int initialPosition = GetRowAndColumn(pieces, value.Item1.gameObject.tag);
        Vector2Int finalPosition = value.Item3;

        MovePiece(piece.GetGameObject(), Geometry.PointFromGrid(finalPosition) + chessBoard.transform.position);

        if (!CheckIfPositionIsFree(pieces, finalPosition.x, finalPosition.y))
        {
            CapturePieceAt(finalPosition);
        }

        SetPositionToObject(finalPosition.x, finalPosition.y, piece.GetGameObject());
        SetPositionToNull(initialPosition.x, initialPosition.y);

        VerifyForCheck(pieces, false);

        ChangePlayer();
    }

    public List<Tuple<Piece, Vector2Int, GameObject[,]>> FindAllPossibleMovesForPiecesOfColor(GameObject[,] gamePlan, string color)
    {
        List<Tuple<Piece, Vector2Int, GameObject[,]>> allPossibleMoves = new List<Tuple<Piece, Vector2Int, GameObject[,]>>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (gamePlan[i, j] != null)
                {
                    if (gamePlan[i, j].tag.StartsWith(color))
                    {
                        //get all possible moves for each piece of certain color
                        Piece piece = gamePlan[i, j].GetComponent<Piece>();
                        Vector2Int currentPieceLocation = new Vector2Int(i, j);
                        List<Vector2Int> pieceMoves;
                        
                        if (color.Equals(colorOfOpponent))
                        {
                            pieceMoves = piece.MoveLocationsForAI(gamePlan, currentPieceLocation);
                        }
                        else
                        {
                            pieceMoves = piece.MoveLocations(gamePlan, currentPieceLocation);
                        }

                        /*foreach(Vector2Int pieceMove in pieceMoves)
                        {
                            Debug.Log(piece.gameObject.tag + " " + pieceMove);
                        }*/

                        //get game plan for this possible move
                        List<Tuple<Piece, Vector2Int, GameObject[,]>> temp = GetAllGamePlansForAllPossibleMoves(gamePlan, pieceMoves, piece, currentPieceLocation);
                        allPossibleMoves.AddRange(temp);
                    }
                }
            }
        }

        //Debug.Log(allPossibleMoves.Count);
        return allPossibleMoves;
    }

    private List<Tuple<Piece, Vector2Int, GameObject[,]>> GetAllGamePlansForAllPossibleMoves(GameObject[,] gamePlan, List<Vector2Int> possibleMoves, Piece piece, Vector2Int currentPosition)
    {
        GameObject[,] tempGamePlan;
        List<Tuple<Piece, Vector2Int, GameObject[,]>> returnList = new List<Tuple<Piece, Vector2Int, GameObject[,]>>();

        foreach (Vector2Int possibleMove in possibleMoves)
        {
            tempGamePlan = new GameObject[8, 8];
            Array.Copy(gamePlan, tempGamePlan, gamePlan.Length);

            //move piece in each possible location
            tempGamePlan[possibleMove.x, possibleMove.y] = piece.gameObject;
            tempGamePlan[currentPosition.x, currentPosition.y] = null;

            //refresh attacked squares
            RefreshAttackedSquares(tempGamePlan, true);

            //check if moving the piece to that position puts you in check
            bool legal = !VerifyForCheck(tempGamePlan, true);

            if (legal)
            {
                returnList.Add(new Tuple<Piece, Vector2Int, GameObject[,]>(piece, possibleMove, tempGamePlan));
            }
        }

        return returnList;
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

        List<Vector2Int> possibleMoves;
        //Debug.Log(currentPosition);
        if (myPiece.gameObject.tag.StartsWith(colorOfLocalPlayer))
        {
            possibleMoves = myPiece.MoveLocations(pieces, currentPosition);
        }
        else
        {
            possibleMoves = myPiece.MoveLocationsForAI(pieces, currentPosition);
        }

        List<bool> legalMoves = GetArrayOfLegalMoves(possibleMoves, myPiece, currentPosition);

        int index = 0;

        if (possibleMoves.Count > 0)
        {
            foreach (Vector2Int possibleMove in possibleMoves)
            {
                if (CheckIfPositionIsFree(pieces, possibleMove.x, possibleMove.y) == false) //check if position is non-empty
                {
                    GameObject objectOverHighlight = GetPieceAtPosition(pieces, possibleMove.x, possibleMove.y);
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
                        GameObject otherPiece = GetPieceAtPosition(pieces, currentPosition.x, currentPosition.y - 3);
                        if (otherPiece.CompareTag("BlackRook1") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int(currentPosition.x, currentPosition.y - 2);
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
                        GameObject otherPiece = GetPieceAtPosition(pieces, currentPosition.x, currentPosition.y + 4);
                        if (otherPiece.CompareTag("BlackRook2") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int(currentPosition.x, currentPosition.y + 2);
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
                        GameObject otherPiece = GetPieceAtPosition(pieces, currentPosition.x, currentPosition.y - 4);
                        if (otherPiece.CompareTag("WhiteRook1") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int(currentPosition.x, currentPosition.y - 2);
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
                        GameObject otherPiece = GetPieceAtPosition(pieces, currentPosition.x, currentPosition.y + 3);
                        if (otherPiece.CompareTag("WhiteRook2") && !movedPieces.Contains(otherPiece))
                        {
                            Vector2Int grid = new Vector2Int(currentPosition.x, currentPosition.y + 2);
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
        GameObject capturedPiece = GetPieceAtPosition(pieces, gridPoint.x, gridPoint.y);

        if (ChosenGameMode == GameMode.SinglePlayer)
        {
            FileManager.instance.ChangePropertyStringValue(capturedPiece.tag, "X");
        }

        if (capturedPiece != null)
        {
            pieces[gridPoint.x, gridPoint.y] = null;
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
