﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public Image CurtainLeft;
	public Image CurtainRight;

	public Image LeftImage;
	public Image RightImage;

	public Image TimeProgressBar;
	
	public Text currentStageCount;
	public Text currentFindDiffCount;
	
	private float stagePlayTime = -1f;

	public GameObject pauseMenuPanel;
	private bool isShownPauseMenu = false;

	public GameObject pauseButton;
	public GameObject startButton;

	public GameObject answerButtonPrefab;
	public List<AnswerButton> leftAnswerButtonList = new List<AnswerButton>();
	public List<AnswerButton> rightAnswerButtonList = new List<AnswerButton>();

	public Transform canvasTrans;

	private float leftImageOffset = 958f;

	private int answerFindCount = 0;

	public float currentStageTotalTime = 0f;

	public Image readyImg;
	public Image goImg;
	public Image gameoverImg;

	public Image youWinImg;

	public Image inCorrectAnswerImage;

	private bool isStageOver = false;

	public Image pauseButtonImage;

	public float incorrectTimePenalty = 5f;
	public float correctTimePlus = 5f;
	
	public int currentStageIndex = 0;

	public Text HintItemCount;
	public Text AddTimeItemCount;

	public int selectedLevelTotalStageCount = 0;
	public int[] selectedLevelStageList;

	bool isShowingHint = false;

	public GameObject userInputPanel;

	bool blockUserInput
	{
		set
		{
			userInputPanel.GetComponent<Image>().raycastTarget = value;
		}
	}

	public static GameManager Ins;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		Ins = this;
	}

	// Use this for initialization
	void Start () {
		blockUserInput = false;
		Debug.Log(NetworkManager.Ins.currentStageIndex);
		currentStageIndex = NetworkManager.Ins.currentStageIndex;
		NetworkManager.Ins.GetStageInfoFromServer(NetworkManager.Ins.currentLevelstageIndexList[currentStageIndex]);
	}

	public void StartStage()
	{
		StartCoroutine("Co_StartStage");

		NetworkManager.Ins.SendStartStageInfo();
	}

	IEnumerator Co_StartStage()
	{
		readyImg.gameObject.SetActive(true);

		yield return new WaitForSeconds(1.5f);
		
		readyImg.gameObject.SetActive(false);
		goImg.gameObject.SetActive(true);

		CurtainLeft.CrossFadeAlpha(0f, 1f, false);
		CurtainRight.CrossFadeAlpha(0f, 1f, false);

		yield return new WaitForSeconds(1f);

		goImg.gameObject.SetActive(false);

		blockUserInput = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (GetCurrentStageTotalTime() > 0 && !isStageOver && !pauseMenuPanel.gameObject.activeSelf)
		{
        	stagePlayTime += Time.deltaTime;

        	TimeProgressBar.fillAmount = (GetCurrentStageTotalTime() - stagePlayTime) / GetCurrentStageTotalTime();
		}

		if (stagePlayTime >= GetCurrentStageTotalTime() && !isStageOver)
		{
			blockUserInput = false;
			isStageOver = true;
			gameoverImg.gameObject.SetActive(true);

			Invoke("ShowPauseMenu", 1.5f);
		}

		//if (Application.platform == RuntimePlatform.Android)
        //{
            if (Input.GetKey(KeyCode.Escape))
            {
                GameObject prefab = Resources.Load("Popup") as GameObject;
                Popup popup = Instantiate(prefab).GetComponent<Popup>();
                popup.transform.SetParent(canvasTrans);
                popup.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
				popup.transform.localScale = Vector3.one;
                popup.Show(POPUPTYPE.MAINMENU);
            }
        //}
    }

    private float GetCurrentStageTotalTime()
    {
        return currentStageTotalTime;
    }

    public void ShowPauseMenu()
	{
		if (isStageOver)
		{
			pauseMenuPanel.transform.Find("Resume").gameObject.SetActive(false);
		}

		if (!isShownPauseMenu)
		{
			isShownPauseMenu = true;
			pauseButton.SetActive(false);
			startButton.SetActive(true);
		}
		else
		{
			isShownPauseMenu = false;
			pauseButton.SetActive(true);
			startButton.SetActive(false);
		}
		pauseMenuPanel.SetActive(isShownPauseMenu);
	}

	public void ChangePauseMenuButton()
	{
		if (!isShownPauseMenu)
		{
			
		}	
		else
		{
			
		}
	}

	public void SetStageImage(Texture2D[] tex)
	{
		//www를 통해서 받은 Texture2D는 UI Sprite에 세팅하기 위해서는 아래와 같이 Sprite를 Crate해야한다.
		Rect rect = new Rect(0, 0, tex[0].width, tex[0].height);
		LeftImage.sprite = Sprite.Create(tex[0], rect, new Vector2(0.5f, 0.5f));
		
		rect = new Rect(0, 0, tex[1].width, tex[1].height);
		RightImage.sprite = Sprite.Create(tex[1], rect, new Vector2(0.5f, 0.5f));
	}

	int buttonListIndex = 0;
	public void SetDiffRects(StageInfo.SpotInfo[] spots, StageInfo.RectInfo[] rects)
	{
		for(int i=0; i<spots.Length; i++)
		{
			AnswerButton btnObj;
			if (leftAnswerButtonList.Count == 0 || i > leftAnswerButtonList.Count-1  || leftAnswerButtonList[i] == null)
			{
				btnObj = GameObject.Instantiate(answerButtonPrefab).GetComponent<AnswerButton>();
				btnObj.transform.SetParent(canvasTrans);
				btnObj.transform.localScale = Vector3.one;
				btnObj.GetComponent<Button>().onClick.AddListener(() => btnObj.ButtonClicked());
				btnObj.index = i;
				leftAnswerButtonList.Add(btnObj);
			}
			else
			{
				btnObj = leftAnswerButtonList[i];
			}
			btnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(rects[i].x, rects[i].y);
			btnObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(spots[i].x, spots[i].y, 0f);
		}

		leftAnswerButtonList.RemoveRange(spots.Length, leftAnswerButtonList.Count-spots.Length);

		for(int i=0; i<spots.Length; i++)
		{
			AnswerButton btnObj;
			if (rightAnswerButtonList.Count == 0 || i > rightAnswerButtonList.Count-1 || rightAnswerButtonList[i] == null)
			{
				btnObj = GameObject.Instantiate(answerButtonPrefab).GetComponent<AnswerButton>();
				btnObj.transform.SetParent(canvasTrans);
				btnObj.transform.localScale = Vector3.one;
				btnObj.GetComponent<Button>().onClick.AddListener(() => btnObj.ButtonClicked());
				btnObj.index = i;
				rightAnswerButtonList.Add(btnObj);
			}
			else
			{
				btnObj = rightAnswerButtonList[i];
			}
			btnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(rects[i].x, rects[i].y);
			btnObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(spots[i].x + leftImageOffset, spots[i].y, 0f);
		}
		rightAnswerButtonList.RemoveRange(spots.Length, rightAnswerButtonList.Count-spots.Length);

		EnableAnswerButtons();
	}

	public void SetDiffCountInfos()
	{
		currentFindDiffCount.text = string.Format("{0}/{1}", answerFindCount.ToString(), leftAnswerButtonList.Count.ToString());
	}

	public void SetStageInfos(int curCount, int maxCount)
	{
        var selected_max_stage = NetworkManager.Ins.selectedLevelTotalStageCount - 1;
        currentStageCount.text = string.Format("{0}/{1}", (currentStageIndex+1).ToString(), selected_max_stage.ToString());
	}

	public void SetUserInfo()
	{
		HintItemCount.text = string.Format("x{0}", NetworkManager.Ins.userInfo.hint_item_count.ToString());
		AddTimeItemCount.text =string.Format("x{0}", NetworkManager.Ins.userInfo.timer_item_count.ToString());
	}

	public void AnswerButtonClicked(int index)
	{
		if (!leftAnswerButtonList[index].circleAnimObject.activeSelf)
		{
			leftAnswerButtonList[index].circleAnimObject.SetActive(true);
		}
		if (!rightAnswerButtonList[index].circleAnimObject.activeSelf)
		{
			rightAnswerButtonList[index].circleAnimObject.SetActive(true);
		}

		leftAnswerButtonList[index].GetComponent<Image>().raycastTarget = false;
		rightAnswerButtonList[index].GetComponent<Image>().raycastTarget = false;

		answerFindCount++;

		stagePlayTime -= correctTimePlus;
		if (stagePlayTime < 0)
		{
			stagePlayTime = 0;
		}

		SetDiffCountInfos();

		Invoke("CheckMoveToNextStage", 2f);
	}

	public void InCorrectPoinClicked()
	{
		//잘못된 곳을 클릭했을 때 남은 시간을 감소시킨다
		stagePlayTime += incorrectTimePenalty;
		Debug.Log(Input.mousePosition);
		Debug.Log(Camera.main.ViewportToScreenPoint(Input.mousePosition));
		inCorrectAnswerImage.rectTransform.anchoredPosition = Input.mousePosition;
		inCorrectAnswerImage.gameObject.SetActive(true);
		
		Camera.main.GetComponent<CameraShake>().shakeDuration = 0.5f;
		Invoke("HideInCorrectPointImage", 0.5f);
	}

	void HideInCorrectPointImage()
	{
		inCorrectAnswerImage.gameObject.SetActive(false);
	}

	void CheckMoveToNextStage()
	{
		if (answerFindCount >= leftAnswerButtonList.Count)
		{
			blockUserInput = false;
			youWinImg.gameObject.SetActive(true);
			foreach(AnswerButton btn in leftAnswerButtonList)
			{
				btn.gameObject.SetActive(false);
			}
			foreach(AnswerButton btn in rightAnswerButtonList)
			{
				btn.gameObject.SetActive(false);
			}

			CurtainLeft.CrossFadeAlpha(1f, 1f, false);
			CurtainRight.CrossFadeAlpha(1f, 1f, false);

			isStageOver = true;

			Invoke("HideYouWinImage", 1f);
		}
	}

	void HideYouWinImage()
	{
		youWinImg.gameObject.SetActive(false);

		ClearAndMoveToNextStage();
	}

	void ClearAndMoveToNextStage()
	{
		DisableAnswerButtons();
		currentStageTotalTime = 0;
		stagePlayTime = -1f;
		isStageOver = false;
		TimeProgressBar.fillAmount = 1f;
		currentFindDiffCount.text = "0";
		answerFindCount = 0;

		NetworkManager.Ins.SendCompleteStageInfo();

		if (currentStageIndex < NetworkManager.Ins.selectedLevelTotalStageCount)
		{
			currentStageIndex = currentStageIndex + 1;

			if (NetworkManager.Ins.currentLevelstageIndexList.Length <= currentStageIndex)
			{
				Debug.Log("마지막 스테이지");
				return;
			}

			NetworkManager.Ins.GetStageInfoFromServer(NetworkManager.Ins.currentLevelstageIndexList[currentStageIndex]);

			if (NetworkManager.Ins.selectedStageLevel == LEVEL.EASY)
			{
				PlayerPrefs.SetInt("CurrentEasyStageIndex", currentStageIndex);
			}	
			else if (NetworkManager.Ins.selectedStageLevel == LEVEL.NORMAL)
			{
				PlayerPrefs.SetInt("CurrentNormalStageIndex", currentStageIndex);
			}
			else if (NetworkManager.Ins.selectedStageLevel == LEVEL.HARD)
			{
				PlayerPrefs.SetInt("CurrentHardStageIndex", currentStageIndex);
			}
		}
		else
		{
			GameObject prefab = Resources.Load("Popup") as GameObject;
			Popup popup = Instantiate(prefab).GetComponent<Popup>();
			popup.transform.SetParent(canvasTrans);
			popup.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			popup.transform.localScale = Vector3.one;
			popup.Show(POPUPTYPE.ENDLEVEL);
		}
	}

	public void ResumeGame()
	{
		pauseMenuPanel.gameObject.SetActive(false);
 
		isShownPauseMenu = false;
	}

	public void RestartGame()
	{
		DisableAnswerButtons();
		pauseMenuPanel.gameObject.SetActive(false);
		isShownPauseMenu = false;
		isShownPauseMenu = false;
		currentStageTotalTime = 0;
		stagePlayTime = -1f;
		isStageOver = false;
		TimeProgressBar.fillAmount = 1f;
		currentFindDiffCount.text = "0";
		answerFindCount = 0;

		gameoverImg.gameObject.SetActive(false);

		CurtainLeft.CrossFadeAlpha(1f, 1f, false);
		CurtainRight.CrossFadeAlpha(1f, 1f, false);

		pauseButton.SetActive(true);
		startButton.SetActive(false);

		NetworkManager.Ins.GetStageInfoFromServer(NetworkManager.Ins.currentLevelstageIndexList[currentStageIndex]);
	}

	public void ExitGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Title");
	}

	public void DisableAnswerButtons()
	{
		for(int i=0; i<leftAnswerButtonList.Count; i++)
		{
			leftAnswerButtonList[i].circleAnimObject.SetActive(false);
			rightAnswerButtonList[i].circleAnimObject.SetActive(false);
			leftAnswerButtonList[i].hintAnimObject.SetActive(false);
			rightAnswerButtonList[i].hintAnimObject.SetActive(false);
		}
	}
	public void EnableAnswerButtons()
	{
		for(int i=0; i<leftAnswerButtonList.Count; i++)
		{
			leftAnswerButtonList[i].gameObject.SetActive(true);
			rightAnswerButtonList[i].gameObject.SetActive(true);

			leftAnswerButtonList[i].GetComponent<Image>().raycastTarget = true;
			rightAnswerButtonList[i].GetComponent<Image>().raycastTarget = true;
		}
	}

	public void ClickedHintButton()
	{
		if (!isShowingHint)
		{
			NetworkManager.Ins.SendUseItem("hint_item");
		}
    }

	public void ShowAndHideHint()
	{
		StartCoroutine("Co_ShowAndHideHint");
	}

	IEnumerator Co_ShowAndHideHint()
	{
		AnswerButton btn = GetNextAnswerButton();
		//아이템 갯수 갱신
		if (btn != null)
		{
			GameObject obj = btn.hintAnimObject;

			obj.SetActive(true);

			yield return new WaitForSeconds(2f);

			obj.SetActive(false);

			isShowingHint = false;
		}

		yield return null;
	}

	public void ClickedAddTimeButton()
	{
		NetworkManager.Ins.SendUseItem("timer_item");
		//아이템 갯수 갱신
	}

	public void AddStageTimeByItem()
	{
		stagePlayTime -= 2.0f;
		if (stagePlayTime < 0f)
		{
			stagePlayTime = 0f;
		}
	}

	public AnswerButton GetNextAnswerButton()
	{
		for(int i=0; i<leftAnswerButtonList.Count; i++)
		{
			if (!leftAnswerButtonList[i].circleAnimObject.activeSelf)
			{
				return leftAnswerButtonList[i];
			}
		}

		return null;
	}

	int GetNextStageNumber()
	{
		for(int i=0; i<NetworkManager.Ins.currentLevelstageIndexList.Length; i++)
		{
			if (NetworkManager.Ins.currentLevelstageIndexList[i] == currentStageIndex)
			{
				return NetworkManager.Ins.currentLevelstageIndexList[i+1];
			}
		}

		return 1;
	}
}
