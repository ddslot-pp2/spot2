using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
	public GameObject shopPanel;
	public GameObject levelSelectPanel;
	public GameObject loginTypePanel;
	public GameObject facebookLoginButton;

	public static TitleManager Ins;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		Ins = this;
	}

	// Use this for initialization
	void Start ()
    {
		if (PlayerPrefs.GetString("uid", "") == "")
		{
			//show select login type
		}
		
		if (PlayerPrefs.GetInt("isFacebookLogin", 0) == 0)
		{
			facebookLoginButton.SetActive(true);
		}
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

	public void OnClickedStartButton()
	{
		if (PlayerPrefs.GetString("uid", "") == "")
		{
			loginTypePanel.SetActive(true);
		}
		else
		{
			NetworkManager.Ins.GetUserInfo();
		}
	}

	public void OnclickedShopButton()
	{
		shopPanel.SetActive(true);
	}

    public void OnClickedShoptItemButton()
    {
        SceneManager.LoadScene("ShopItem");
    }

	public void OnSelectedEasyLevel()
	{
		NetworkManager.Ins.GetLevelStageList(LEVEL.EASY);
	}

	public void OnSelectedNormalLevel()
	{
		NetworkManager.Ins.GetLevelStageList(LEVEL.NORMAL);
	}

	public void OnSelectedHardLevel()
	{
		NetworkManager.Ins.GetLevelStageList(LEVEL.HARD);
	}

	public void OnSelectedHellLevel()
	{
		
	}

	public void OnSelectGuestLogin()
	{
		PlayerPrefs.SetString("uid", SystemInfo.deviceUniqueIdentifier);

		NetworkManager.Ins.GetUserInfo();
	}

	public void OnSelectFacebookLogin()
	{
		//show facebook login
	}

	public void OnFinishedGetUserInfo()
	{
		loginTypePanel.SetActive(false);
		levelSelectPanel.SetActive(true);
	}
}
