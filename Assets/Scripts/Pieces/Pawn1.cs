using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Realtime;

public class Pawn1 : MonoBehaviour
{
    private PhotonView myPhotonView;
    private string myTag;

    private Vector2Int myCoordinates;
    private GameObject myObject;

    public Material selectedMaterial;
    public Material defaultMaterial;

    private bool selected = false;

    public GameObject tileHighlightPrefab;
    private GameObject tileHighlight1;
    private GameObject tileHighlight2;


    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        myTag = gameObject.tag;
        myCoordinates = new Vector2Int();
        
    }

    void Update()
    {
        /*if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Ray screenRay = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(screenRay, out RaycastHit hit))
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        myObject = ARChessGameManager.FindGameObjectWithTag(hitObject.tag);
                        myCoordinates = ARChessGameManager.GetRowAndColumn(hitObject.tag);
                        bool available = ARChessGameManager.CheckIfPositionIsFree(myCoordinates.x + 1, myCoordinates.y);

                        if (available)
                        {
                            MovePiece();
                            myPhotonView.RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, myTag, myCoordinates.x, myCoordinates.y);
                        }

                       
                    }
                }
            }
        }*/
    }


    private void MovePiece()
    {
        //myObject.transform.position += (Vector3.forward * 0.6f);
        Vector3 initialPosition = myObject.transform.position;
        Vector3 targetPosition = myObject.transform.position + (Vector3.forward * 0.6f);

        //myObject.transform.position = Vector3.SmoothDamp(initialPosition, targetPosition, ref velocity, 0.001f);
        StartCoroutine(MoveObject(myObject, initialPosition, targetPosition));
        ARChessGameManager.pieces[myCoordinates.x + 1, myCoordinates.y] = myObject;
        ARChessGameManager.instance.SetPositionToNull(myCoordinates.x, myCoordinates.y);
        //Debug.Log(myTag + " my " +myCoordinates);
        //ARChessGameManager.PrintPieces();
    }

    [PunRPC]
    private void MovePieceForOpponent(string tag, int x, int y)
    {
        GameObject piece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        Vector2Int coordinates = ARChessGameManager.instance.GetRowAndColumn(tag);
        //Debug.Log(tag + " op " + coordinates);
        //piece.transform.position -= (Vector3.forward * 0.6f);
        Vector3 initialPosition = piece.transform.position;
        Vector3 targetPosition = piece.transform.position - (Vector3.forward * 0.6f);

        //piece.transform.position = Vector3.SmoothDamp(piece.transform.position, targetPosition,ref velocity, 0.3f);
        StartCoroutine(MoveObject(piece, initialPosition, targetPosition));
        ARChessGameManager.pieces[coordinates.x - 1, coordinates.y] = piece;
        ARChessGameManager.instance.SetPositionToNull(coordinates.x, coordinates.y);
        //ARChessGameManager.PrintPieces();
    }

    public IEnumerator MoveObject(GameObject piece, Vector3 initial, Vector3 final)
    {
        float totalMovementTime = 0.5f; //the amount of time you want the movement to take
        float currentMovementTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(piece.transform.position, final) > 0)
        {
            currentMovementTime += Time.deltaTime;
            piece.transform.position = Vector3.Lerp(initial, final, currentMovementTime / totalMovementTime);
            yield return null;
        }
    }

    //possible moves for the pawn: 1 square in front, or 2 if before middle of the board
    private void MoveOneSquare()
    {
        Vector3 initialPosition = myObject.transform.position;
        Vector3 targetPosition = myObject.transform.position + (Vector3.forward * 0.6f);
        StartCoroutine(MoveObject(myObject, initialPosition, targetPosition));
        ARChessGameManager.pieces[myCoordinates.x + 1, myCoordinates.y] = myObject;
        ARChessGameManager.instance.SetPositionToNull(myCoordinates.x, myCoordinates.y);
    }

    private void MoveTwoSquares()
    {
        Vector3 initialPosition = myObject.transform.position;
        Vector3 targetPosition = myObject.transform.position + 2 * (Vector3.forward * 0.6f);
        StartCoroutine(MoveObject(myObject, initialPosition, targetPosition));
        ARChessGameManager.pieces[myCoordinates.x + 2, myCoordinates.y] = myObject;
        ARChessGameManager.instance.SetPositionToNull(myCoordinates.x, myCoordinates.y);
    }

    private void ShowPossibleMoves()
    {
        if (myCoordinates.x < 4) //can move two squares
        {
            Vector3 firstSquarePosition = myObject.transform.position + (Vector3.forward * 0.6f) + new Vector3(0, 0.01f, 0);
            Vector3 secondSquarePosition = myObject.transform.position + 2 * (Vector3.forward * 0.6f) + new Vector3(0, 0.01f, 0);
            tileHighlight1 = Instantiate(tileHighlightPrefab, firstSquarePosition, Quaternion.identity);
            tileHighlight2 = Instantiate(tileHighlightPrefab, secondSquarePosition, Quaternion.identity);
            tileHighlight1.SetActive(true);
            tileHighlight2.SetActive(true);    
        }
        else //can move only one square
        {
            Vector3 firstSquarePosition = myObject.transform.position + (Vector3.forward * 0.6f) + new Vector3(0, 0.01f, 0);
            tileHighlight1 = Instantiate(tileHighlightPrefab, firstSquarePosition, Quaternion.identity);
            tileHighlight1.SetActive(true);
        }
        
    }

    private void Move()
    {
        //MoveSelector moveSelector = GetComponent<MoveSelector>();
        //moveSelector.EnterState(myObject);
    }

    private void HidePossibleMoves()
    {
        if (myCoordinates.x < 4)
        {
            tileHighlight1.SetActive(false);
            tileHighlight2.SetActive(false);
        }
        else
        {
            tileHighlight1.SetActive(false);
        }
        this.enabled = false;
    }

    void OnMouseDown()
    {
        //Debug.Log(myTag);
        if (myPhotonView.IsMine)
        {
            myObject = ARChessGameManager.instance.FindGameObjectWithTag(myTag);
            myCoordinates = ARChessGameManager.instance.GetRowAndColumn(myTag);
            if (!selected)
            {
                SelectPiece();
                ShowPossibleMoves();
                Move();
                //myPhotonView.RPC("SelectPieceForOpponent", RpcTarget.OthersBuffered, myTag);
            }
            else
            {
                DeselectPiece();
                HidePossibleMoves();
                //myPhotonView.RPC("DeselectPieceForOpponent", RpcTarget.OthersBuffered, myTag);
            }
            /*myObject = ARChessGameManager.FindGameObjectWithTag(myTag);
            //Debug.Log(myObject == null);
            myCoordinates = ARChessGameManager.GetRowAndColumn(myTag);
            bool available = ARChessGameManager.CheckIfPositionIsFree(myCoordinates.x + 1, myCoordinates.y);
            //Debug.Log(myCoordinates);
            //Debug.Log(myCoordinates + " " + myObject.tag + " " + available);


            if (available)
            {
                //MovePiece();
                SelectPiece();
                //myPhotonView.RPC("MovePieceForOpponent", RpcTarget.OthersBuffered, myTag, myCoordinates.x, myCoordinates.y);

            }*/
        }
        
    }

    public void SelectPiece()
    {
        this.enabled = true;
        MeshRenderer renderers = myObject.GetComponent<MeshRenderer>();
        renderers.material = selectedMaterial;
        selected = true;
    }
    public void DeselectPiece()
    {
        this.enabled = false;
        MeshRenderer renderers = myObject.GetComponent<MeshRenderer>();
        renderers.material = defaultMaterial;
        selected = false;
    }

    [PunRPC]
    private void SelectPieceForOpponent(string tag)
    {
        GameObject piece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        MeshRenderer renderers = piece.GetComponent<MeshRenderer>();
        renderers.material = selectedMaterial;
        selected = true;
    }

    [PunRPC]
    private void DeselectPieceForOpponent(string tag)
    {
        GameObject piece = ARChessGameManager.instance.FindGameObjectWithTag(tag);
        MeshRenderer renderers = piece.GetComponent<MeshRenderer>();
        renderers.material = defaultMaterial;
        selected = false;
    }
}
