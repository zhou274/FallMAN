using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GamePolygon;
public class MenuManager : MonoBehaviour {


	public Text CurrentLevelText;
   

    int CurrentLevel;
	

	// Use this for initialization
	void Start () {
        SoundManager.Instance.StopMusic();

        SoundManager.Instance.PlayMusic(SoundManager.Instance.Menu);
        CurrentLevel = PlayerPrefs.GetInt("Level", 1);
        CurrentLevelText.text = "当前关卡 " + CurrentLevel;
        
	}


	public void LoadLevel(){

		SceneManager.LoadScene ("Game");
	}


    public void AddCoins()
    {
        CoinManager.Instance.AddCoins(100);
    }

    public void Restart()
    {

        SceneManager.LoadScene("Game");
    }

    public void Home()
    {
        SceneManager.LoadScene("Menu");

    }

    public void Shop()
    {
        SceneManager.LoadScene("Shop");

    }

   



}
