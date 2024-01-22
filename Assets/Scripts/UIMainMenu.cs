using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
	[SerializeField] Button StartGame_Btn;
	[SerializeField] Button Exit_Btn;
	
    // Start is called before the first frame update
    void Start()
    {
        StartGame_Btn.onClick.AddListener(StartNewGame);
        Exit_Btn.onClick.AddListener(EndGame);
    }
    private void StartNewGame(){

		if(ScenesManager.Instance.enabled){
			Debug.Log("SceneManager Instance is enabled");
        }
        ScenesManager.Instance.LoadNewGame();
	}
	private void EndGame(){
#if UNITY_EDITOR
        Debug.Log("Quitting Editor");
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("Quitting Game");
        Application.Quit();
    }

}
