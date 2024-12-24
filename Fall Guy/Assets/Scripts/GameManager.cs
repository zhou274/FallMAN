using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using GamePolygon;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;


public class GameManager : MonoBehaviour
{
    int CurrentLevel;
    public static GameManager instance;
    public GameObject GameOverUI, Blast;
    public Text currentText, nextText;
    [HideInInspector]
    public bool GameEnd, GameEndWin;
    public GameObject ActiveBody, Deadbody, pole;
    public MMFeedbacks GameOverFeedBack;


    public GameObject[] Level;
    private StarkAdManager starkAdManager;
    public string clickid;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);

        }
        CurrentLevel = PlayerPrefs.GetInt("Level", 0);
        Instantiate(Level[CurrentLevel]);
    }

    private void Start()
    {
        GameEnd = false;
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlayMusic(SoundManager.Instance.Game);
        currentText.text = "" + (CurrentLevel);
        nextText.text = "" + (CurrentLevel + 1);


    }

    public void GameWin()
    {
        GameEndWin = true;
        StartCoroutine(ShowGameWin());

    }

    public void GameOver()
    {
        ShowInterstitialAd("3hai1a0e71d53t86r7",
           () => {
               Debug.LogError("--插屏广告完成--");
               var data = new JsonData
               {
                   ["event_type"] = "game_addiction",
                   ["extra"] = "{product_name: '插屏广告完成'}",
               };
               StarkSDK.API.StarkSendToTAQ(data);
           },
           (it, str) => {
               Debug.LogError("Error->" + str);
               StarkSDKSpace.AndroidUIManager.ShowToast("广告加载异常，请稍后再试");
           });
        FindObjectOfType<AdManager>().ShowAdmobInterstitial();

        GameOverFeedBack.PlayFeedbacks();
        ActiveBody.SetActive(false);
        Deadbody.SetActive(true);
        pole.SetActive(false);
        GameEnd = true;
        FindObjectOfType<AdManager>().ShowAdmobInterstitial();
        StartCoroutine(ShowGameOver());


    }

    IEnumerator ShowGameWin()
    {
        ShowInterstitialAd("3hai1a0e71d53t86r7",
           () => {
               Debug.LogError("--插屏广告完成--");
               var data = new JsonData
               {
                   ["event_type"] = "game_addiction",
                   ["extra"] = "{product_name: '插屏广告完成'}",
               };
               StarkSDK.API.StarkSendToTAQ(data);
           },
           (it, str) => {
               Debug.LogError("Error->" + str);
               StarkSDKSpace.AndroidUIManager.ShowToast("广告加载异常，请稍后再试");
           });
        FindObjectOfType<AdManager>().ShowAdmobInterstitial();

        Blast.SetActive(true);
        PlayerPrefs.SetInt("Level", CurrentLevel + 1);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("GameWin");


    }

    IEnumerator ShowGameOver()
    {
        SoundManager.Instance.StopMusic();

        SoundManager.Instance.PlaySound(SoundManager.Instance.GameOverFx);

        yield return new WaitForSeconds(2.0f);
        Debug.Log("GameOVer");
        GameOverUI.SetActive(true);

    }


    public void Reload()
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    Debug.Log("xxx");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void Menu()
    {

        SceneManager.LoadScene("Menu");
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


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
