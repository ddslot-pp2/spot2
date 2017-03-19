using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButton : MonoBehaviour {
	public GameObject circleAnimObject;

	public int index;

	// Use this for initialization
	void Start () {	
		circleAnimObject = transform.Find("circle1").gameObject;
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
