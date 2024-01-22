using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AILogic;

public enum PlayerType{
	HUMAN,
	AI,
	NONE
}
public class Battleship : MonoBehaviour
{

    public delegate void SwitchPlayer(PlayerType player);
    public static event SwitchPlayer OnSwitchPlayer;

	public delegate void PlayerScored(PlayerType scoringPlayer, int score);
    public static event PlayerScored OnPlayerScored;

    int playerScore  = 0;
    int AIScore = 0;
    public static bool gameStarted = false;
    private int pointsToWin = 17;
    public static PlayerType currentPlayer = PlayerType.NONE;
	public static PlayerBoard playerBoard;
	public static EnemyBoard  enemyBoard;

    [SerializeField]BoardManager BoardManager;
    [SerializeField] GameObject explosionPrefab;

	void Start(){
        currentPlayer = PlayerType.NONE;
		gameStarted = false;
        playerScore = 0;
        AIScore = 0;
    }
	
    void OnEnable(){
        BoardManager.OnStartGame += StartGame;
        UIBoardManager.OnFirstPlayerSelected += switchPlayer;
        BoardManager.OnAttack += HandleAttack;
        OnAttack += HandleAttack;

	}
	
	void OnDisable(){
        BoardManager.OnStartGame -= StartGame;
        UIBoardManager.OnFirstPlayerSelected -= switchPlayer;
		BoardManager.OnAttack -= HandleAttack;
        OnAttack -= HandleAttack;
    }

	void StartGame(bool startGame){
		if(startGame){
            playerScore = 0;
            AIScore = 0;
            gameStarted = true;
		}
	}

	void HandleAttack((int row, int col) coordinate, PlayerType currentPlayer){
		bool didHit = false;
        Vector3 explosionOffset = new Vector3(0f,0.5f,0f);
		
		switch(currentPlayer){
			case PlayerType.AI:
				Debug.Log("Handling an attack on coordinate");
				var playerBoardUnit = playerBoard.gameBoard[coordinate.row, coordinate.col];
				var playerUnit = playerBoardUnit.GetComponentInChildren<BoardUnit>();
				Instantiate(explosionPrefab,playerUnit.transform.position+explosionOffset, Quaternion.Euler(-90f,0f,0f) );
                playerUnit.GotAttacked();
                didHit = verifyAttack(playerUnit);
				if(didHit){
					AIScore++;
                    OnPlayerScored?.Invoke(currentPlayer, AIScore);
                    checkIfWinner();
                    Debug.Log("AI SCORE " + AIScore);
                    continueTurn(currentPlayer);
				} else{
                    Debug.Log("AI Missed");
                    switchPlayer(currentPlayer);
				}
                break;
			case PlayerType.HUMAN:
                Debug.Log("Should handle human");
				var enemyBoardUnit = enemyBoard.gameBoard[coordinate.row, coordinate.col];
				var enemyUnit =enemyBoardUnit.GetComponentInChildren<BoardUnit>();
                Instantiate(explosionPrefab, enemyUnit.transform.position + explosionOffset, Quaternion.Euler(-90f, 0f, 0f));
                enemyUnit.GotAttacked();
                didHit = verifyAttack(enemyUnit);
				if(didHit){	
					playerScore++;
					OnPlayerScored?.Invoke(currentPlayer, playerScore);
                   
					Debug.Log("Player Score" + playerScore);
					checkIfWinner();
					continueTurn(currentPlayer);
				} else{
					switchPlayer(currentPlayer);

				}
				
				break;
			case PlayerType.NONE:
			default:
                Debug.Log("Error in HandleAttack");
                break;
        }
	}
	bool verifyAttack(BoardUnit attackedUnit){
		if(attackedUnit.isOccupied){
			return true;
		}
		return false;
	}
	void switchPlayer(PlayerType Player){
		if(currentPlayer == PlayerType.NONE){
            Debug.Log($"FIRST PLAYER: {Player}");
            currentPlayer = Player;
            OnSwitchPlayer?.Invoke(Player);
        }
        else
        {
            switch (Player)
            {
                case PlayerType.HUMAN:
                    currentPlayer = PlayerType.AI;

                    OnSwitchPlayer?.Invoke(currentPlayer);
                    break;
                case PlayerType.AI:
                    currentPlayer = PlayerType.HUMAN;
                    // BoardManager.playerTurn = true;
                    Debug.Log($"BS PT: {BoardManager.playerTurn}");
                    OnSwitchPlayer?.Invoke(currentPlayer);
                    break;
                default:
                    Debug.Log("Error in switchPlayer switch");
                    break;
            }
        }
    }
	void continueTurn(PlayerType playerType){
        Debug.Log("Current Player is " + playerType);
        OnSwitchPlayer?.Invoke(playerType);
    }

	void checkIfWinner(){
		if(AIScore == pointsToWin){
            //TODO
            //END MENU WITH AI WIN
            PlayerPrefs.SetString("GameResult", "DEFEAT");
            ScenesManager.Instance.LoadEndScene();
        }
		if(playerScore == pointsToWin){
            Debug.Log("PLAYER WINNER!");
            PlayerPrefs.SetString("GameResult", "VICTORY!");
			ScenesManager.Instance.LoadEndScene();

            //TODO
            //END MENU WITH HUMAN WIN
        }
	}
}
