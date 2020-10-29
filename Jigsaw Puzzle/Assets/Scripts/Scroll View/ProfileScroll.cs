using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mopsicus.InfiniteScroll;
using UnityEngine.UI;

public class ProfileScroll : MonoBehaviour
{
    [SerializeField]
    private InfiniteScroll infiniteScroll;
    [SerializeField]
    private RawImage tempImage;
    [SerializeField]
    private Text collectionText;
    [SerializeField]
    private Text completedText;
    [SerializeField]
    private Text inprogressText;

    private int currentIndexLoad;
    private int height = 390;

    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    Coroutine queueRunning;

    public Texture imgNotLoadSprite;
    public Sprite imgNotTick;
    public Sprite imgTick;
    private Sprite imgFill;

    private Texture[] inCompletedTexture;
    private Texture[] completedTexture;

    private bool isIncompletedMode = true;

    Color brownColor;
    

    // Start is called before the first frame update
    private void Awake()
    {
        infiniteScroll.OnHeight += onHeight;
        infiniteScroll.OnFill += OnFill;
        inCompletedTexture = new Texture[DataManager.Instance.inCompletedLink.Count];
        completedTexture = new Texture[DataManager.Instance.completedLink.Count];
    }
    private void Start()
    {     
        SetDataScroll(true);
        PassInformation.Instance.SetIsCompletedMode(true);
        brownColor = inprogressText.color;
    }
    private void OnFill(int index, GameObject item)
    {
        PuzzleItem puzzleItem = item.GetComponent<PuzzleItem>();
        if(isIncompletedMode)
        {
            puzzleItem.isFinished = false;
        }
        else
        {
            puzzleItem.isFinished = true;
        }
        puzzleItem.currentIndex = index;
        puzzleItem.isContinue = true;
        if(index * 2 + 1 < GetSaveSlot().Count)
        {
            puzzleItem.OnFillLinkTwo(GetSaveSlot()[index * 2 + 1].imageLink);
            puzzleItem.slotSaveIndex2 = GetSaveSlot()[index * 2 + 1].slotIndex;
            puzzleItem.SetActiveImg(true, 1);
            puzzleItem.rowNumber2 = GetSaveSlot()[index * 2 + 1].rowNumber;
        }
        else
        {
            puzzleItem.SetActiveImg(false, 1);
        }
        puzzleItem.OnFillLinkOne(GetSaveSlot()[index * 2].imageLink);
        puzzleItem.slotSaveIndex1 = GetSaveSlot()[index * 2].slotIndex;
        puzzleItem.rowNumber1 = GetSaveSlot()[index * 2 ].rowNumber;

        if (currentIndexLoad != 0 && currentIndexLoad >= index)
        {
            puzzleItem.OnFillCategories(imgNotLoadSprite, imgNotLoadSprite);
            if (index * 2 + 1 < GetSaveSlot().Count)
            {
                OnFillProfileImg(puzzleItem, index * 2 + 1,false);
            }
            OnFillProfileImg(puzzleItem, index * 2,true);
        }
        else
        {
            puzzleItem.OnFillCategories(imgNotLoadSprite, imgNotLoadSprite);
            currentIndexLoad = index;

            if (GetImageLink()[index * 2] != null)
            {
                OnFillProfileImg(puzzleItem, index * 2,true);
            }
            else
            {
                coroutineQueue.Enqueue(LoadImageDavinci(index * 2, () => { OnFillProfileImg(puzzleItem, index * 2,true);}));
            }

            if (index * 2 + 1 < GetSaveSlot().Count)
            {
                if (GetImageLink()[index * 2 + 1] != null)
                {
                    OnFillProfileImg(puzzleItem, index * 2 + 1,false);
                }
                else
                    coroutineQueue.Enqueue(LoadImageDavinci(index * 2 + 1, () => { OnFillProfileImg(puzzleItem, index * 2 + 1,false);}));
            }
        }
    }
    private void OnFillProfileImg(PuzzleItem item, int index, bool isOne)
    {
        if (isOne)
        {
            item.onFillOne(GetImageLink()[index], index);
        }
        else
        {
            item.onFillTwo(GetImageLink()[index], index);
        }

        if (isOne)
            item.OnFillProfileOne(GetSaveSlot()[index].percentFinish, imgFill);
        else
        {
            item.OnFillProfileTwo(GetSaveSlot()[index].percentFinish, imgFill);
        }

    }
    private int onHeight(int index)
    {
        return (int)(height * CommonUltilities.GetRescaleRatio());
    }
    public void SetDataScroll(bool _isCompleted)
    {
        isIncompletedMode = _isCompleted;
        infiniteScroll.RecycleAll();
        if (GetSaveSlot().Count > 0)
        {
            if (isIncompletedMode)
            {
                imgFill = imgNotTick;
            }
            else
            {
                imgFill = imgTick;
            }
            currentIndexLoad = 0;

            ImageLinkManager.Instance.GetImageLoad().success = false;

            coroutineQueue.Clear();
            if (queueRunning == null)
                queueRunning = StartCoroutine(CoroutineCoordinator());
            else
            {
                StopCoroutine(queueRunning);
                queueRunning = StartCoroutine(CoroutineCoordinator());
            }      
            int count = 0;
            count = GetSaveSlot().Count;
            if (count % 2 == 0)
            {
                infiniteScroll.InitData(count / 2);
            }
            else
            {
                if (count == 1)
                    infiniteScroll.InitData(count);
                else
                {
                    infiniteScroll.InitData(count / 2 + 1);
                }
            }          
        }      
    }
    IEnumerator CoroutineCoordinator()
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
    IEnumerator LoadImageDavinci(int index, System.Action action)
    {
        ImageLinkManager.Instance.GetImageLoad().success = false;
        ImageLinkManager.Instance.GetImageLoad().load(GetSaveSlot()[index].imageLink).setCached(true).into(tempImage).start();
        
        while (!ImageLinkManager.Instance.GetImageLoad().success)
        {
            yield return null;
        }      

        GetImageLink()[index] = tempImage.mainTexture;

        if (action != null)
        {   
            action();
        }
        yield return null;
    }
    public void ResetDataScroll()
    {
        SetDataScroll(isIncompletedMode);
    }
    private Texture[] GetImageLink()
    {
        if (isIncompletedMode)
        {
            return inCompletedTexture;
        }
        else
        {
            return completedTexture;
        }
    }
    private List<SaveSlot> GetSaveSlot()
    {
        if (isIncompletedMode)
        {
            return DataManager.Instance.inCompletedLink;
        }
        else
        {
            return DataManager.Instance.completedLink;
        }
    }
    // button function
    public void InProgressButton()
    {
        if(isIncompletedMode == false)
        {
            PassInformation.Instance.SetIsCompletedMode(true);
            SetDataScroll(true);
            isIncompletedMode = true;
            inprogressText.color = brownColor;
            completedText.color = new Color(brownColor.r, brownColor.b, brownColor.g, 0.5f);
            collectionText.color = new Color(brownColor.r, brownColor.b, brownColor.g, 0.5f);
        }
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
    }
    public void CompletedButton()
    {
        if(isIncompletedMode)
        {
            PassInformation.Instance.SetIsCompletedMode(false);
            SetDataScroll(false);
            isIncompletedMode = false;
            completedText.color = brownColor;
            inprogressText.color = new Color(brownColor.r, brownColor.b, brownColor.g, 0.5f);
            collectionText.color = new Color(brownColor.r, brownColor.b, brownColor.g, 0.5f);
        }
        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.POP_UP);
    }
    public void CollectionButton()
    {

    }
}
