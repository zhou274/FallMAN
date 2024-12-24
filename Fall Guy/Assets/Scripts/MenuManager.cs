using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GamePolygon;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;

public class MenuManager : MonoBehaviour {


	public Text CurrentLevelText;
    private StarkAdManager starkAdManager;

    int CurrentLevel;

    public string clickid;
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
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    
                    Debug.Log("观看完毕 获得奖励");
                    CoinManager.Instance.AddCoins(100);
                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        //CoinManager.Instance.AddCoins(100);
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
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }

    }
    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }



}
