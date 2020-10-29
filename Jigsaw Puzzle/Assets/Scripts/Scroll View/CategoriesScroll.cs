using Mopsicus.InfiniteScroll;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoriesScroll : MonoBehaviour
{
    // scroll variable
    [SerializeField]
    private InfiniteScroll infiniteScroll;
    private int height = 390;
    public Texture imgNotLoadSprite;
    public RawImage tempImage;

    // load categories variable
    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    private List<ImageLink> mainAssetLink;

    private int currentIndexLoad;
    private int currentSubIndex;

    Coroutine queueRunning;

    [SerializeField]
    private Sprite imgTick;
    [SerializeField]
    private Sprite imgNotTick;
    private void Awake()
    {
        infiniteScroll.OnHeight += onHeight;
        infiniteScroll.OnFill += onFill;
      //  Davinci.ClearAllCachedFiles();
#if UNITY_EDITOR

#endif
    }
    private void Update()
    {
       // Debug.LogError(coroutineQueue.Count);
    }
    // scroll function
    private void onFill(int index, GameObject item)
    {
        PuzzleItem temp = item.GetComponent<PuzzleItem>();
        temp.currentIndex = index;
        temp.OnFillLink(mainAssetLink[index * 2].link, mainAssetLink[index * 2 + 1].link);
        temp.SetOffActivePercentPartOne();
        temp.SetOffActivePercentPartTwo();
        if (currentIndexLoad != 0 && currentIndexLoad >= index)
        {
            temp.OnFillCategories(imgNotLoadSprite, imgNotLoadSprite);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2] == null)
                {
                    temp.onFillOne(ImageLinkManager.Instance.failedToLoadImage, index * 2);
                }
                else
                {
                    temp.onFillOne(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2], index * 2);
                    if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2].link))
                    {
                        int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2].link));
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
                if (ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2 + 1] == null)
                {
                    temp.onFillTwo(ImageLinkManager.Instance.failedToLoadImage, index * 2 + 1);
                }
                else
                {
                    temp.onFillTwo(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2 + 1], index * 2 + 1);
                    if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2 + 1].link))
                    {
                        int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2 + 1].link));
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
                temp.onFillOne(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2], index * 2);
                if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2].link));
                    if (DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish == 100)
                    {
                        temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgTick);
                    }
                    else
                    {
                        temp.OnFillProfileOne(DataManager.Instance.saveFile.saveSlotList[tempIndex].percentFinish, imgNotTick);
                    }
                }
                temp.onFillTwo(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2 + 1], index * 2 + 1);
                if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2 + 1].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2 + 1].link));
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
            if(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2] != null)
            {
                temp.onFillOne(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2], index * 2);
                if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2].link));
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
                if(Application.internetReachability == NetworkReachability.NotReachable)
                {
                    temp.onFillOne(ImageLinkManager.Instance.failedToLoadImage, index * 2);
                }
                else
                {
                    coroutineQueue.Enqueue(LoadImageDavinci(index * 2, () => { temp.onFillOne(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2], index * 2);
                        if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2].link))
                        {
                            int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2].link));
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
            }

            if (ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2 + 1] != null)
            {
                temp.onFillTwo(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2 + 1], index * 2 + 1);
                if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2 + 1].link))
                {
                    int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2 + 1].link));
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
                    coroutineQueue.Enqueue(LoadImageDavinci(index * 2 + 1, () => { temp.onFillTwo(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index * 2 + 1], index * 2 + 1);
                        if (DataManager.Instance.imageLinkKeys.Contains(mainAssetLink[index * 2 + 1].link))
                        {
                            int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(mainAssetLink[index * 2 + 1].link));
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

    // load image funciton
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
        ImageLinkManager.Instance.GetImageLoad().load(mainAssetLink[index].link).setCached(true).into(tempImage).start();
        float timer = 0;
        while (!ImageLinkManager.Instance.GetImageLoad().success)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index] = tempImage.mainTexture;

        if(timer < 0.001f)
        {
            yield return new WaitForSecondsRealtime(0.08f);
        }
        if (action != null)
        {
            action();
        }
        if(ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index] == ImageLinkManager.Instance.failedToLoadImage)
        {
            ImageLinkManager.Instance.GetMainTexture(currentSubIndex)[index] = null;
        }     
        yield return null;
    }

    // sub categories function
    public void SubCategoriesButton(int _index)
    {      
        currentSubIndex = _index;
        currentIndexLoad = 0;
        ImageLinkManager.Instance.GetImageLoad().success = false;

        coroutineQueue.Clear();
        if(queueRunning == null)
        queueRunning = StartCoroutine(CoroutineCoordinator());
        else
        {
            StopCoroutine(queueRunning);
            queueRunning = StartCoroutine(CoroutineCoordinator());
        }

        switch (_index)
        {
            case 1:
                mainAssetLink = ImageLinkManager.Instance.cityLinkAsset.ImageLink;
                break;
            case 2:
                mainAssetLink = ImageLinkManager.Instance.natureLinkAsset.ImageLink;
                break;
            case 3:
                mainAssetLink = ImageLinkManager.Instance.flowerLinkAsset.ImageLink;
                break;
            case 4:
                mainAssetLink = ImageLinkManager.Instance.foodLinkAsset.ImageLink;
                break;
            case 5:
                mainAssetLink = ImageLinkManager.Instance.animalLinkAsset.ImageLink;
                break;
            case 6:
                mainAssetLink = ImageLinkManager.Instance.macroLinkAsset.ImageLink;
                break;
            case 7:
                mainAssetLink = ImageLinkManager.Instance.artLinkAsset.ImageLink;
                break;
            case 8:
                mainAssetLink = ImageLinkManager.Instance.animeLinkAsset.ImageLink;
                break;
            case 9:
                mainAssetLink = ImageLinkManager.Instance.loveLinkAsset.ImageLink;
                break;
            case 10:
                mainAssetLink = ImageLinkManager.Instance.fantasyLinkAsset.ImageLink;
                break;
            case 11:
                mainAssetLink = ImageLinkManager.Instance.musicLinkAsset.ImageLink;
                break;
            case 12:
                mainAssetLink = ImageLinkManager.Instance.holidayLinkAsset.ImageLink;
                break;
            case 13:
                mainAssetLink = ImageLinkManager.Instance.minimalismLinkAsset.ImageLink;
                break;
            case 14:
                mainAssetLink = ImageLinkManager.Instance.smilesLinkAsset.ImageLink;
                break;
            case 15:
                mainAssetLink = ImageLinkManager.Instance.spaceLinkAsset.ImageLink;
                break;
            case 16:
                mainAssetLink = ImageLinkManager.Instance.sportLinkAsset.ImageLink;
                break;
            case 17:
                mainAssetLink = ImageLinkManager.Instance.wordsLinkAsset.ImageLink;
                break;
            default:
                break;
        }
        infiniteScroll.RecycleAll();
        infiniteScroll.InitData(mainAssetLink.Count / 2);
    } 
}
