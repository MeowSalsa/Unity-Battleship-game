using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBoard : Board
{
    public PlayerBoard(GameObject playerUnitPrefab)
    {
        PlayerboardUnitPrefab = playerUnitPrefab;
        ClearBoard();
    }

    public void CreatePlayerBoard()
    {
        int row = 1;
        int col = 1;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var temp = GameObject.Instantiate(PlayerboardUnitPrefab, new Vector3(i, 0, j), PlayerboardUnitPrefab.transform.rotation) as GameObject;
                var tempUI = temp.GetComponentInChildren<BoardUnit>();
                string name = string.Format("B1:[{0:00},{1:00}]", row, col);
                tempUI.BoardUnitText.text = name;
                tempUI.col = j;
                tempUI.row = i;
                temp.name = name;
                gameBoard[i, j] = temp;
                col++;
            }
            col = 1;
            row++;
        }
    }
}
