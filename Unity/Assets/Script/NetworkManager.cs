using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class StageInfo
{
	[Serializable]
	public struct SpotInfo
	{
		public int x;
		public int y;
	}

	[Serializable]
	public struct RectInfo
	{
		public int x;
		public int y;
	}

	public bool result;
	public int current_stage_count;
	public int total_stage_count;
	public float play_time;
	public SpotInfo[] spots;
	public RectInfo[] rects;
	public string left_img;
	public string right_img;
	public bool is_max_stage;
}

[Serializable]
public class TotalLevelInfo
{
	public bool result;
	public int total_stage_count;
	public string level;
}

public class LevelInfo
{
	public bool result;
	public string level;
	public int[] stage_count;
	public int size;
}

public class UserInfo
{
	public bool result;
	public string login_type;
	public int complete_level_1;
	public int complete_level_2;
	public int complete_level_3;
	public int complete_level_4;
	public int complete_level_5;
	public int hint_item_count;
	public int timer_item_count;
	public int watch_ad_count;
}

[Serializable]
public class UseItemInfo
{
	public bool result;
    //public bool is_use_item;
    public int hint_item_count;
	public int timer_item_count;
}

[Serializable]
public class ItemInfo{
	public bool result;
	public string os;
	public string[] items;
}

public enum LEVEL
{
	EASY = 1,
	NORMAL,
	HARD,
	HELL
}

public class NetworkManager : MonoBehaviour {
	public static NetworkManager Ins;

	[HideInInspector]
	public int selectedLevelTotalStageCount;
	[HideInInspector]
	public int[] currentLevelstageIndexList;

	[HideInInspector]
	public LEVEL selectedStageLevel;
	[HideInInspector]
	public int currentStageIndex = 0;

	[HideInInspector]
	public UserInfo userInfo;

    private string req_url = "http://t.05day.com";
    //private string req_url = "http://127.0.0.1:3000";

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
	{
		//PlayerPrefs.DeleteAll();
		Ins = this;
		DontDestroyOnLoad(gameObject);
	}
	
