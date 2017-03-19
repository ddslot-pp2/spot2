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
	public Text maxStageCount;
	public Text maxFindDiffCount;
	
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
		//CurtainLeft.CrossFadeAlpha(0f, 1f, false);
		//CurtainRight.CrossFadeAlpha(0f, 1f, false);

		//StartCoroutine("Co_StartStage");
	}

	public void StartStage()
	{
		StartCoroutine("Co_StartStage");
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
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (GetCurrentStageTotalTime() > 0 && !isStageOver && !pauseMenuPanel.gameObject.activeSelf)
		{
        	stagePlayTime += Time.deltaTime;

        	TimeProgressBar.fillAmount = (GetCurrentStageTotalTime() - stagePlayTime) / GetCurrentStageTotalTime();
		}

		if (stagePlayTime >= GetCurrentStageTotalTime())
		{
			isStageOver = true;
			gameoverImg.gameObject.SetActive(true);
		}
    }

    private float GetCurrentStageTotalTime()
    {
        return currentStageTotalTime;
    }

    public void ShowPauseMenu()
	{
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

		EnableAnswerButtons();
	}

	public void SetDiffCountInfos()
	{
		currentFindDiffCount.text = answerFindCount.ToString();
		maxFindDiffCount.text = leftAnswerButtonList.Count.ToString();
	}

	public void SetStageInfos(int curCount, int maxCount)
	{
		currentStageCount.text = curCount.ToString();
		maxStageCount.text = maxCount.ToString();
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
		if (stagePlayTime >= GetCurrentStageTotalTime())
		{
			stagePlayTime = GetCurrentStageTotalTime();
		}

		SetDiffCountInfos();

		Invoke("CheckMoveToNextStage", 2f);
	}

	public void InCorrectPoinClicked()
	{
		//잘못된 곳을 클릭했을 때 남은 시간을 감소시킨다
		stagePlayTime += incorrectTimePenalty;
		inCorrectAnswerImage.rectTransform.anchoredPosition = Input.mousePosition;
		inCorrectAnswerImage.gameObject.SetActive(true);

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

		NetworkManager.Ins.GetStageInfoFromServer(int.Parse(currentStageCount.text)+1);
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
		currentStageTotalTime = 0;
		stagePlayTime = -1f;
		isStageOver = false;
		TimeProgressBar.fillAmount = 1f;
		currentFindDiffCount.text = "0";
		answerFindCount = 0;

		CurtainLeft.CrossFadeAlpha(1f, 1f, false);
		CurtainRight.CrossFadeAlpha(1f, 1f, false);

		NetworkManager.Ins.GetStageInfoFromServer(1);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void DisableAnswerButtons()
	{
		for(int i=0; i<leftAnswerButtonList.Count; i++)
		{
			leftAnswerButtonList[i].circleAnimObject.SetActive(false);
			rightAnswerButtonList[i].circleAnimObject.SetActive(false);
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
}