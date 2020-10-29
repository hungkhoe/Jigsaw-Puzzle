using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleItem : MonoBehaviour
{
    public RawImage imgPuzzle1;
    public RawImage imgPuzzle2;

    public int currentIndex;

    public string assetLinkOne;
    public string assetLinkTwo;

    public Image percentImage1;
    public Image percentImage2;

    public Text percentText1;
    public Text percentText2;

    public GameObject imgObject1;
    public GameObject imgObject2;

    public int slotSaveIndex1;
    public int slotSaveIndex2;

    public int rowNumber1;
    public int rowNumber2;

    public bool isContinue;
    public bool isLoadImgDone1;
    public bool isLoadImgDone2;
    public bool isFinished;
    private void Start()
    {
        percentImage1.GetComponent<RectTransform>().sizeDelta = new Vector2(percentImage1.GetComponent<RectTransform>().sizeDelta.x * CommonUltilities.GetRescaleRatio()
                    , percentImage1.GetComponent<RectTransform>().sizeDelta.y * CommonUltilities.GetRescaleRatio());

        percentImage2.GetComponent<RectTransform>().sizeDelta = new Vector2(percentImage2.GetComponent<RectTransform>().sizeDelta.x * CommonUltilities.GetRescaleRatio()
                  , percentImage2.GetComponent<RectTransform>().sizeDelta.y * CommonUltilities.GetRescaleRatio());
    }
    public void onFill(Texture sprite1, Texture sprite2)
    {
        imgPuzzle1.texture = sprite1;
        imgPuzzle2.texture = sprite2;
    }
    public void onFillOne(Texture sprite1, int index)
    {
        if(sprite1 != null)
        {
            if (index == (currentIndex * 2))
            {
                imgPuzzle1.texture = sprite1;
                if (imgPuzzle1.texture != ImageLinkManager.Instance.failedToLoadImage)
                {
                    isLoadImgDone1 = true;
                }
            }
        }        
    }
    public void onFillTwo(Texture sprite2, int index)
    {
        if (sprite2 != null)
        {
            if (index == (currentIndex * 2 + 1))
            {
                imgPuzzle2.texture = sprite2;
                if(imgPuzzle2.texture != ImageLinkManager.Instance.failedToLoadImage)
                {
                    isLoadImgDone2 = true;
                }               
            }
        }             
    }
    public void OnFillLink(string _assetLinkOne, string _assetLinkTwo)
    {
        assetLinkOne = _assetLinkOne;
        assetLinkTwo = _assetLinkTwo;
    }
    public void OnFillLinkOne(string _link)
    {
        assetLinkOne = _link;
    }
    public void OnFillLinkTwo(string _link)
    {
        assetLinkTwo = _link;
    }
    public void OnFillProfilePuzzle(int percent1, Texture sprite1, int percent2, Texture sprite2)
    {
        if (sprite1 != null)
        {
            imgPuzzle1.texture = sprite1;
            percentImage1.gameObject.SetActive(true);
            percentText1.text = percent1.ToString();
        }
        if(sprite2 != null)
        {
            imgPuzzle2.texture = sprite2;
            percentImage2.gameObject.SetActive(true);
            percentText2.text = percent2.ToString();
        }
    }
    public void OnFillProfileOne(int percent1, Sprite sprite1)
    {
        if (sprite1 != null)
        {
            percentImage1.sprite = sprite1;
            percentImage1.gameObject.SetActive(true);
            if (percent1 != 100)
            {
                percentText1.text = percent1.ToString() + "%";
                percentText1.gameObject.SetActive(true);
            }                
            else
                percentText1.gameObject.SetActive(false);
        }
    }
    public void OnFillProfileTwo(int percent2, Sprite sprite2)
    {
        if (sprite2 != null)
        {
            percentImage2.sprite = sprite2;
            percentImage2.gameObject.SetActive(true);
            if(percent2 != 100)
            {
                percentText2.text = percent2.ToString() + "%";
                percentText2.gameObject.SetActive(true);
            }
            else
                percentText2.gameObject.SetActive(false);
        }
    }
    public void OpenPuzzleDifficultSelection(int index)
    {
        // pass index từ nút, mỗi nút pass 1 index riêng
        if(isContinue == false)
        {       
            if (index == 0)
            {
                if(isLoadImgDone1)
                {
                    MainMenuUI.Instance.GetDifficultSelectionManager().ChangeImagePreview(imgPuzzle1);
                    PassInformation.Instance.SetContentLink(assetLinkOne);
                    if (DataManager.Instance.imageLinkKeys.Contains(assetLinkOne))
                    {
                        int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(assetLinkOne));
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
            else
            {
                if(isLoadImgDone2)
                {
                    MainMenuUI.Instance.GetDifficultSelectionManager().ChangeImagePreview(imgPuzzle2);
                    PassInformation.Instance.SetContentLink(assetLinkTwo);

                    if (DataManager.Instance.imageLinkKeys.Contains(assetLinkTwo))
                    {
                        int tempIndex = DataManager.Instance.imageLinkKeys.FindIndex(s => s.Equals(assetLinkTwo));
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
        }
        else
        {
            if(index == 0)
            {
                if(isLoadImgDone1)
                {
                    MainMenuUI.Instance.GetDifficultSelectionManager().ChangeImagePreview(imgPuzzle1);
                    PassInformation.Instance.SetSlotNumber(slotSaveIndex1);
                    PassInformation.Instance.SetContentLink(assetLinkOne);
                    PassInformation.Instance.SetRowNumber(rowNumber1);
                    PassInformation.Instance.SetIsRotatingMode(DataManager.Instance.saveFile.saveSlotList[slotSaveIndex1].isRotating);
                    if (DataManager.Instance.saveFile.saveSlotList[slotSaveIndex1].percentFinish == 100)
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
                MainMenuUI.Instance.DownloadTexture();
            }
            else
            {
                if(isLoadImgDone2)
                {
                    MainMenuUI.Instance.GetDifficultSelectionManager().ChangeImagePreview(imgPuzzle2);
                    PassInformation.Instance.SetSlotNumber(slotSaveIndex2);
                    PassInformation.Instance.SetContentLink(assetLinkTwo);
                    PassInformation.Instance.SetRowNumber(rowNumber2);
                    PassInformation.Instance.SetIsRotatingMode(DataManager.Instance.saveFile.saveSlotList[slotSaveIndex2].isRotating);
                    if (DataManager.Instance.saveFile.saveSlotList[slotSaveIndex2].percentFinish == 100)
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
                MainMenuUI.Instance.DownloadTexture();
            }
          
        }
        
    }
    public void OnFillCategories(Texture sprite1, Texture sprite2)
    {
        if(sprite1 != null)
        imgPuzzle1.texture = sprite1;
        if(sprite2 !=null)
        imgPuzzle2.texture = sprite2;

        isLoadImgDone1 = false;
        isLoadImgDone2 = false;
    }
    public void SetActiveImg(bool active, int index)
    {
        if(index == 0)
        {
            imgObject1.SetActive(active);
        }
        else
        {
            imgObject2.SetActive(active);
        }
    }
    public void SetOffActivePercentPartOne()
    {
        percentText1.gameObject.SetActive(false);
        percentImage1.gameObject.SetActive(false);
    }
    public void SetOffActivePercentPartTwo()
    {
        percentText2.gameObject.SetActive(false);
        percentImage2.gameObject.SetActive(false);
    }
}
