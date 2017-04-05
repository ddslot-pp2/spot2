using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour {
    public static ShopManager Ins;

    public Sprite hintItemIcon;
    public Sprite timerItemIcon;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Ins = this;
    }

	public ShopItem[] itemList;

    public enum ITEM { NONE=0, HINT, INC_TIMER };
	void Start ()
    {
		NetworkManager.Ins.GetItemInfoList();
	}

	void Update ()
    {
		
	}

	public void UpdateItemsInfo(string[] itemInfoList)
	{
        int index = 1;
        for(int i=0; i<itemInfoList.Length; i++)
        {
            string[] split = itemInfoList[i].Split('_');
            if (split[0] == "HINT")
            {
                itemList[index].itemIcon.sprite = hintItemIcon;
                itemList[index].countText.text = string.Format("x{0}", split[1]);
            }           
            else if (split[0] == "INC")
            {
                itemList[index].itemIcon.sprite = timerItemIcon;
                itemList[index].countText.text = string.Format("x{0}", split[2]);
            }

            index++;
        }
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}
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
}
