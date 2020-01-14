using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    /// <summary>
    /// The Size of the Tile
    /// </summary>
    private const float TILE_SIZE = 1.0f;

    /// <summary>
    /// The offset of the tile
    /// </summary>
    private const float TILE_OFFSET = 0.5f;

    /// <summary>
    /// Determines the selected Point on the X axis on the board
    /// </summary>
    private int SelectionX  = -1;

    /// <summary>
    /// Determines the selected point on the Y axis on the board
    /// </summary>
    private int SelectionY  = -1;

    /// <summary>
    /// Holds a list of the Chess men.
    /// </summary>
    public List<GameObject> ChessmanPrefabs;

    /// <summary>
    /// Holds the array of Chess Men in the Board
    /// </summary>
    public ChessMan[,] ChessMen { get; set; }
    /// <summary>
    /// Holds the Selected ChessMan
    /// </summary>
    public ChessMan SelectedChessMan { get; set; }
    /// <summary>
    /// Holds the orientation of the Chess Man
    /// </summary>
    private Quaternion Orientation = Quaternion.Euler(0, 180, 0);

    /// <summary>
    /// True if it the turn of the white party to play
    /// </summary>
    private bool isWhiteTurn = true;





    /// <summary>
    /// Runs on the first
    /// </summary>
    private void Start()
    {
        SpawnAllChessMen();
    }


    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        DrawChessBoard();
        UpdateSelection();

        if(Input.GetMouseButtonDown(0))
        {
            if(SelectionX >= 0 && SelectionY >= 0)
            {
                if(SelectedChessMan == null)
                {
                    SelectChessMan(SelectionX, SelectionY);
                }
                else
                {
                    MoveChessMan(SelectionX, SelectionY);
                }
            }
        }
    }


    /// <summary>
    /// Holds the chess men that are active
    /// </summary>
    private List<GameObject> ActiveChessMan = new List<GameObject>();


    /// <summary>
    /// Draws the Chess Board in the world using Debug.Drawline
    /// </summary>
    private void DrawChessBoard()
    {
        Vector3 WidthLine = Vector3.right * 8;
        Vector3 HeightLine = Vector3.forward * 8;
        Vector3 start = new Vector3();
        for (int i = 0; i<= 8; i++)
        {
            start = Vector3.forward * i;
            Debug.DrawLine(start, start + WidthLine);

            for(int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + HeightLine);
            }
        }

        Debug.DrawLine(Vector3.forward * SelectionY + Vector3.right * SelectionX, Vector3.forward * (SelectionY + 1) + Vector3.right * (SelectionX + 1));
    }

    /// <summary>
    /// Updates the Selection on the chess board Using Raycast from the camera
    /// </summary>
    private void UpdateSelection()
    {
        if (!Camera.main) return;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            SelectionX = (int)hit.point.x;
            SelectionY = (int)hit.point.z;
        }
        else
        {
            SelectionX = -1;
            SelectionY = -1;
        }
    }

    /// <summary>
    /// Spawns the Chess men at respective Positions using the index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Position"></param>
    private void SpawnChessman(int index, int x, int y)
    {
        GameObject go = Instantiate(ChessmanPrefabs[index], GetTileCenter(x, y), Orientation) as GameObject;
        go.transform.SetParent(transform);
        ChessMen[x, y] = go.GetComponent<ChessMan>();
        ChessMen[x, y].UpdatePosition(x, y);
        ActiveChessMan.Add(go);
    }

    /// <summary>
    /// Returns the center of a tile for a Chess Man to be placed on
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    /// <summary>
    /// Spawns all the chess men on the board
    /// </summary>
    private void SpawnAllChessMen()
    {
        //Clear the list of Active Chess men
        ActiveChessMan.Clear();

        //Create a new Array to hold the List of Chess Men
        ChessMen = new ChessMan[8, 8];

        //Spawn the White Men

        //King
        SpawnChessman(0, 3, 0);

        //Queen
        SpawnChessman(1, 4, 0);

        //Rooks
        SpawnChessman(2, 0, 0);
        SpawnChessman(2, 7, 0);

        //Bishops
        SpawnChessman(3, 2, 0);
        SpawnChessman(3, 5, 0);

        //Knights
        SpawnChessman(4, 1, 0);
        SpawnChessman(4, 6, 0);

        //Pawns
        for(int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1);
        }


        //Spawn the Black Men

        //King
        SpawnChessman(4, 3, 7);

        //Queen
        SpawnChessman(5, 4, 7);

        //Rooks
        SpawnChessman(6, 0, 7);
        SpawnChessman(6, 7, 7);

        //Bishops
        SpawnChessman(9, 2, 7);
        SpawnChessman(9, 5, 7);

        //Knights
        SpawnChessman(10, 1, 7);
        SpawnChessman(10, 6, 7);

        //Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6);
        }
    }

    /// <summary>
    /// Selects a Chess Man at a location
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void SelectChessMan(int x, int y)
    {
        //Check if a chessman exists at that position
        if (ChessMen[x, y] == null) return;

        //Check if the right party is making the move
        if (ChessMen[x, y].isWhite != isWhiteTurn) return;

        //Select the Chess Man at that position
        SelectedChessMan = ChessMen[x, y];
    }

    /// <summary>
    /// Moves a chess man to a selected location
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void MoveChessMan(int x, int y)
    {
        if (SelectedChessMan.PossibleMove(x, y))
        {
            //Set the Chess Man at the index of the Selected Chess Man to null because he will be moved out of that position
            ChessMen[SelectedChessMan.CurrentX, SelectedChessMan.CurrentY] = null;
            //Change the position of the Selected Chess Man to the position of the clicked tile
            SelectedChessMan.transform.position = GetTileCenter(x, y);
            //Set the Chess Man at the index of Chess Men to that of the Selected ChessMan
            ChessMen[x, y] = SelectedChessMan;
            //Update the Position Of the Selected ChessMan to the current position it now occupies
            ChessMen[x,y].UpdatePosition(x, y);
        }

        SelectedChessMan = null;
    }
}
