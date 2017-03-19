using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class NetworkManager : MonoBehaviour {
	public static NetworkManager Ins;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		Ins = this;
	}
	
	// Use this for initialization
	void Start () {
		GetStageInfoFromServer(1);
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
		WWW www = new WWW("http://t.05day.com/stage-info/" + stageIndex);

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

			GameManager.Ins.StartStage();
		}
	}
}
