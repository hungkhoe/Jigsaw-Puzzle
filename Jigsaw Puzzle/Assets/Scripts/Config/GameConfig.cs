using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameConfig
{
    public const int VERTICAL_PUZZLE_SIZE = 390;
    public const int PUZZLE_TOTAL_DEFAULT = 36;
    public const float PUZZLE_DEFAULT_SCALE = 2.25f;
    public const float PUZZLE_DEFAULT_SCALE_IN_SCROLL = 432;
    public const float PUZZLE_DEFAULT_OFFSET = 0.66f;
    public const float PUZZLE_DEFAULT_WIDTH_WORLDPOS = 1.3f;
    public const float PUZZLE_DISTANCE_RIGHTPOS_DEFAULT = 0.22f;
    public const float PUZZLE_BOX_DEFAULT_SIZE = 0.33f;
    public const float PUZZLE_BOX_RAY_DEFAULT_SIZE = 1.9f;
    public const float PUZZLE_MAIN_AXIS_DISTANCE_MERGE_DEFAULT = 0.53f;
    public const float PUZZLE_SUB_AXIS_DISTANCE_MERGE_DEFAULT = 0.18f;
    public const float PUZZLE_ROW_DEFAULT = 6;
    public const float PUZZLE_SCALE_SMALL = 0.81f;

    public const float CAMERA_DEFUALT_ORTHOGRAPHIC = 5f;
    public const float CAMERA_LIMIT_DRAG_DISTANCE_LEFT = -12f;
    public const float CAMERA_LIMIT_DRAG_DISTANCE_RIGHT = -7f;
    public const float CAMERA_LIMIT_DRAG_DISTANCE_UP = 2f;
    public const float CAMERA_LIMIT_DRAG_DISTANCE_DOWN = -0.6f;

    public const float NEW_PUZZLE_DEFAULT_SCALE_INIT = 51;
    public const float NEW_PUZZLE_DEFAULT_SCALE_IN_WORLD = 42.5f;
    public const float NEW_PUZZLE_SCALE_IN_SCROLL = 5376;
    public const float NEW_PUZZLE_SCALE_IN_SCROLL_WORLD = 8160;
    public const float NEW_PUZZLE_DEFAULT_OFFSET = 0.66f;
    public const float NEW_PUZZLE_LEFT_DISTANCE = 1;
    public const float NEW_PUZZLE_RIGHT_DISTANCE = -1;
    public const float NEW_PUZZLE_UP_DISTANCE = -0.95f;
    public const float NEW_PUZZLE_DOWN_DISTANCE = 0.95f;

    public const float TRANSLATE_X_NOT_IN_SCROLL = 0.1306694f;
    public const float TRANSLATE_Y_NOT_IN_SCROLL = 0.9037781f;
    public const float TRANSLATE_X_IN_SCROLL = -0.1306694f;
    public const float TRANSLATE_Y_IN_SCROLL = 0.9027780f;

    public const int HINT_SKILL_COST = 30;

    public const string BANNER_UNIT_ANDROID = "ca-app-pub-3940256099942544/6300978111";
    public const string NATIVE_ADS_UNIT_ANDROID = "ca-app-pub-8186932248580560/7023553286";
    public const string REWARDED_VIDEO_UNIT_ANDROID = "ca-app-pub-8186932248580560/1962798292";
    public const string INTERSTITIAL_UNIT_ANDROID = "ca-app-pub-8186932248580560/9333682541";

    public const float OFFSET_DAILY_Y = 350f;

    // PUZZLE PHOTOSHOP SOURCE 3 CONFIG


    public const float CATEGORIES_OFFSET_Y = 400;

    public const string PREF_COIN = "coin";
    public const string PREF_BACKGROUND = "background";
    public const string PREF_LAST_LOG_IN = "loginTime";
    public const string PREF_TODAY_LINK = "todayPuzzleLink";
    public const string PREF_EDITOR_CHOICE_LINK = "editorLink";

    public enum IngameButton
    {
        HOME,
        LOCK,
        CHANGE_BACKGROUND,
        HINT,
        PREVIEW
    }
}

[Serializable]
public struct ImageStruct
{
    public List<ImageLink> ImageLink;
}
[Serializable]
public struct ImageLink
{
    public string link;
}
[Serializable]
public struct SaveFile
{
    public List<SaveSlot> saveSlotList;    
}
[Serializable]
public struct SaveSlot
{
    public int rowNumber;
    public int slotIndex;
    public int totalCornerNotInPos;
    public int totalCorner;
    public string imageLink;
    public int percentFinish;
    public bool isRotating;
    public List<PuzzleData> puzzleDataList;
}
[Serializable]
public struct PuzzleData
{   
    public bool isInRightSpot;
    public bool isInScroll;
    public float currentPosX;
    public float currentPosY;
    public float rotateAngle;
}

public enum SoundEffect
{
    DAT_PUZZLE_1,
    DAT_PUZZLE_2,
    POP_UP,
    VICTORY_SOUND
}
public enum MainMusic
{
    THEME_SONG_1,
    THEME_SONG_2,
    THEME_SONG_3,
    THEME_SONG_4
}



