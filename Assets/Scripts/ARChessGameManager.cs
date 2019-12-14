using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

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

    public Material selectedMaterial;
    public Material defaultMaterialBlack;
    public Material defaultMaterialWhite;

    public GameObject tileHighlightPrefab;
    private List<GameObject> tileHighlights;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[8, 8];
        uI_InformPanelGameObject.SetActive(true);
        tileHighlights = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region UI callback methods

    public void JoinRandomRoom()
    {
        uI_InformText.text = "Searching for available rooms";
        PhotonNetwork.JoinRandomRoom();
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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        uI_InformText.text = message;
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        adjustButton.SetActive(false);
        raycastCenterImage.SetActive(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "Joined " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";
        }
        else
        {
            uI_InformText.text = "Joined " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
        }

        Debug.Log("joined " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InformText.text = newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount;
        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion

    #region Private methods

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _gameObject.SetActive(false);
    }

    public void AddPiece(GameObject piece, int row, int col)
    {
        //PhotonView objectPhotonView = piece.GetComponent<PhotonView>();
        //PhotonNetwork.AllocateSceneViewID(objectPhotonView);
        //Debug.Log(objectPhotonView.ViewID);

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

    public GameObject GetPieceAtPosition(int x, int y)
    {
        return pieces[x, y];
    }
    public void MovePiece(GameObject piece, Vector3 finalPosition)
    {
        StartCoroutine(MoveObject(piece, piece.transform.position, finalPosition));
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

    public void SelectPiece(Piece myPiece)
    {
        MeshRenderer renderers = myPiece.GetComponent<MeshRenderer>();
        renderers.material = selectedMaterial;
        myPiece.selected = true;
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
        else
        {
            renderers.material = defaultMaterialWhite;
        }
        foreach(GameObject highlight in tileHighlights)
        {
            highlight.SetActive(false);
        }
        //tileHighlights = new List<GameObject>();
        myPiece.selected = false;
    }

    private void ShowPossibleMoves(Piece myPiece)
    {
        Vector2Int currentPosition = GetRowAndColumn(myPiece.gameObject.tag);
        List<Vector2Int> possibleMoves = myPiece.MoveLocations(currentPosition);
        foreach(Vector2Int possibleMove in possibleMoves)
        {
            Vector3 tileHighlightPosition = Geometry.PointFromGrid(possibleMove);
            GameObject tileHighlight = Instantiate(tileHighlightPrefab, tileHighlightPosition, Quaternion.identity);
            tileHighlights.Add(tileHighlight);
            if (CheckIfPositionIsFree(possibleMove.x, possibleMove.y) == false) //check if position is empty
            {
                GameObject objectOverHighlight = GetPieceAtPosition(possibleMove.x, possibleMove.y);
                MeshRenderer renderers = objectOverHighlight.GetComponent<MeshRenderer>();
                renderers.material = selectedMaterial;
            }
            
        }
    }

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject capturedPiece = GetPieceAtPosition(gridPoint.x, gridPoint.y);
        if (capturedPiece.tag.Contains("King")) //Game over
        {

        }
        //pieces[gridPoint.x, gridPoint.y] = null;
        Destroy(capturedPiece);
        //capturedPiece.SetActive(false);


    }


    #endregion
}
