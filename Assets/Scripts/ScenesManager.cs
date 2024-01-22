using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

	private void Awake(){
        Instance = this;
        Debug.Log("I am awake!"+ "GameScene" + Scene.GameScene.ToString() );
    }

	//Keep these in order of scenes in Build Settings.
	public enum Scene{
		MainMenu,
		GameScene,
		EndScene
	}

	 public void loadScene(Scene scene){
        SceneManager.LoadScene(scene.ToString());
    }
	 public void LoadNewGame(){
        SceneManager.LoadScene(Scene.GameScene.ToString());
    }
	 public void LoadNextScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
	 public void LoadEndScene(){
        SceneManager.LoadScene(Scene.EndScene.ToString());
    }
	
}
