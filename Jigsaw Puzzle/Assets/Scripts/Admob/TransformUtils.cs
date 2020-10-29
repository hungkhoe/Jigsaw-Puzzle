using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TransformUtils
{
    public static Transform GetTransformRecursive(Transform parent, string name)
    {

        Transform transformResult = parent.Find(name);
        if (transformResult != null)
        {
            return transformResult;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            transformResult = parent.GetChild(i).Find(name);
            if (transformResult != null)
            {
                return transformResult;
            }
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            for (int j = 0; j < parent.GetChild(i).childCount; j++)
            {
                transformResult = GetTransformRecursive(parent.GetChild(i).GetChild(j), name);
                if (transformResult != null)
                {
                    return transformResult;
                }
            }
        }

        return null;
    }

    public static Transform GetTransformRecursive(GameObject parentObj, string name)
    {
        Transform parent = parentObj.transform;
        Transform transformResult = parent.Find(name);
        if (transformResult != null)
        {
            return transformResult;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            transformResult = parent.GetChild(i).Find(name);
            if (transformResult != null)
            {
                return transformResult;
            }
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            for (int j = 0; j < parent.GetChild(i).childCount; j++)
            {
                transformResult = GetTransformRecursive(parent.GetChild(i).GetChild(j), name);
                if (transformResult != null)
                {
                    return transformResult;
                }
            }
        }

        return null;
    }

    public static GameObject GetGameObjectRecursive(Transform parent, string name)
    {
        Transform transformResult = GetTransformRecursive(parent, name);
        if (transformResult != null)
        {
            return transformResult.gameObject;
        }
        return null;
    }

    public static GameObject GetGameObjectRecursive(GameObject parent, string name)
    {
        Transform transformResult = GetTransformRecursive(parent, name);
        if (transformResult != null)
        {
            return transformResult.gameObject;
        }
        return null;
    }

    public static Transform GetTransformRecursive(Transform grand, string nameParent, string name)
    {
        return GetTransformRecursive(GetTransformRecursive(grand, nameParent), name);
    }

    public static Text GetTextRecursive(Transform parent, string name)
    {
        return GetTransformRecursive(parent, name).GetComponent<Text>();
    }

    public static Button GetButtonRecursive(Transform parent, string name)
    {
        return GetTransformRecursive(parent, name).GetComponent<Button>();
    }

    public static Image GetImageRecursive(Transform parent, string name)
    {
        return GetTransformRecursive(parent, name).GetComponent<Image>();
    }
}

