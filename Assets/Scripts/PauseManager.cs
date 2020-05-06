using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using SimpleJSON;
using System.IO;
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

    private void Awake()
    {
        path = Application.persistentDataPath + "/ARChessGameUserSave.json";
        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            userJSON = (JSONObject)JSON.Parse(jsonString);
            userID = userJSON["ID"];
            username = userJSON["Username"];
            numberOfWins = userJSON["NumberOfWins"];
            numberOfLosses = userJSON["NumberOfLosses"];
        }
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

        userJSON["NumberOfLosses"] = numberOfLosses + 1;
        File.WriteAllText(path, userJSON.ToString());

        RaiseQuitEvent(data);

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        SceneLoader.Instance.LoadScene("Scene_Start");
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
}
