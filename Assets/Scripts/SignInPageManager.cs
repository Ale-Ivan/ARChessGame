using UnityEngine;
using UnityEngine.UI;

using System.Security.Cryptography;
using System.Text;
using System;

using SimpleJSON;
using System.IO;
using System.Threading;

public class SignInPageManager : MonoBehaviour
{
    [Header("Create user")]
    public InputField PlayerNameInputField;
    public InputField PasswordInputField;
    public InputField ConfirmPasswordInputField;

    public GameObject CreateCanvas;
    public GameObject CreateBox;
    public GameObject CreateButton;
    public GameObject ErrorBox_Create;
    public Text ErrorText_Create;

    [Header("SignIn")]
    public InputField UserNameInputField_SignIn;
    public InputField PasswordInputField_SignIn;

    public GameObject SignInCanvas;
    public GameObject SignInBox;
    public GameObject SignInButton;
    public GameObject ErrorBox_SignIn;
    public Text ErrorText_SignIn;

    private void Awake()
    {
        string path = Application.persistentDataPath + "/ARChessGameUserSave.json";
        if (File.Exists(path))
        {
            CreateCanvas.SetActive(false);
            SignInCanvas.SetActive(true);
        }
    }

    public void OnCreatePlayerButtonClicked()
    {
        if (string.IsNullOrEmpty(PlayerNameInputField.text.Trim()) || string.IsNullOrEmpty(PasswordInputField.text.Trim()) || string.IsNullOrEmpty(ConfirmPasswordInputField.text.Trim()))
        {
            CreateBox.SetActive(false);
            CreateButton.SetActive(false);
            ErrorBox_Create.SetActive(true);
            ErrorText_Create.text = "All fields must be filled out!";
            return;
        }

        var sha256 = new SHA256CryptoServiceProvider();

        //password to hash
        byte[] passwordBytes = Encoding.ASCII.GetBytes(PasswordInputField.text);
        var sha256Password = sha256.ComputeHash(passwordBytes);

        //confirm password to hash
        byte[] confirmPasswordBytes = Encoding.ASCII.GetBytes(ConfirmPasswordInputField.text);
        var sha256ConfirmPassword = sha256.ComputeHash(confirmPasswordBytes);

        if (!Convert.ToBase64String(sha256Password).Equals(Convert.ToBase64String(sha256ConfirmPassword)))
        {
            CreateBox.SetActive(false);
            CreateButton.SetActive(false);
            ErrorBox_Create.SetActive(true);
            ErrorText_Create.text = "Passwords do not match!";
            return;
        }

        JSONObject user = new JSONObject();
        user.Add("ID", CreateUniqueID());
        user.Add("Username", PlayerNameInputField.text);
        user.Add("Password", Convert.ToBase64String(sha256Password));
        user.Add("NumberOfWins", 0);
        user.Add("NumberOfLosses", 0);
        user.Add("IsLoggedIn", true);

        string path = Application.persistentDataPath + "/ARChessGameUserSave.json";
        File.WriteAllText(path, user.ToString());

        //Connect to Photon
        PhotonFunctions.ConnectToPhoton(PlayerNameInputField.text);

        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }

    private static int CreateUniqueID()
    {
        Thread.Sleep(1);
        System.Random randomNumberGenerator = new System.Random();
        return randomNumberGenerator.Next();
    }

    public void OnCloseErrorButtonClicked_Create()
    {
        ErrorBox_Create.SetActive(false);
        CreateBox.SetActive(true);
        CreateButton.SetActive(true);
    }

    public void OnSignInButtonClicked()
    {
        string username;
        string password;
        string path = Application.persistentDataPath + "/ARChessGameUserSave.json";
        string jsonString = File.ReadAllText(path);
        JSONObject userJSON = (JSONObject)JSON.Parse(jsonString);
        username = userJSON["Username"];
        password = userJSON["Password"];

        var sha256 = new SHA256CryptoServiceProvider();
        byte[] passwordBytes = Encoding.ASCII.GetBytes(this.PasswordInputField_SignIn.text);
        var sha256Password = sha256.ComputeHash(passwordBytes);

        if (!username.Equals(UserNameInputField_SignIn.text) || !Convert.ToBase64String(sha256Password).Equals(password))
        {
            SignInBox.SetActive(false);
            SignInButton.SetActive(false);
            ErrorBox_SignIn.SetActive(true);
            ErrorText_SignIn.text = "Wrong username or password!";
            return;
        }

        userJSON["IsLoggedIn"] = true;
        File.WriteAllText(path, userJSON.ToString());

        //Connect to Photon
        PhotonFunctions.ConnectToPhoton(UserNameInputField_SignIn.text);

        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }

    public void OnCloseErrorButtonClicked_SignIn()
    {
        ErrorBox_SignIn.SetActive(false);
        SignInBox.SetActive(true);
        SignInButton.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Start");
    }
}
