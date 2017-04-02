using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

<<<<<<< HEAD
public class ShopManager : MonoBehaviour {
	public ShopItem[] itemList;
=======
public class ShopManager : MonoBehaviour
{
>>>>>>> origin/master

    public enum ITEM { NONE=0, HINT, INC_TIMER };
	void Start ()
    {
		
	}

	void Update ()
    {
		
	}

<<<<<<< HEAD
	public void UpdateItemsInfo()
	{

	}

	public void Close()
	{
		gameObject.SetActive(false);
	}
=======
    void request_item_info()
    {

    }

    public void back_to_lobby_btn_clicked()
    {
        SceneManager.LoadScene("Title");
    }

    public void add_hint_x10_btn_clicked()
    {
        Debug.Log("힌트 10개 추가 버튼 클릭");
    }

    public void add_hint_x50_btn_clicked()
    {
        Debug.Log("힌트 50개 추가 버튼 클릭");
    }

    public void add_increase_timer_x10_btn_clicked()
    {
        Debug.Log("타이머 증가 10개 추가 버튼 클릭");
    }

    public void add_increase_timer_x50_btn_clicked()
    {
        Debug.Log("타이머 증가 50개 추가 버튼 클릭");
    }
>>>>>>> origin/master
}
