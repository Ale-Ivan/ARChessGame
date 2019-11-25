using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;

    public GameObject[] chessPiecesModels;

    public int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI chessPieceType_Text;
    public Button next_Button;
    public Button previous_Button;

    public GameObject uI_Selection;
    public GameObject uI_AfterSelection;


    #region UNITY methods
    // Start is called before the first frame update
    void Start()
    {
        playerSelectionNumber = 0;
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);

        chessPiecesModels[0].SetActive(true);
        chessPiecesModels[1].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region UI callback methods

    public void NextPiece()
    {
        Debug.Log(playerSelectionNumber);

        playerSelectionNumber++;

        if (playerSelectionNumber >= chessPiecesModels.Length)
        {
            playerSelectionNumber = 0;
        }

        next_Button.enabled = false;
        previous_Button.enabled = false;


        if (playerSelectionNumber == 0)
        {
            ShowBlackPiece();
            chessPieceType_Text.text = "Black";  
        }
        else
        {
            ShowWhitePiece();
            chessPieceType_Text.text = "White";
        }
    }

    public void PreviousPiece()
    {
        Debug.Log(playerSelectionNumber);


        playerSelectionNumber--;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = chessPiecesModels.Length - 1;
        }

        next_Button.enabled = false;
        previous_Button.enabled = false;

        if (playerSelectionNumber == 0)
        {
            ShowBlackPiece();
            chessPieceType_Text.text = "Black";
        }
        else
        {
            ShowWhitePiece();
            chessPieceType_Text.text = "White";
        }
    }

    public void OnSelectButtonClicked()
    {
        uI_Selection.SetActive(false);
        uI_AfterSelection.SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable {
            { MultiplayerARChessGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnReselectButtonClicked()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
    }


    public void OnPlayButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }
    #endregion

    #region Private methods

    private void ShowBlackPiece()
    {
        chessPiecesModels[0].SetActive(true);
        chessPiecesModels[1].SetActive(false);

        next_Button.enabled = true;
        previous_Button.enabled = true;
    }

    private void ShowWhitePiece()
    {
        chessPiecesModels[0].SetActive(false);
        chessPiecesModels[1].SetActive(true);

        next_Button.enabled = true;
        previous_Button.enabled = true;
    }

    #endregion
}
