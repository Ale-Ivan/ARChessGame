using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    SinglePlayer = 0,
    MultiplayerAtRandom = 1,
    MultiplayerWithFriend = 2
}

public class GameModesManager : Singleton<GameModesManager>
{
    [Header("ShowAndHide")]
    public GameObject OptionsToPlay;
    public GameObject MultiplayerRoomName;

    public InputField RoomNameInputField;

    private string RoomName;

    public void OnSinglePlayerButtonClicked()
    {
        ARChessGameManager.ChosenGameMode = GameMode.SinglePlayer;
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnPlayAtRandomButtonClicked()
    {
        ARChessGameManager.ChosenGameMode = GameMode.MultiplayerAtRandom;
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnPlayWithFriendButtonClicked()
    {
        ARChessGameManager.ChosenGameMode = GameMode.MultiplayerWithFriend;
        OptionsToPlay.SetActive(false);
        MultiplayerRoomName.SetActive(true);
    }

    public void OnConfirmButtonClicked()
    {
        RoomName = RoomNameInputField.text;
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Start");
    }

    public string GetRoomName()
    {
        return RoomName;
    }
}
