using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using SimpleJSON;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject BackButtonOptions;

    private string path;
    private JSONObject userJSON;
    private int userID;
    private string username;
    private int numberOfWins;
    private int numberOfLosses;

    private void Start()
    {
        userID = FileManager.instance.ReadIntFromFile("UserID");
        username = FileManager.instance.ReadStringFromFile("Username");
        numberOfLosses = FileManager.instance.ReadIntFromFile("NumberOfLosses");
    }

    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackButtonOptions.SetActive(true);
            }
        }
    }

    public void OnQuitButtonClicked()
    {
        object[] data = new object[]
        {
            userID, username
        };

        //only if singleplayer
        if (ARChessGameManager.ChosenGameMode == GameMode.SinglePlayer)
        {
            FileManager.instance.DeleteEntriesThatStartWith(ARChessGameManager.colorOfLocalPlayer);
            FileManager.instance.DeleteEntriesThatStartWith(ARChessGameManager.colorOfOpponent);
        }

        FileManager.instance.ChangePropertyIntValue("NumberOfLosses", numberOfLosses + 1);

        RaiseQuitEvent(data);

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        //SceneLoader.Instance.LoadScene("Scene_Start");
        //PhotonFunctions.DisconnectFromPhoton();
    }

    private void RaiseQuitEvent(object[] data)
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

        PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerQuitGameCode, data, raiseEventOptions, sendOptions);
    }

    public void OnPauseButtonClicked()
    {
        FileManager.instance.AddNewProperty("ColorOfLocalPlayer", ARChessGameManager.colorOfLocalPlayer);
        FileManager.instance.AddNewProperty("ColorOfOpponent", ARChessGameManager.colorOfOpponent);
        FileManager.instance.SaveGameState();
    }

    public void OnPlayOnButtonClicked()
    {
        BackButtonOptions.SetActive(false);
    }
}
