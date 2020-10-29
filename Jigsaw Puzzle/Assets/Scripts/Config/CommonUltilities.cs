using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using UnityEngine.UI;

public static class CommonUltilities 
{  
   public static float GetRescaleRatio()
    {
        float mainScreenX = 1080;
        float mainScreenY = 2400;

        float currentScreenX = Screen.width;
        float currentScreenY = Screen.height;

        float mainRatio = mainScreenY / mainScreenX;
        float currentRatio = currentScreenY / currentScreenX;

        float tempRatio = mainRatio / currentRatio;

        return tempRatio;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rnd = new System.Random();
        for (var i = 0; i < list.Count; i++)
            list.Swap(i, rnd.Next(i, list.Count));
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    public static float GetPuzzleRatio(int _currentColPuzzles)
    {
        return GameConfig.PUZZLE_ROW_DEFAULT / _currentColPuzzles;
    }
    public static float GetOthographicRatio()
    {
        return GameConfig.CAMERA_DEFUALT_ORTHOGRAPHIC / Camera.main.orthographicSize;
    }
}
