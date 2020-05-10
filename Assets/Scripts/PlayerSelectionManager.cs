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

    public GameObject uI_Selection;
    public GameObject uI_AfterSelection;
    public GameObject bottomBarForSelection;

    public GameObject[] pieces;

    public Mesh[] possibleMeshes;
    public Material[] possibleColorMaterials;
    private string[] colors = { "Black", "White" }; //only black and white

    public Image lockImage;

    private bool unlockedExtraBlack;
    private bool unlockedExtraWhite;

    public static PlayerSelectionManager instance;

    public GameObject uI_ForegroundGameObject;

    public GameObject selectionPiece;

    private float originalXPosition;
    private float originalYPosition;
    private float originalZPosition;

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

        bottomBarForSelection.SetActive(true);

        unlock_Button.SetActive(false);

        lockImage.enabled = false;

        unlockedExtraBlack = false;
        unlockedExtraWhite = false;

        uI_ForegroundGameObject.SetActive(true);

        originalXPosition = selectionPiece.transform.position.x;
        originalYPosition = selectionPiece.transform.position.y;
        originalZPosition = selectionPiece.transform.position.z;
    }

    #endregion

    #region UI callback methods

    public void NextPiece()
    {
        playerSelectionNumber++;

        if (playerSelectionNumber >= 4)
        {
            playerSelectionNumber = 0;
        }

        next_Button.enabled = false;
        previous_Button.enabled = false;

        Material chosenColor;
        Mesh chosenMesh;
       
        if (playerSelectionNumber == 0 || playerSelectionNumber == 2)
        {
            chosenColor = possibleColorMaterials[0];
        } 
        else
        {
            chosenColor = possibleColorMaterials[1];
        }

        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            chosenMesh = possibleMeshes[0];
        } 
        else
        {
            chosenMesh = possibleMeshes[1];
        }

        if (playerSelectionNumber == 2 || playerSelectionNumber == 3)
        {
            selectionPiece.transform.rotation = Quaternion.identity;
            selectionPiece.transform.position = new Vector3(originalXPosition, originalYPosition - 0.05f, originalZPosition);
        }
        else
        {
            selectionPiece.transform.rotation = Quaternion.Euler(-90, 0, 0);
            selectionPiece.transform.position = new Vector3(originalXPosition, originalYPosition, originalZPosition);
        }


        selectionPiece.GetComponent<MeshRenderer>().material = chosenColor;
        selectionPiece.GetComponent<MeshFilter>().mesh = chosenMesh;

        //black and white only
        chessPieceType_Text.text = colors[playerSelectionNumber % 2];

        ShowLock();

        next_Button.enabled = true;
        previous_Button.enabled = true;
    }

    public void PreviousPiece()
    {
        playerSelectionNumber--;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = 3;
        }

        next_Button.enabled = false;
        previous_Button.enabled = false;

        Material chosenColor;
        Mesh chosenMesh;

        if (playerSelectionNumber == 0 || playerSelectionNumber == 2)
        {
            chosenColor = possibleColorMaterials[0];
        }
        else
        {
            chosenColor = possibleColorMaterials[1];
        }

        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            chosenMesh = possibleMeshes[0];
        }
        else
        {
            chosenMesh = possibleMeshes[1];
        }

        if (playerSelectionNumber == 2 || playerSelectionNumber == 3)
        {
            selectionPiece.transform.rotation = Quaternion.identity;
            selectionPiece.transform.position = new Vector3(originalXPosition, originalYPosition - 0.05f, originalZPosition);
        }
        else
        {
            selectionPiece.transform.rotation = Quaternion.Euler(-90, 0, 0);
            selectionPiece.transform.position = new Vector3(originalXPosition, originalYPosition, originalZPosition);
        }

        selectionPiece.GetComponent<MeshRenderer>().material = chosenColor;
        selectionPiece.GetComponent<MeshFilter>().mesh = chosenMesh;


        //black and white only
        chessPieceType_Text.text = colors[playerSelectionNumber % 2];

        ShowLock();

        next_Button.enabled = true;
        previous_Button.enabled = true;

    }

    private void ShowLock()
    {
        if ((playerSelectionNumber == 2 && !unlockedExtraBlack) || (playerSelectionNumber == 3 && !unlockedExtraWhite))
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

    public string GetRoomName()
    {
        return "";
    }

    public void OnSelectButtonClicked()
    {
        uI_Selection.SetActive(false);
        uI_AfterSelection.SetActive(true);  
    }

    public void OnUnlockButtonClicked()
    {
        if (playerSelectionNumber == 2)
        {
            unlockedExtraBlack = true;
        }
        else if (playerSelectionNumber == 3)
        {
            unlockedExtraWhite = true;
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
        uI_ForegroundGameObject.SetActive(false);

        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable {
            { MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);

        SceneLoader.Instance.LoadScene("Scene_GameModes");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Start");
    }
    #endregion
}
