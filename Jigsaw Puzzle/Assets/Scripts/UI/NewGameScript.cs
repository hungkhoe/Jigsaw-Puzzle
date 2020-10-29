using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    RawImage[] newGameRawImage;

    private bool[] isLoadingTextureDone;
    private string[] contentLink;
    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    private Coroutine queueRunning;
    private Coroutine queueDownload;

    [SerializeField]
    private DifficultSelectionUI difficultSelectionScript;
    [SerializeField]
    private GameObject difficultSelectionPanel;
    [SerializeField]
    private GameObject loadingProgress;
    [SerializeField]
    private GameObject startImage;

    private bool isLoadingTexture;
    private bool isLoadingProgress;
    void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        isLoadingTextureDone = new bool[4];
        contentLink = new string[4];

        int random = 0;
        bool isRepeat = true;

        while(isRepeat)
        {
            random = Random.Range(0, 1100);
            string link = ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[random].link;
            if(!DataManager.Instance.imageLinkKeys.Contains(link))
            {
                contentLink[0] = link;
                isRepeat = false;
            }
        }

        isRepeat = true;
        while (isRepeat)
        {
            random = Random.Range(0, 1100);
            string link = ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[random].link;
            if (!DataManager.Instance.imageLinkKeys.Contains(link) && link != contentLink[0])
            {
                contentLink[1] = link;
                isRepeat = false;
            }
        }

        isRepeat = true;
        while (isRepeat)
        {
            random = Random.Range(0, 1100);
            string link = ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[random].link;
            if (!DataManager.Instance.imageLinkKeys.Contains(link) && link != contentLink[0] && link != contentLink[1])
            {
                contentLink[2] = link;
                isRepeat = false;
            }
        }

        isRepeat = true;
        while (isRepeat)
        {
            random = Random.Range(0, 1100);
            string link = ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[random].link;
            if (!DataManager.Instance.imageLinkKeys.Contains(link) && link != contentLink[0] && link != contentLink[1] && link != contentLink[2])
            {
                contentLink[3] = link;
                isRepeat = false;
            }
        }

        queueRunning = StartCoroutine(CoroutineCoordinator());

        for (int i = 0; i < 4; i++)
        {
            coroutineQueue.Enqueue(LoadImageDavinci(i, () => { }));
        }

        GameObject newGameObject = new GameObject();
        newGameObject.AddComponent<PassInformation>();
        PassInformation.Instance.SetDifficultSelection(difficultSelectionScript);
    }
    private IEnumerator LoadImageDavinci(int index, System.Action action)
    {
        ImageLinkManager.Instance.GetImageLoad().success = false;
        ImageLinkManager.Instance.GetImageLoad().load(contentLink[index]).setCached(true).into(newGameRawImage[index]).start();

        float timer = 0;
        while (!ImageLinkManager.Instance.GetImageLoad().success)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (timer < 0.001f)
        {
            yield return new WaitForSecondsRealtime(0.10f);
        }
        if (action != null)
        {
            action();
        }
        isLoadingTextureDone[index] = true;
        yield return null;
    }
    private IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
            {
                yield return StartCoroutine(coroutineQueue.Dequeue());
            }
            yield return null;
        }
    }
    public void OpenNewGame(int index)
    {
        if(isLoadingTextureDone[index])
        {
            OpenCloseDiffcultSelection(index);
        }
    }

    public void OpenCloseDiffcultSelection(int index)
    {
        if(isLoadingProgress == false)
        {
            MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
            if (difficultSelectionPanel.activeSelf)
            {
                difficultSelectionPanel.SetActive(false);
                PassInformation.Instance.SetIsNewGame(false);
                PassInformation.Instance.ResetSlotNumber();
                isLoadingTexture = false;
                GameManager.Instance.SetIsOpenningSetting(false);
            }
            else
            {
                difficultSelectionPanel.SetActive(true);
                difficultSelectionScript.ChangeImagePreview(newGameRawImage[index]);
                PassInformation.Instance.SetContentLink(contentLink[index]);
                DownloadTexture();
                GameManager.Instance.SetIsOpenningSetting(true);
            }
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
        isLoadingTexture = false;

        string contentLink = PassInformation.Instance.GetContentLink();
        string newString = contentLink.Replace("300x300", "1280x1280");

        ImageLinkManager.Instance.GetImageLoad().success = false;
        ImageLinkManager.Instance.GetImageLoad().load(newString).setCached(true).into(difficultSelectionScript.GetRawImgPreview()).start();

        while (!ImageLinkManager.Instance.GetImageLoad().success)
        {
            yield return null;
        }

        PassInformation.Instance.SetImgPreview();

        yield return new WaitForSeconds(0.1f);

        
        isLoadingTexture = true;

        queueDownload = null;
        yield return null;
    }
    public void StartButton()
    {
        if (isLoadingProgress == false)
        {
            PassInformation.Instance.SetRpwNumberIngame();
            Puzzle.Game.Ads.AdManager.Instance.ShowInterstitial();
            isLoadingProgress = true;
            loadingProgress.SetActive(true);
            startImage.SetActive(false);
            DownloadTexture();
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
            if (timer >= 6f)
            {
                isLoadingProgress = false;
                loadingProgress.SetActive(false);
                startImage.SetActive(true);
                CommonUtils.ShowShortToast("Failed to load. Retry");
                yield break;
            }
            else if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                CommonUtils.ShowShortToast("No internet connection!");
                isLoadingProgress = false;
                loadingProgress.SetActive(false);
                startImage.SetActive(true);
                yield break;
            }
            yield return null;
        }

        if (PassInformation.Instance.GetImagePreview() != ImageLinkManager.Instance.failedToLoadImage)
        {
            PassInformation.Instance.SetImgPreview();

            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_sceneName);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
