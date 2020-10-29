using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Puzzle.Game.Ads;

public class MainMenuUI : MonoBehaviour, Admob.IAdInterface
{
    public enum MainScrollButton
    { 
        DAILY_PUZZLE,
        EDITOR_CHOICE,
        MYSTERY_PUZZLE
    }

    // Start is called before the first frame update
    static MainMenuUI instance;   
    public static MainMenuUI Instance
    {
        get
        {
            return instance;
        }
    }
    [SerializeField]
    private GameObject difficultSelectionPanel;
    [SerializeField]
    private GameObject continueProgressPanel;
    [SerializeField]
    private GameObject coinBankPanel;
    [SerializeField]
    private GameObject freecoinPanel;
    // Main Panel //
    [SerializeField]
    private GameObject categoriesPanel;
    [SerializeField]
    private GameObject dailyPanel;
    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private GameObject profilePanel;
    [SerializeField]
    private GameObject categoriesContent;
    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private Transform subTitle;
    // ==============================
    [SerializeField]
    private DifficultSelectionUI difficultSelectionScript;
    [SerializeField]
    private RawImage[] mainScrollViewImage;
    [SerializeField]
    private TextMeshProUGUI coinText;
    [SerializeField]
    private Image[] mainButtonImage;
    [SerializeField]
    private Sprite activeButton;
    [SerializeField]
    private Sprite deactiveButton;
    // ============= Categories Variable ============ //
    [SerializeField]
    private Transform[] categoriesImage;
    [SerializeField]
    private Text subTitleText;
    [SerializeField]
    private GameObject subTitleObject;
    [SerializeField]
    private GameObject backToMainCategoriesButton;
    [SerializeField]
    private GameObject subCategoriesScroll;
    [SerializeField]
    private GameObject mainCategoriesScroll;

    // ============= FreeCoin Variable ============ //
    [SerializeField]
    private Sprite presentUnOpenSprite;
    [SerializeField]
    private Sprite presentOpenSprite;
    [SerializeField]
    private Image presentImage;
    [SerializeField]
    private Text watchButtonTxt;
    [SerializeField]
    private TextMeshProUGUI freeCoinText;
    [SerializeField]
    private TextMeshProUGUI coinAmountReceiveText;
    [SerializeField]
    private TextMeshProUGUI textAfterAds1;
    [SerializeField]
    private TextMeshProUGUI textAfterAds2;
    [SerializeField]
    private Image freeCoinImage;

    [SerializeField]
    private ScrollPuzzle mainScrollPuzzle;
    [SerializeField]
    private ProfileScroll profileScroll;

    private GameObject currentPanelOpen;
    private Image currentButtonImage;

    private int currentButtonIndex = -1;
    private int coinReward = 4;

    private bool isWatchVideoSuccess;
    private bool isLoadingNewTextureDone;

    [SerializeField]
    RawImage editorChoiceImg;
    [SerializeField]
    RawImage todayChoiceImg;

    public bool isLoadingDoneEditor;
    public bool isLoadingDoneToday;

    [SerializeField]
    private DailyPuzzle dailyPuzzleScript;
    [SerializeField]
    private GameObject loadingProgress;
    [SerializeField]
    private GameObject startImage;

    private bool isReward = false;
    private bool isLoadingTexture = false;

    [SerializeField]
    private Sprite musicOn;
    [SerializeField]
    private Sprite musicOff;
    [SerializeField]
    private Image musicButtonImg;

    [SerializeField]
    private Sprite soundOn;
    [SerializeField]
    private Sprite soundOff;
    [SerializeField]
    private Image soundButtonImg;

    [SerializeField]
    private Animator presentAnimator;
    [SerializeField]
    private GameObject rateAppPanel;
    [SerializeField]
    private GameObject mainCoinPrefab;
    [SerializeField]
    private GameObject [] coinFlyingContainer;
    [SerializeField]
    private Transform coinDestination;

    private Coroutine queueDownload;

    private string contentLinkOne;
    private string contentLinkTwo;

    public float speedEffect;

