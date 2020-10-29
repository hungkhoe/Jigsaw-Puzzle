using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;
using Puzzle.Game.Ads;

public class PlayerController : MonoBehaviour, Admob.IAdInterface
{
    // Start is called before the first frame update
    [SerializeField]
    ScrollRect puzzleScroll;
    [SerializeField]
    Transform boardPuzzle;
    [SerializeField]
    Transform puzzleScrollContent;

    private GameObject currClickPuzzle;
    private Transform currClickPuzzleTransform;

    private float currentMousePositionX;

    private bool isDraggingScroll;
    private bool isSwappingPuzzle;
    private bool isFirstTimeEnter;
    private bool isMouseOnScroll;

    private float moveInDistance = 0;
    private bool isReward = false;

    List<GameObject> randomList;
    private void Start()
    {
        randomList = new List<GameObject>();
    }
    private void Update()
    {       
        if (isSwappingPuzzle)
        {
            float width = (Camera.main.ScreenToWorldPoint(Input.mousePosition)).x - currentMousePositionX;
            if (width <= -0.8f)
            {
                MovePuzzleNextOrPrevious(false);
            }
            else if (width >= 1)
            {
                MovePuzzleNextOrPrevious(true);
            }
        }
        if(currClickPuzzle !=null)
        {
            if (isMouseOnScroll && isDraggingScroll == false)
            {
                int layerMask = LayerMask.GetMask("PuzzleScroll");
                RaycastHit2D hit = Physics2D.Raycast(currClickPuzzle.transform.position, UnityEngine.Vector2.up, 1000, layerMask);
                if(hit.collider != null)
                {
                    if(currClickPuzzle.GetComponent<PuzzleController>().GetIsMerge() == false)
                    {
                        GameManager.Instance.GetPuzzleManager().DecreaseOutPuzzle();                     
                    }
                    isDraggingScroll = true;
                    MovePuzzleInScroll();
                }                
            }
        }    
    }
    //======== Scroll Puzzle Event=======//
    public void OnScrollMouseUp()
    {
        isDraggingScroll = false;
        isSwappingPuzzle = false;
        isFirstTimeEnter = false;
    }
    public void OnScrollPuzzleExit()
    {
        if (currClickPuzzle != null)
        {
            if (currClickPuzzle.GetComponent<PuzzleController>().GetIsMerge())
            {
                return;
            }
        }

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y <= 0.3f)
            return;

