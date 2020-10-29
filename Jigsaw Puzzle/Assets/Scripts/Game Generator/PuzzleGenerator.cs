using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PuzzleGenerator : MonoBehaviour
{
    public Texture image = null;                // will contain the jigsaw projected picture
    public Vector2 size = new Vector2(5, 5);     // how many pieces will this puzzle have (x,y)
    public string topLeftPiece = "11";          // topleft piece - format YX (1,2,3,4,5) so 11 to 55 - 25 unique start possiblities    
    private float spacing = 0;
    private float defaultScale = 11.45f;

    [SerializeField]
    private PuzzleMain main = null;

    [SerializeField]
    private Transform firstScrollInit;
    [SerializeField]
    internal Transform contentPuzzleScroll;
    [SerializeField]
    private Transform lineBoard;

    [SerializeField]
    private UnityEngine.UI.Image borderPiecesButtonImage;
    [SerializeField]
    private Sprite borderPiecesOn;
    [SerializeField]
    private Sprite borderPiecesOff;

    [SerializeField]
    private UnityEngine.UI.Image moveInButtonImage;
    [SerializeField]
    private Sprite moveInPuzzleOn;
    [SerializeField]
    private Sprite moveInPuzzleOff;

    private GameObject linesH = null;
    private GameObject linesV = null;

    List<Vector3> puzzlePosition = new List<Vector3>();
    private List<GameObject> pieces = new List<GameObject>();
    private List<GameObject> defaultPieces = new List<GameObject>();

    private PuzzleController[,] boardMatrix;

    private float width;

    private int totalCorner = 0;
    private int totalCornerNotInPos = 0;
    private int outScrollPuzzle = 0;

    private bool isShowingCorner = false;

    private string contentLink;
    private int slotIndex = -1;
    private bool isLoad;
    private bool isIncompletedMode;
    private bool isRotatingMode;
    // Update is called once per frame
    private void Start()
    {      
        if (PassInformation.Instance != null)
        {
            image = PassInformation.Instance.GetImagePreview();
            if(PassInformation.Instance.GetIsLoad())
            {
                isIncompletedMode = PassInformation.Instance.GetIsInCompletedMode();
                float sizeTemp = DataManager.Instance.saveFile.saveSlotList[PassInformation.Instance.GetSlotNumber()].rowNumber;
                if(PassInformation.Instance.GetIsNewGame())
                {
                    size = new Vector2(PassInformation.Instance.GetPiecesRow(), PassInformation.Instance.GetPiecesRow());
                    isRotatingMode = PassInformation.Instance.GetIsRotatingMode();
                }
                else
                {
                    isRotatingMode = DataManager.Instance.saveFile.saveSlotList[PassInformation.Instance.GetSlotNumber()].isRotating;
                    size = new Vector2(sizeTemp, sizeTemp);
                }                
                slotIndex = DataManager.Instance.saveFile.saveSlotList[PassInformation.Instance.GetSlotNumber()].slotIndex;
                isLoad = true;
                
            }
            else
            {
                size = new Vector2(PassInformation.Instance.GetPiecesRow(), PassInformation.Instance.GetPiecesRow());
                isRotatingMode = PassInformation.Instance.GetIsRotatingMode();
            }            
            contentLink = PassInformation.Instance.GetContentLink();
        }

        if (main != null)
        {
            // main puzzle initialization
            // check if TopLeftPiece Exists , if not we will take '11'
            main.GetBasePieces();
            if (main.GetBase(topLeftPiece) == null)
                topLeftPiece = "11";
            // initialization of this puzzle
            // create horizontal and vertical lines
            SetLines();
            // create pieces of this puzzle
            SetPieces();
            // create mouse control 'hit' plane
            //   SetPlane();
            GameManager.Instance.GetPlayerController().OnInitMoveInDistance((int)size.x);           
        }     
    }
    // Create horizontal lines to display on puzzle
    private void SetLinesHorizontal()
    {
        // we must have a valid topLeftPiece
        if (topLeftPiece.Length != 2) return;
        // get starting x-line from top left piece
        int tpX = System.Convert.ToInt32(topLeftPiece.Substring(1, 1));
        // get starting y-line from top left piece
        int tpY = System.Convert.ToInt32(topLeftPiece.Substring(0, 1));
        // we will recreate so destroy if we already have lines
        if (linesH != null) GameObject.Destroy(linesH);
        // create a cube primitive for these lines
        linesH = GameObject.CreatePrimitive(PrimitiveType.Cube);
        linesH.name = "lines-horizontal";
        // add lines to puzzle
        linesH.transform.parent = lineBoard.transform;
        // set 'transparent' material to lines horizontal
        linesH.GetComponent<Renderer>().material = main.linesHorizontal;
        // set the right scale (z = very thin) rotation and position so it will cover the puzzle
        linesH.transform.localScale = new Vector3(-1, -1 * (1 / size.y) * (size.y - 1), 0.0001F);
        linesH.transform.rotation = transform.rotation;
        // move this 'thin' cube so that it floats just above the puzzle
        linesH.transform.localPosition = new Vector3(0, 0, -1);
        // scale the texture in relation to specified size
        linesH.GetComponent<Renderer>().material.mainTextureScale = new Vector2(-0.2F * size.x, -0.2F * (size.y - 1));
        // set the right offset in relation to the specified size and the specified topLeftPiece
        linesH.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(((5 - size.x) * -0.2F) + ((tpX - 1) * 0.2F), 0.005F + ((tpY - 1) * -0.2F));
        linesH.active = true;
        linesH.AddComponent<SortingGroup>();
        linesH.GetComponent<SortingGroup>().sortingOrder = 0;
        linesH.GetComponent<SortingGroup>().sortingLayerName = "BoardMap";
    }

    // Create vertical lines to display on puzzle
    private void SetLinesVertical()
    {
        // we must have a valid topLeftPiece
        if (topLeftPiece.Length != 2) return;
        // get starting x-line from top left piece
        int tpX = System.Convert.ToInt32(topLeftPiece.Substring(1, 1));
        // get starting y-line from top left piece
        int tpY = System.Convert.ToInt32(topLeftPiece.Substring(0, 1));
        // we will recreate so destroy if we already have lines
        if (linesV != null) GameObject.Destroy(linesV);
        // create a cube primitive for these line
        linesV = GameObject.CreatePrimitive(PrimitiveType.Cube);
        linesV.name = "lines-vertical";
        // add lines to puzzle
        linesV.transform.parent = lineBoard.transform;
        // set 'transparent' material to lines horizonta
        linesV.GetComponent<Renderer>().material = main.linesVertical;
        // set the right scale (z = very thin) rotation and position so it will cover the puzzl
        linesV.transform.localScale = new Vector3(-1 * (1 / size.x) * (size.x - 1), -1, 0.0001F);
        linesV.transform.rotation = transform.rotation;
        // move this 'thin' cube so that it floats just above the puzzle
        linesV.transform.localPosition = new Vector3(0, 0, -1);
        // scale the texture in relation to specified size
        linesV.GetComponent<Renderer>().material.mainTextureScale = new Vector2(-0.2F * (size.x - 1), -0.2F * size.y);
        // set the right offset in relation to the specified size and the specified topLeftPiece
        linesV.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-0.2F * ((5 - size.x) + 1) + ((tpX - 1) * 0.2F), 0 + +((tpY - 1) * -0.2F));
        linesV.active = true;
        linesV.AddComponent<SortingGroup>();
        linesV.GetComponent<SortingGroup>().sortingOrder = 0;
        linesV.GetComponent<SortingGroup>().sortingLayerName = "BoardMap";
    }
    private void SetLines()
    {
        // create puzzle lines
        SetLinesHorizontal();
        SetLinesVertical();
    }
    private string GetType(Vector2 pos)
    {
        float x = pos.x;
        float y = pos.y;

        string pt = "C";
        if (y == 1)
        {
            if (x == 1) pt = "TL";
            else
                if (x == size.x) pt = "TR";
            else
                pt = "T";
        }
        else
            if (y == size.y)
        {
            if (x == 1) pt = "BL";
            else
                if (x == size.x) pt = "BR";
            else
                pt = "B";
        }
        else
                if (x == 1)
            pt = "L";
        else
                    if (x == size.x)
            pt = "R";
        return pt;
    }
    private Vector3 PiecePosition(Vector3 pos)
    {
        float dX = transform.localScale.x / (size.x / GetRatio());
        float dY = transform.localScale.y / (size.y / GetRatio());

        // determine the position related x/y vector for this piece
        Vector3 positionVector =
            ((((transform.localScale.x / 2) * -1) + (dX * (pos.x - 1)) + (dX * (0 / 2))) * transform.right * -1) +
            (((transform.localScale.y / 2)) - (dY * (pos.y - 1)) - (dY * (0 / 2))) * transform.up;

        // set piece position to its right spot on the puzzle
        return positionVector = transform.position + positionVector;
    }

    // initialize specific piece with right scale, position and texture (scale/offset)
    private void InitPiece(GameObject puzzlePiece, Vector2 pos)
    {
        // set right scale for piece in world space
        //Vector3 scale5x5 = Vector3.Scale(new Vector3(11.45f, 11.45f, 1), transform.localScale);
        Vector3 scale5x5 = Vector3.Scale(new Vector3(GameConfig.NEW_PUZZLE_DEFAULT_SCALE_INIT, GameConfig.NEW_PUZZLE_DEFAULT_SCALE_INIT, 1), transform.localScale);
        // determine the puzzle size related scale vector
        Vector3 CxR = new Vector3(1 / (0.2F * size.x), 1 / (0.2F * size.y), 1);
        // set piece to world space so we can work with puzzle dimensions
        puzzlePiece.transform.parent = null;
        // set right scale for piece in world space
        puzzlePiece.transform.localScale = Vector3.Scale(scale5x5, CxR);
        // set piece position to its right spot on the puzzle
        puzzlePiece.transform.position = PiecePosition(pos);
        // we now are gonna work with local scale for the texture so 1 = puzzle width/height
        float scaleX = 1 / (0.2F * size.x);
        float scaleY = 1 / (0.2F * size.y);
        // set surface/image material to scatteredPieces material (can be set on JigsawMainClass)
        puzzlePiece.GetComponent<Renderer>().material = main.scatteredPieces;
        // set piece base material to pieceBase material (can be set on JigsawMainClass)
        puzzlePiece.GetComponent<Renderer>().materials[1] = main.pieceBase;
        // set ouline
        puzzlePiece.GetComponent<MeshRenderer>().materials[1] = main.outline;
        // set surface texture to the puzzle image
        puzzlePiece.GetComponent<Renderer>().material.mainTexture = image;
        // scale the surface texture
        puzzlePiece.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scaleX, scaleY);
        // determine the texture offset related to the size and piece position
        puzzlePiece.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.2F * scaleX * (pos.x - 1), -0.2F * scaleY * (pos.y - 1 + (5 - size.y)));
        // set layout
        puzzlePiece.layer = 8;
    }

    // create a new piece from a specific prototype on a specific position with a specific piece type
    private GameObject CreateNewPiece(Vector2 piece, Vector2 pos, string pType)
    {
        GameObject puzzlePiece = null;
        // get piece prototype from main
        Transform basePiece = main.GetPiece("" + piece.y + "" + piece.x, pType);
        if (basePiece != null)
        {
            // prototype has been found so make an instance
            puzzlePiece = GameObject.Instantiate(basePiece.gameObject, new Vector3(pos.x * 2F, pos.y * -2F, 0), Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject;          
            // add collider to puzzle Pience
            puzzlePiece.AddComponent<BoxCollider2D>();
            // add puzzle controller
            puzzlePiece.AddComponent<PuzzleController>();
        }
        return puzzlePiece;
    }

    // Create or set (initialize) all pieces of the current puzzle
    private void SetPieces()
    {
        // we have to have a valid piece
        if (topLeftPiece.Length != 2) return;
        if (size.x <= 1 || size.y <= 1) return;

        // determine topleft piece x and y line
        int tpX = System.Convert.ToInt32(topLeftPiece.Substring(1, 1));
        int tpY = System.Convert.ToInt32(topLeftPiece.Substring(0, 1));
        int bX = tpX;

        int idX = 1;
        int idY = 1;

        Vector3 tempInitPos = firstScrollInit.localPosition;
        int index = 0;
        // loop vertical rows of the puzzle
        boardMatrix = new PuzzleController[(int)size.x, (int)size.y];
        for (int y = 1; y <= size.y; y++)
        {
            // loop horizontal columns of the puzzle
            for (int x = 1; x <= size.x; x++)
            {               
                // get piece type of current position
                string pType = GetType(new Vector2(x, y));
                // create a new puzzlePiece
                GameObject puzzlePiece = CreateNewPiece(new Vector2(tpX, tpY), new Vector2(x, y), pType);
                //set name
                //puzzlePiece.name = index.ToString();
                if (puzzlePiece != null)
                {
                    // add puzzlePiece to this puzzle's pieces and to lookup table
                    InitPiece(puzzlePiece, new Vector2(x, y));
                    pieces.Add(puzzlePiece);
                    defaultPieces.Add(puzzlePiece);
                }
                puzzlePiece.transform.parent = GameManager.Instance.GetPlayerController().GetBoardPuzzleImageTransform();
                puzzlePiece.GetComponent<PuzzleController>().OnInit(x - 1, y - 1, index, puzzlePiece.transform.localPosition);
                puzzlePiece.transform.parent = contentPuzzleScroll;
                puzzlePiece.transform.localPosition = tempInitPos;
                puzzlePiece.transform.localScale = new Vector3(GameConfig.NEW_PUZZLE_SCALE_IN_SCROLL, GameConfig.NEW_PUZZLE_SCALE_IN_SCROLL, 1);
                puzzlePiece.GetComponent<PuzzleController>().SetNewTargetPosInScroll(tempInitPos, index);
                if (pType != "C")
                {
                    puzzlePiece.GetComponent<PuzzleController>().SetIsCorner(true);
                    totalCorner++;
                    totalCornerNotInPos++;
                }
                puzzlePosition.Add(puzzlePiece.transform.localPosition);
                tempInitPos.x += 200;

                if (index != 0)
                {
                    pieces[index].GetComponent<PuzzleController>().SetConnectedNeighbor(pieces[index - 1]);
                }
                boardMatrix[x - 1, y - 1] = puzzlePiece.GetComponent<PuzzleController>();                
               // calculate cua source
               tpX++;
                if (tpX == bX + size.x || tpX == 6)
                {
                    if (tpX == 6)
                    {
                        tpX = 1;
                        idX++;
                    }
                    else
                        tpX = bX;
                }
                index++;
            }
            tpX = bX;
            idX = 1;
            tpY++;
            if (tpY == 6)
            {
                tpY = 1;
                idY++;
            }           
        }
        width = pieces[0].transform.localPosition.x - pieces[1].transform.localPosition.x;
        contentPuzzleScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(GetContentWidth(), 0);

        RandomListPuzzle();

        if (isLoad)
        {
            if (PassInformation.Instance.GetIsNewGame() == false)
            {
                LoadFile(slotIndex);
            }
        }
    }
    private float GetRatio()
    {
        return GameConfig.NEW_PUZZLE_DEFAULT_SCALE_INIT / 11.45f;
    }
    public float GetContentWidth()
    {
        if(isShowingCorner)
        {
            return Mathf.Abs(width * totalCorner);
        }
        else
        {
            return Mathf.Abs(width * contentPuzzleScroll.transform.childCount);
        }
    }
    public void RescaleContentSize(int _index)
    {
        // scale lai size
        contentPuzzleScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(GetContentWidth(), 0);
        // di chuyen puzzle den vi tri moi		
        MoveAllPuzzleToNewPos(_index);
    }
    public float GetWidthBetweenTwoPuzzle()
    {
        return width;
    }
    public int GetColNumber()
    {
        return (int)size.x;
    }
    public void MoveCurrentPuzzleToNext(int _index)
    {
        contentPuzzleScroll.GetChild(_index).GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[_index], _index);
        if (contentPuzzleScroll.childCount > 1)
        {
            contentPuzzleScroll.GetChild(_index + 1).GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[_index + 1], _index + 1);
        }
    }
    public void MoveCurrentPuzzleToPrevious(int _index)
    {
        contentPuzzleScroll.GetChild(_index).GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[_index], _index);
        contentPuzzleScroll.GetChild(_index - 1).GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[_index - 1], _index - 1);
    }
    public void MoveAllPuzzleToNewPos(int _index)
    {
        // di chuyen puzzle den vi tri moi		
        for (int i = _index; i < contentPuzzleScroll.transform.childCount; i++)
        {
            SetOnePuzzleToNewPos(contentPuzzleScroll.GetChild(i).GetComponent<PuzzleController>(), i);
        }
    }
    public void SetOnePuzzleToNewPos(PuzzleController _puzzle, int _index)
    {
        _puzzle.SetNewTargetPosInScroll(puzzlePosition[_index], _index);
    }
    public void RandomListPuzzle()
    {
        pieces.Shuffle();
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].transform.localPosition = puzzlePosition[i];
            pieces[i].GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[i], i);
            pieces[i].transform.SetSiblingIndex(i);
            if (isRotatingMode)
            {
                if(isLoad == true && PassInformation.Instance.GetIsNewGame())
                {
                    int random = Random.Range(0, 4);
                    for (int j = 0; j < random; j++)
                    {
                        pieces[i].GetComponent<PuzzleController>().RotateInScrollOneTime();
                    }
                    pieces[i].GetComponent<PuzzleController>().SetTotalRotate();
                }      
                else if (isLoad == false)
                {
                    int random = Random.Range(0, 4);
                    for (int j = 0; j < random; j++)
                    {
                        pieces[i].GetComponent<PuzzleController>().RotateInScrollOneTime();
                    }
                    pieces[i].GetComponent<PuzzleController>().SetTotalRotate();
                }
            }
        }   
    }
    public void ShowCornerPuzzleButton()
    {
#if UNITY_EDITOR      
        if (!isShowingCorner)
        {
            ShowCornerPuzzle();
        }
        else
        {
            ShowAllPuzzle();
        }
#else
        if(Input.touchCount == 1)
        {
            if (!isShowingCorner)
            {
                ShowCornerPuzzle();
            }
            else
            {
                ShowAllPuzzle();
            }
        }
#endif
    }
    public void ShowCornerPuzzle()
    {
        int index = 0;

        isShowingCorner = true;
        borderPiecesButtonImage.sprite = borderPiecesOn;
        borderPiecesButtonImage.SetNativeSize();
        for (int i = 0; i < pieces.Count; i++)
        {
            if(pieces[i].GetComponent<PuzzleController>().GetIsMerge())
            {
                continue;
            }
            if (!pieces[i].GetComponent<PuzzleController>().GetIsCorner())
            {
                if(!pieces[i].GetComponent<PuzzleController>().GetIsRightPos())
                {
                    pieces[i].SetActive(false);
                }                
                if(!pieces[i].GetComponent<PuzzleController>().GetIsInScroll())
                {
                    if (!pieces[i].GetComponent<PuzzleController>().GetIsRightPos())
                    {
                        DecreaseOutPuzzle();
                    }                    
                }
            }
            else
            {
                if (pieces[i].GetComponent<PuzzleController>().GetIsInScroll())
                {
                    pieces[i].transform.SetSiblingIndex(index);
                    pieces[i].transform.localPosition = puzzlePosition[index];
                    pieces[i].GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[index], index);
                    index++;
                }
            }
        }
        totalCorner = index;
        contentPuzzleScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(GetWidthBetweenTwoPuzzle() * totalCorner), 0);
    }
    public void ShowAllPuzzle()
    {
        borderPiecesButtonImage.sprite = borderPiecesOff;
        borderPiecesButtonImage.SetNativeSize();
        isShowingCorner = false;
        int index = 0;
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].GetComponent<PuzzleController>().GetIsMerge())
            {
                continue;
            }
            pieces[i].SetActive(true);
            if (pieces[i].GetComponent<PuzzleController>().GetIsInScroll())
            {
                pieces[i].SetActive(true);
                pieces[i].transform.SetSiblingIndex(index);
                pieces[i].transform.localPosition = puzzlePosition[index];
                pieces[i].GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[index], index);
                index++;
            }
            else
            {
                if(!pieces[i].GetComponent<PuzzleController>().GetIsCorner())
                {
                    if (!pieces[i].GetComponent<PuzzleController>().GetIsRightPos())
                    {
                        IncreaseOutPuzzle();
                    }                   
                }
            }
        }
        contentPuzzleScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(GetContentWidth(), 0);
    }
    public void DecreaseCornerPuzzle()
    {
        totalCorner--;
    }
    public void IncreaseCornerPuzzle()
    {
        totalCorner++;
    }
    public bool GetIsShowingCorner()
    {
        return isShowingCorner;
    }
    public void DecreaseTotalCornerNotInPos()
    {
        totalCornerNotInPos--;
        if(totalCornerNotInPos == 0)
        {
            ShowAllPuzzle();
        }
    }
    public PuzzleController[,] GetBoardMatrix()
    {
        return boardMatrix;
    }
    public void MoveInPuzzle()
    {
        if(outScrollPuzzle > 0)
        {
            int index = 0;
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].GetComponent<PuzzleController>().GetIsMerge())
                {
                    continue;
                }
                if (pieces[i].GetComponent<PuzzleController>().GetIsRightPos() == false)
                {
                    if(isShowingCorner)
                    {
                        if(pieces[i].GetComponent<PuzzleController>().GetIsCorner())
                        {
                            if (pieces[i].GetComponent<PuzzleController>().GetIsInScroll() == false)
                            {
                                IncreaseCornerPuzzle();
                            }
                            pieces[i].transform.SetParent(contentPuzzleScroll, true);
                            pieces[i].transform.localPosition = puzzlePosition[index];
                            pieces[i].GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[index], index);
                            pieces[i].GetComponent<PuzzleController>().SetScaleNewDefault();
                            pieces[i].transform.SetSiblingIndex(index);
                            
                            index++;
                        }
                    }
                    else 
                    {
                        pieces[i].transform.SetParent(contentPuzzleScroll, true);
                        pieces[i].transform.localPosition = puzzlePosition[index];
                        pieces[i].GetComponent<PuzzleController>().SetNewTargetPosInScroll(puzzlePosition[index], index);
                        pieces[i].GetComponent<PuzzleController>().SetScaleNewDefault();
                        pieces[i].transform.SetSiblingIndex(index);
                        index++;
                    }                                 
                }             
            }
            contentPuzzleScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(width * index), 0);
            outScrollPuzzle = 0;
            moveInButtonImage.sprite = moveInPuzzleOff;
        }
    }
    public void IncreaseOutPuzzle()
    {
        outScrollPuzzle++;
        moveInButtonImage.sprite = moveInPuzzleOn;
    }
    public void DecreaseOutPuzzle()
    {
        outScrollPuzzle--;
        if(outScrollPuzzle < 0)
        {
            outScrollPuzzle = 0;
        }
        if(outScrollPuzzle == 0)
        {
            moveInButtonImage.sprite = moveInPuzzleOff;
        }
    }
    public bool GetIsPlayingRotatingMode()
    {
        return isRotatingMode;
    }
    public List<GameObject> GetPiecesList()
    {
        return pieces;
    }

    // ======= Save File ============ //
    public void SaveFile()
    {
        float currentScore = GameManager.Instance.GetCurrentScore();
        float totalScore = GameManager.Instance.GetTotalScore();
        float percent = (currentScore / totalScore) * 100f;
        int percentFinish =  (int)Mathf.Round(percent);

        SaveSlot _slot = new SaveSlot();
        _slot.puzzleDataList = new List<PuzzleData>();

        _slot.imageLink = contentLink;
        _slot.percentFinish = percentFinish;    
        _slot.totalCornerNotInPos = totalCornerNotInPos;
        _slot.totalCorner = totalCorner;
        _slot.rowNumber = (int)size.x;
        _slot.isRotating = isRotatingMode;
        if(slotIndex != - 1)
        {
            _slot.slotIndex = slotIndex;
        }
        for (int i = 0; i < defaultPieces.Count; i++)
        {
            PuzzleData _data = new PuzzleData();
            _data.currentPosX = defaultPieces[i].transform.localPosition.x;
            _data.currentPosY = defaultPieces[i].transform.localPosition.y;
            _data.rotateAngle = defaultPieces[i].transform.eulerAngles.z;
            _data.isInRightSpot = defaultPieces[i].GetComponent<PuzzleController>().GetIsRightPos();
            _data.isInScroll = defaultPieces[i].GetComponent<PuzzleController>().GetIsInScroll();
            _slot.puzzleDataList.Add(_data);    
        }

        if(DataManager.Instance!= null) 
        DataManager.Instance.SaveGame(_slot,slotIndex);
    }
    public void LoadFile(int index)
    {
        SaveSlot _saveSlot = DataManager.Instance.saveFile.saveSlotList[index];

        totalCorner = _saveSlot.totalCorner;
        totalCornerNotInPos = _saveSlot.totalCornerNotInPos;

        for(int i = 0; i < defaultPieces.Count; i++)
        {
            if (_saveSlot.puzzleDataList[i].isInScroll == false)
            {
                defaultPieces[i].transform.SetParent(GameManager.Instance.GetPlayerController().GetBoardPuzzleImageTransform());
                defaultPieces[i].GetComponent<PuzzleController>().SetScalePuzzle(CommonUltilities.GetPuzzleRatio(GameManager.Instance.GetPuzzleManager().GetColNumber()));
                defaultPieces[i].GetComponent<PuzzleController>().SetIsInParent(false);
                if (_saveSlot.puzzleDataList[i].isInRightSpot)
                {
                    defaultPieces[i].GetComponent<PuzzleController>().SetToRightPos();
                    GameManager.Instance.IncreaseCurrentScore();
                }
                else
                {                  
                    if (isRotatingMode)
                    {
                        int tempIndex = (int)_saveSlot.puzzleDataList[i].rotateAngle / 90;                      
                        for (int j = 0; j < tempIndex; j++)
                        {
                            defaultPieces[i].GetComponent<PuzzleController>().RotateOutScroll();
                        }
                    }
                    defaultPieces[i].transform.localPosition = new Vector3(_saveSlot.puzzleDataList[i].currentPosX, _saveSlot.puzzleDataList[i].currentPosY, -1);
                    IncreaseOutPuzzle();
                }                         
            }
            else
            {
                if(isRotatingMode)
                {
                    int tempIndex = (int)_saveSlot.puzzleDataList[i].rotateAngle / 90;
                    for (int j = 0; j < tempIndex; j++)
                    {
                        defaultPieces[i].GetComponent<PuzzleController>().RotateInScrollOneTime();
                    }
                    defaultPieces[i].GetComponent<PuzzleController>().SetTotalRotate();
                }             
            }
        }

        StartCoroutine(GroupLoadedPuzzle());
        RescaleContentSize(0);
    }
    IEnumerator GroupLoadedPuzzle()
    {
        yield return new WaitForFixedUpdate();

        for (int i = 0; i < defaultPieces.Count; i++)
        {
            if (defaultPieces[i].GetComponent<PuzzleController>().GetIsInScroll() == false)
            {
                if (defaultPieces[i].GetComponent<PuzzleController>().GetIsRightPos() == false)
                {
                    defaultPieces[i].GetComponent<PuzzleController>().ShootRayBoxCast();
                }
            }
        }
        yield return null;
    }
    public void SetSaveSlotIndex(int index)
    {
        slotIndex = index;
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
}
