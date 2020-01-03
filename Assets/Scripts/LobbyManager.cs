using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;

using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject UI_LoginGameObject;

    [Header("Lobby UI")]
    public GameObject UI_LobbyGameObject;
    public GameObject UI_3DGameObject;

    [Header("Connection Status UI")]
    public GameObject UI_ConnectionStatusGameObject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;

    #region UNITY METHODS
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            //Activate only Lobby UI
            UI_LobbyGameObject.SetActive(true);
            UI_3DGameObject.SetActive(true);

            UI_ConnectionStatusGameObject.SetActive(false);
            UI_LoginGameObject.SetActive(false);
        }
        else
        {
            //Activate only Login UI
            UI_LobbyGameObject.SetActive(false);
            UI_3DGameObject.SetActive(false);
            UI_ConnectionStatusGameObject.SetActive(false);

            UI_LoginGameObject.SetActive(true);
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }

    #endregion

    #region UI callback methods

    public void OnEnterGameButtonClicked()
    {
        string playerName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            UI_LobbyGameObject.SetActive(false);
            UI_3DGameObject.SetActive(false);
            UI_LoginGameObject.SetActive(false);

            showConnectionStatus = true;
            UI_ConnectionStatusGameObject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player name is invalid or empty!");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        //SceneManager.LoadScene("Scene_Loading");
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }

    #endregion

    #region Photon Callback methods

    public override void OnConnected()
    {
        Debug.Log("We connected to Internet!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon!");

        UI_LobbyGameObject.SetActive(true);
        UI_3DGameObject.SetActive(true);

        UI_LoginGameObject.SetActive(false);
        UI_ConnectionStatusGameObject.SetActive(false);
    }

    #endregion

    public void Quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit ();
#endif
    }

}
