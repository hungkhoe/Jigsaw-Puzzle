using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine.UI;

namespace Puzzle.Game.Ads
{
    public class Admob
    {
        public interface IAdInterface
        {
            void onRewardedVideo();
            void onAdClosed();
        }

        private readonly AdManager adManager;
        //private BannerView banner;
        private InterstitialAd interstitial;
        private RewardBasedVideoAd rewardBasedVideo;

        //Native Ad
        //private UnifiedNativeAd nativeAds;
        //private bool isNativeLoaded = false;

        //private UnityEngine.GameObject panelNativeAd;
        //private RawImage adIcon;
        //private RawImage adChoise;
        //private Text txtAdHeadLine;
        //private Text txtAdAdvertiser;
        //private Text txtCallToAction;

        //private UnityEngine.GameObject ColliderNativeAd;

        private IAdInterface _listener = null;
        public string keyListener = "IAdInterface";
        Dictionary<string, IAdInterface> listenerContainer = new Dictionary<string, IAdInterface>();

        public Admob(AdManager adManager)
        {
            this.adManager = adManager;
#if UNITY_ANDROID
            CreateAd();

            LoadInterstitialAd();
            LoadRewardedAd();
#endif
        }

        public void RegisterAdmobListener(string key, Admob.IAdInterface mLis)
        {
            unregisterAdmobListener(key);
            this.keyListener = key;
            listenerContainer.Add(key, mLis);
            this._listener = mLis;
        }

        public void unregisterAdmobListener(string key)
        {

            listenerContainer.Remove(key);
            _listener = null;
        }

        private void CreateAd()
        {
            // Initialize an InterstitialAd.
            RequestInterstitial();

            // Get singleton reward based video ad reference.
            this.rewardBasedVideo = RewardBasedVideoAd.Instance;
            // Called when an ad request failed to load.
            //this.rewardBasedVideo.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when the user should be rewarded for interacting with the ad.
            this.rewardBasedVideo.OnAdRewarded += HandleUserEarnedReward;
            // Called when the ad is closed.
            this.rewardBasedVideo.OnAdClosed += HandleRewardedAdClosed;
        }

        public void ShowInterstitial()
        {
#if !UNITY_EDITOR
            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
            }
            else {
                LoadInterstitialAd();
            }
#endif
        }

        public bool ShowRewardedAd()
        {
#if !UNITY_EDITOR
            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
            {
                CommonUtils.ShowShortToast("No Internet Connection!");
                return false;
            }
            else
            {
                if (this.rewardBasedVideo.IsLoaded())
                {
                    this.rewardBasedVideo.Show();
                    return true;
                }
                else
                {
                 CommonUtils.ShowShortToast("Ad is not ready!");
                    LoadRewardedAd();
                    return false;
                }
            }
#else
            HandleUserEarnedReward(null, null);
            return true;

#endif
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            //Debug.Log("HandleRewardedAdFailedToLoad");
            //LoadRewardedAd();
        }


        private void HandleUserEarnedReward(object sender, Reward e)
        {
            IAdInterface tmpListener = null;
            if (listenerContainer.TryGetValue(keyListener, out tmpListener))
            {
                //adManager.StartCoroutine(CallbackToMainThread(tmpListener));
                tmpListener.onRewardedVideo();
            }
        }

        private IEnumerator CallbackToMainThread(IAdInterface tmpListener)
        {
            tmpListener.onRewardedVideo();
            yield return null;
        }

        private IEnumerator CallbackToMainThreadOnAdClosed(IAdInterface tmpListener)
        {
            tmpListener.onAdClosed();
            yield return null;
        }

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            LoadRewardedAd();

            IAdInterface tmpListener = null;
            if (listenerContainer.TryGetValue(keyListener, out tmpListener))
            {
                // adManager.StartCoroutine(CallbackToMainThreadOnAdClosed(tmpListener));
                tmpListener.onAdClosed();
            }
        }

        private void HandleInterstitialClosed(object sender, EventArgs e)
        {
            LoadInterstitialAd();
        }

        private void LoadInterstitialAd()
        {
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder()
                //.AddTestDevice("33BE2250B43518CCDA7DE426D04EE232")
                .Build();
            // Load the interstitial with the request.
            this.interstitial.LoadAd(request);
        }

        private void LoadRewardedAd()
        {
#if UNITY_ANDROID
           string adUnitId_Rewarded = GameConfig.REWARDED_VIDEO_UNIT_ANDROID;
            
#elif UNITY_IPHONE
        string adUnitId_Rewarded = GameConfig.REWARDED_AD_UNIT_IOS;
#else
        string adUnitId_Rewarded = "unexpected_platform";
#endif
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder()
                //.AddTestDevice("33BE2250B43518CCDA7DE426D04EE232")
                .Build();
            // Load the rewarded ad with the request.
            this.rewardBasedVideo.LoadAd(request, adUnitId_Rewarded);
        }


        private void RequestInterstitial()
        {
#if UNITY_ANDROID
            string adUnitId = GameConfig.INTERSTITIAL_UNIT_ANDROID;           
#elif UNITY_IPHONE
        string adUnitId = GameConfig.FULL_AD_UNIT_IOS;
#else
        string adUnitId = "unexpected_platform";
#endif
            this.interstitial = new InterstitialAd(adUnitId);

            // Register for ad events.
            this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
        }
    }
}
