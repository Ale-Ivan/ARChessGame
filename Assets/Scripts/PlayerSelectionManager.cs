using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;

    private int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI chessPieceType_Text;
    public Button next_Button;
    public Button previous_Button;
    public GameObject unlock_Button;
    public InputField roomNameInputField;

    public GameObject uI_Selection;
    public GameObject uI_AfterSelection;
    public GameObject bottomBarForSelection;

    public Material[] possibleMaterialColors;

    public GameObject selectionPiece;
    private string[] colors = { "Black", "White", "Blue", "Red" };

    public Image lockImage;

    private bool unlockedBlue;
    private bool unlockedRed;

    public static PlayerSelectionManager instance;

    private string roomName;

    public GameObject uI_ForegroundGameObject;
    public GameObject uI_RoomNameGameObject;

    #region UNITY methods
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSelectionNumber = 0;
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);

        selectionPiece.SetActive(true);

        bottomBarForSelection.SetActive(true);

        unlock_Button.SetActive(false);

        lockImage.enabled = false;

        unlockedBlue = false;
        unlockedRed = false;

        uI_RoomNameGameObject.SetActive(false);
        uI_ForegroundGameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region UI callback methods

    public void NextPiece()
    {
        playerSelectionNumber++;

        if (playerSelectionNumber >= possibleMaterialColors.Length)
        {
            playerSelectionNumber = 0;
        }

        next_Button.enabled = false;
        previous_Button.enabled = false;

        selectionPiece.GetComponent<MeshRenderer>().material = possibleMaterialColors[playerSelectionNumber];
        chessPieceType_Text.text = colors[playerSelectionNumber];

        ShowLock();

        next_Button.enabled = true;
        previous_Button.enabled = true;
    }

    public void PreviousPiece()
    {
        playerSelectionNumber--;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = possibleMaterialColors.Length - 1;
        }

        next_Button.enabled = false;
        previous_Button.enabled = false;

        selectionPiece.GetComponent<MeshRenderer>().material = possibleMaterialColors[playerSelectionNumber];
        chessPieceType_Text.text = colors[playerSelectionNumber];

        ShowLock();

        next_Button.enabled = true;
        previous_Button.enabled = true;
    }

    private void ShowLock()
    {
        if ((playerSelectionNumber == 2 && !unlockedBlue) || (playerSelectionNumber == 3 && !unlockedRed))
        {
            lockImage.enabled = true;
            bottomBarForSelection.SetActive(false);
            unlock_Button.SetActive(true);
        }
        else
        {
            lockImage.enabled = false;
            bottomBarForSelection.SetActive(true);
            unlock_Button.SetActive(false);
        }
    }

    public void OnConfirmButtonClicked()
    {
        roomName = roomNameInputField.text;
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public string GetRoomName()
    {
        return this.roomName;
    }

    public void OnSelectButtonClicked()
    {
        uI_Selection.SetActive(false);
        uI_AfterSelection.SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable {
            { MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnUnlockButtonClicked()
    {
        if (playerSelectionNumber == 2)
        {
            unlockedBlue = true;
        } 
        else if (playerSelectionNumber == 3)
        {
            unlockedRed = true;
        }

        unlock_Button.SetActive(false);
        lockImage.enabled = false;
        bottomBarForSelection.SetActive(true);
    }

    public void OnReselectButtonClicked()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
    }

    public void OnPlayButtonClicked()
    {
        uI_RoomNameGameObject.SetActive(true);
        uI_ForegroundGameObject.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }
    #endregion

    #region Private methods

   

    #endregion
}
