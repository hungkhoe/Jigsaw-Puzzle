using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultSelectionUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private RawImage imagePreview;
    [SerializeField]
    private Slider piecesSlider;
    [SerializeField]
    private Sprite[] puzzlePreviewLine;
    [SerializeField]
    private Image puzzlePreviewLineImage;
    [SerializeField]
    private GameObject enableRotateButton;
    [SerializeField]
    private GameObject disableRotateButton;

    private int piecesMode;
    private bool isRotate = false;
    private bool isFinishedDownload = false;
    public void ChangeImagePreview(RawImage _image)
    {
        imagePreview.texture = _image.texture;        
    }
    public int GetRowNumber()
    {
        piecesMode = (int)piecesSlider.value;
        switch (piecesMode)
        {
            case 1:
                return 6;
            case 2:
                return 8;
            case 3:
                return 10;
            case 4:
                return 11;
            case 5:
                return 12;
            case 6:
                return 20;
            default:
                break;
        }
        return 0;
    }
    public Texture GetImagePreview()
    {
        return imagePreview.texture;
    }    public RawImage GetRawImgPreview()
    {
        return imagePreview;
    }
    public void ChangeImagePuzzlePreviewLine()
    {
        puzzlePreviewLineImage.sprite = puzzlePreviewLine[(int)piecesSlider.value - 1];
    }
    public void RotateButton()
    {
        if(isRotate == false)
        {
            PassInformation.Instance.SetIsRotatingMode(true);
            disableRotateButton.SetActive(false);
            enableRotateButton.SetActive(true);
            isRotate = true;
        }
        else
        {
            PassInformation.Instance.SetIsRotatingMode(false);
            enableRotateButton.SetActive(false);
            disableRotateButton.SetActive(true);
            isRotate = false;
        }
    }
    public void SetIsRotatingMode()
    {
        PassInformation.Instance.SetIsRotatingMode(isRotate);
    }  

}
