using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ImageLinkManager : MonoBehaviour
{
    // Start is called before the first frame update   
    private static ImageLinkManager instance = null;
    public static ImageLinkManager Instance
    {
        get
        {
            return instance;
        }      
    }

    public Texture failedToLoadImage;
    // Main Scroll Link Asset
    public ImageStruct mainmenuLinkAsset;

    public Texture[] mainmenuTextureContainer;

    // Categories Link Asset
    public ImageStruct cityLinkAsset;
    public ImageStruct natureLinkAsset;
    public ImageStruct flowerLinkAsset;
    public ImageStruct foodLinkAsset;
    public ImageStruct animalLinkAsset;
    public ImageStruct macroLinkAsset;
    public ImageStruct artLinkAsset;
    public ImageStruct animeLinkAsset;
    public ImageStruct loveLinkAsset;
    public ImageStruct fantasyLinkAsset;
    public ImageStruct musicLinkAsset;
    public ImageStruct holidayLinkAsset;
    public ImageStruct minimalismLinkAsset;
    public ImageStruct smilesLinkAsset;
    public ImageStruct spaceLinkAsset;
    public ImageStruct sportLinkAsset;
    public ImageStruct wordsLinkAsset;

    public Texture[] cityTextureCotainer;
    public Texture[] natureTextureCotainer;
    public Texture[] flowerTextureCotainer;
    public Texture[] foodTextureCotainer;
    public Texture[] animalTextureCotainer;
    public Texture[] macroTextureCotainer;
    public Texture[] artTextureCotainer;
    public Texture[] animeTextureCotainer;
    public Texture[] loveTextureCotainer;
    public Texture[] fantasyTextureCotainer;
    public Texture[] musicTextureCotainer;
    public Texture[] holidaysTextureCotainer;
    public Texture[] minimalismTextureCotainer;
    public Texture[] smilesTextureCotainer;
    public Texture[] spaceTextureCotainer;
    public Texture[] sportTextureCotainer;
    public Texture[] wordTextureCotainer;

    private Davinci imageLoad;
    public Davinci loadImagePreview;

    public Texture2D editorImage;
    public Texture2D todayImage;

    public ImageStruct thisMonthLinkAsset;
    public Texture[] thisMonthTextureContainer;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            imageLoad = Davinci.get();
            loadImagePreview = Davinci.get();
            loadImagePreview.oneTexture = true;
            DontDestroyOnLoad(imageLoad);
            DontDestroyOnLoad(loadImagePreview);
            OnInit();

            DontDestroyOnLoad(this);
        } 
    }
    private void OnInit()
    {     
        // load categories
        // city asset

        TextAsset json = Resources.Load<TextAsset>("CityAsset");
        cityLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(cityLinkAsset);
        //nature asset
        json = Resources.Load<TextAsset>("NatureAsset");
        natureLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(natureLinkAsset);

        // flower asset
        json = Resources.Load<TextAsset>("FlowerAsset");
        flowerLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(flowerLinkAsset);

        // food asset
        json = Resources.Load<TextAsset>("FoodAsset");
        foodLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(foodLinkAsset);

        //animal aaset
        json = Resources.Load<TextAsset>("AnimalAsset");
        animalLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(animalLinkAsset);

        //macro asset
        json = Resources.Load<TextAsset>("MacroAsset");
        macroLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(macroLinkAsset);

        //Art asset
        json = Resources.Load<TextAsset>("ArtAsset");
        artLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(artLinkAsset);

        //anime asset
        json = Resources.Load<TextAsset>("AnimeAsset");
        animeLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(animeLinkAsset);

        //love asset
        json = Resources.Load<TextAsset>("LoveAsset");
        loveLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(loveLinkAsset);

        //fantasy asset
        json = Resources.Load<TextAsset>("FantasyAsset");
        fantasyLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(fantasyLinkAsset);

        //music asset
        json = Resources.Load<TextAsset>("MusicAsset");
        musicLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(musicLinkAsset);

        //holidays asset
        json = Resources.Load<TextAsset>("HolidaysAsset");
        holidayLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(holidayLinkAsset);

        //minimalism asset
        json = Resources.Load<TextAsset>("MinimalismAsset");
        minimalismLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(minimalismLinkAsset);

        //smiles asset
        json = Resources.Load<TextAsset>("SmilesAsset");
        smilesLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(smilesLinkAsset);

        //space asset
        json = Resources.Load<TextAsset>("SpaceAsset");
        spaceLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(spaceLinkAsset);

        //sport asset
        json = Resources.Load<TextAsset>("SportAsset");
        sportLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(sportLinkAsset);

        //word asset
        json = Resources.Load<TextAsset>("WordAsset");
        wordsLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);
        AddLinkToMainList(wordsLinkAsset);

        mainmenuLinkAsset.ImageLink.Shuffle();

        mainmenuTextureContainer = new Texture[mainmenuLinkAsset.ImageLink.Count];

        cityTextureCotainer = new Texture[cityLinkAsset.ImageLink.Count];

        natureTextureCotainer = new Texture[natureLinkAsset.ImageLink.Count];

        flowerTextureCotainer = new Texture[flowerLinkAsset.ImageLink.Count];

        foodTextureCotainer = new Texture[foodLinkAsset.ImageLink.Count];

        animalTextureCotainer = new Texture[animalLinkAsset.ImageLink.Count];

        macroTextureCotainer = new Texture[macroLinkAsset.ImageLink.Count];

        artTextureCotainer = new Texture[artLinkAsset.ImageLink.Count];

        animeTextureCotainer = new Texture[animeLinkAsset.ImageLink.Count];

        loveTextureCotainer = new Texture[loveLinkAsset.ImageLink.Count];

        fantasyTextureCotainer = new Texture[fantasyLinkAsset.ImageLink.Count];

        musicTextureCotainer = new Texture[musicLinkAsset.ImageLink.Count];

        holidaysTextureCotainer = new Texture[holidayLinkAsset.ImageLink.Count];

        minimalismTextureCotainer = new Texture[minimalismLinkAsset.ImageLink.Count];

        smilesTextureCotainer = new Texture[smilesLinkAsset.ImageLink.Count];

        spaceTextureCotainer = new Texture[spaceLinkAsset.ImageLink.Count];

        sportTextureCotainer = new Texture[sportLinkAsset.ImageLink.Count];

        wordTextureCotainer = new Texture[wordsLinkAsset.ImageLink.Count];

        LoadMonthData();

        thisMonthTextureContainer = new Texture[thisMonthLinkAsset.ImageLink.Count];
    }
    public Texture[] GetMainTexture(int index)
    {
        switch (index)
        {
            case 1:
                return cityTextureCotainer;
            case 2:
                return natureTextureCotainer;
            case 3:
                return flowerTextureCotainer;
            case 4:
                return foodTextureCotainer;
            case 5:
                return animalTextureCotainer;
            case 6:
                return macroTextureCotainer;
            case 7:
                return artTextureCotainer;
            case 8:
                return animeTextureCotainer;
            case 9:
                return loveTextureCotainer;
            case 10:
                return fantasyTextureCotainer;
            case 11:
                return musicTextureCotainer;
            case 12:
                return holidaysTextureCotainer;
            case 13:
                return minimalismTextureCotainer;
            case 14:
                return smilesTextureCotainer;
            case 15:
                return spaceTextureCotainer;
            case 16:
                return sportTextureCotainer;
            case 17:
                return wordTextureCotainer;
            default:
                break;
        }
        return null;
    }
    public Davinci GetImageLoad()
    {
        if(imageLoad == null)
        {
            imageLoad = Davinci.get();
            return imageLoad;
        }
        return imageLoad;
    }
    private void AddLinkToMainList(ImageStruct _imageStruct)
    {
        for(int i = 0; i < _imageStruct.ImageLink.Count; i++)
        {
            mainmenuLinkAsset.ImageLink.Add(_imageStruct.ImageLink[i]);
        }
    }
    public void LoadMonthData()
    {
        DateTime newDateTemp = System.DateTime.Now;
        string monthName = newDateTemp.ToString("MMMM");
        TextAsset json = Resources.Load<TextAsset>(monthName);
        thisMonthLinkAsset = JsonUtility.FromJson<ImageStruct>(json.text);        
    }
}
