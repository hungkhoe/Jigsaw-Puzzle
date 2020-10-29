using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    private Material puzzleMat;     
    [SerializeField]
    private Image lockIconButton;
    [SerializeField]
    private Image arrowImage;
    [SerializeField]
    private Image previewButtonImage;
    [SerializeField]
    private Image backgroundButtonImage;
    [SerializeField]
    private Sprite imageLock;
    [SerializeField]
    private Sprite imageUnlock;
    [SerializeField]
    private Sprite previewButtonOn;
    [SerializeField]
    private Sprite previewButtonOff;
    [SerializeField]
    private Sprite backgroundButtonOn;
    [SerializeField]
    private Sprite backgroundButtonOff;
    [SerializeField]
    private Transform showPreviewPos;
    [SerializeField]
    private Transform extensionTransform;
    [SerializeField]
    private SpriteRenderer previewImage;
    [SerializeField]
    private SpriteRenderer backgroundImage;

    [SerializeField]
    private PuzzleGenerator puzzleManager;
    [SerializeField]
    private GameObject puzzleComplete;
    [SerializeField]
    private TextMeshProUGUI coinText;
    [SerializeField]
    private TextMeshProUGUI coinTextWin;

    [SerializeField]
    private GameObject backgroundScroll;
    [SerializeField]
    private GameObject puzzleScroll;

    [SerializeField]
    private Sprite[] backgroundArray;

    [SerializeField]
    private GameObject coinIcon;
    [SerializeField]
    private GameObject textCoinAmount;
    [SerializeField]
    private GameObject watchIcon;

    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private GameObject coinBankPanel;

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
    private GameObject newGameScrollView;

    [SerializeField]
    private GameObject victoryObject;

    [SerializeField]
    private GameObject rateAppPanel;

    [SerializeField]
    private Image coinWinImage;
    [SerializeField]
    private TextMeshProUGUI textCountWinImage;

    [SerializeField]
    private GameObject[] coinFlyingContainer;
    [SerializeField]
    private Transform coinDestination;

    [SerializeField]
    private Image popUpNotification;
    [SerializeField]
    private Text textNotification;

    private PlayerController playerController;
    private CameraControl cameraControl;

    private int totalScore;
    private int currentScore;

    private bool isOpenExtension;
    private bool isShowingPreview;
    private bool isBackgroundScrollOn;
    private bool isRunningPreviewEffect;
    private bool isRunningExtensionCour;

    public bool isSettingOpen = false;

    private Vector3 defaultCameraPos;
    private float defaultOrthographicSize = 5;
    public bool isVictoryRunning = false;

    public float speedEffect;

    private Coroutine notiCour;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        OnInit();
    }
    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            puzzleManager.SaveFile();
        }
    }

    private void Update()
    {
        if (Puzzle.Game.Ads.AdManager.Instance != null)
            Puzzle.Game.Ads.AdManager.Instance.onShowNativeAd();
    }

    // ========== Get Controller ========== ///
    public PuzzleGenerator GetPuzzleManager()
    {
        return puzzleManager;
    }
    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    // ========= In Game Button Function ======== ///
    public void InGameButton(int _index)
    {

#if UNITY_EDITOR
        switch (_index)
        {
            case 1:
                // lock button          
                LockMoveScreen();
                break;
            case 2:
                // swap back ground button
                OpenCloseBackGroundScroll();
                break;
            case 3:
                // hint skill button
                playerController.HintSkill();
                PopUpNotification("Using Hint");
                break;
            case 4:
                //preview button
                ShowPreviewImage();
                break;
            case 5:
                // home button
                BackToHome();
                break;
            case 6:
                OpenCloseExtension();
                break;
            default:
                break;
        }
#else
  if(Input.touchCount == 1)
        {
            switch (_index)
            {
                case 1:
                    // lock button          
                    LockMoveScreen();
                    break;
                case 2:
                    // swap back ground button
                    OpenCloseBackGroundScroll();
                    break;
                case 3:
                    // hint skill button
                    playerController.HintSkill();
                    PopUpNotification("Using Hint");
                    break;
                case 4:
                    //preview button
                    ShowPreviewImage();
                    break;
                case 5:
                    // home button
                    BackToHome();
                    break;
                case 6:
                    OpenCloseExtension();
                    break;
                default:
                    break;
            }
        }
#endif
    }
    private void BackToHome()
    {
        puzzleManager.SaveFile();
        SceneManager.LoadScene("MainMenu");
        MusicManager.Instance.PlayNewMainMusic(MainMusic.THEME_SONG_1);

        Puzzle.Game.Ads.AdManager.Instance.OnCloseNativeAds();
        Puzzle.Game.Ads.AdManager.Instance.ReloadBottomAd();
    }
    private void ShowPreviewImage()
    {
        if (isRunningPreviewEffect == false)
        {
            isRunningPreviewEffect = true;
            if (isShowingPreview)
            {
                isShowingPreview = false;
                previewButtonImage.sprite = previewButtonOff;
                previewButtonImage.SetNativeSize();
                FadeOutSprite(previewImage);
            }
            else
            {
                isShowingPreview = true;
                previewButtonImage.sprite = previewButtonOn;
                previewButtonImage.SetNativeSize();
                FadeInSprite(previewImage);
                PopUpNotification("Show Preview Image");
            }
        }  
    }
    private void LockMoveScreen()
    {
        if(cameraControl.GetIsLockMoveScreen())
        {
            cameraControl.SetIsLockMoveScreen(false);
            lockIconButton.sprite = imageUnlock;
            PopUpNotification("Unlock Screen");
        }
        else
        {
            cameraControl.SetIsLockMoveScreen(true);
            lockIconButton.sprite = imageLock;
            PopUpNotification("Lock Screen");
        }
    }
    private void OpenCloseExtension()
    {
        // open extension
        if(isRunningExtensionCour == false)
        {
            if (!isOpenExtension)
            {
                isOpenExtension = true;
                StartCoroutine(OpenExtensionCour());
            }
            else
            {
                isOpenExtension = false;
                StartCoroutine(CloseExtensionCour());
            }
            arrowImage.transform.Rotate(0, 180, 0);
        }
    }
    private void OpenCloseBackGroundScroll()
    {
        if(isBackgroundScrollOn)
        {
            backgroundScroll.SetActive(false);
            puzzleScroll.SetActive(true);
            isBackgroundScrollOn = false;
            backgroundButtonImage.sprite = backgroundButtonOff;
        }
        else
        {
            backgroundScroll.SetActive(true);
            puzzleScroll.SetActive(false);
            isBackgroundScrollOn = true;
            backgroundButtonImage.sprite = backgroundButtonOn;
            PopUpNotification("Change Background");
        }
    }
    public void ChangeBackGroundButton(int index)
    {
        PlayerPrefs.SetInt(GameConfig.PREF_BACKGROUND, index);
        backgroundImage.sprite = backgroundArray[index];
    }
    public void MoveInButton()
    {
        puzzleManager.MoveInPuzzle();
#if !UNITY_EDITOR
        if(Input.touchCount == 1)
        {
            puzzleManager.MoveInPuzzle();
        }
#endif
    }
    public void OpenCoinBankButton()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);

        if (coinBankPanel.activeSelf)
        {
            coinBankPanel.SetActive(false);
            isSettingOpen = false;
        }
        else if (!coinBankPanel.activeSelf)
        {
            coinBankPanel.SetActive(true);
            isSettingOpen = true;
        }
    }
    public void SetIsOpenningSetting(bool _isOpenSetting)
    {
        isSettingOpen = _isOpenSetting;
    }
    public bool GetIsOpenExtension()
    {
        return isOpenExtension;
    }
    public bool GetIsOpenBackGroundScroll()
    {
        return isBackgroundScrollOn;
    }
    public bool GetIsShowingPreviewImage()
    {
        return isShowingPreview;
    }
    // ========= On Init Function ========= //
    private void OnInit()
    {
        if (Puzzle.Game.Ads.AdManager.Instance != null)
            Puzzle.Game.Ads.AdManager.Instance.SetColliderPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 4.6f));

        backgroundImage.sprite = backgroundArray[PlayerPrefs.GetInt(GameConfig.PREF_BACKGROUND)];
        playerController = GetComponent<PlayerController>();
        cameraControl = GetComponent<CameraControl>();
        SetTotalScore();
        InitCoinText();

        int random = UnityEngine.Random.Range(1, 4);

        if(MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayNewMainMusic((MainMusic)random);

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
        }

        defaultCameraPos = Camera.main.transform.position;
    }    

    // ========= Ultilities function ========= //
    public void FadeIn(Image _image, float maxValue = 1)
    {
        StartCoroutine(FadeInCour(_image, maxValue));
    }
    public void FadeIn(TextMeshProUGUI _image)
    {
        StartCoroutine(FadeInCour(_image));
    }
    public void FadeIn(Text _image)
    {
        StartCoroutine(FadeInCour(_image));
    }
    IEnumerator FadeInCour(Text _image)
    {
        _image.gameObject.SetActive(true);
        while (_image.color.a < 1)
        {
            float alpha = _image.color.a;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, (alpha + 0.1f));
            yield return null;
        }
    }
    IEnumerator FadeInCour(Image _image, float maxValue = 1)
    {
        _image.gameObject.SetActive(true);
        while (_image.color.a < maxValue)
        {
            float alpha = _image.color.a;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, (alpha + 0.1f));
            yield return null;
        }
    }
    IEnumerator FadeInCour(TextMeshProUGUI _image)
    {
        _image.gameObject.SetActive(true);
        while (_image.color.a < 1)
        {
            float alpha = _image.color.a;
            _image.color = new Color(1, 1, 1, (alpha + 0.1f));
            yield return null;
        }
    }
    public void FadeInSprite(SpriteRenderer _sprite)
    {
        StartCoroutine(FadeInSpriteCour(_sprite));
    }
    IEnumerator FadeInSpriteCour(SpriteRenderer _sprite)
    {        
        while (_sprite.color.a < 1)
        {
            float alpha = _sprite.color.a;
            _sprite.color = new Color(1, 1, 1, (alpha + 0.1f));
            yield return null;
        }
        isRunningPreviewEffect = false;
    }
    public void FadeOut(Image _image)
    {
        StartCoroutine(FadeOutCour(_image));
    }
    public void FadeOut(Text _image)
    {
        StartCoroutine(FadeOutCour(_image));
    }
    IEnumerator FadeOutCour(Image _image)
    {
        while (_image.color.a > 0)
        {
            float alpha = _image.color.a;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, (alpha - 0.1f));
            yield return null;
        }
        _image.gameObject.SetActive(false);
    }
    IEnumerator FadeOutCour(Text _image)
    {
        while (_image.color.a > 0)
        {
            float alpha = _image.color.a;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, (alpha - 0.1f));
            yield return null;
        }
        _image.gameObject.SetActive(false);
    }
    public void FadeOut(TextMeshProUGUI _image)
    {
        StartCoroutine(FadeOutCour(_image));
    }
    IEnumerator FadeOutCour(TextMeshProUGUI _image)
    {
        while (_image.color.a > 0)
        {
            float alpha = _image.color.a;
            _image.color = new Color(1, 1, 1, (alpha - 0.1f));
            yield return null;
        }
        _image.gameObject.SetActive(false);
    }
    public void FadeOutSprite(SpriteRenderer _sprite)
    {
        StartCoroutine(FadeOutCourSprite(_sprite));
    }
    IEnumerator FadeOutCourSprite(SpriteRenderer _sprite)
    {
        while (_sprite.color.a > 0)
        {
            float alpha = _sprite.color.a;
            _sprite.color = new Color(1, 1, 1, (alpha - 0.1f));
            yield return null;
        }
        isRunningPreviewEffect = false;
    }  
    IEnumerator OpenExtensionCour()
    {
        float translateX = extensionTransform.localPosition.x - extensionTransform.GetComponent<RectTransform>().rect.width;
        Vector3 target = new Vector3(translateX, extensionTransform.localPosition.y, extensionTransform.localPosition.z);
        isRunningExtensionCour = true;
        while (extensionTransform.localPosition != target)
        {
            extensionTransform.localPosition = Vector3.MoveTowards(extensionTransform.localPosition, target, 25);
            yield return null;
        }
        isRunningExtensionCour = false;
    }
    IEnumerator CloseExtensionCour()
    {
        float translateX = extensionTransform.localPosition.x + extensionTransform.GetComponent<RectTransform>().rect.width;
        Vector3 target = new Vector3(translateX, extensionTransform.localPosition.y, extensionTransform.localPosition.z);
        isRunningExtensionCour = true;
        while (extensionTransform.localPosition != target)
        {
            extensionTransform.localPosition = Vector3.MoveTowards(extensionTransform.localPosition, target, 25);
            yield return null;
        }
        isRunningExtensionCour = false;
    }  
    public void ShrinkObject(GameObject _obj)
    {
        StartCoroutine(ShrinkObjectCour(_obj));
    }
    IEnumerator ShrinkObjectCour(GameObject _obj)
    {
        while(_obj.transform.localScale.x != 0)
        {
            float scale = _obj.transform.localScale.x;
            scale = Mathf.MoveTowards(scale, 0, 0.05f);
            _obj.transform.localScale = new Vector3(scale ,scale);
            yield return null;
        }
    }
    public void PopUpNotification(string text)
    {
        textNotification.text = text;
        textNotification.color = new Color(textNotification.color.r, textNotification.color.g, textNotification.color.b, 1);
        popUpNotification.color = new Color(popUpNotification.color.r, popUpNotification.color.g, popUpNotification.color.b, 0.5f);
        if (notiCour == null)
            notiCour = StartCoroutine(PopUpNotificationCour());
        else
        {
            StopCoroutine(notiCour);
            notiCour = StartCoroutine(PopUpNotificationCour());
        }
    }
    IEnumerator PopUpNotificationCour()
    {
        FadeIn(textNotification);
        FadeIn(popUpNotification,0.5f);
        yield return new WaitForSecondsRealtime(0.8f);
        FadeOut(textNotification);
        FadeOut(popUpNotification);
        yield return null;
    }

    // ========== Score Function =========== //
    public void IncreaseCurrentScore()
    {
        currentScore++;
        if(currentScore == totalScore)
        {
            int coinPrize = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
            int multiply = 1;
            if(puzzleManager.GetIsPlayingRotatingMode())
            {
                multiply = 2;
            }
            switch (totalScore)
            {
                case 36:                 
                    coinPrize += 10 * multiply;
                    coinTextWin.text = (10 * multiply).ToString();
                    break;
                case 64:
                    coinPrize += 20 * multiply;
                    coinTextWin.text = (20 * multiply).ToString();
                    break;
                case 100:
                    coinPrize += 30 * multiply;
                    coinTextWin.text = (30 * multiply).ToString();
                    break;
                case 121:
                    coinPrize += 40 * multiply;
                    coinTextWin.text = (40 * multiply).ToString();
                    break;
                case 144:
                    coinPrize += 50 * multiply;
                    coinTextWin.text = (50 * multiply).ToString();
                    break;
                case 400:
                    coinPrize += 60 * multiply;
                    coinTextWin.text = (60 * multiply).ToString();
                    break;
                default:
                    break;
            }
            coinText.text = coinPrize.ToString();
            PlayerPrefs.SetInt(GameConfig.PREF_COIN, coinPrize);            
            StartCoroutineWinGame();
        }
    }
    private void SetTotalScore()
    {
        if (PassInformation.Instance != null)
        {
            totalScore = PassInformation.Instance.GetPiecesRow() * PassInformation.Instance.GetPiecesRow();
            previewImage.sprite = Sprite.Create(PassInformation.Instance.GetImagePreview(), new Rect(0, 0, 1280, 1280), new Vector2(0.5f, 0.5f));
        }
        else
        {
            totalScore = 36;
        }
    }
    private void StartCoroutineWinGame()
    {
        StartCoroutine(WinGameCour());
    }
    IEnumerator WinGameCour()
    {
        puzzleManager.SaveFile();
        isVictoryRunning = true;      
        while (Camera.main.transform.position != defaultCameraPos || Camera.main.orthographicSize != 5)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, defaultCameraPos, 0.05f);
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, defaultOrthographicSize, 0.05f);
            yield return null;
        }

        isShowingPreview = true;
        previewButtonImage.sprite = previewButtonOn;
        previewButtonImage.SetNativeSize();
        FadeInSprite(previewImage);

        while (previewImage.color.a < 1)
        {
            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(1);

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayNewSoundEffect(SoundEffect.VICTORY_SOUND);

        victoryObject.SetActive(true);
        puzzleComplete.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        FadeIn(coinWinImage);
        FadeIn(coinTextWin);
        yield return new WaitForSeconds(2);

        FadeOut(coinWinImage);
        FadeOut(coinTextWin);
        ShrinkObject(victoryObject);

        isVictoryRunning = false;
        newGameScrollView.SetActive(true);
    }
    public int GetCurrentScore()
    {
        return currentScore;
    }
    public int GetTotalScore()
    {
        return totalScore;
    }
   
    // ========== Money Function ========== //
    private void InitCoinText()
    {
        int coin = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
        coinText.text = coin.ToString();
        if(coin >= 30)
        {
            watchIcon.SetActive(false);
        }
        else if(coin < 30)
        {
            textCoinAmount.SetActive(false);
            coinIcon.SetActive(false);
        }
    }
    public void DecreaseCoin(int number)
    {
        int coin = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
        coin -= number;
        PlayerPrefs.SetInt(GameConfig.PREF_COIN, coin);
        coinText.text = coin.ToString();

        if (coin >= 30)
        {
            watchIcon.SetActive(false);
            textCoinAmount.SetActive(true);
            coinIcon.SetActive(true);
        }
        else if (coin < 30)
        {
            textCoinAmount.SetActive(false);    
            coinIcon.SetActive(false);
            watchIcon.SetActive(true);
        }
    }
    public void IncreaseCoin(int number)
    {
        int coin = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
        coin += number;
        PlayerPrefs.SetInt(GameConfig.PREF_COIN, coin);
        coinText.text = coin.ToString();
        if (coin >= 30)
        {
            watchIcon.SetActive(false);
            textCoinAmount.SetActive(true);
            coinIcon.SetActive(true);
        }
        else if (coin < 30)
        {
            textCoinAmount.SetActive(false);
            coinIcon.SetActive(false);
            watchIcon.SetActive(true);
        }
    }

    // ============ Setting Function ======== //
    public void SettingButton()
    {
        if(MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
        }

        if (settingPanel.activeSelf)
        {
            isSettingOpen = false;
            settingPanel.SetActive(false);
        }
        else
        {
            isSettingOpen = true;
            settingPanel.SetActive(true);
        }
    }
    public void MusicButton()
    {
        MusicManager.Instance.SetSFX(!MusicManager.Instance.GetIsSFXOn());
        if (MusicManager.Instance.GetIsSFXOn() == false)
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

    // ============= Coin Effect ============= //
    public void CoinEffect(Transform originPos, Action callBack = null)
    {
        StartCoroutine(MoveAllCoinToDestination(originPos, callBack));
    }
    IEnumerator MoveAllCoinToDestination(Transform originPos, Action callBack = null)
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < coinFlyingContainer.Length; i++)
        {
            if (i == 4)
            {
                StartCoroutine(MoveCoinToDestination(coinFlyingContainer[i], originPos, callBack));
            }
            else
                StartCoroutine(MoveCoinToDestination(coinFlyingContainer[i], originPos));
            yield return new WaitForSecondsRealtime(0.07f);
        }
        yield return null;
    }
    IEnumerator MoveCoinToDestination(GameObject _coin, Transform originPos, Action callBack = null)
    {
        _coin.transform.position = new Vector3(originPos.position.x, originPos.position.y, 0);
        _coin.SetActive(true);
        while (_coin.transform.position != coinDestination.position)
        {
            _coin.transform.position = Vector3.MoveTowards(_coin.transform.position, coinDestination.position, speedEffect);
            yield return null;
        }
        _coin.SetActive(false);
        _coin.transform.position = originPos.position;
        if (callBack != null)
        {
            callBack();
        }
    }
    public void SetActiveFalseAllCoin()
    {
        for (int i = 0; i < coinFlyingContainer.Length; i++)
        {
            coinFlyingContainer[i].SetActive(false);
        }
    }
}
