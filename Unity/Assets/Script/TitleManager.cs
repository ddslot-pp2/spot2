using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(RequestItemInfo(http://images.earthcam.com/ec_metros/ourcams/fridays.jpg));
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

	public void OnClickedStartButton()
	{
		SceneManager.LoadScene("Main");
	}

    public void OnClickedShoptItemButton()
    {
        SceneManager.LoadScene("ShopItem");
    }

    
    // 입장하면서 유저 아이템 정보가져오기 인데 싱글턴으로;
    IEnumerator RequestItemInfo(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        //www.responseHeaders

        if (www.isDone)
        {

        }
        else
        {

        }
    }

}
