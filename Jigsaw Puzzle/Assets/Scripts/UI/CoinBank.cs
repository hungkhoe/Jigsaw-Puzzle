using JigsawPuzzle.IAP;
using Puzzle.Game.Ads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class CoinBank : MonoBehaviour, IAPManager.IAPListener, Admob.IAdInterface
{
    [SerializeField]
    Text goldPackOne, goldPackTwo, goldPackThree;
    [SerializeField]
    Transform packOneButton, packTwoButton, packThreeButton, packFourButton;
    private bool isReward = false;
    // Start is called before the first frame update
    void Start()
    {
        if(IAPManager.Instance != null)
        {
            IAPManager.Instance.RegisterIAPListener("shop", this);

            goldPackOne.text = IAPManager.Instance.GetLocalizedPriceString(IAPManager.IAPConst.goldPack1);
            goldPackTwo.text = IAPManager.Instance.GetLocalizedPriceString(IAPManager.IAPConst.goldPack2);
            goldPackThree.text = IAPManager.Instance.GetLocalizedPriceString(IAPManager.IAPConst.goldPack3);
        }      
    }

    public void FreeCoinButton()
    {
        AdManager.Instance.AdmobHandler.RegisterAdmobListener("freeCoinBank", this);
        AdManager.Instance.ShowRewardedAd();

    }

    public void BuyGoldPackOne()
    {
        IAPManager.Instance.RegisterShopItem(IAPManager.IAPConst.goldPack1, 500);
        IAPManager.Instance.BuyProductID(IAPManager.IAPConst.goldPack1);             
    }
    public void BuyGoldPackTwo()
    {
        IAPManager.Instance.RegisterShopItem(IAPManager.IAPConst.goldPack1, 2000);
        IAPManager.Instance.BuyProductID(IAPManager.IAPConst.goldPack1);                     
    }
    public void BuyGoldPackThree()
    {
        IAPManager.Instance.RegisterShopItem(IAPManager.IAPConst.goldPack1, 5000);
        IAPManager.Instance.BuyProductID(IAPManager.IAPConst.goldPack1);     
    }

    public void OnProductIDPurchased(PurchaseEventArgs eventArgs, object item)
    {
        if (item != null)
        {
            var result = (int)item;
            if (MainMenuUI.Instance != null)
            {
                MainMenuUI.Instance.IncreasePlayerCoin(result);
                switch (result)
                {
                    case 500:
                        MainMenuUI.Instance.CoinEffect(packTwoButton);
                        break;
                    case 2000:
                        MainMenuUI.Instance.CoinEffect(packThreeButton);
                        break;
                    case 5000:
                        MainMenuUI.Instance.CoinEffect(packFourButton);
                        break;
                    default:
                        break;
                }
            }                
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseCoin(result);
                switch (result)
                {
                    case 500:
                        GameManager.Instance.CoinEffect(packTwoButton);
                        break;
                    case 2000:
                        GameManager.Instance.CoinEffect(packThreeButton);
                        break;
                    case 5000:
                        GameManager.Instance.CoinEffect(packFourButton);
                        break;
                    default:
                        break;
                }
            }
                
        }
    }

    public void onRewardedVideo()
    {
        isReward = true;
    }

    public void onAdClosed()
    {
        if(isReward)
        {
            if (MainMenuUI.Instance != null)
            {
                MainMenuUI.Instance.IncreasePlayerCoin(100);
                MainMenuUI.Instance.CoinEffect(packOneButton);
            }                
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseCoin(100);
                GameManager.Instance.CoinEffect(packOneButton);
            }                
            isReward = false;
        }      
    }
}
