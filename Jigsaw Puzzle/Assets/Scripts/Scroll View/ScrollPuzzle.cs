using Mopsicus.InfiniteScroll;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPuzzle : MonoBehaviour
{
    public InfiniteScroll infiniteScroll;

    [SerializeField]
    private RawImage tempImage;

    public Texture imgNotLoadSprite;

    private int height = 390;

    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();

    private int currentIndexLoad;

    Coroutine queueRunning;

    [SerializeField]
    private Sprite imgTick;
    [SerializeField]
    private Sprite imgNotTick;
    // Start is called before the first frame update

    private void Awake()
    {
        infiniteScroll.OnHeight += onHeight;
        infiniteScroll.OnFill += onFill;
    }
    private void onFill(int index, GameObject item)
    {
        PuzzleItem temp = item.GetComponent<PuzzleItem>();
        temp.currentIndex = index;
        temp.OnFillLink(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link, ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link);
        temp.SetOffActivePercentPartOne();
        temp.SetOffActivePercentPartTwo();
        if (currentIndexLoad != 0 && currentIndexLoad >= index)
        {
            temp.OnFillCategories(imgNotLoadSprite, imgNotLoadSprite);
            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                if(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2] == null)
                {
                    temp.onFillOne(ImageLinkManager.Instance.failedToLoadImage, index * 2);
                }
                else
                {
                    temp.onFillOne(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2], index * 2);
                    if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link))
                    {
                        int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link));
                        if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                        {
                            temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                        }
                        else
                        {
                            temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                        }
                    }
                }
                if (ImageLinkManager.Instance.mainmenuTextureContainer[index * 2 + 1] == null)
                {
                    temp.onFillTwo(ImageLinkManager.Instance.failedToLoadImage, index * 2 + 1);
                }
                else
                {
                    temp.onFillTwo(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2 + 1], index * 2 + 1);
                    temp.onFillOne(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2], index * 2);
                    if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link))
                    {
                        int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link));
                        if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                        {
                            temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                        }
                        else
                        {
                            temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                        }
                    }
                }
            }
            else
            {
                temp.onFillOne(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2], index * 2);
                if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link));
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                    }
                    else
                    {
                        temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                    }
                }
                temp.onFillTwo(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2 + 1], index * 2 + 1);
                if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link));
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                    }
                    else
                    {
                        temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                    }
                }
            }           
        }
        else
        {
            temp.OnFillCategories(imgNotLoadSprite, imgNotLoadSprite);
            currentIndexLoad = index;
            if (ImageLinkManager.Instance.mainmenuTextureContainer[index * 2] != null)
            {
                temp.onFillOne(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2], index * 2);
                if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link));
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                    }
                    else
                    {
                        temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                    }
                }
            }
            else
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    temp.onFillOne(ImageLinkManager.Instance.failedToLoadImage, index * 2);
                }
                else
                coroutineQueue.Enqueue(LoadImageDavinci(index * 2, () => { temp.onFillOne(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2], index * 2);
                    if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link))
                    {
                        int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2].link));
                        if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                        {
                            temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                        }
                        else
                        {
                            temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                        }
                    }
                }));
            }

            if (ImageLinkManager.Instance.mainmenuTextureContainer[index * 2 + 1] != null)
            {
                temp.onFillTwo(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2 + 1], index * 2 + 1);
                if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link));
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                    }
                    else
                    {
                        temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                    }
                }
            }
            else
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    temp.onFillTwo(ImageLinkManager.Instance.failedToLoadImage, index * 2 + 1);
                }
                else
                {
                    coroutineQueue.Enqueue(LoadImageDavinci(index * 2 + 1, () => { temp.onFillTwo(ImageLinkManager.Instance.mainmenuTextureContainer[index * 2 + 1], index * 2 + 1);
                        if (DataManager.Instance.imageLinkKeys.Contains(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link))
                        {
                            int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index * 2 + 1].link));
                            if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                            {
                                temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                            }
                            else
                            {
                                temp.OnFillProfileTwo(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                            }
                        }
                    }));
                }
            }               
        }
    }
    private int onHeight(int index)
    { 
        return (int)(height * CommonUltilities.GetRescaleRatio());
    }
    void Start()
    {
        infiniteScroll.RecycleAll();
        infiniteScroll.InitData(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink.Count / 2);
        ClearQueue();    
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
        ImageLinkManager.Instance.GetImageLoad().load(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink[index].link).setCached(true).into(tempImage).start();
        float timer = 0;
        while (!ImageLinkManager.Instance.GetImageLoad().success)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        ImageLinkManager.Instance.mainmenuTextureContainer[index] = tempImage.mainTexture;

        if (timer < 0.001f)
        {
            yield return new WaitForSecondsRealtime(0.08f);
        }
        if (action != null)
        {
            action();
        }
        if (ImageLinkManager.Instance.mainmenuTextureContainer[index] == ImageLinkManager.Instance.failedToLoadImage)
        {
            ImageLinkManager.Instance.mainmenuTextureContainer[index] = null;
        }
        yield return null;
    }
    public void ClearQueue()
    {
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
        infiniteScroll.RecycleAll();
        infiniteScroll.InitData(ImageLinkManager.Instance.mainmenuLinkAsset.ImageLink.Count / 2);       
    }
}
