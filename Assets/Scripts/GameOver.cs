using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOver : MonoBehaviour
{
    [SerializeField]TMP_Text gameResult;

    void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
	void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		if(scene.name == "EndScene"){
            string result = PlayerPrefs.GetString("GameResult", "Default");
			if(result.Equals("DEFEAT")){
                gameResult.color = Color.red;
            }
            gameResult.text = result;
        }
    }
	public void RestartGame(){
        ScenesManager.Instance.LoadNewGame();
    }
	public void ExitGame(){
	#if UNITY_EDITOR
        Debug.Log("Quitting Editor");
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
