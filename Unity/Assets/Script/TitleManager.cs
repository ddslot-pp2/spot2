using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Purchasing;

public class TitleManager : MonoBehaviour, IStoreListener {
	public GameObject shopPanel;
	public GameObject levelSelectPanel;
	public GameObject loginTypePanel;
	public GameObject facebookLoginButton;

	public static TitleManager Ins;

	void Awake()
	{
		Ins = this;
	}

	void Start ()
    {
        // iap 초기화
        InitializePurchasing();

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
		//if (Application.platform == RuntimePlatform.Android)
        //{
            if (Input.GetKey(KeyCode.Escape))
            {
                GameObject prefab = Resources.Load("Popup") as GameObject;
                Popup popup = Instantiate(prefab).GetComponent<Popup>();
                popup.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
                popup.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                popup.Show(POPUPTYPE.EXITGAME);
            }
        //} 
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


    // iap 구매 관련
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
}
