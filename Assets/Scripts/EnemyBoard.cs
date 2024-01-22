using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SharedDelegates;

public class EnemyBoard : Board
{
    GameObject cubePrefab;
    int[] aiShipSizes = new int[5] { 2, 3, 3, 4, 5 };
   	
	public EnemyBoard(GameObject enemyUnitPrefab, GameObject prefab)
    {
        EnemyboardUnitPrefab = enemyUnitPrefab;
        cubePrefab = prefab;
        ClearBoard();
    }
    /// <summary>
    /// Creates the AI board in the game.
    /// </summary>
    public void CreateAIBoard()
    {
        int row = 1;
        int col = 1;
        for (int i = 11; i < 21; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var tmp = GameObject.Instantiate(EnemyboardUnitPrefab, new Vector3(i, 0, j), EnemyboardUnitPrefab.transform.rotation) as GameObject;
                var tmpUI = tmp.GetComponentInChildren<BoardUnit>();
                string name = string.Format("B2:[{0:00},{1:00}]", row, col);
                tmpUI.BoardUnitText.text = name;
                tmpUI.col = col - 1;
                tmpUI.row = row - 1;
                tmp.name = name;
                gameBoard[tmpUI.row, tmpUI.col] = tmp;
                col++;
            }
            col = 1;
            row++;
        }
    }
    /// <summary>
    /// Gets a random location and attempts to place it in the game space.
    /// </summary>
    public void PlaceShips()
    { 	
        for (int i = 0; i < aiShipSizes.Length; i++)
        {
            var randomLocation = RandomLocation();
            Debug.Log("Ship with ID " + i + "placed at Row, Col" + randomLocation.Item1 + " , " + randomLocation.Item2 + "orient " + randomLocation.Item3);
            CheckBoardForPlacement(randomLocation, aiShipSizes[i]);
        }
    }
    /// <summary>
    /// Generates a random (X,Y) coordinate between 0-9. Chooses a horizontal or vertical position.
    /// </summary>
    /// <returns> Tuple consistiong of (int row,int column,bool horizontal)</returns>
    private ValueTuple<int, int, bool> RandomLocation()
    {
        int row = UnityEngine.Random.Range(0, 9);
        int col = UnityEngine.Random.Range(0, 9);
        bool horizontal = ChooseOrientation(row, col);
        (int row, int col, bool horizontal) randomLocation = (row: row, col: col, horizontal: horizontal);
        return randomLocation;
    }
    /// <summary>
    /// Chooses whether its vertical or not based off the seeds which are the row and column values.
    /// </summary>
    /// <param name="seed1"> Integer created when RandomLocation() initializes the row value.</param> 
    /// <param name="seed2"> Integer created when RandomLocation() initializes the column value. </param> 
    /// <returns>Boolean representing horizontal or not. Horizontal is true, vertical is false.</returns>
    private bool ChooseOrientation(int seed1, int seed2)
    {
        if (seed1 + seed2 / 2 >= 5)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Checks whether the ship can be placed in that location, otherwise chooses new random location and tries again.
    /// </summary>
    /// <param name="randomLocation"></param> Tuple consisting of random integer row, integer column, horizontal bool.
    /// <param name="shipSize"></param> Integer representing ship size.
    private void CheckBoardForPlacement((int row, int col, bool horizontal) randomLocation, int shipSize)
    {
        int row = randomLocation.row;
        int col = randomLocation.col;
        bool horizontal = randomLocation.horizontal;

        GameObject AIBoard = gameBoard[row, col];
        var AIBoardUnit = AIBoard.GetComponentInChildren<BoardUnit>();
        //Location occupied, or ship too big. Try again.
        if (AIBoardUnit.isOccupied || (row + shipSize > 9) || (col + shipSize > 9))
        {
            Debug.Log(string.Format("LOCATION OCCUPIED AT [{0},{1}]", row, col) + " Trying again!");
            var newRandomLocation = RandomLocation();
            CheckBoardForPlacement(newRandomLocation, shipSize);
            return;
        }
        else
        {
            bool okToPlace = true;
            if (!horizontal && (row + shipSize < 10))
            {
                for (int i = 0; i < shipSize; i++)
                {
                    GameObject checkingAIBoard = gameBoard[row + i, col];
                    BoardUnit checkingBoardUnit = checkingAIBoard.GetComponentInChildren<BoardUnit>();	void OnEnable(){
        Debug.Log("Enabled from Board");
    }
                    if (checkingBoardUnit.isOccupied)
                    {
                        okToPlace = false;
                    }
                }
            }
            if (horizontal && (col + shipSize < 10))
            {
                for (int i = 0; i < shipSize; i++)
                {
                    GameObject checkingAIBoard = gameBoard[row, col + i];
                    BoardUnit checkingBoardUnit = checkingAIBoard.GetComponentInChildren<BoardUnit>();
                    if (checkingBoardUnit.isOccupied)
                    {
                        okToPlace = false;
                    }
                }
            }
            if (okToPlace)
            {
                if (!horizontal)
                {
                    for (int i = 0; i < shipSize; i++)
                    {
                        // these game objects can be removed since they are only places for visual debugging
                        // GameObject visual = GameObject.Instantiate(cubePrefab,
                        //                                            new Vector3(row + i + 11, 0.9f, col),
						//cubePrefab.transform.rotation) as GameObject;
						// visual.GetComponent<Renderer>().material.color = Color.yellow;

                        GameObject aiBoard = gameBoard[row + i, col];
                        aiBoard.GetComponentInChildren<BoardUnit>().isOccupied = true;
                        gameBoard[row + i, col] = aiBoard;

                        //visual.gameObject.name = string.Format("EN-R-[{0},{1}]", row + i, col);

                        Debug.Log(string.Format("Enemy ship will be placed at location[{0}, {1}]", row + i, col));

                    }
                }

                if (horizontal)
                {
                    for (int i = 0; i < shipSize; i++)
                    {
                        // these game objects can be removed since they are only places for visual debugging
						// GameObject visual = GameObject.Instantiate(cubePrefab,
                        //                                           new Vector3(row + 11, 0.9f, col + i),
                        //                                           cubePrefab.transform.rotation) as GameObject;
						// visual.GetComponent<Renderer>().material.color = Color.magenta;

                        GameObject aiBoard = gameBoard[row, col + i];
                        aiBoard.GetComponentInChildren<BoardUnit>().isOccupied = true;
                        gameBoard[row, col + i] = aiBoard;

                        //visual.gameObject.name = string.Format("EN-C-[{0},{1}]", row, col + i);

                        Debug.Log(string.Format("Enemy ship will be placed at location[{0}, {1}]", row, col + i));

                    }
                }
            }
            //Can't place
            else
            {
                Debug.Log("AI COULDNT PLACE. TRYING AGAIN");
                var newRandomLocation = RandomLocation();
                CheckBoardForPlacement(newRandomLocation, shipSize);
            }
        }
    }
}
