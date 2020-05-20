using SimpleJSON;
using System;
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

    public bool ExistsFile()
    {
        return File.Exists(path);
    }

    public void AddNewProperty(string property, string value)
    {
        userJSON.Add(property, value);
    }

    public void AddNewBoolProperty(string property, bool value)
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
        if (!userJSON.IsNull)
        {
            return userJSON[property];
        }
        return "";
    }

    public int ReadIntFromFile(string property)
    {
        if (!userJSON.IsNull)
        {
            return userJSON[property];
        }
        return -1;
    }

    public bool ReadBoolFromFile(string property)
    {
        if (!userJSON.IsNull)
        {
            return userJSON[property];
        }
        return false;
    }

    public void DeleteEntry(string property)
    {
        if (File.Exists(path))
        {
            jsonString = File.ReadAllText(path);
            userJSON = (JSONObject)JSON.Parse(jsonString);

            if (userJSON.HasKey(property))
            {
                userJSON.Remove(property);
            }

            File.WriteAllText(path, userJSON.ToString());
        }
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

    public List<Tuple<string, string>> GetPositionsOfPieces(string color)
    {
        List<Tuple<string,string>> piecePositions = new List<Tuple<string, string>>();

        if (File.Exists(path))
        {
            jsonString = File.ReadAllText(path);
            userJSON = (JSONObject)JSON.Parse(jsonString);

            foreach(KeyValuePair<string, JSONNode> node in userJSON)
            {
                if (node.Key.StartsWith(color))
                {
                    piecePositions.Add(new Tuple<string, string>(node.Key, node.Value));
                }
            }

        }

        return piecePositions;
    }

    public void DeleteEntriesRelatedToLastGame()
    {
        DeleteEntriesThatStartWith(ARChessGameManager.colorOfLocalPlayer);
        DeleteEntriesThatStartWith(ARChessGameManager.colorOfOpponent);
        DeleteEntry("PlayWithoutUser");
        DeleteEntry("CurrentPlayer");
        DeleteEntry("OtherPlayer");
        DeleteEntry("GamePaused");
    }
}
