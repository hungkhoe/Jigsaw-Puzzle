using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mopsicus.InfiniteScroll;
using UnityEngine.UI;

public class DailyScroll : MonoBehaviour
{
    [SerializeField]
    private InfiniteScroll infiniteScroll;
    [SerializeField]
    private RawImage tempImage;

    private int height = 390;
    private int currentIndexLoad;  
  


    // Start is called before the first frame update
    void Start()
    {
        infiniteScroll.OnHeight += onHeight;
        infiniteScroll.OnFill += OnFill;      
    }
    private void OnFill(int index, GameObject item)
    {
        item.GetComponent<PuzzleItem>().currentIndex = index;
        if (currentIndexLoad != 0 && currentIndexLoad >= index)
        {
          
        }
        else
        {
            
        }
    }
    private int onHeight(int index)
    {
        return (int)(height * CommonUltilities.GetRescaleRatio());
    }

}
