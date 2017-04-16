using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Purchasing;
using UnityEngine.Advertisements;

public class ShopManager : MonoBehaviour, IStoreListener {
    public static ShopManager Ins;

    public Sprite hintItemIcon;
    public Sprite timerItemIcon;

    public GameObject rewardAdButton;

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
		//NetworkManager.Ins.GetItemInfoList();
        // iap 초기화
        InitializePurchasing();

        if (PlayerPrefs.GetInt(System.DateTime.Now.ToString(), 0) >= 3)
        {
            rewardAdButton.GetComponent<Button>().interactable = false;
        }
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

    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    public const string productId1 = "test_timer_item_10";

    private bool IsInitialized()
    {
        return (storeController != null && extensionProvider != null);
    }
        
    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        var module = StandardPurchasingModule.Instance();

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

        builder.AddProduct(productId1, ProductType.Consumable, new IDs
        {
           // { productId1, AppleAppStore.Name },
            { productId1, GooglePlay.Name },
        });

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyProductID(string productId)
    {
        try
        {
            if (IsInitialized())
            {
                Product p = storeController.products.WithID(productId);

                if (p != null && p.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
                    storeController.InitiatePurchase(p);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (UnityException e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    public void OnInitialized(IStoreController sc, IExtensionProvider ep)
    {
        Debug.Log("OnInitialized : PASS");

        storeController = sc;
        extensionProvider = ep;
    }

    public void OnInitializeFailed(InitializationFailureReason reason)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + reason);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

        if (args.purchasedProduct.definition.id == productId1)
        {
            NetworkManager.Ins.PurchaseItemRequest(productId1);
        }
        else
        {

        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }


    public void OnTestTimerItemButton()
    {
        BuyProductID(productId1);
        Debug.Log("구매구매");
    }

    /*
    public void RestorePurchase()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = extensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions
                (
                    (result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
                );
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }
    */

    public void OnClickedRewadAdButton()
    {
        if (PlayerPrefs.GetInt(System.DateTime.Now.ToString(), 0) < 3)
        {
            ShowRewardedAd();
        }
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
        else
        {
            
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                int adCount = PlayerPrefs.GetInt(System.DateTime.Now.ToString(), 0);
                adCount++;
                PlayerPrefs.SetInt(System.DateTime.Now.ToString(), adCount);
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}
