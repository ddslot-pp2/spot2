using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum POPUPTYPE
{
	EXITGAME,
	MAINMENU,
	BUYITEM,
	ENDLEVEL
}
public class Popup : MonoBehaviour {
	public POPUPTYPE type;

	public Text body;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickedCofirmButton()
	{
		switch (type)
		{
			case POPUPTYPE.EXITGAME:
			{
				Application.Quit();
				break;
			}
			case POPUPTYPE.MAINMENU:
			case POPUPTYPE.ENDLEVEL:
			{
				SceneManager.LoadSceneAsync("Title");
				break;
			}
			case POPUPTYPE.BUYITEM:
			{
				break;
			}
		} 
	}

	public void Show(POPUPTYPE type_)
	{
		type = type_;
		SetBodyText();

		gameObject.SetActive(true);
	}

	void SetBodyText()
	{
		switch(type)
		{
			case POPUPTYPE.MAINMENU:
				body.text = "Go to Main Menu?";
				break;
			case POPUPTYPE.EXITGAME:
				body.text = "Quit Game?";
				break;
			case POPUPTYPE.BUYITEM:
				body.text = "Buy this Item?";
				break;
			case POPUPTYPE.ENDLEVEL:
				body.text = "Good Job! Level Complete!";
				break;
		}		
	}

	public void OnClickedCancleButton()
	{
		Destroy(gameObject);
	}
}
