using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    private string path;
    private string jsonString;
    private JSONObject userJSON = new JSONObject();

    public static FileManager instance;

    private void Awake()
    {
        instance = this;
        path = Application.persistentDataPath + "/ARChessGameUserSave.json";
        if (File.Exists(path))
        {
            jsonString = File.ReadAllText(path);
            userJSON = (JSONObject)JSON.Parse(jsonString);
        }
    }

    public void AddNewProperty(string property, string value)
    {
        userJSON.Add(property, value);
    }

    public void ChangePropertyStringValue(string property, string value)
    {
        userJSON[property] = value;
    }

    public void ChangePropertyIntValue(string property, int value)
    {
        userJSON[property] = value;
        File.WriteAllText(path, userJSON.ToString());
    }

    public string ReadStringFromFile(string property)
    {
        return userJSON[property];
    }

    public int ReadIntFromFile(string property)
    {
        return userJSON[property];
    }

    public void DeleteEntriesThatStartWith(string startString)
    {
        string[] tags = { "King", "Queen", "Bishop1", "Bishop2", "Knight1", "Knight2", "Rook1", "Rook2", 
            "Pawn1", "Pawn2", "Pawn3", "Pawn4", "Pawn5", "Pawn6", "Pawn7", "Pawn8" };

        //possible tags
        string queen = "Queen2";

        if (File.Exists(path))
        {
            jsonString = File.ReadAllText(path);
            userJSON = (JSONObject)JSON.Parse(jsonString);

            foreach(string tag in tags)
            {
                if (userJSON.HasKey(startString + tag))
                {
                    userJSON.Remove(startString + tag);
                }
            }

            if (userJSON.HasKey(startString + queen))
            {
                userJSON.Remove(startString + queen);
            }

            userJSON.Remove("ColorOfLocalPlayer");
            userJSON.Remove("ColorOfOpponent");

            File.WriteAllText(path, userJSON.ToString());
        }
    }

    public void SaveGameState()
    {
        GameObject[,] allPieces = ARChessGameManager.pieces;
        if (File.Exists(path))
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (allPieces[i, j] != null)
                    {
                        userJSON[allPieces[i, j].tag] = i + "" + j;
                    }
                }
            }

            File.WriteAllText(path, userJSON.ToString());
        }

    }
}
