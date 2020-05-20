using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System.IO;

public class StartPageManager : MonoBehaviour
{
    private string username;
    private int numberOfWins = 0;
    private int numberOfLosses = 0;
    private bool isLoggedIn;

    [Header("UserProfile")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI NumberOfWins;
    public TextMeshProUGUI NumberOfLosses;

    private bool isUserProfileActive = false;
    public GameObject userProfileButton;
    public GameObject userProfile;

    [Header("PlayOptions")]
    public GameObject Options;
    public GameObject OptionsIfFile;
    public GameObject OptionsIfNotFile;
    private bool isCreateUserInfoActive = false;
    public GameObject createUserInfo;

    private bool isPlayWithoutUserInfoActive = false;
    public GameObject playWithoutUserInfo;

    private JSONObject userJSON;
    string path;

    public GameObject ExitButtonOptions;

    private void Awake()
    {
        path = Application.persistentDataPath + "/ARChessGameUserSave.json";
        TitleText.text = "Welcome!";
        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            userJSON = (JSONObject) JSON.Parse(jsonString);
            isLoggedIn = userJSON["IsLoggedIn"];
            if (isLoggedIn)
            {
                username = userJSON["Username"];
                numberOfWins = userJSON["NumberOfWins"];
                numberOfLosses = userJSON["NumberOfLosses"];

                PlayerName.text = username;
                TitleText.text = "Welcome back, " + username + "!";
                NumberOfWins.text = numberOfWins.ToString();
                NumberOfLosses.text = numberOfLosses.ToString();

                userProfileButton.SetActive(true);   
            }   
        }
    }

    public void OnPlayButtonClicked()
    {
        Options.SetActive(true);
        if (File.Exists(path))
        {
            isLoggedIn = userJSON["IsLoggedIn"];
            if (isLoggedIn)
            {
                //Connect to Photon
                PhotonFunctions.ConnectToPhoton(username);
                SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
            }
            else
            {
                OptionsIfFile.SetActive(true);
            }
        } 
        else
        {
            OptionsIfNotFile.SetActive(true);
        }
    }

    public void OnExitButtonClicked()
    {
        //disconnect from Photon
        PhotonFunctions.DisconnectFromPhoton();

        ExitButtonOptions.SetActive(true);  
    }

    public void OnYesButtonClicked()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit ();
        #endif
    }

    public void OnNoButtonClicked()
    {
        ExitButtonOptions.SetActive(false);
    }

    public void OnUserButtonClicked()
    {
        if (isUserProfileActive)
        {
            isUserProfileActive = false;
        } 
        else
        {
            isUserProfileActive = true;
        }

        this.userProfile.SetActive(isUserProfileActive);
    }

    public void OnSignOutButtonClicked()
    {
        //disconnect from Photon
        PhotonFunctions.DisconnectFromPhoton();

        userProfileButton.SetActive(false);
        TitleText.text = "Welcome!";
        userProfile.SetActive(false);

        userJSON["IsLoggedIn"] = false;
        File.WriteAllText(path, userJSON.ToString());
    }

    public void OnSignInButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Sign_In");
    }

    public void OnDeleteAccountButtonClicked()
    {        
        //disconnect from Photon
        PhotonFunctions.DisconnectFromPhoton();

        File.Delete(path);
        userProfileButton.SetActive(false);
        TitleText.text = "Welcome!";
        userProfile.SetActive(false);
    }

    public void ShowAndHideCreateUserInfo()
    {
        if (isCreateUserInfoActive)
        {
            isCreateUserInfoActive = false;
        } 
        else
        {
            isCreateUserInfoActive = true;
        }
        createUserInfo.SetActive(isCreateUserInfoActive);
    }

    public void ShowAndHidePlayWithoutUserInfo()
    {
        if (isPlayWithoutUserInfoActive)
        {
            isPlayWithoutUserInfoActive = false;
        }
        else
        {
            isPlayWithoutUserInfoActive = true;
        }
        playWithoutUserInfo.SetActive(isPlayWithoutUserInfoActive);
    }

    public void OnCreateUserButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Sign_In");
    }

    public void OnPlayWithoutUserButtonClicked()
    {
        string randomUserName = "User" + Random.Range(0, 10000);

        if (File.Exists(path))
        {
            userJSON.Add("PlayWithoutUser", true);
            File.WriteAllText(path, userJSON.ToString());
        }

        PhotonFunctions.ConnectToPhoton(randomUserName);
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }

    public void OnBackButtonClicked()
    {
        Options.SetActive(false);
    }
}