    private void Awake()
    {
        instance = this;
#if !UNITY_EDITOR
        
#endif
    }
    void Start()
    {
        OnInit();
        ScaleCategoriesChild();
        OnInitMainImage();
    }
    private void OnInit()
    {
        if(PlayerPrefs.HasKey(GameConfig.PREF_COIN))
        {
            int coin = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
            coinText.text = coin.ToString();
        }
        else
        {
            PlayerPrefs.SetInt(GameConfig.PREF_COIN, 200);
            int coin = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
            coinText.text = coin.ToString();
        }
        currentPanelOpen = mainMenuPanel;
        currentButtonImage = mainButtonImage[0];

        if (MusicManager.Instance.GetIsMuicOn() == false)
        {
            soundButtonImg.sprite = soundOff;
        }
        else
        {
            soundButtonImg.sprite = soundOn;
        }    

        if (MusicManager.Instance.GetIsSFXOn() == false)
        {
            musicButtonImg.sprite = musicOff;
        }
        else
        {
            musicButtonImg.sprite = musicOn;
        }
        musicButtonImg.SetNativeSize();
        soundButtonImg.SetNativeSize();

        RectTransform child = editorChoiceImg.GetComponent<RectTransform>();
        child.sizeDelta = new Vector2(child.rect.width * CommonUltilities.GetRescaleRatio(), child.rect.height * CommonUltilities.GetRescaleRatio());

        child = todayChoiceImg.GetComponent<RectTransform>();
        child.sizeDelta = new Vector2(child.rect.width * CommonUltilities.GetRescaleRatio(), child.rect.height * CommonUltilities.GetRescaleRatio());
    }
    public void OpenClosePuzzleDifficultSelection()
    {
        // sử dụng cho các nút mở pop up difficult selection panel
        if(isLoadingTexture == false)
        {
            MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
            if (difficultSelectionPanel.activeSelf)
            {
                difficultSelectionPanel.SetActive(false);
                PassInformation.Instance.SetIsNewGame(false);
                PassInformation.Instance.ResetSlotNumber();
                isLoadingNewTextureDone = false;
            }
            else
            {
                difficultSelectionPanel.SetActive(true);
            }
        }          
    }
    public void OpenContinueProgress()
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (continueProgressPanel.activeSelf)
        {
            continueProgressPanel.SetActive(false);
            PassInformation.Instance.ResetSlotNumber();
            isLoadingNewTextureDone = false;
        }
        else
        {
            continueProgressPanel.SetActive(true);
        }
    }   
    public void StartButton()
    {
        if(isLoadingTexture == false)
        {
            PassInformation.Instance.SetRowNumberImage();
            Puzzle.Game.Ads.AdManager.Instance.ShowInterstitial();
            isLoadingTexture = true;
            loadingProgress.SetActive(true);
            startImage.SetActive(false);
            if(queueDownload == null)
            {
                DownloadTexture();
            }
            StartCoroutine(LoadYourAsyncScene("Gameplay"));
        }
    }
    private IEnumerator LoadYourAsyncScene(string _sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        float timer = 0;
        while (difficultSelectionScript.GetRawImgPreview().texture.width != 1280)
        {
            timer += Time.deltaTime;
            if(timer >= 6f)
            {
                isLoadingTexture = false;
                loadingProgress.SetActive(false);
                startImage.SetActive(true);
                CommonUtils.ShowShortToast("Failed to load. Retry");
                yield break;
            }
            else if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                CommonUtils.ShowShortToast("No internet connection!");
                isLoadingTexture = false;
                loadingProgress.SetActive(false);
                startImage.SetActive(true);
                yield break;
            }
            yield return null;
        }

        if (PassInformation.Instance.GetImagePreview() != ImageLinkManager.Instance.failedToLoadImage)
        {
            PassInformation.Instance.SetImgPreview();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }       
    }
    public DifficultSelectionUI GetDifficultSelectionManager()
    {
        //returen difficult selection script
        return difficultSelectionScript;
    }
    public bool GetIsLoadingNewTextureDone()
    {
        return isLoadingNewTextureDone;
    }
    public void ResetIsLoadingTexture()
    {
        isLoadingNewTextureDone = false;
    }
    public void IncreasePlayerCoin(int amount)
    {
        int coin = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
        coin += amount;
        PlayerPrefs.SetInt(GameConfig.PREF_COIN, coin);
        coinText.text = coin.ToString();
    }

    // ============= Button Function =========== //
    public void CoinBankButton()
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (coinBankPanel.activeSelf)
        {
            coinBankPanel.SetActive(false);
            SetActiveFalseAllCoin();
        }
        else if (!coinBankPanel.activeSelf)
        {
            coinBankPanel.SetActive(true);
        }
    }
    public void FreeCoinButton()
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (freecoinPanel.activeSelf)
        {
            freecoinPanel.SetActive(false);
            SetActiveFalseAllCoin();
        }
        else if (!freecoinPanel.activeSelf)
        {
            freecoinPanel.SetActive(true);
        }
    }
    public void MainButton(int index)
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (index != currentButtonIndex)
        {          
            if (currentPanelOpen == categoriesPanel)
            {
                CloseSubCategories();
            }        
            currentPanelOpen.SetActive(false);
            currentButtonImage.sprite = deactiveButton;
            currentButtonImage = mainButtonImage[index];
            currentButtonImage.sprite = activeButton;
            currentButtonIndex = index;
            switch (index)
            {
                // 0 - menu , 1 - daily , 2 - categories , 3 - profile
                case 0:
                    currentPanelOpen = mainMenuPanel;
                    currentPanelOpen.SetActive(true);
                    mainScrollPuzzle.ClearQueue();
                    OnInitMainImage();
                    break;
                case 1:
                    currentPanelOpen = dailyPanel;
                    currentPanelOpen.SetActive(true);
                    dailyPuzzleScript.ResetCoroutine();
                    break;
                case 2:
                    currentPanelOpen = categoriesPanel;
                    currentPanelOpen.SetActive(true);   
                        break;
                case 3:
                    currentPanelOpen = profilePanel;
                    currentPanelOpen.SetActive(true);
                    DataManager.Instance.LoadGame();
                    profileScroll.ResetDataScroll();
                    break;
                default:
                    break;
            }
        }       
    }
    public void MainScrollViewButton(int index)
    {
        //index follow enum 
        // 0 = daily puzzle , 1 = editor choice, 2 = mystery puzzle
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (index == 0)
        {
            if (isLoadingDoneToday)
            {
                isLoadingNewTextureDone = true;
                difficultSelectionScript.ChangeImagePreview(mainScrollViewImage[index]);
                string contentLink = contentLinkTwo;
                contentLink = contentLink.Replace("1280x1280", "300x300");
                    
                PassInformation.Instance.SetContentLink(contentLink);
                if (DataManager.Instance.imageLinkKeys.Contains(contentLink))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(contentLink));
                    PassInformation.Instance.SetSlotNumber(tempIndex);
                    PassInformation.Instance.SetRowNumber(DataManager.Instance.saveFile.saveSlotList[tempIndex].rowNumber);
                    PassInformation.Instance.SetIsRotatingMode(DataManager.Instance.saveFile.saveSlotList[tempIndex].isRotating);
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        PassInformation.Instance.SetIsCompletedMode(false);
                        PassInformation.Instance.SetIsNewGame(true);
                        OpenClosePuzzleDifficultSelection();
                    }
                    else
                    {
                        PassInformation.Instance.SetIsCompletedMode(true);
                        OpenContinueProgress();
                    }
                }
                else
                {
                    OpenClosePuzzleDifficultSelection();
                }
            }
        }
        else if (index == 1)
        {
            if (isLoadingDoneEditor)
            {
                isLoadingNewTextureDone = true;
                difficultSelectionScript.ChangeImagePreview(mainScrollViewImage[index]);
                string contentLink = PlayerPrefs.GetString(GameConfig.PREF_EDITOR_CHOICE_LINK);
                contentLink = contentLink.Replace("1280x1280", "300x300");

                PassInformation.Instance.SetContentLink(contentLink);
                if (DataManager.Instance.imageLinkKeys.Contains(contentLink))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(contentLink));
                    PassInformation.Instance.SetSlotNumber(tempIndex);
                    PassInformation.Instance.SetRowNumber(DataManager.Instance.saveFile.saveSlotList[tempIndex].rowNumber);
                    PassInformation.Instance.SetIsRotatingMode(DataManager.Instance.saveFile.saveSlotList[tempIndex].isRotating);
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        PassInformation.Instance.SetIsCompletedMode(false);
                        PassInformation.Instance.SetIsNewGame(true);
                        OpenClosePuzzleDifficultSelection();
                    }
                    else
                    {
                        PassInformation.Instance.SetIsCompletedMode(true);
                        OpenContinueProgress();
                    }
                }
                else
                {
                    OpenClosePuzzleDifficultSelection();
                }
            }
        }
    }
    public void SettingButton()
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
        else
        {
            settingPanel.SetActive(true);
        }
    }
    public void MusicButton()
    {
        MusicManager.Instance.SetSFX(!MusicManager.Instance.GetIsSFXOn());
        if(MusicManager.Instance.GetIsSFXOn() == false)
        {
            musicButtonImg.sprite = musicOff;
        }
        else
        {
            musicButtonImg.sprite = musicOn;
        }
        musicButtonImg.SetNativeSize();
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
    }
    public void SoundEffectButton()
    {
        MusicManager.Instance.SetMusic(!MusicManager.Instance.GetIsMuicOn());

        if (MusicManager.Instance.GetIsMuicOn() == false)
        {
            soundButtonImg.sprite = soundOff;
        }
        else
        {
            soundButtonImg.sprite = soundOn;
        }
        soundButtonImg.SetNativeSize();
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
    }
    public void PolicyButton()
    {
        Application.OpenURL("https://sites.google.com/view/aestudioprc");
    }
    public void RateAppButton()
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (rateAppPanel.activeSelf)
        {
            rateAppPanel.SetActive(false);
        }
        else
        {
            rateAppPanel.SetActive(true);
        }
    }

    // ============ Categories Function ========== //
    private void ScaleCategoriesChild()
    {
        float initYPos = categoriesContent.transform.GetChild(0).localPosition.y;
        int index = 0;
        for (int i = 0; i < categoriesContent.transform.childCount; i++)
        {
            if( i != 0 && i % 2 == 0)
            {
                index++;
            }
            RectTransform child = categoriesContent.transform.GetChild(i).GetComponent<RectTransform>();
            child.sizeDelta = new Vector2(child.rect.width * CommonUltilities.GetRescaleRatio(), child.rect.height * CommonUltilities.GetRescaleRatio());

            child.transform.localPosition = new Vector2(child.transform.localPosition.x, initYPos - (index * GameConfig.CATEGORIES_OFFSET_Y * CommonUltilities.GetRescaleRatio()));

            for(int j = 0; j < child.childCount; j++)
            {
                child.GetChild(j).GetComponent<RectTransform>().sizeDelta = new Vector2(child.GetChild(j).GetComponent<RectTransform>().rect.width * CommonUltilities.GetRescaleRatio()
                    , child.GetChild(j).GetComponent<RectTransform>().rect.height * CommonUltilities.GetRescaleRatio());
            }
        }

        for (int i = 0; i < categoriesImage.Length; i++)
        {
            categoriesImage[i].GetComponent<RectTransform>().sizeDelta = new Vector2(categoriesImage[i].GetComponent<RectTransform>().rect.width * CommonUltilities.GetRescaleRatio(),
               categoriesImage[i].GetComponent<RectTransform>().rect.width * CommonUltilities.GetRescaleRatio());
        }
        
        categoriesContent.GetComponent<RectTransform>().sizeDelta = new Vector2(categoriesContent.GetComponent<RectTransform>().sizeDelta.x * 2,
            categoriesContent.GetComponent<RectTransform>().sizeDelta.y * CommonUltilities.GetRescaleRatio());

        subTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(subTitle.GetComponent<RectTransform>().sizeDelta.x * CommonUltilities.GetRescaleRatio(),
           subTitle.GetComponent<RectTransform>().sizeDelta.y);
    }
    public void CategoriesButton(string _categoryName)
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        mainCategoriesScroll.SetActive(false);
        subCategoriesScroll.SetActive(true);
        subTitleObject.SetActive(true);
        subTitleText.text = _categoryName;
        backToMainCategoriesButton.SetActive(true);
    }
    public void CloseSubCategories()
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        mainCategoriesScroll.SetActive(true);
        subCategoriesScroll.SetActive(false);
        subTitleObject.SetActive(false);
        backToMainCategoriesButton.SetActive(false);
    }

    // ============== Free Coin Function ========== //
    public void RewardVideoSuccess()
    {
        freeCoinImage.color = new Color(1, 1, 1, 1);
        textAfterAds1.color = new Color(1, 1, 1, 1);
        textAfterAds2.color = new Color(1, 1, 1, 1);
        freeCoinText.text = "THANKS FOR WATCHING";
        watchButtonTxt.text = "CLAIM";
        isWatchVideoSuccess = true;
        presentImage.sprite = presentOpenSprite;
        presentAnimator.enabled = true;
        coinAmountReceiveText.text = coinReward.ToString();
    }
    private void CloseFreeCoinAfterReceiving()
    {
        freeCoinImage.color = new Color(1, 1, 1, 0);
        textAfterAds1.color = new Color(1, 1, 1, 0);
        textAfterAds2.color = new Color(1, 1, 1, 0);
        freeCoinText.text = "FREE COINS WATCH ADS TO GET REWARD";
        watchButtonTxt.text = "WATCH";
        isWatchVideoSuccess = false;
        presentAnimator.enabled = false;
        presentImage.sprite = presentUnOpenSprite;
    }
    public void WatchFreeCoinButton()
    {
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        if (isWatchVideoSuccess == false)
        {
            // TODO : WATCH VIDEO
            coinReward = 100;
            AdManager.Instance.AdmobHandler.RegisterAdmobListener("freeCoinPresent", this);
            AdManager.Instance.ShowRewardedAd();
#if UNITY_EDITOR
            RewardVideoSuccess();
#endif 
        }
        else
        {
            IncreasePlayerCoin(coinReward);            
            CoinEffect(mainCoinPrefab.transform, () => { FreeCoinButton();
                CloseFreeCoinAfterReceiving();
            });
           
        }
    }
    public void DownloadTexture()
    {
        if (queueDownload == null)
            queueDownload = StartCoroutine(LoadImageDavinci());
        else
        {
            StopCoroutine(queueDownload);
            queueDownload = StartCoroutine(LoadImageDavinci());
        }
    }
    IEnumerator LoadImageDavinci()
    {
        isLoadingNewTextureDone = false;

        string contentLink = PassInformation.Instance.GetContentLink();
        string newString = contentLink.Replace("300x300", "1280x1280");

        ImageLinkManager.Instance.loadImagePreview.success = false;
        ImageLinkManager.Instance.loadImagePreview.load(newString).setCached(true).into(difficultSelectionScript.GetRawImgPreview()).start();

        while (!ImageLinkManager.Instance.loadImagePreview.success)
        {
            yield return null;
        }
        PassInformation.Instance.SetImgPreview();

        yield return new WaitForSeconds(0.1f);
        isLoadingNewTextureDone = true;

        queueDownload = null;
        yield return null;
    }
    public void onRewardedVideo()
    {
        isReward = true;
    }
    public void onAdClosed()
    {
        if(isReward)
        {
            isReward = false;
            RewardVideoSuccess();
        }
    }

    // ============= Load Main Image ============= //
    private void OnInitMainImage()
    {
        isLoadingDoneEditor = false;
        isLoadingDoneToday = false;

        int editorChoiceIndex = 0;

        contentLinkOne = "";
        contentLinkTwo = "";       


        contentLinkTwo = ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[System.DateTime.Now.Day - 1].link;
        contentLinkTwo = contentLinkTwo.Replace("300x300", "1280x1280");

        if (dayCheck() == true)
        {
            editorChoiceIndex = UnityEngine.Random.Range(1, 550);
            contentLinkOne = ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[editorChoiceIndex].link;
            contentLinkOne = contentLinkOne.Replace("300x300", "1280x1280");
            PlayerPrefs.SetString(GameConfig.PREF_EDITOR_CHOICE_LINK, contentLinkOne);
            contentLinkTwo = ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[System.DateTime.Now.Day - 1].link;
            contentLinkTwo = contentLinkTwo.Replace("300x300", "1280x1280");         
            dailyPuzzleScript.ReInitDaily();
        }
        else
        {
            if (PlayerPrefs.HasKey(GameConfig.PREF_EDITOR_CHOICE_LINK))
            {
                contentLinkOne = PlayerPrefs.GetString(GameConfig.PREF_EDITOR_CHOICE_LINK);
            }
            else
            {
                editorChoiceIndex = UnityEngine.Random.Range(1, 550);
                contentLinkOne = ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[editorChoiceIndex].link;
                contentLinkOne = contentLinkOne.Replace("300x300", "1280x1280");
                PlayerPrefs.SetString(GameConfig.PREF_EDITOR_CHOICE_LINK, contentLinkOne);
            }           
        }

        if(ImageLinkManager.Instance.todayImage == null)
        {
            StartCoroutine(DownloadTexture(todayChoiceImg, contentLinkTwo, () => { 
                if(todayChoiceImg.texture != ImageLinkManager.Instance.failedToLoadImage)
                {
                    isLoadingDoneToday = true;
                    ImageLinkManager.Instance.todayImage = (Texture2D)todayChoiceImg.texture;
                }               
            }));
        }
        else
        {
            todayChoiceImg.texture = ImageLinkManager.Instance.todayImage;
            isLoadingDoneToday = true;
        }

        if (ImageLinkManager.Instance.editorImage == null)
        {
            StartCoroutine(DownloadTexture(editorChoiceImg, contentLinkOne, () => { 
                if(editorChoiceImg.texture != ImageLinkManager.Instance.failedToLoadImage)
                {
                    isLoadingDoneEditor = true;
                    ImageLinkManager.Instance.editorImage = (Texture2D)editorChoiceImg.texture;
                }               
            }));
        }
        else
        {
            editorChoiceImg.texture = ImageLinkManager.Instance.editorImage;
            isLoadingDoneEditor = true;
        }      
    }
    public bool dayCheck()
    {
        if (PlayerPrefs.HasKey(GameConfig.PREF_LAST_LOG_IN) == false)
        {
            DateTime newDateTemp = System.DateTime.Now;
            string newStringDateTemp = Convert.ToString(newDateTemp);
            PlayerPrefs.SetString(GameConfig.PREF_LAST_LOG_IN, newStringDateTemp);
            return false;
        }

        string stringDate = PlayerPrefs.GetString(GameConfig.PREF_LAST_LOG_IN);
        DateTime oldDate = Convert.ToDateTime(stringDate);
        DateTime newDate = System.DateTime.Now;

        if (oldDate.Day != newDate.Day || oldDate.Month != newDate.Month)
        {
            if(oldDate.Month != newDate.Month)
            {
                for(int i = 0; i < ImageLinkManager.Instance.thisMonthTextureContainer.Length;i++)
                {
                    ImageLinkManager.Instance.thisMonthTextureContainer[i] = null;
                }
                ImageLinkManager.Instance.LoadMonthData();
            }
            string newStringDate = Convert.ToString(newDate);
            PlayerPrefs.SetString(GameConfig.PREF_LAST_LOG_IN, newStringDate);
            ImageLinkManager.Instance.editorImage = null;
            ImageLinkManager.Instance.todayImage = null;
            return true;
        }
        return false;
    }
    IEnumerator DownloadTexture(RawImage img, string link, System.Action action)
    {
        Davinci temp = Davinci.get();
        temp.load(link).setCached(true).into(img).start();

        while (!temp.success)
        {
            yield return null;
        }

        Destroy(temp.gameObject);

        if (action != null)
        {
            action();
        }

        yield return null;
    }

    // ============= Coin Effect ============= //
    public void CoinEffect(Transform originPos, Action callBack = null)
    {
        StartCoroutine(MoveAllCoinToDestination(originPos,callBack));
    }
    IEnumerator MoveAllCoinToDestination(Transform originPos, Action callBack = null)
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < coinFlyingContainer.Length; i++)
        {
            if (i == 4)
            {
                StartCoroutine(MoveCoinToDestination(coinFlyingContainer[i], originPos,callBack));
            }
            else
                StartCoroutine(MoveCoinToDestination(coinFlyingContainer[i], originPos));
            yield return new WaitForSecondsRealtime(0.07f);            
        }
        yield return null;
    }
    IEnumerator MoveCoinToDestination(GameObject _coin, Transform originPos, Action callBack = null)
    {
        _coin.transform.position = new Vector3(originPos.position.x, originPos.position.y,0);
        _coin.SetActive(true);
        while (_coin.transform.position  != coinDestination.position)
        {
            _coin.transform.position = Vector3.MoveTowards(_coin.transform.position, coinDestination.position, speedEffect);            
            yield return null;
        }
        _coin.SetActive(false);
        _coin.transform.position = originPos.position;
        if(callBack != null)
        {
            callBack();
        }
    }
    public void SetActiveFalseAllCoin()
    {
        for(int i = 0; i < coinFlyingContainer.Length; i++)
        {
            coinFlyingContainer[i].SetActive(false);
        }
    }
}
