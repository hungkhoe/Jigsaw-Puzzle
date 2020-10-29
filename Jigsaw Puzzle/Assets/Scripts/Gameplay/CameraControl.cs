using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private GameObject camera_GameObject;
    // Update is called once per frame
    private Camera cameraMain;
    private Vector3 newPosition;
    private Vector3 startPosition;

    private Vector2 DragStartPosition;
    private Vector2 DragNewPosition;
    private Vector2 Finger0Position;

    private bool isLockMoveScreen = false;
    private bool isDragScreen = false;
    private bool isZooming;
    float DistanceBetweenFingers;

    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.008f;        // The rate of change of the orthographic size in orthographic mode.

    private void Start()
    {
        cameraMain = camera_GameObject.GetComponent<Camera>();
    }
    void Update()
    {
        if(GameManager.Instance.GetPlayerController().GetCurrClickPuzzle() != null)
        {
            return;
        }
        if (GameManager.Instance.isVictoryRunning)
            return;
        if (GameManager.Instance.GetIsOpenBackGroundScroll())
            return;
        if (GameManager.Instance.isSettingOpen)
        {
            return;
        }
        if (Input.touchCount == 0 && isZooming)
        {
            isZooming = false;
        }
#if UNITY_EDITOR
        OnDragCamera();
#else
        if(Input.touchCount == 1)
        {
            OnDragCamera();
        }
        else if(Input.touchCount == 2)
        {
            OnZoomCameraTest();
        }        
#endif
    }
    private Vector3 GetWorldPosition()
    {
        return cameraMain.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnDragCamera()
    {
        if (!isLockMoveScreen)
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDragScreen = false;
                if (Puzzle.Game.Ads.AdManager.Instance != null)
                    Puzzle.Game.Ads.AdManager.Instance.SetColliderPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - (4.6f / CommonUltilities.GetOthographicRatio())));
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (GameManager.Instance.GetPlayerController().GetCurrClickPuzzle() == null && GameManager.Instance.GetPlayerController().GetIsDraggingScroll() == false)
                {
                    if(Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.4f)
                    {
                        isDragScreen = true;
                        startPosition = GetWorldPosition();
                    }                               
                }
            }
            if (isDragScreen)
            {
                if(!isZooming)
                {
                    newPosition = GetWorldPosition();
                    Vector3 positionDifference = newPosition - startPosition;
                    camera_GameObject.transform.Translate(-positionDifference);
                    if (camera_GameObject.transform.position.x >= GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_RIGHT)
                    {
                        camera_GameObject.transform.position = new Vector3(GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_RIGHT, camera_GameObject.transform.position.y, -10);
                    }
                    else if (camera_GameObject.transform.position.x <= GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_LEFT)
                    {
                        camera_GameObject.transform.position = new Vector3(GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_LEFT, camera_GameObject.transform.position.y, -10);
                    }

                    if (camera_GameObject.transform.position.y >= GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_UP)
                    {
                        camera_GameObject.transform.position = new Vector3(camera_GameObject.transform.position.x, GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_UP, -10);
                    }
                    else if (camera_GameObject.transform.position.y <= GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_DOWN)
                    {
                        camera_GameObject.transform.position = new Vector3(camera_GameObject.transform.position.x, GameConfig.CAMERA_LIMIT_DRAG_DISTANCE_DOWN, -10);
                    }
                }              
            }            
        }
    }
    private void OnZoomingCamera()
    {
        if (GameManager.Instance.GetPlayerController().GetCurrClickPuzzle() == null && GameManager.Instance.GetPlayerController().GetIsDraggingScroll() == false)
        {
            if (Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                isZooming = true;

                DragNewPosition = GetWorldPositionOfFinger(1);
                Vector2 PositionDifference = DragNewPosition - DragStartPosition;

                if (Vector2.Distance(DragNewPosition, Finger0Position) < DistanceBetweenFingers)
                {
                    camera_GameObject.GetComponent<Camera>().orthographicSize += (PositionDifference.magnitude) * 2f;
                    if(camera_GameObject.GetComponent<Camera>().orthographicSize > 6f)
                    {
                        camera_GameObject.GetComponent<Camera>().orthographicSize = 6f;
                    }
                }                 

                if (Vector2.Distance(DragNewPosition, Finger0Position) >= DistanceBetweenFingers)
                {
                    camera_GameObject.GetComponent<Camera>().orthographicSize -= (PositionDifference.magnitude) * 2f;
                    if (camera_GameObject.GetComponent<Camera>().orthographicSize < 3f)
                    {
                        camera_GameObject.GetComponent<Camera>().orthographicSize = 3f;
                    }
                }                  
                DistanceBetweenFingers = Vector2.Distance(DragNewPosition, Finger0Position);
            }
            DragStartPosition = GetWorldPositionOfFinger(1);
            Finger0Position = GetWorldPositionOfFinger(0);
        }       
    }
    private void OnZoomCameraTest()
    {
        //Update Plane
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // If the camera is orthographic...
        if (cameraMain.orthographic)
        {
            // ... change the orthographic size based on the change in distance between the touches.
          
            cameraMain.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
            if (cameraMain.orthographicSize > 5.5f)
            {
                cameraMain.orthographicSize = 5.5f;
            }
            // Make sure the orthographic size never drops below zero.
            cameraMain.orthographicSize = Mathf.Max(cameraMain.orthographicSize, 3);
            if (Puzzle.Game.Ads.AdManager.Instance != null)
                Puzzle.Game.Ads.AdManager.Instance.SetColliderPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - (4.6f / CommonUltilities.GetOthographicRatio())));
        }
        else
        {
            // Otherwise change the field of view based on the change in distance between the touches.
            cameraMain.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

            // Clamp the field of view to make sure it's between 0 and 180.
            cameraMain.fieldOfView = Mathf.Clamp(cameraMain.fieldOfView, 0.1f, 179.9f);
        }
    }

    public void SetIsLockMoveScreen(bool _isDragScreen)
    {
        isLockMoveScreen = _isDragScreen;
    }
    public bool GetIsLockMoveScreen()
    {
        return isLockMoveScreen;
    }
    private Vector2 GetWorldPositionOfFinger(int FingerIndex)
    {
        return camera_GameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.GetTouch(FingerIndex).position);
    }
}