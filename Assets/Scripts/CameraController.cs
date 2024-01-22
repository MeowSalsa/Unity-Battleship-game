
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera playerBoardCam;
    [SerializeField] CinemachineVirtualCamera enemyBoardCam;

    PlayerType currentPlayer;
    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = PlayerType.NONE;
    }
    
    void OnEnable()
    {
        Battleship.OnSwitchPlayer += OnSwitchPlayer;
    }
    void OnDisable()
    {
        Battleship.OnSwitchPlayer -= OnSwitchPlayer;
    }
    void OnSwitchPlayer (PlayerType player)
    {
        //Delay to allow effects to catch up before cam switch.
         StartCoroutine(DelaySetPlayer(player));
		 //currentPlayer = player;
    }
	IEnumerator DelaySetPlayer(PlayerType player){
        Debug.Log("WAITING!");
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Setting player {player}");
        currentPlayer = player;
        Debug.Log($"DELAYED: {currentPlayer}");
    }
    // Update is called once per frame
    void Update()
    {Debug.Log($"CurrentPlayer {currentPlayer}");
        switch (currentPlayer)
        {
            case PlayerType.AI:
                Debug.Log("CAM SWITCH TO PLAYER BOARD");
                enemyBoardCam.Priority = 0;
                playerBoardCam.Priority = 10;
                break;
            case PlayerType.HUMAN:
                Debug.Log("CAM SWITCH TO ENEM BOARD");
                playerBoardCam.Priority = 0;
                enemyBoardCam.Priority = 10;
                break;
            case PlayerType.NONE:
                enemyBoardCam.Priority = 0;
                playerBoardCam.Priority = 10;
                break;
        }
    }
}
