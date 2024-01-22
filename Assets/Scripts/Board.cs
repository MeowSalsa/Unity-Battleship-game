using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject PlayerboardUnitPrefab;
    public GameObject EnemyboardUnitPrefab;

    public GameObject[,] gameBoard = new GameObject[10, 10];

    public void ClearBoard()
    {
        for (int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                gameBoard[i, j] = null;
									
            }
        }
    }
}
