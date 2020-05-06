using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayWithoutUserManager : MonoBehaviour
{
    public InputField PlayerNameInputField;

    public void OnPlayButtonClicked()
    {
        //Connect to Photon
        PhotonFunctions.ConnectToPhoton(PlayerNameInputField.text);

        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Start");
    }
}
