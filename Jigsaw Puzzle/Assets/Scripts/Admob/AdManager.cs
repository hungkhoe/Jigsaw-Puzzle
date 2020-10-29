using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;

namespace Puzzle.Game.Ads
{
    public class AdManager : MonoBehaviour, Admob.IAdInterface
    {
        private System.Random m_random = new System.Random();

        private Admob _admob = null;
        private static AdManager _ads = null;
        public Admob AdmobHandler
        {
            get { return _admob; }
        }
        public static AdManager Instance
        {
            get { return _ads; }
        }

        private BannerView banner;
        private UnifiedNativeAd nativeAds;

        private GameObject ColliderNativeAd;
        private GameObject panelNativeAd;
        private RawImage adIcon;
        private RawImage adChoise;
        private Text txtAdHeadLine;
        private Text txtAdAdvertiser;
        private Text txtCallToAction;

        private bool isBannerLoaded = false;
        private bool isNativeLoaded = false;

        void Awake()
        {
            if (_ads == null)
            {
                _ads = this;
                MobileAds.Initialize(HandleInitCompleteAction);

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void HandleInitCompleteAction(InitializationStatus initStatus)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                _admob = new Admob(this);
                Instance.AdmobHandler.RegisterAdmobListener("AdManager", this);

                //Request Ads
                ReloadBottomAd();
            });
        }

        void Start()
        {
            CreateUINativeAd();
        }
        public void ShowInterstitial()
        {
            Debug.Log("Show Interstitial!!!");
            _admob.ShowInterstitial();
        }

        public bool ShowRewardedAd()
        {
            Debug.Log("Show Rewarded video!!!");
            return _admob.ShowRewardedAd();
        }

        public void onRewardedVideo()
        {
            Debug.Log("got callback from here to reward video");
        }

        public void onAdClosed() { }

        /// <summary>
        /// Load Ad Banner
        /// </summary>
        #region Load Ad Banner
        private void RequestBanner()
        {
#if UNITY_ANDROID
            string adUnitId = GameConfig.BANNER_UNIT_ANDROID;
#elif UNITY_IPHONE
        string adUnitId = GameConfig.REWARDED_AD_UNIT_IOS;
#else
        string adUnitId = "unexpected_platform";
#endif
            AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            this.banner = new BannerView(adUnitId, adSize, AdPosition.Bottom);

            this.banner.OnAdFailedToLoad += HandleBannerFailedToLoad;
            this.banner.OnAdLoaded += HandleBannerLoaded;

            this.banner.LoadAd(CreateAdRequest());

        }

        private void HandleBannerFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            
        }

        private void HandleBannerLoaded(object sender, System.EventArgs e)
        {
            isBannerLoaded = true;
        }
        #endregion


        #region Load Ad Native
        private void RequestNative()
        {
            AdLoader adLoader = new AdLoader.Builder(GameConfig.NATIVE_ADS_UNIT_ANDROID)
                .ForUnifiedNativeAd()
                .Build();
            adLoader.OnUnifiedNativeAdLoaded += OnNativeAdsLoaded;
            //adLoader.OnAdFailedToLoad += OnNativeFailedToLoad;

            adLoader.LoadAd(CreateAdRequest());
        }

        private void OnNativeAdsLoaded(object sender, UnifiedNativeAdEventArgs e)
        {
            CreateUINativeAd();
            this.nativeAds = e.nativeAd;
            isNativeLoaded = true;
            
        }

        private void CreateUINativeAd()
        {
            if (panelNativeAd != null) Destroy(panelNativeAd);
            if (ColliderNativeAd != null) Destroy(ColliderNativeAd);

            var _canvas = Resources.Load<GameObject>("CanvasNativeAds");
            var _collider = Resources.Load<GameObject>("ColliderNativeAd");

            panelNativeAd = Instantiate<GameObject>(_canvas, transform);
            ColliderNativeAd = Instantiate<GameObject>(_collider, transform);

            adIcon = TransformUtils.GetGameObjectRecursive(panelNativeAd.transform.GetChild(0), "adIcon").GetComponent<RawImage>();
            adChoise = TransformUtils.GetGameObjectRecursive(panelNativeAd.transform.GetChild(0), "adChoise").GetComponent<RawImage>();
            txtAdHeadLine = TransformUtils.GetTextRecursive(panelNativeAd.transform.GetChild(0), "txtHeadLine");
            txtAdAdvertiser = TransformUtils.GetTextRecursive(panelNativeAd.transform.GetChild(0), "txtAdvertiser");
            txtCallToAction = TransformUtils.GetTextRecursive(panelNativeAd.transform.GetChild(0), "txtCallToAction");

            panelNativeAd.SetActive(false);
            ColliderNativeAd.SetActive(false);

            SetColliderPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 4.6f));
        }

        public void onShowNativeAd()
        {
            if (isNativeLoaded)
            {
                isNativeLoaded = false;
                Texture2D iconTexture = this.nativeAds.GetIconTexture();
                Texture2D iconAdChoices = this.nativeAds.GetAdChoicesLogoTexture();

                string headline = this.nativeAds.GetHeadlineText();
                string cta = this.nativeAds.GetCallToActionText();
                string advertiser = this.nativeAds.GetAdvertiserText();

                adIcon.texture = iconTexture;
                adChoise.texture = iconAdChoices;
                txtAdHeadLine.text = headline;
                txtAdAdvertiser.text = advertiser;
                txtCallToAction.text = cta;

                nativeAds.RegisterHeadlineTextGameObject(ColliderNativeAd);

                panelNativeAd.SetActive(true);
                ColliderNativeAd.SetActive(true);             
            }            
        }

        public void OnCloseNativeAds()
        {
            if(panelNativeAd != null)
            {
                panelNativeAd.SetActive(false);
                ColliderNativeAd.SetActive(false);
            }      
        }

        public void SetColliderPosition(UnityEngine.Vector3 pos)
        {
            if (ColliderNativeAd != null)
            {
                ColliderNativeAd.transform.position = pos;
            }
        }
#endregion

        /// <summary>
        /// Helper Method to Create Ad Request.
        /// </summary>
        /// <returns></returns>
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
              //.AddTestDevice("33BE2250B43518CCDA7DE426D04EE232")
              .Build();
        }
        public void ReloadBottomAd()
        {

            //if (m_random.Next(100) % 2 == 0)
            //{
            //    if (panelNativeAd != null) Destroy(panelNativeAd);
            //    if (ColliderNativeAd != null) Destroy(ColliderNativeAd);
            //    RequestBanner();
            //}
            //else
            //{
            //    RequestNative();
            //}
            RequestNative();
        }

    }
}