        isDraggingScroll = false;
        isSwappingPuzzle = false;
        isMouseOnScroll = false;
        if (currClickPuzzle != null)
        {
          
            MovePuzzleOutScroll();
        }   
    }
    public void OnScrollPuzzleMouseDown()
    {
        if(currClickPuzzle != null)
        {
            if(currClickPuzzle.GetComponent<PuzzleController>().GetIsMerge())
            {
                return;
            }
        }
        isDraggingScroll = true;
        puzzleScroll.enabled = true;
    }
    public void OnScrollPuzzleEnter()
    {
        isMouseOnScroll = true;
        if (currClickPuzzle != null)
        {
            if (currClickPuzzle.GetComponent<PuzzleController>().GetIsMerge())
            {
                return;
            }
        }        
        if(isFirstTimeEnter == false)
        {
            isFirstTimeEnter = true;
            return;
        }         
    }

    // ====== Move Puzze Event=======//
    private void MovePuzzleInScroll()
    {
        // Sau khi Move lai puzzle vao scroll set parent
        if(currClickPuzzle.GetComponent<PuzzleController>().GetIsMerge() == false)
        {
            currClickPuzzle.GetComponent<PuzzleController>().SetTotalRotate();
            currClickPuzzle.GetComponent<PuzzleController>().ResetRotateTransform();
            currClickPuzzleTransform.SetParent(puzzleScrollContent, true);
            currClickPuzzleTransform.localPosition = new UnityEngine.Vector3(currClickPuzzleTransform.localPosition.x, currClickPuzzleTransform.localPosition.y, -10);            
            currClickPuzzleTransform.GetComponent<PuzzleController>().SetScaleNewDefault();            
            // Set Sibling index cho vi tri moi           
            double tempIndex = Math.Round(currClickPuzzleTransform.localPosition.x / GameManager.Instance.GetPuzzleManager().GetWidthBetweenTwoPuzzle());
            if (puzzleScrollContent.childCount == 1)
            {
                tempIndex = 0;
            }
            currClickPuzzleTransform.SetSiblingIndex((int)Mathf.Abs((float)tempIndex));
            if(GameManager.Instance.GetPuzzleManager().GetIsShowingCorner())
            {
                GameManager.Instance.GetPuzzleManager().IncreaseCornerPuzzle();
            }
            // Scale lai vi tri cua content va cac puzzle
            GameManager.Instance.GetPuzzleManager().RescaleContentSize((int)Mathf.Abs((float)tempIndex));
            // reset ve vi tri moi khi move vao scroll view
         //   currClickPuzzle.GetComponent<PuzzleController>().ResetToTargetPos();
            // store mouse position khi cho puzzle lai vao scroll
            currentMousePositionX = currClickPuzzleTransform.position.x;
            // set co the swap = true
            isSwappingPuzzle = true;          
        }  
    }
    private void MovePuzzleOutScroll()
    {
        if (currClickPuzzle.GetComponent<PuzzleController>().GetIsInScroll())
        {
            GameManager.Instance.GetPuzzleManager().IncreaseOutPuzzle();
        }
        puzzleScroll.enabled = false;        
        if (currClickPuzzle.transform.parent != boardPuzzle)
        {
            currClickPuzzleTransform.GetComponent<PuzzleController>().SetScaleDefault();
        }      
        currClickPuzzleTransform.SetParent(boardPuzzle);
        currClickPuzzle.GetComponent<PuzzleController>().SetIsInParent(false);
        currClickPuzzleTransform.GetComponent<PuzzleController>().SetScalePuzzle(CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber()));
        currClickPuzzleTransform.position = new UnityEngine.Vector3(currClickPuzzleTransform.position.x, currClickPuzzleTransform.position.y, -1);
        if (GameManager.Instance.GetPuzzleManager().GetIsShowingCorner())
        {
            if(currClickPuzzle.GetComponent<PuzzleController>().GetIsCorner())
            GameManager.Instance.GetPuzzleManager().DecreaseCornerPuzzle();
        }
        GameManager.Instance.GetPuzzleManager().RescaleContentSize(currClickPuzzle.GetComponent<PuzzleController>().GetCurrentIndex());
    }
    private void MovePuzzleNextOrPrevious(bool _isNext)
    {
        if(currClickPuzzle != null)
        {
            int currIndex = currClickPuzzleTransform.GetSiblingIndex();
            currentMousePositionX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            if (_isNext)
            {               
                if(currIndex + 1 < puzzleScrollContent.childCount)
                {
                    currClickPuzzleTransform.SetSiblingIndex(currIndex + 1);
                    GameManager.Instance.GetPuzzleManager().MoveCurrentPuzzleToNext(currIndex);
                }                                         
            }
            else
            {
                if(currIndex != 0)
                {
                    currClickPuzzleTransform.SetSiblingIndex(currIndex - 1);
                    GameManager.Instance.GetPuzzleManager().MoveCurrentPuzzleToPrevious(currIndex);
                }                
            }
            //currClickPuzzle.GetComponent<PuzzleController>().ResetToTargetPos();
        }
    }
    private void MovePuzzleToRightSpot(GameObject _puzzleONe)
    {
        bool temp = _puzzleONe.GetComponent<PuzzleController>().GetIsInScroll();
        // register de move out khoi scroll
        OnRegisterCurrentPuzzle(_puzzleONe);
        // move ra ngoai scroll
        if (temp)
            MovePuzzleOutScroll();
        // set vi tri ra x = 0
        currClickPuzzleTransform.localPosition = new Vector3(0, currClickPuzzleTransform.localPosition.y, currClickPuzzleTransform.localPosition.z);
        // move ve vi chi dung'
        currClickPuzzle.GetComponent<PuzzleController>().ResetRotateTransform();
        currClickPuzzle.GetComponent<PuzzleController>().MoveToRightPos();
        // reset lai puzzle
        currClickPuzzle = null;
    }
    public bool GetIsDraggingScroll()
    {
        return isDraggingScroll;
    }
    public void OnRegisterCurrentPuzzle(GameObject _gameObj)
    {
        currClickPuzzle = _gameObj;
        currClickPuzzleTransform = currClickPuzzle.GetComponent<Transform>();
    }
    public void ResetCurrPuzzle()
    {
        currClickPuzzle = null;
        isDraggingScroll = false;
        isSwappingPuzzle = false;
    }   
    public Transform GetBoardPuzzleImageTransform()
    {
        return boardPuzzle;
    }
    public GameObject GetCurrClickPuzzle()
    {
        return currClickPuzzle;
    } 
    // ======= Puzzle Skill ====== ///
    public void HintSkill()
    {
        int currenScore = GameManager.Instance.GetCurrentScore();
        int totalScore = GameManager.Instance.GetTotalScore();

        if (currenScore < totalScore)
        {
            int coin = PlayerPrefs.GetInt(GameConfig.PREF_COIN);
            if (coin >= GameConfig.HINT_SKILL_COST)
            {
                if(CheckHintUse())
                {
                    NewHintSkill(GameConfig.HINT_SKILL_COST);
                }              
            }
            else
            {
                // TODO: Watch Video
                if(CheckHintUse())
                {
                    AdManager.Instance.AdmobHandler.RegisterAdmobListener("hint", this);
                    AdManager.Instance.ShowRewardedAd();
                }
            }
        }        
    }
    public void onRewardedVideo()
    {
        isReward = true;
    }
    public void onAdClosed()
    {
        if(isReward)
        {
            NewHintSkill(0);
            isReward = false;
        }
    }
    public bool CheckHintUse()
    {
        randomList.Clear();

        for (int i = 0; i < GameManager.Instance.GetPuzzleManager().GetPiecesList().Count; i++)
        {
            if (GameManager.Instance.GetPuzzleManager().GetPiecesList()[i].GetComponent<PuzzleController>().GetIsRightPos() == false &&
                GameManager.Instance.GetPuzzleManager().GetPiecesList()[i].GetComponent<PuzzleController>().GetIsMerge() == false)
            {
                randomList.Add(GameManager.Instance.GetPuzzleManager().GetPiecesList()[i]);
            }
        }
        if(randomList.Count > 0)
        {
            return true;
        }
        return false;
    }
    public void NewHintSkill(int skillCost)
    {
        int totalMove = 0;
        int random = UnityEngine.Random.Range(0, randomList.Count);
        PuzzleController firstPuzzle = randomList[random].GetComponent<PuzzleController>();
        // move puzzle dau tien ve vi tri dung
        MovePuzzleToRightSpot(randomList[random]);
        totalMove++;
        // ====== tim puzzle thu 2 canh no de move vao vi tri dung ===== //
        if (firstPuzzle.GetNeighbor() != null)
        {
            if (!firstPuzzle.GetNeighbor().GetComponent<PuzzleController>().GetIsRightPos() && !firstPuzzle.GetNeighbor().GetComponent<PuzzleController>().GetIsMerge())
            {
                MovePuzzleToRightSpot(firstPuzzle.GetNeighbor());
                totalMove++;
            }
        }

        if(puzzleScrollContent.localPosition.x < ((-433 * CommonUltilities.GetRescaleRatio()) + (-200 * totalMove)))
        {
            puzzleScrollContent.localPosition = new Vector3(puzzleScrollContent.localPosition.x + 200 * totalMove, puzzleScrollContent.localPosition.y, puzzleScrollContent.localPosition.z);
        }        
        GameManager.Instance.DecreaseCoin(skillCost);
    }

    // ======= Function =========//
    public void SetIsFirstTimerEnterTrue()
    {
        isFirstTimeEnter = true;
    }
    public void OnInitMoveInDistance(int column)
    {
        switch (column)
        {
            case 6:
                moveInDistance = 0.1f;
                break;
            case 8:
                moveInDistance = -0.15f;
                break;
            case 10:
                moveInDistance = -0.25f;
                break;
            case 11:
                moveInDistance = -0.3f;
                break;
            case 12:
                moveInDistance = -0.32f;
                break;
            case 20:
                moveInDistance = -0.48f;
                break;
            default:
                break;
        }

    }
}