	// Use this for initialization
	void Start () {
		//GetStageInfoFromServer(1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetStageInfoFromServer(int currentStage)
	{
		StartCoroutine("Co_GetStageInfoFromServer", currentStage);
	}

	IEnumerator Co_GetStageInfoFromServer(int stageIndex)
	{
		WWW www = new WWW(req_url + "/stage-info/" + stageIndex);

        Debug.Log(req_url + "/stage-info/" + stageIndex);


        yield return www;

		Debug.Log(www.text);

		StageInfo info =  JsonUtility.FromJson<StageInfo>(www.text);

		if (info.result)
		{
			//마지막 스테이지
			if (info.is_max_stage)
			{

			}
			
			//게임 왼쪽 오른쪽 이미지 세팅
			if (info.left_img != null && info.right_img != null)
			{
				WWW left_imge_www = new WWW(info.left_img);

				yield return left_imge_www;

				WWW right_image_www = new WWW(info.right_img);

				yield return right_image_www;

				if (left_imge_www.texture != null && right_image_www.texture)
				{
					Texture2D[] images = new Texture2D[2];
					images[0] = (Texture2D)left_imge_www.texture;
					images[1] = (Texture2D)right_image_www.texture;

					GameManager.Ins.SetStageImage(images);
				}
			}

			if (info.spots.Length > 0 && info.rects.Length > 0)
			{
				GameManager.Ins.SetDiffRects(info.spots, info.rects);
			}

			GameManager.Ins.SetStageInfos(info.current_stage_count, info.total_stage_count);
			GameManager.Ins.SetDiffCountInfos();
			GameManager.Ins.currentStageTotalTime = info.play_time-1;
			GameManager.Ins.SetUserInfo();

			GameManager.Ins.StartStage();
		}
	}

	int levelStageStartIndex = 1;
	int levelStageEndIndex = 0;
	public void GetLevelStageList(LEVEL level)
	{
		StartCoroutine("Co_GetLevelStageList", level);
	}

	IEnumerator Co_GetLevelStageList(LEVEL level)
	{
		WWW www = new WWW(req_url +  "/stage-info/total-stage/" + (int)level);

		yield return www;

		Debug.Log(www.text);

		TotalLevelInfo totalInfo = JsonUtility.FromJson<TotalLevelInfo>(www.text);

		if (totalInfo.result)
		{
			selectedStageLevel = level;
			selectedLevelTotalStageCount = totalInfo.total_stage_count;
			
			string url;
			if (selectedLevelTotalStageCount < 10)
			{
				url = req_url + "/stage-info/" + (int)level + "/1/" + selectedLevelTotalStageCount;
			}
			else
			{
				levelStageEndIndex += 10;
				url = req_url + "stage-info/" + (int)level + "/" + levelStageStartIndex + "/" + levelStageEndIndex;
			}

			WWW www2 = new WWW(url);

            Debug.Log(url);


            yield return www2;

			Debug.Log(www2.text);

			LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(www2.text);

			if (levelInfo.result)
			{
				currentLevelstageIndexList = levelInfo.stage_count;

                var completeStage = 0;
                if (level == LEVEL.EASY)
                {
                    completeStage = userInfo.complete_level_1;
                }
                else if (level == LEVEL.NORMAL)
                {
                    completeStage = userInfo.complete_level_2;
                }
                else if (level == LEVEL.HARD)
                {
                    completeStage = userInfo.complete_level_3;
                }

                Debug.Log("completeStage: " + completeStage);
                var is_end_stage = true;
                for (var i = 0; i < currentLevelstageIndexList.Length; ++i)
                {
                    if (currentLevelstageIndexList[i] > completeStage)
                    {
                        currentStageIndex = i;
                        is_end_stage = false;
                        break;
                    }
                }

                if (is_end_stage)
                {
                    Debug.Log("마지막 스테이지");
                }

                         /*
				if (level == LEVEL.EASY)
				{
					currentStageIndex = PlayerPrefs.GetInt("CurrentEasyStageIndex", 0);
				}
				else if (level == LEVEL.NORMAL)
				{
					currentStageIndex = PlayerPrefs.GetInt("CurrentNormalStageIndex", 0);
				}
				else if (level == LEVEL.HARD)
				{
					currentStageIndex = PlayerPrefs.GetInt("CurrentHardStageIndex", 0);
				}
                	       */

                SceneManager.LoadScene("Main");
			}
		}
	}

	public void GetUserInfo(string LoginType = "guest")
	{
		StartCoroutine("Co_GetUserInfo", LoginType);
	}

	IEnumerator Co_GetUserInfo(string LoginType)
	{
		WWW www = new WWW(req_url + "/user-info/fetch/" + LoginType + "/" + PlayerPrefs.GetString("uid", SystemInfo.deviceUniqueIdentifier));

        Debug.Log(req_url);

		yield return www;
		
		Debug.Log(www.text);

		UserInfo userInfo = JsonUtility.FromJson<UserInfo>(www.text);

		if (userInfo.result)
		{
			this.userInfo = userInfo;

			TitleManager.Ins.OnFinishedGetUserInfo();
		}
	}

    // 스테이지 시작 했다고 로그성 알려주기
	public void SendStartStageInfo()
	{
		StartCoroutine("Co_SendStartStageInfo");
	}

	IEnumerator Co_SendStartStageInfo()
	{
		string url = string.Format(req_url + "/user-info/start-stage/{0}/{1}", PlayerPrefs.GetString("uid", SystemInfo.deviceUniqueIdentifier), currentLevelstageIndexList[GameManager.Ins.currentStageIndex].ToString());
		WWW www = new WWW(url);

		yield return www;
	}

	public void SendCompleteStageInfo()
	{
		StartCoroutine("Co_SendCompleteStageInfo");
	}

	IEnumerator Co_SendCompleteStageInfo()
	{
        var level = (int)selectedStageLevel;
        var stage_count = currentLevelstageIndexList[GameManager.Ins.currentStageIndex];
        Debug.Log("stage_count: " + stage_count);

        string url = string.Format(req_url + "/user-info/stage-complete/{0}/{1}/{2}", PlayerPrefs.GetString("uid", SystemInfo.deviceUniqueIdentifier), level.ToString(), stage_count);
		WWW www = new WWW(url);

		yield return www;
	}
	
	public void SendUseItem(string itemId)
	{
		StartCoroutine("Co_SendUseItem", itemId);
	}

	IEnumerator Co_SendUseItem(string itemId)
	{
		string url = string.Format(req_url + "/user-info/use-item/{0}/{1}", PlayerPrefs.GetString("uid", SystemInfo.deviceUniqueIdentifier), itemId.ToString());
		WWW www = new WWW(url);

		yield return www;

		Debug.Log(www.text);

		UseItemInfo useItemInfo = JsonUtility.FromJson<UseItemInfo>(www.text);

		if (useItemInfo.result)
		{
			userInfo.hint_item_count = useItemInfo.hint_item_count;
			userInfo.timer_item_count = useItemInfo.timer_item_count;

            Debug.Log("hint_item_count: " + userInfo.hint_item_count);
            Debug.Log("timer_item_count: " + userInfo.timer_item_count);


            GameManager.Ins.SetUserInfo();
			if (itemId == "hint_item")
			{
				GameManager.Ins.ShowAndHideHint();
			}
			else if (itemId == "timer_item")
			{
				GameManager.Ins.AddStageTimeByItem();
			}
		}
	}

    // 아이템 구매하기
    public void PurchaseItemRequest(string item_id)
    {
        StartCoroutine("Co_PurchaseItemRequest", item_id);
    }

    IEnumerator Co_PurchaseItemRequest(string item_id)
    {
        string url = string.Format(req_url + "/user-info/purchase-item/{0}/{1}", PlayerPrefs.GetString("uid", SystemInfo.deviceUniqueIdentifier), item_id);
        WWW www = new WWW(url);

        yield return www;

        Debug.Log(www.text);

        UseItemInfo itemInfo = JsonUtility.FromJson<UseItemInfo>(www.text);

        if (itemInfo.result)
        {
            var hint_item_count = itemInfo.hint_item_count;
            var timer_item_count = itemInfo.timer_item_count;

            Debug.Log("hint_item_count: "  + hint_item_count);
            Debug.Log("timer_item_count: " + timer_item_count);
        }
    }

    // os별 특성에 맞춰서 아이템 정보 가져오기
    public void GetItemInfoList()
	{
		StartCoroutine("Co_GetItemInfoList");
	}

	IEnumerator Co_GetItemInfoList()
	{
		string os = "android";
		#if UNITY_IOS
		os = "ios"
		#endif

		string url = string.Format(req_url + "/user-info/item-list/{0}", os);
		WWW www = new WWW(url);

		yield return www;

		Debug.Log(www.text);

		ItemInfo itemInfo = JsonUtility.FromJson<ItemInfo>(www.text);

		if (itemInfo.result)
		{
			ShopManager.Ins.UpdateItemsInfo(itemInfo.items);
		}
	}
}
