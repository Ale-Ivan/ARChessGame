using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

using TMPro;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    public GameObject drawOptions;
    public GameObject opponentDrawOptions;
    public GameObject uI_InformPanelGameObject;
    public TextMeshProUGUI uI_InformText;

    public GameObject gameOverPanel;

    public void OnDrawButtonClicked()
    {
        drawOptions.SetActive(true);
    }

    public void OnPlayWithoutDrawButtonClicked()
    {
        drawOptions.SetActive(false);
    }

    public void OnEndGameWithDrawButtonClicked()
    {
        uI_InformText.text = "Waiting for opponent to accept the draw request...";
        uI_InformPanelGameObject.SetActive(true);

        drawOptions.SetActive(false);

        object[] data = new object[]
        {
            PhotonNetwork.LocalPlayer.NickName
        };
        RaiseEventDraw(data, 4);
    }

    private void RaiseEventDraw(object[] data, int eventNumber)
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

        if (eventNumber == 4)
        {
            PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerRequestedDraw, data, raiseEventOptions, sendOptions);
        }
        else if (eventNumber == 5)
        {
            PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.OpponentAcceptedDraw, data, raiseEventOptions, sendOptions);
        }
        else if (eventNumber == 6)
        {
            PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.OpponentDeniedDraw, data, raiseEventOptions, sendOptions);
        }
    }

    public void OnAcceptDrawRequestButtonClicked()
    {
        int numberOfLosses = FileManager.instance.ReadIntFromFile("NumberOfWins");
        FileManager.instance.ChangeNumericPropertyValue("NumberOfWins", numberOfLosses + 0.5);

        object[] data = new object[]
        {
            PhotonNetwork.LocalPlayer.NickName
        };
        RaiseEventDraw(data, 5);

        opponentDrawOptions.SetActive(false);

        gameOverPanel.SetActive(true);
        ARChessGameManager.instance.EndGame();
    }

    public void OnDenyDrawRequestButtonClicked()
    {
        object[] data = new object[]
        {
            PhotonNetwork.LocalPlayer.NickName
        };
        RaiseEventDraw(data, 6);

        opponentDrawOptions.SetActive(false);
        //uI_InformPanelGameObject.SetActive(false);
    }
}
