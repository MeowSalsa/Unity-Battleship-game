using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SharedDelegates;

public class AILogic : MonoBehaviour
{
    public static event AttackEventHandler OnAttack;
    static public bool AI_Turn = false;
	static List<ValueTuple<int,int>> previouslyHitTiles = new List<ValueTuple<int,int>>();
	List<ValueTuple<int,int>> allPreviouslyAttackedTiles = new List<ValueTuple<int,int>>();
	static public PlayerBoard playerboard;
	int maxShipSize = 5;
	int miss = 0;


	void Start(){
        previouslyHitTiles.Clear();
		AI_Turn = false;
		allPreviouslyAttackedTiles.Clear();
		miss = 0;
    }
	void OnEnable(){
        Debug.Log("Enemy board subscribed to OnSwitchPlayer");
        Battleship.OnSwitchPlayer += switchPlayer;
        
    }
	void OnDisable(){
        Battleship.OnSwitchPlayer -= switchPlayer;
        
    }
	void switchPlayer(PlayerType player){
        float minWait = 3.0f;
        float maxWait = 4.0f;
        if (player == PlayerType.AI){
			Debug.Log("AI SwitchPlayer");
            AI_Turn = true;
            float randomFloat =  UnityEngine.Random.Range(minWait, maxWait);
            Invoke("AIAttack", randomFloat);
        }
		else{
            AI_Turn = false;
        } 
    }

	void AIAttack(){
        Debug.Log("AI ATTACKING!!!!");
        // BoardUnit AttackedUnit = new BoardUnit();
        // OnAttackUnit.Invoke(AttackedUnit);
		if(previouslyHitTiles.Count == maxShipSize){
            previouslyHitTiles.Clear();
        }
        (int row, int col) attackingCoordinates = (-9,-9);
        if(previouslyHitTiles.Count==0){
				attackingCoordinates = AttackRandomLocation();
				allPreviouslyAttackedTiles.Add(attackingCoordinates);
			} else{
				attackingCoordinates = AttackCalculatedLocation();
				allPreviouslyAttackedTiles.Add(attackingCoordinates);
			}
		
		Debug.Log($"attacking {attackingCoordinates} ");
		AI_Turn = false;
		Debug.Log($"Attacking Random {attackingCoordinates} + {previouslyHitTiles.Count}");
        var help = playerboard.gameBoard[attackingCoordinates.row, attackingCoordinates.col].GetComponentInChildren<BoardUnit>();
        OnAttack?.Invoke(attackingCoordinates, PlayerType.AI);
		if(help.isOccupied){
            Debug.Log("twas occupado");
            previouslyHitTiles.Add(attackingCoordinates);
			miss=0;
		}else{
			miss++;
			if(miss == 4){
				previouslyHitTiles.Clear();
			}
            Debug.Log("TILE WASNT OCCUPIED!");
        }
	}
    private ValueTuple<int, int> AttackRandomLocation()
    {
		bool unAttacked = false;
		(int row, int col) location = (-9,-9);
		while(!unAttacked){
        int row = UnityEngine.Random.Range(0, 9);
        int col = UnityEngine.Random.Range(0, 9);
         location = (row: row, col: col);
		unAttacked = VerifyUnhitLocation(location);
		}
		return location;
    }
	bool VerifyUnhitLocation((int row, int col) location){
		if(allPreviouslyAttackedTiles.Contains(location)){
			Debug.Log("Already attacked, GO NEXT");
			return false;
		}else{
		return true;
		}
	}
    private ValueTuple<int,int> AttackCalculatedLocation()
    {
        (int row, int col) calculatedLocation = (-9,-9);
        // Implement a simple backtracking algorithm for AI attacks
        foreach (var previousAttack in previouslyHitTiles)
        {
            int row = previousAttack.Item1;
            int col = previousAttack.Item2;
            
            // Check the surrounding positions (up, down, left, right)
            for (int i = 0; i < 4; i++)
            {
                int newRow = row;
                int newCol = col;
				Debug.Log($"Previous hit tile: {row}, {col}");
				switch(i){ 
					case 0:
                        newRow--;
                        Debug.Log($"try right {newRow} {newCol}");
                        break;
					case 1:
                        newRow++;
                        Debug.Log($"try left {newRow} {newCol}");
                        break;
					case 2:
                        Debug.Log($"try up {newRow} {newCol}");
                        newCol--;
                        break;
					case 3:
                        Debug.Log($"try down {newRow} {newCol}");
                        newCol++;
                        break;
					default:
                        Debug.Log($"AIlogic.cs line 100. Error in AttackCalculatedLocation() i value:{i}");
                        newCol = -999;
                        newRow = -999;
                        break;
                }
                // Check if the new position is within the bounds of the board
                if (newRow >= 0 && newRow <= 9 && newCol >= 0 && newCol <= 9)
                {
                    calculatedLocation = (row: newRow, col: newCol);
                    var  anticipatedLocation= playerboard.gameBoard[newRow, newCol].GetComponentInChildren<BoardUnit>();
					if(!anticipatedLocation.isAttacked){
                        break;
                    }

                }
            }
        }
        Debug.Log($"Predicted location {calculatedLocation}");
        return calculatedLocation;
    }

}
