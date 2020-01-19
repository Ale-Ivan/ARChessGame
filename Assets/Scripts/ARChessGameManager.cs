using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;
using ExitGames.Client.Photon;

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

    public Material selectedMaterial;
    public Material defaultMaterialBlack;
    public Material defaultMaterialWhite;
    public Material defaultMaterialBlue;
    public Material defaultMaterialRed;

    public GameObject tileHighlightPrefab;
    private List<GameObject> tileHighlights;
    public List<GameObject> highlightedObjects;

    public GameObject chessBoard;

    private bool isPieceSelected;
    private Piece selectedPiece;

    private static int numberOfBlackQueens;
    private static int numberOfWhiteQueens;
    private static int numberOfBlueQueens;
    private static int numberOfRedQueens;

    private string[] currentPlayers = { "White", "Red" };
    private string[] otherPlayers = { "Black", "Blue" };

    public string currentPlayer;
    public string otherPlayer;

    public List<GameObject> movedPieces;

    public int specialMove;

    private ulong attackedSquares; //store the squares that are attacked by the opponent's pieces; 2 hex digits/row;

    private Vector2Int kingPosition;

    public string colorOfLocalPlayer;
    public string colorOfOpponent;

    public GameObject chatGameObject;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[8, 8];
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

        kingPosition = new Vector2Int();
    }

    #region UI callback methods

    public void JoinWantedRoom()
    {
        

        uI_InformText.text = "Searching for available rooms";

        string roomName = PlayerSelectionManager.instance.GetRoomName();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);

        //PhotonNetwork.JoinRandomRoom();
        searchForGamesButtonGameObject.SetActive(false);
        raycastCenterImage.SetActive(false);
    }

    public void OnQuitMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
    }

    #endregion

    #region Photon callback methods

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //Debug.Log(message);
        uI_InformText.text = message;
        //CreateAndJoinRoom();
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
            //uI_InformText.text = "Joined room " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";
        }
        else
        {
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
        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
        chatGameObject.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion

    #region Private methods

    private void CreateAndJoinRoom()
    {
        string roomName = PlayerSelectionManager.instance.GetRoomName();
        //string randomRoomName = "Room " + Random.Range(0, 10000);
        //uI_InformText.text = "Created room " + roomName;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _gameObject.SetActive(false);
    }

    public void SetAttackSquare(int row, int column)
    {
        int square = 63 - (row * 8 + column);
        attackedSquares |= (1UL << square);
    }

    public bool VerifyForCheck()
    {
        Vector2Int myKingPosition;

        //Debug.Log(colorOfLocalPlayer + " " + colorOfOpponent);

        myKingPosition = GetRowAndColumn(colorOfLocalPlayer + "King");

        /*if (checkColorOfTheLocalPlayer_GameObject.E("White"))
        {
            myKingPosition = GetRowAndColumn("WhiteKing");
        }
        else if (checkColorOfTheLocalPlayer_GameObject.tag.StartsWith("Black"))
        {
            myKingPosition = GetRowAndColumn("BlackKing");
        }
        else if (checkColorOfTheLocalPlayer_GameObject.tag.StartsWith("Blue"))
        {
            myKingPosition = GetRowAndColumn("BlueKing");
        }
        else
        {
            myKingPosition = GetRowAndColumn("RedKing");
        }*/

        kingPosition = myKingPosition;

        int myKingSquare = 63 - (myKingPosition.x * 8 + myKingPosition.y);
        if ((attackedSquares & (1UL << myKingSquare)) == 0) //the bit is not set to 1
        {
            return false;
        }
        else //I am in check
        {
            Check();
            return true;
        }
    }

    public void RefreshAttackedSquares()
    {
        attackedSquares = 0x0000000000000000;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    if (pieces[i, j].tag.StartsWith(colorOfOpponent))
                    {
                        Vector2Int currentPosition = new Vector2Int(i, j);

                        Piece piece = pieces[i, j].GetComponent<Piece>();
                        piece.GetAttackLocations(currentPosition);
                    }
                }
            }
        }


        /*if (checkColorOfTheLocalPlayer_GameObject.tag.StartsWith("Black")) //local player is black
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i,j] != null)
                    {
                        if (pieces[i,j].tag.StartsWith("White")) //the opponent is white
                        {
                            Vector2Int currentPosition = new Vector2Int(i, j);

                            Piece piece = pieces[i, j].GetComponent<Piece>();
                            piece.GetAttackLocations(currentPosition);
                        }
                    }
                }
            }
        }
        else if (checkColorOfTheLocalPlayer_GameObject.tag.StartsWith("White")) //local player is white
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j] != null)
                    {
                        if (pieces[i, j].tag.StartsWith("Black")) //the opponent is black
                        {
                            Vector2Int currentPosition = new Vector2Int(i, j);

                            Piece piece = pieces[i, j].GetComponent<Piece>();
                            piece.GetAttackLocations(currentPosition);
                        }
                    }
                }
            }
        }*/
    }

    public void AddPiece(GameObject piece, int row, int col)
    {
        pieces[row, col] = piece;
    }

    public static void PrintPieces()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i,j] != null)
                {
                    Debug.Log(pieces[i, j].tag + " " + i + " " + j);
                }
            }
        }
    }

    public bool CheckIfPositionIsFree(int i, int j)
    {
        if (pieces[i, j] == null)
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

    public Vector2Int GetRowAndColumn(string wantedTag)
    {
        Vector2Int objectCoordinates = new Vector2Int();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    if (pieces[i, j].tag == wantedTag)
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

    public static void IncrementNumberOfBlueQueens()
    {
        numberOfBlueQueens++;
    }

    public static int GetNumberOfBlueQueens()
    {
        return numberOfBlueQueens;
    }

    public static void IncrementNumberOfRedQueens()
    {
        numberOfRedQueens++;
    }

    public static int GetNumberOfRedQueens()
    {
        return numberOfRedQueens;
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
        else if (myPiece.gameObject.tag.StartsWith("Blue"))
        {
            renderers.material = defaultMaterialBlue;
        }
        else if (myPiece.gameObject.tag.StartsWith("Red"))
        {
            renderers.material = defaultMaterialRed;
        }
        
        foreach(GameObject highlight in tileHighlights)
        {
            highlight.SetActive(false);
        }

        foreach(GameObject highlightedObject in highlightedObjects)
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
            else if (highlightedObject.gameObject.tag.StartsWith("Blue"))
            {
                renderer.material = defaultMaterialBlue;
            }
            else if (highlightedObject.gameObject.tag.StartsWith("Red"))
            {
                renderer.material = defaultMaterialRed;
            }
        }
        
        tileHighlights = new List<GameObject>();
        highlightedObjects = new List<GameObject>();

        myPiece.selected = false;

        selectedPiece = null;
        isPieceSelected = false;
    }

    private void ShowPossibleMoves(Piece myPiece)
    {
        Vector2Int currentPosition = GetRowAndColumn(myPiece.gameObject.tag);

        //Debug.Log(currentPosition);
        List<Vector2Int> possibleMoves = myPiece.MoveLocations(currentPosition);
        if (possibleMoves.Count > 0)
        {
            foreach (Vector2Int possibleMove in possibleMoves)
            {

                if (CheckIfPositionIsFree(possibleMove.x, possibleMove.y) == false) //check if position is non-empty
                {
                    GameObject objectOverHighlight = GetPieceAtPosition(possibleMove.x, possibleMove.y);
                    //if ((myPiece.gameObject.tag.StartsWith("White") && objectOverHighlight.tag.StartsWith("Black")) || (myPiece.gameObject.tag.StartsWith("Black") && objectOverHighlight.tag.StartsWith("White")))
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
                else
                { 
                    Vector3 tileHighlightPosition = Geometry.PointFromGrid(possibleMove);
                    //Debug.Log(tileHighlightPosition);
                    GameObject tileHighlight = Instantiate(tileHighlightPrefab, tileHighlightPosition + chessBoard.transform.position, Quaternion.identity);
                    tileHighlight.tag = "Highlight";
                    tileHighlights.Add(tileHighlight);
                }
            }
        }

        if (myPiece.gameObject.tag.Equals("BlackKing") || myPiece.gameObject.tag.Equals("BlueKing"))
        {
            if (!movedPieces.Contains(myPiece.gameObject)) //the king is not moved
            {
                //check in the left of the currentPosition
                //x - row, y - column
                if (CheckIfPositionIsFree(currentPosition.x, currentPosition.y - 1) && CheckIfPositionIsFree(currentPosition.x, currentPosition.y - 2))
                {
                    GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y - 3);
                    if ((otherPiece.tag.Equals("BlackRook1") || otherPiece.tag.Equals("BlueRook1")) && !movedPieces.Contains(otherPiece))
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
                if (CheckIfPositionIsFree(currentPosition.x, currentPosition.y + 1) && CheckIfPositionIsFree(currentPosition.x, currentPosition.y + 2) && CheckIfPositionIsFree(currentPosition.x, currentPosition.y + 3))
                {
                    GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y + 4);
                    if ((otherPiece.tag.Equals("BlackRook2") || otherPiece.tag.Equals("BlueRook2")) && !movedPieces.Contains(otherPiece))
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

        if (myPiece.gameObject.tag.Equals("WhiteKing") || myPiece.gameObject.tag.Equals("RedKing"))
        {
            if (!movedPieces.Contains(myPiece.gameObject)) //the king is not moved
            {
                //check in the left of the currentPosition
                if (CheckIfPositionIsFree(currentPosition.x, currentPosition.y - 1) && CheckIfPositionIsFree(currentPosition.x, currentPosition.y - 2) && CheckIfPositionIsFree(currentPosition.x, currentPosition.y - 3))
                {
                    GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y - 4);
                    if ((otherPiece.tag.Equals("WhiteRook1") || otherPiece.tag.Equals("RedRook1") ) && !movedPieces.Contains(otherPiece))
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
                if (CheckIfPositionIsFree(currentPosition.x, currentPosition.y + 1) && CheckIfPositionIsFree(currentPosition.x, currentPosition.y + 2))
                {
                    GameObject otherPiece = GetPieceAtPosition(currentPosition.x, currentPosition.y + 3);
                    if ((otherPiece.tag.Equals("WhiteRook2") || otherPiece.tag.Equals("RedRook2")) && !movedPieces.Contains(otherPiece))
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

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject capturedPiece = GetPieceAtPosition(gridPoint.x, gridPoint.y);
        if (capturedPiece.tag.Equals(currentPlayer + "King"))
        {
            uI_InformText.text = "Game Over! " + otherPlayer + " player wins!";
            uI_InformPanelGameObject.SetActive(true);

            gameOverPanel.SetActive(true);
            EndGame();
        }
        else if (capturedPiece.tag.Equals(otherPlayer + "King"))
        {
            uI_InformText.text = "Game Over! " + currentPlayer + " player wins!";
            uI_InformPanelGameObject.SetActive(true);

            gameOverPanel.SetActive(true);
            EndGame();
        }

        /*if (capturedPiece.tag.Equals("BlackKing")) //Game over
        {
            uI_InformText.text = "Game Over! White player wins!";
            uI_InformPanelGameObject.SetActive(true);

            gameOverPanel.SetActive(true);
            EndGame();
        }
        else if (capturedPiece.tag.Equals("WhiteKing")) //Game over
        {
            uI_InformText.text = "Game Over! Black player wins!";
            uI_InformPanelGameObject.SetActive(true);

            gameOverPanel.SetActive(true);
            EndGame();
        }*/
        //pieces[gridPoint.x, gridPoint.y] = null;
        
        if (capturedPiece != null)
        {
            Destroy(capturedPiece);
        }
        
        //capturedPiece.SetActive(false);
    }

    private void EndGame()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    pieces[i, j].SetActive(false);
                }
            }
        }
    }

    private void Check()
    {
        uI_InformText.text = "You are in CHECK! Save your King!";
        uI_InformPanelGameObject.SetActive(true);

        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
    }

    public void OnGameOverButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion
}
