using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateUs : MonoBehaviour
{
    // Start is called before the first frame update    
    Color halfColor;
    Color fullColor;

    [SerializeField]
    Image[] starButton;

    int totalStar = 0;

    void Start()
    {
        halfColor = new Color(0.5f, 0.5f, 0.5f, 1);
        fullColor = new Color(1, 1, 1, 1);
    }

    public void SubmitButton()
    {
        if(totalStar > 3)
        {
            this.gameObject.SetActive(false);
            CommonUtils.RateApp();          
        }
        else if (totalStar <= 3)
        {
            this.gameObject.SetActive(false);
            CommonUtils.ShowShortToast("Thanks for your feedback!");
        }
    }

    public void StartRateButton(int index)
    {
        for(int i = 0; i < 5; i++)
        {
            if (i < index)
                starButton[i].color = fullColor;
            else
                starButton[i].color = halfColor;
        }
        totalStar = index;
    }
}
