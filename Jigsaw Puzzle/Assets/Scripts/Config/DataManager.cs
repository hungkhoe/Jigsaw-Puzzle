using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DataManager : MonoBehaviour
{    
    public SaveFile saveFile;

    private static DataManager instance = null;
    public static DataManager Instance
    {
        get
        {
            return instance;
        }
    }

    public List<SaveSlot> inCompletedLink;
    public List<SaveSlot> completedLink;

    public List<string> imageLinkKeys = new List<string>();

    public Dictionary<string, SaveSlot> saveSlotDictionary;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    void Start()
    {
        LoadGame();
    }

    private void OnEnable()
    {
        inCompletedLink = new List<SaveSlot>();
        completedLink = new List<SaveSlot>();
    }

    public void LoadGame()
    {
        inCompletedLink.Clear();
        completedLink.Clear();
        imageLinkKeys.Clear();

        string path = Application.persistentDataPath + "SaveFile.json";
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(path);
        }
        catch
        {

        }
       
        if (jsonString != null)
        {
            saveFile = JsonUtility.FromJson<SaveFile>(jsonString);
            for (int i = 0; i < saveFile.saveSlotList.Count; i++)
            {
                if (saveFile.saveSlotList[i].percentFinish == 100)
                {
                    completedLink.Add(saveFile.saveSlotList[i]);
                }
                else
                {
                    inCompletedLink.Add(saveFile.saveSlotList[i]);                       
                }
                imageLinkKeys.Add(saveFile.saveSlotList[i].imageLink);
            }
        }
        else
        {
            saveFile = new SaveFile();
            saveFile.saveSlotList = new List<SaveSlot>();
        }
    }

    public void SaveGame(SaveSlot _saveSlot, int _slotIndex)
    {
        if(_slotIndex != -1)
        {
            saveFile.saveSlotList[_slotIndex] = _saveSlot;
        }   
        else
        {
            _saveSlot.slotIndex = saveFile.saveSlotList.Count;          
            saveFile.saveSlotList.Add(_saveSlot);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GetPuzzleManager().SetSaveSlotIndex(_saveSlot.slotIndex);
            }
        }      
        string save = JsonUtility.ToJson(saveFile);
        System.IO.File.WriteAllText(Application.persistentDataPath + "SaveFile.json", save);
        LoadGame();
    }
}
