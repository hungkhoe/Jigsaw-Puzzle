using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PuzzleController : MonoBehaviour
{
    public enum MergeDirection
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM,
        NONE
    }
    // Start is called before the first frame update    
    [SerializeField]
    private int idPositionX, idPositionY;
    [SerializeField]
    BoxCollider2D collider2D;

    private Transform puzzleTransform;
    private GameObject connectedNeighbor;

    private int currentIndex = 0;
    private int totalChild;
    private int totalRotate;
    private float currentPositionX, currentPositionY;
    private float movingSpeed = 800;

    private bool isHeld = false;
    private bool isInScroll = false;
    private bool isInRightPos = false;
    private bool isMerged = false;
    private bool isCorner = false;
    private Vector3 targetPos;
    private Vector3 rightPos;

    private MergeDirection mergeDir = MergeDirection.NONE;

    private float timer = 0;

    private void Start()
    {
        puzzleTransform = GetComponent<Transform>();
        collider2D = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
#if UNITY_EDITOR
        OnUpdate();
#else
        if(Input.touchCount == 1)
        {
             OnUpdate();
        }
#endif

    }

    // ============ Pointer Event =============== //
    private void OnMouseDown()
    {
        if(GameManager.Instance.isSettingOpen)
        {
            return;
        }
#if UNITY_EDITOR
        OnClickDownPuzzle();
#else
        if(Input.touchCount == 1)
        {
            OnClickDownPuzzle();
        }
#endif
    }
    private void OnMouseUp()
    {
        if (timer < 0.15f)
        {
            // rotate;
            if(!isInScroll)
            {
                if(!isMerged)
                {
                    RotateOutScroll();
                }              
            }           
        }
        if (!isInScroll)
        {
            if(Camera.main.WorldToViewportPoint(puzzleTransform.transform.position).y < 0.4f)
            {
                if(isMerged == false)
                {
                    MusicManager.Instance.PlayNewSoundEffect(SoundEffect.DAT_PUZZLE_1);
                    puzzleTransform.transform.position = new Vector3(puzzleTransform.transform.position.x, puzzleTransform.transform.position.y + 1.6f, puzzleTransform.transform.position.z);
                }               
            }
            else
            {
                if (this.transform.eulerAngles.z == 0 && this.transform.eulerAngles.x == 0)
                {
                    if (Vector2.Distance(GetRealPos(), rightPos) < (GameConfig.PUZZLE_DISTANCE_RIGHTPOS_DEFAULT * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber())))
                    {
                        if (isMerged)
                        {
                            MoveAllChildToRightPos();
                        }
                        else
                        {
                            MoveToRightPos();
                        }
                    }
                    else
                    {
                        if (!isMerged)
                        {
                            ShootRayBoxCast();
                        }
                        else
                        {
                            for (int i = 0; i < puzzleTransform.parent.childCount; i++)
                            {
                                puzzleTransform.parent.GetChild(i).GetComponent<PuzzleController>().ShootRayBoxCast();
                            }
                        }
                    }
                }
                else
                {
                    if (timer > 0.15f)
                    {
                        MusicManager.Instance.PlayNewSoundEffect(SoundEffect.DAT_PUZZLE_1);
                    }
                }
            }

           
        }
        if (isMerged)
        {
            for (int i = 0; i < puzzleTransform.parent.childCount; i++)
            {
                puzzleTransform.parent.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
            }
        }
        GameManager.Instance.GetPlayerController().ResetCurrPuzzle();
        collider2D.enabled = true;
        isHeld = false;
    } 
    private void OnClickDownPuzzle()
    {
        timer = 0;
        int layer = LayerMask.GetMask("ExtensionPanel");        
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero, 100, layer);

        if (hit.collider == null)
        {
            GameManager.Instance.GetPlayerController().OnRegisterCurrentPuzzle(gameObject);
            collider2D.enabled = false;
            isHeld = true;
            if(isMerged == false)
            {
                currentPositionX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - puzzleTransform.position.x;
                currentPositionY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - puzzleTransform.position.y;
            }
            else
            {
                currentPositionX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - puzzleTransform.parent.position.x;
                currentPositionY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - puzzleTransform.parent.position.y;
            }
     
            if (!isInScroll)
            {
                GameManager.Instance.GetPlayerController().SetIsFirstTimerEnterTrue();
                if (isMerged)
                {
                    for (int i = 0; i < puzzleTransform.parent.childCount; i++)
                    {
                        puzzleTransform.parent.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                    }                   
                }
            }
        }      
    }
    public void OnDragPuzzle()
    {
        if(GameManager.Instance.GetPlayerController().GetIsDraggingScroll() == false)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);           
            if (isMerged)
            {        
                if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.35f && Camera.main.ScreenToViewportPoint(Input.mousePosition).y < 0.9f
                    && Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.95f && Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.05f)
                {
                    puzzleTransform.parent.position = new Vector3(mousePos.x - currentPositionX, (mousePos.y - currentPositionY) + (0.8f * CommonUltilities.GetPuzzleRatio((int)GameManager.Instance.GetPuzzleManager().size.x)), puzzleTransform.parent.position.z);
                }
                else if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.9f)
                {
                    if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.95f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.05f)
                    {
                        puzzleTransform.parent.position = new Vector3(puzzleTransform.parent.position.x, puzzleTransform.parent.position.y, puzzleTransform.parent.position.z);
                    }
                    else
                    {
                        puzzleTransform.parent.position = new Vector3(mousePos.x - currentPositionX, puzzleTransform.parent.position.y, puzzleTransform.parent.position.z);
                    }
                }
                else if(Camera.main.ScreenToViewportPoint(Input.mousePosition).y < 0.35f)
                {
                    if(Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.95f && Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.05f)
                    {
                        puzzleTransform.parent.position = new Vector3(mousePos.x - currentPositionX, puzzleTransform.parent.position.y, puzzleTransform.parent.position.z);
                    }
                    else
                    {
                        puzzleTransform.parent.position = new Vector3(puzzleTransform.parent.position.x, puzzleTransform.parent.position.y, puzzleTransform.parent.position.z);
                    }
                }
                else if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.95f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.05f)
                {
                    puzzleTransform.parent.position = new Vector3(puzzleTransform.parent.position.x, (mousePos.y - currentPositionY) + (0.8f * CommonUltilities.GetPuzzleRatio((int)GameManager.Instance.GetPuzzleManager().size.x)), puzzleTransform.parent.position.z);
                }
            }
            else
            {
                if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y < 0.9f && Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.95f && Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.05f)
                {
                    puzzleTransform.position = new Vector3(mousePos.x - currentPositionX, (mousePos.y - currentPositionY) + (0.8f * CommonUltilities.GetPuzzleRatio((int)GameManager.Instance.GetPuzzleManager().size.x)), puzzleTransform.position.z);
                }
                else if (Camera.main.WorldToViewportPoint(puzzleTransform.position).y >= 1)
                {
                    if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.95f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.05f)
                    {
                        puzzleTransform.position = new Vector3(puzzleTransform.position.x, puzzleTransform.position.y, puzzleTransform.position.z);
                    }
                    else
                    {
                        puzzleTransform.position = new Vector3(mousePos.x - currentPositionX, puzzleTransform.position.y, puzzleTransform.position.z);
                    }                
                }
                else if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.95f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.05f)
                {
                    puzzleTransform.position = new Vector3(puzzleTransform.position.x, (mousePos.y - currentPositionY) + (0.8f * CommonUltilities.GetPuzzleRatio((int)GameManager.Instance.GetPuzzleManager().size.x)), puzzleTransform.position.z);
                }
            }           
        }    
    }
    //============= Puzzle Controller ============//
    private void OnUpdate()
    {
        if (!isInRightPos)
        {
            if (isHeld)
            {
                timer += Time.deltaTime;
                if(timer < 0.15f)
                {
                    // rotate;                    
                    return;
                }
                OnDragPuzzle();
            }
        }
    }    
    public void OnInit(int _positionX, int _positionY, int _index, Vector2 _rightPos)
    {
        idPositionX = _positionX;
        idPositionY = _positionY;
        currentIndex = _index;
        rightPos = _rightPos;
        rightPos.z = -1;      
    }
    public void SetIsInParent(bool _isInScroll)
    {
        isInScroll = _isInScroll;
    }
    public void SetNewTargetPosInScroll(Vector3 _targetPos, int _index)
    {        
        targetPos = _targetPos;
        currentIndex = _index;
        ResetToTargetPos();     
        isInScroll = true;       
    }   
    public void ResetToTargetPos()
    {
        ResetRotateTransform();
        if (puzzleTransform != null)
        {
            puzzleTransform.localPosition = targetPos;
        }      
        else
        {
            transform.localPosition = targetPos;
        }
        RotateInScroll();  
    }       
    public void ShootRayBoxCast()
    {
        if (this.transform.eulerAngles.z == 0 && this.transform.eulerAngles.x == 0)
        {
            int layer = LayerMask.GetMask("PuzzleControl");
            var puzzleResult = Physics2D.OverlapBoxAll(transform.position, new Vector2(GameConfig.PUZZLE_BOX_RAY_DEFAULT_SIZE * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber()),
                GameConfig.PUZZLE_BOX_RAY_DEFAULT_SIZE * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber())), 0, layer);
            mergeDir = MergeDirection.NONE;
            bool isNormal = false;
            for (int i = 0; i < puzzleResult.Length; i++)
            {
                if (puzzleResult[i] != null)
                {
                    if (!OnCheckNeighbor(puzzleResult[i].transform.GetComponent<PuzzleController>()))
                    {                        
                        continue;
                    }
                    else
                    {
                        if (puzzleResult[i].GetComponent<PuzzleController>().isInScroll)
                            continue;
                        if(puzzleResult[i].transform.eulerAngles.z == 0 && puzzleResult[i].transform.eulerAngles.x == 0)
                        {
                            MergeParentPuzzle(puzzleResult[i].transform);
                            MoveToMergePuzzle(mergeDir, puzzleResult[i].transform);
                            break;
                        }                        
                    }
                }
            }
            if(isNormal == false)
            {
                if(timer > 0.15f)
                {
                    MusicManager.Instance.PlayNewSoundEffect(SoundEffect.DAT_PUZZLE_1);
                }              
            }
        }            
    }
    public void MoveAllChildToRightPos()
    {
        Transform tempParent = puzzleTransform.transform.parent;
        for (int i = tempParent.childCount - 1; i >= 0; --i)
        {
            tempParent.GetChild(i).GetComponent<PuzzleController>().MoveToRightPos();
        }
        Destroy(tempParent.gameObject);
    }
    public void SetConnectedNeighbor(GameObject _neighbor)
    {
        connectedNeighbor = _neighbor;
    }
    private void MergeParentPuzzle(Transform _puzzleNeighbor)
    {
        if (!isMerged)
        {            
            if (_puzzleNeighbor.GetComponent<PuzzleController>().isMerged)
            {
                puzzleTransform.SetParent(_puzzleNeighbor.parent);
                GameManager.Instance.GetPuzzleManager().DecreaseOutPuzzle();
            }
            else
            {
                GameObject tempParent = new GameObject();
                tempParent.transform.SetParent(GameManager.Instance.GetPlayerController().GetBoardPuzzleImageTransform(), false);
                puzzleTransform.SetParent(tempParent.transform);
                _puzzleNeighbor.SetParent(tempParent.transform);
                _puzzleNeighbor.GetComponent<PuzzleController>().isMerged = true;
                tempParent.name = ("Parent " + puzzleTransform.name);
                GameManager.Instance.GetPuzzleManager().DecreaseOutPuzzle();
                GameManager.Instance.GetPuzzleManager().DecreaseOutPuzzle();
            }            
        }
        else
        {
            if (!_puzzleNeighbor.GetComponent<PuzzleController>().isMerged)
            {
                _puzzleNeighbor.SetParent(puzzleTransform.parent);
                _puzzleNeighbor.GetComponent<PuzzleController>().isMerged = true;
                totalChild = 1;
                GameManager.Instance.GetPuzzleManager().DecreaseOutPuzzle();
            }
            else
            {
                Transform parentTemp = _puzzleNeighbor.parent;
                for (int i = parentTemp.childCount - 1; i >= 0; --i)
                {
                    totalChild++;
                    parentTemp.GetChild(i).transform.SetParent(puzzleTransform.parent);
                }
                if(parentTemp != puzzleTransform.parent)
                Destroy(parentTemp.gameObject);
            }
        }
    }
    public void SetIsCorner(bool _isCorner)
    {
        isCorner = _isCorner;
    }
    public GameObject GetNeighbor()
    {
        return connectedNeighbor;
    }
    private Vector2 GetRealPos()
    {
        if (isMerged)
        {
            return GameManager.Instance.GetPlayerController().GetBoardPuzzleImageTransform().InverseTransformPoint(puzzleTransform.position);
        }
        else
        {
            return puzzleTransform.localPosition;
        }
    }
    public  Vector2 GetRightPos()
    {
        return rightPos;
    }
    public int GetCurrentIndex()
    {
        return currentIndex;
    }
    public int GetIDPositionX()
    {
        return idPositionX;
    }
    public int GetIDPositionY()
    {
        return idPositionY;
    }
    public bool GetIsRightPos()
    {
        return isInRightPos;
    }
    public bool GetIsMerge()
    {
        return isMerged;
    }
    public bool GetIsCorner()
    {
        return isCorner;
    }
    public bool GetIsInScroll()
    {
        return isInScroll;
    }
    private bool OnCheckNeighbor(PuzzleController puzzleNeighbor)
    {
        Vector2 distance = puzzleNeighbor.transform.position - transform.position;
        float mainDistanceRight = GameConfig.NEW_PUZZLE_RIGHT_DISTANCE * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber());
        float mainDistanceLeft = GameConfig.NEW_PUZZLE_LEFT_DISTANCE * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber());
        float mainDistanceUp = GameConfig.NEW_PUZZLE_UP_DISTANCE * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber());
        float mainDistanceDown = GameConfig.NEW_PUZZLE_DOWN_DISTANCE * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber());

        float subDistance = GameConfig.PUZZLE_SUB_AXIS_DISTANCE_MERGE_DEFAULT * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber());
        // check left and right
        if (puzzleNeighbor.idPositionY == idPositionY)
        {
            // check neighbor on left
            if (puzzleNeighbor.idPositionX == idPositionX - 1)
            {
                if (distance.x > mainDistanceRight && distance.x < (-0.62f * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber())) && Mathf.Abs(distance.y) < subDistance)
                {
                    mergeDir = MergeDirection.RIGHT;
                    return true;
                }
            }
            // check neighbor on right
            else if (puzzleNeighbor.idPositionX == idPositionX + 1)
            {
                if (distance.x < mainDistanceLeft && distance.x > (0.62f * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber())) && Mathf.Abs(distance.y) < subDistance)
                {
                    mergeDir = MergeDirection.LEFT;
                    return true;
                }
            }
        }
        else if (puzzleNeighbor.idPositionX == idPositionX)
        {
            // check neighbor on top
            if (puzzleNeighbor.idPositionY == idPositionY + 1)
            {
                if (distance.y > mainDistanceUp && distance.y < (-0.57f * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber())) && Mathf.Abs(distance.x) < subDistance)
                {
                    mergeDir = MergeDirection.TOP;
                    return true;
                }
            }
            // check neighbor on Bottom
            else if (puzzleNeighbor.idPositionY == idPositionY - 1)
            {
                if (distance.y < mainDistanceDown && distance.y > (0.57f * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber())) && Mathf.Abs(distance.x) < subDistance)
                {
                    mergeDir = MergeDirection.BOTTOM;
                    return true;
                }
            }
        }
        return false;
    }

    // =========== Coroutine Event ========== //
    public void MoveToRightPos()
    {
        bool solo = false;
        solo = !isMerged;
        if (isMerged)
        {            
           puzzleTransform.SetParent(GameManager.Instance.GetPlayerController().GetBoardPuzzleImageTransform());
           
           isMerged = false;
        }
        else
        {
            if (!isInScroll)
            {
                GameManager.Instance.GetPuzzleManager().DecreaseOutPuzzle();
            }
        }
        if(!isInRightPos)
        {
            isInRightPos = true;
            GameManager.Instance.IncreaseCurrentScore();
            gameObject.SetActive(true);
            StartCoroutine(MoveToRightPosCour(solo));
        }      
    }
    private IEnumerator MoveToRightPosCour(bool _solo)
    {       
        while (puzzleTransform.localPosition != rightPos)
        {
            puzzleTransform.localPosition = Vector3.MoveTowards(puzzleTransform.localPosition, rightPos, Time.deltaTime * 20);
            yield return null;
        }
        if(_solo)
        {
            LightUpAround();
        }
        collider2D.enabled = false;        
        puzzleTransform.localPosition = new Vector3(puzzleTransform.localPosition.x, puzzleTransform.localPosition.y, 0);
        GetComponent<PuzzleController>().enabled = false;
        if (isCorner)
        {
            GameManager.Instance.GetPuzzleManager().DecreaseTotalCornerNotInPos();
        }
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayNewSoundEffect(SoundEffect.DAT_PUZZLE_2);
    }
    private void MoveToMergePuzzle(MergeDirection _moveDir, Transform _puzzleNeighbor)
    {
        //TODO
        float boxsizeX = Mathf.Abs(_puzzleNeighbor.GetComponent<PuzzleController>().rightPos.x - rightPos.x);
        float boxsizeY = Mathf.Abs(_puzzleNeighbor.GetComponent<PuzzleController>().rightPos.y - rightPos.y);
        Vector3 movePos = _puzzleNeighbor.localPosition;

        switch (_moveDir)
        {
            case MergeDirection.LEFT:
                movePos.x -= boxsizeX;
                break;
            case MergeDirection.RIGHT:
                movePos.x += boxsizeX;
                break;
            case MergeDirection.BOTTOM:
                movePos.y -= boxsizeY;
                break;
            case MergeDirection.TOP:
                movePos.y += boxsizeY;
                break;
            default:
                break;
        }
        Vector3 offset = movePos - puzzleTransform.localPosition;

        if (isMerged)
        {
            int childToEnd = puzzleTransform.parent.childCount - totalChild;
            for (int i = 0; i < childToEnd; i++)
            {
                StartCoroutine(puzzleTransform.parent.GetChild(i).GetComponent<PuzzleController>().MovePuzzleToSpecificPoint(offset));
            }
            totalChild = 0;
        }
        else
        {
            StartCoroutine(MovePuzzleToSpecificPoint(offset));           
        }

    } 
    private IEnumerator MovePuzzleToSpecificPoint(Vector3 _offset)
    {
        Vector3 target = _offset + puzzleTransform.localPosition;
        while (puzzleTransform.localPosition != target)
        {
            puzzleTransform.localPosition = Vector3.MoveTowards(puzzleTransform.localPosition, target, Time.deltaTime * 10);
            yield return null;
        }

        for(int i = 0; i < puzzleTransform.parent.childCount; i++)
        {
            puzzleTransform.parent.GetChild(i).GetComponent<PuzzleController>().StartLightUp();
        }
        isMerged = true;
        mergeDir = MergeDirection.NONE;
    }      

    // ========== Light Up Coroutine ========== //
    private void StartLightUp()
    {
        StartCoroutine(LightUpCour());
    }
    IEnumerator LightUpCour()
    {
        float light = GetComponent<Renderer>().material.GetFloat("_LightUp");
        while (light < 0.5f)
        {
            GetComponent<Renderer>().material.SetFloat("_LightUp", light + 0.05f);
            light = GetComponent<Renderer>().material.GetFloat("_LightUp");
            yield return null;
        }

        while (light > 0.01f)
        {
            GetComponent<Renderer>().material.SetFloat("_LightUp", light - 0.05f);
            light = GetComponent<Renderer>().material.GetFloat("_LightUp");
            yield return null;
        }        
    }
    private void LightUpAround()
    {
        bool isMerge = false;
        int colNum = GameManager.Instance.GetPuzzleManager().GetColNumber();

        if(idPositionX + 1 != GameManager.Instance.GetPuzzleManager().GetColNumber())
        {
            if (GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX + 1, idPositionY].isInRightPos)
            {
                GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX + 1, idPositionY].StartLightUp();
                isMerge = true;
            }
        }      

        if(idPositionX > 0)
        {
            if (GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX - 1, idPositionY].isInRightPos)
            {
                GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX - 1, idPositionY].StartLightUp();
                isMerge = true;
            }
        }


        if (idPositionY > 0)
        {
            if (GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX, idPositionY - 1].isInRightPos)
            {
                GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX, idPositionY - 1].StartLightUp();
                isMerge = true;
            }
        }

        if (idPositionY + 1 != GameManager.Instance.GetPuzzleManager().GetColNumber())
        {
            if (GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX, idPositionY + 1].isInRightPos)
            {
                GameManager.Instance.GetPuzzleManager().GetBoardMatrix()[idPositionX, idPositionY + 1].StartLightUp();
                isMerge = true;
            }
        }     

        if(isMerge)
        {
            StartLightUp();
        }
    }
    // ============= Set Function Event =========== //
    public void SetScalePuzzle(float _scaleRatio)
    {
        float scaleRatio = GameConfig.NEW_PUZZLE_DEFAULT_SCALE_IN_WORLD * _scaleRatio;
        if(puzzleTransform != null)
        puzzleTransform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
        else
        {
            transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
        }
    }  
    public void SetScaleDefault()
    {
        puzzleTransform.localScale = new Vector3(GameConfig.NEW_PUZZLE_SCALE_IN_SCROLL_WORLD, GameConfig.NEW_PUZZLE_SCALE_IN_SCROLL_WORLD, 1);
    }
    public void SetScaleNewDefault()
    {
        puzzleTransform.localScale = new Vector3(GameConfig.NEW_PUZZLE_SCALE_IN_SCROLL, GameConfig.NEW_PUZZLE_SCALE_IN_SCROLL
            , 1);
    }
    public void SetToRightPos()
    {
        transform.localPosition = new Vector3(rightPos.x, rightPos.y, 0);
        isInRightPos = true;
        if(collider2D != null)
        {
            collider2D.enabled = false;
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
        }
       
        GetComponent<PuzzleController>().enabled = false;
    }
    public void SetTotalRotate()
    {
        totalRotate = (int)transform.eulerAngles.z / 90;
    }

    // ============ Transform Rotate ============= //
    public void RotateOutScroll()
    {
        if (GameManager.Instance.GetPuzzleManager().GetIsPlayingRotatingMode())
        {
            transform.Rotate(0, 0, 90);
            transform.Translate(GameConfig.TRANSLATE_X_NOT_IN_SCROLL * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber()), GameConfig.TRANSLATE_Y_NOT_IN_SCROLL
                * CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber()), 0);
        }            
    }
    public void RotateInScroll()
    {
        if(GameManager.Instance.GetPuzzleManager().GetIsPlayingRotatingMode())
        {
            for(int i = 0; i < totalRotate; i ++)
            {
                transform.Rotate(0, 0, 90);
                transform.Translate(0, GameConfig.TRANSLATE_Y_IN_SCROLL / (2 * CommonUltilities.GetOthographicRatio()), 0);
                targetPos = transform.localPosition;
            }        
        }
    }
    public void RotateInScrollOneTime()
    {
        transform.Rotate(0, 0, 90);
        transform.Translate(0, GameConfig.TRANSLATE_Y_IN_SCROLL / (2 * CommonUltilities.GetOthographicRatio()), 0);
        targetPos = transform.localPosition;
    }
    public void ResetRotateTransform()
    {       
        if(GameManager.Instance.GetPuzzleManager().GetIsPlayingRotatingMode())
        transform.eulerAngles = new Vector3(0, 180, 0);
    }
}

