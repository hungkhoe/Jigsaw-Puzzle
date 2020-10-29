using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinuePanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject continueButton;

    private bool isLoadingNewTextureDone;
    
    void Start()
    {
        
    }

    public void ContinueButton()
    {
        Puzzle.Game.Ads.AdManager.Instance.ShowInterstitial();
        MainMenuUI.Instance.DownloadTexture();
        StartCoroutine(LoadYourAsyncScene("Gameplay"));
    }

    private IEnumerator LoadYourAsyncScene(string _sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        while(MainMenuUI.Instance.GetDifficultSelectionManager().GetRawImgPreview().texture.width != 1280)
        {
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void NewGameButton()
    {
        MainMenuUI.Instance.OpenClosePuzzleDifficultSelection();    
        PassInformation.Instance.SetIsNewGame(true);
        MainMenuUI.Instance.GetDifficultSelectionManager().SetIsRotatingMode();
    }

    public void OpenContinuePanelFinished()
    {
        continueButton.SetActive(false);
    }
}
