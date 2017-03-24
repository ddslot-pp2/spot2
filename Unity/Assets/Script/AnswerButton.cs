using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButton : MonoBehaviour {
	[HideInInspector]
	public GameObject circleAnimObject;
	[HideInInspector]
	public GameObject hintAnimObject;

	public int index;

	// Use this for initialization
	void Start () {	
		circleAnimObject = transform.Find("circle1").gameObject;
		hintAnimObject = transform.Find("Hint").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ButtonClicked()
	{
		circleAnimObject.SetActive(true);
		GetComponent<AudioSource>().Play();
		GameManager.Ins.AnswerButtonClicked(index);
	}
}
