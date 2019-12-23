using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class King : Piece
{
    private GameObject chessBoard;

    private void Awake()
    {
        piecePhotonView = GetComponent<PhotonView>();
        chessBoard = GameObject.FindGameObjectWithTag("ChessBoard");
    }

    void Start()
    {
        selected = false;
    }

    public override PhotonView GetPhotonView()
    {
        return piecePhotonView;
    }

    public override List<Vector2Int> MoveLocations(Vector2Int piecePosition)
    {
        List<Vector2Int> locations = new List<Vector2Int>();

        if (piecePosition.x != 7)
        {
            Vector2Int forwardOne = new Vector2Int(piecePosition.x + 1, piecePosition.y);
            locations.Add(forwardOne);

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightUp = new Vector2Int(piecePosition.x + 1, piecePosition.y + 1);
                locations.Add(diagonalRightUp);
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftUp = new Vector2Int(piecePosition.x + 1, piecePosition.y - 1);
                locations.Add(diagonalLeftUp);
            }
        }

        if (piecePosition.x != 0)
        {
            Vector2Int backwardsOne = new Vector2Int(piecePosition.x - 1, piecePosition.y);
            locations.Add(backwardsOne);

            if (piecePosition.y != 7)
            {
                Vector2Int diagonalRightDown = new Vector2Int(piecePosition.x - 1, piecePosition.y + 1);
                locations.Add(diagonalRightDown);
            }

            if (piecePosition.y != 0)
            {
                Vector2Int diagonalLeftDown = new Vector2Int(piecePosition.x - 1, piecePosition.y - 1);
                locations.Add(diagonalLeftDown);
            }
        }

        if (piecePosition.y != 0)
        {
            Vector2Int leftOne = new Vector2Int(piecePosition.x, piecePosition.y - 1);
            locations.Add(leftOne);
        }

        if (piecePosition.y != 7)
        {
            Vector2Int rightOne = new Vector2Int(piecePosition.x, piecePosition.y + 1);
            locations.Add(rightOne);
        }

        return locations;
    }

    
}
