using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassInformation : MonoBehaviour
{
    static PassInformation instance;
    public static PassInformation Instance
    {
        get
        {
            return instance;
        }
    }
    // Start is called before the first frame update
    [SerializeField]
    DifficultSelectionUI difficultSelection;

    private Texture gameplayImage;
    private int rowNumber;

    [SerializeField]
    private string contentLink;

    private int slotNumber;

    private bool isLoad = false;
    private bool isIncompletedMode;
    private bool isNewGame = false;
    private bool isRotatingGameMode = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void SetRowNumberImage()
    {
        rowNumber = MainMenuUI.Instance.GetDifficultSelectionManager().GetRowNumber();
    }
    public void SetRpwNumberIngame()
    {
        rowNumber = difficultSelection.GetRowNumber();
    }
    public void SetRowNumber(int number)
    {
        rowNumber = number;
        gameplayImage = MainMenuUI.Instance.GetDifficultSelectionManager().GetImagePreview();
    }
    public void SetImgPreview()
    {
        if (MainMenuUI.Instance != null)
        {
            gameplayImage = MainMenuUI.Instance.GetDifficultSelectionManager().GetImagePreview();
        }
        else
        {
            gameplayImage = difficultSelection.GetImagePreview();
        }
       
    }
    public Texture2D GetImagePreview()
    {
        return (Texture2D)gameplayImage;
    }
    public int GetPiecesRow()
    {
        Destroy(gameObject);
        return rowNumber;
    }
    public void SetContentLink(string _contentLink)
    {
        contentLink = _contentLink;
    }
    public string GetContentLink()
    {
        return contentLink;
    }
    public void SetSlotNumber(int index)
    {
        slotNumber = index;
        isLoad = true;
    }
    public void ResetSlotNumber()
    {
        slotNumber = -1;
        isLoad = false;
    }
    public bool GetIsLoad()
    {
        return isLoad;
    }
    public int GetSlotNumber()
    {
        if(isLoad)
        {
            return slotNumber;
        }
        else
        {
            return -1;
        }
    }
    public void SetIsCompletedMode(bool isIncompleted)
    {
        isIncompletedMode = isIncompleted;
    }
    public bool GetIsInCompletedMode()
    {
        return isIncompletedMode;
    }
    public void SetIsNewGame(bool _isNewGame)
    {
        isNewGame = _isNewGame;
    }
    public bool GetIsNewGame()
    {
        return isNewGame;
    }
    public void SetIsRotatingMode(bool _isRotatingMode)
    {
        isRotatingGameMode = _isRotatingMode;
    }
    public bool GetIsRotatingMode()
    {
        return isRotatingGameMode;
    }
    public void SetDifficultSelection(DifficultSelectionUI _script)
    {
        difficultSelection = _script;
    }
}
