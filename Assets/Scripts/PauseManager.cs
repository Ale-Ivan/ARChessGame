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
    public GameObject pauseCanvas;

    private int userID;
    private string username;
    private int numberOfLosses;

    public static bool isGamePaused;

    private void Start()
    {
        userID = FileManager.instance.ReadIntFromFile("UserID");
        username = FileManager.instance.ReadStringFromFile("Username");
        numberOfLosses = FileManager.instance.ReadIntFromFile("NumberOfLosses");

        isGamePaused = false;
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
        if (ARChessGameManager.ChosenGameMode == GameMode.SinglePlayer)
        {
            if (FileManager.instance.ExistsFile())
            {
                FileManager.instance.DeleteEntriesRelatedToLastGame();

                FileManager.instance.ChangeNumericPropertyValue("NumberOfLosses", numberOfLosses + 1);
            }
            SceneLoader.Instance.LoadScene("Scene_Start");
        }
        else
        {
            object[] data = new object[]
            {
                userID, username
            };

            FileManager.instance.ChangeNumericPropertyValue("NumberOfLosses", numberOfLosses + 1);

            RaiseQuitEvent(data);

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
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
        //create a modal for assuring this is what the user wants
        pauseCanvas.SetActive(true);
    }

    public void OnYesPauseButtonClicked()
    {
        if (FileManager.instance.ExistsFile())
        {
            FileManager.instance.AddNewProperty("ColorOfLocalPlayer", ARChessGameManager.colorOfLocalPlayer);
            FileManager.instance.AddNewProperty("ColorOfOpponent", ARChessGameManager.colorOfOpponent);
            FileManager.instance.AddNewBoolProperty("GamePaused", true);
            FileManager.instance.AddNewProperty("CurrentPlayer", ARChessGameManager.currentPlayer);
            FileManager.instance.AddNewProperty("OtherPlayer", ARChessGameManager.otherPlayer);

            FileManager.instance.SaveGameState();
        }

        SceneLoader.Instance.LoadScene("Scene_Start");
    }

    public void OnNoPauseButtonClicked()
    {
        pauseCanvas.SetActive(false);
    }

    public void OnPlayOnButtonClicked()
    {
        BackButtonOptions.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_GameModes");
    }
}
