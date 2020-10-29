using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DailyPuzzle : MonoBehaviour
{
    // Start is called before the first frame update

    private ImageStruct thisMonthLinkAsset;
    private Texture[] thisMonthTextureContainer;
    [SerializeField]
    private GameObject[] thisMonthGameObject;
    [SerializeField]
    private RawImage [] thisMonthGameObjectRawImage;
    [SerializeField]
    private Text [] percentText;
    [SerializeField]
    private UnityEngine.UI.Text dateText;
    [SerializeField]
    private GameObject contentScroll;

    private bool [] isLoadingDoneContainer;

    private int daysCount;
    private float childHeight;

    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    Coroutine queueRunning;

    private bool isFirstTime = true;

    private Color defaultColor;

    void Start()
    {
        OnInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnInit()
    {
        coroutineQueue.Clear();

        if (MainMenuUI.Instance.dayCheck())
        {
            ImageLinkManager.Instance.LoadMonthData();
        }

        DateTime newDateTemp = System.DateTime.Now;

        // set text date
        string monthName = newDateTemp.ToString("MMMM");
        dateText.text = monthName.ToUpper() + ", " + newDateTemp.Year;

        // init slot
        daysCount = newDateTemp.Day;

        for (int i = 0; i < thisMonthGameObject.Length; i++)
        {
            thisMonthGameObject[i].SetActive(false);
        }   

        for (int i = 0; i < daysCount; i++)
        {
            thisMonthGameObject[i].SetActive(true);
        }

        ScaleItem();

        int totalRow = 0;

        if(daysCount % 3 == 0)
        {
            totalRow = daysCount / 3;
        }
        else
        {
            totalRow = (int)Math.Round((double)(daysCount / 3)) + 1;
        }

        isLoadingDoneContainer = new bool[daysCount];

        contentScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(contentScroll.GetComponent<RectTransform>().sizeDelta.x, childHeight * totalRow + (childHeight / 1.6f));

        queueRunning = StartCoroutine(CoroutineCoordinator());

        defaultColor = percentText[0].color;

        InitDailyItem();
    }
    private void ScaleItem()
    {
        float initYPos = thisMonthGameObject[0].transform.localPosition.y;
        int index = 0;
        int count = 0;
        for (int i = 0; i < thisMonthGameObject.Length; i++)
        {
            if (count == 3)
            {
                count = 0;
                index++;
            }

            RectTransform child = thisMonthGameObject[i].transform.GetComponent<RectTransform>();

            child.sizeDelta = new Vector2(child.rect.width * CommonUltilities.GetRescaleRatio(), child.rect.height * CommonUltilities.GetRescaleRatio());
            child.transform.localPosition = new Vector2(child.transform.localPosition.x, initYPos - (index * GameConfig.OFFSET_DAILY_Y * CommonUltilities.GetRescaleRatio()));
            childHeight = child.rect.height;

            for (int j = 0; j < child.childCount; j++)
            {
                child.GetChild(j).GetComponent<RectTransform>().sizeDelta = new Vector2(child.GetChild(j).GetComponent<RectTransform>().rect.width * CommonUltilities.GetRescaleRatio()
                    , child.GetChild(j).GetComponent<RectTransform>().rect.height * CommonUltilities.GetRescaleRatio());                
            }

            float fontSize = child.GetChild(0).GetComponent<UnityEngine.UI.Text>().fontSize * CommonUltilities.GetRescaleRatio();
            child.GetChild(0).GetComponent<UnityEngine.UI.Text>().fontSize = (int)fontSize;

            count++;
        }
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
    private IEnumerator LoadImageDavinci(int index, System.Action action)
    {
        ImageLinkManager.Instance.GetImageLoad().success = false;
        ImageLinkManager.Instance.GetImageLoad().load(
            ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[index].link).setCached(true).into(thisMonthGameObjectRawImage[index]).start();

        float timer = 0;
        while (!ImageLinkManager.Instance.GetImageLoad().success)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        ImageLinkManager.Instance.thisMonthTextureContainer[index] = thisMonthGameObjectRawImage[index].mainTexture;

        if (timer < 0.001f)
        {
            yield return new WaitForSecondsRealtime(0.10f);
        }

        if (action != null)
        {
            action();
        }

        isLoadingDoneContainer[index] = true;

        bool isSaved = false;
        if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[index].link))
        {
            isSaved = true;
        }
        if (isSaved)
        {
            int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[index].link));
            percentText[index].gameObject.SetActive(true);
            percentText[index].text = DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish.ToString() + "%";
            if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
            {
                percentText[index].color = new Color(0, 1, 0, 1);
            }
            else
            {
                percentText[index].color = defaultColor;
            }
        }

        if (ImageLinkManager.Instance.thisMonthTextureContainer[index] == ImageLinkManager.Instance.failedToLoadImage)
        {
            ImageLinkManager.Instance.thisMonthTextureContainer[index] = null;
        }
        yield return null;
    }
    private void InitDailyItem()
    {
        for (int i = 0; i < daysCount; i++)
        {
            percentText[i].gameObject.SetActive(false);
            bool isSaved = false;
            if(DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[i].link))
            {
                isSaved = true;
            }
            if (ImageLinkManager.Instance.thisMonthTextureContainer[i] == null)
            {
                coroutineQueue.Enqueue(LoadImageDavinci(i, () => { }));
            }
            else
            {
                thisMonthGameObjectRawImage[i].texture = ImageLinkManager.Instance.thisMonthTextureContainer[i];
                isLoadingDoneContainer[i] = true;
                if (isSaved)
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[i].link));
                    percentText[i].gameObject.SetActive(true);
                    percentText[i].text = DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish.ToString() + "%";
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        percentText[i].color = new Color(0, 1, 0, 1);
                    }
                    else
                    {
                        percentText[i].color = defaultColor;
                    }
                }
            }
        }
    }
    public void ResetCoroutine()
    {
        if(isFirstTime)
        {
            isFirstTime = false;
            return;
        }

        coroutineQueue.Clear();
        if (queueRunning == null)
            queueRunning = StartCoroutine(CoroutineCoordinator());
        else
        {
            StopCoroutine(queueRunning);
            queueRunning = StartCoroutine(CoroutineCoordinator());
        }

        InitDailyItem();
    }
    public void DailyButton(int index)
    {
        if (isLoadingDoneContainer[index] == true)
        {
            MainMenuUI.Instance.GetDifficultSelectionManager().ChangeImagePreview(thisMonthGameObjectRawImage[index]);
            PassInformation.Instance.SetContentLink(ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[index].link);
            if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[index].link))
            {
                int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.thisMonthLinkAsset.ImageLink[index].link));
                PassInformation.Instance.SetSlotNumber(tempIndex);
                PassInformation.Instance.SetRowNumber(DataManager.Instance.saveFile.saveSlotList[tempIndex].rowNumber);
                PassInformation.Instance.SetIsRotatingMode(DataManager.Instance.saveFile.saveSlotList[tempIndex].isRotating);
                if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                {
                    PassInformation.Instance.SetIsCompletedMode(false);
                    PassInformation.Instance.SetIsNewGame(true);
                    MainMenuUI.Instance.OpenClosePuzzleDifficultSelection();
                }
                else
                {
                    PassInformation.Instance.SetIsCompletedMode(true);
                    MainMenuUI.Instance.OpenContinueProgress();
                }
            }
            else
            {
                MainMenuUI.Instance.OpenClosePuzzleDifficultSelection();
            }
            MainMenuUI.Instance.DownloadTexture();
        }
    }
    public void ReInitDaily()
    {
        DateTime newDateTemp = System.DateTime.Now;

        // set text date
        string monthName = newDateTemp.ToString("MMMM");
        dateText.text = monthName.ToUpper() + ", " + newDateTemp.Year;

        // init slot
        daysCount = newDateTemp.Day;

        for (int i = 0; i < thisMonthGameObject.Length; i++)
        {
            thisMonthGameObject[i].SetActive(false);
        }

        for (int i = 0; i < daysCount; i++)
        {
            thisMonthGameObject[i].SetActive(true);
        }

        int totalRow = 0;

        if (daysCount % 3 == 0)
        {
            totalRow = daysCount / 3;
        }
        else
        {
            totalRow = (int)Math.Round((double)(daysCount / 3)) + 1;
        }

        isLoadingDoneContainer = new bool[daysCount];

        contentScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(contentScroll.GetComponent<RectTransform>().sizeDelta.x, childHeight * totalRow + (childHeight / 1.6f));

        InitDailyItem();
    }
}
