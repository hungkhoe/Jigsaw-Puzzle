using UnityEngine;


public class CommonUtils
{
    public static void ShareApp()
    {
#if UNITY_ANDROID
        var shareSubject = "WoodPuzzle";
        var shareMessage = "url";

        //isProcessing = true;

        //call createChooser method of activity class
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity =
            unity.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        shareMessage = "https://play.google.com/store/apps/details?id=" + context.Call<string>("getPackageName");


        //Create intent for action send
        AndroidJavaClass intentClass =
            new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject =
            new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>
            ("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

        //put text and subject extra
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        intentObject.Call<AndroidJavaObject>
            ("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
        intentObject.Call<AndroidJavaObject>
            ("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);


        AndroidJavaObject chooser =
            intentClass.CallStatic<AndroidJavaObject>
            ("createChooser", intentObject, "Share your high score");
        currentActivity.Call("startActivity", chooser);

        //yield return new WaitUntil(() => isFocus);
        //isProcessing = false;
#endif
    }

//    public static void ShareHighScore(int _highScore, GameMode _modeID)
//    {
//#if UNITY_ANDROID
//        var shareSubject = "WoodPuzzle";
//        var shareMessage = "url";
//        string strGameMode = "";
//        switch (_modeID)
//        {
//            case GameMode.LEVEL_MODE:
//                strGameMode = "Level Mode. ";
//                break;
//            case GameMode.TIME_MODE:
//                strGameMode = "Time Mode. ";
//                break;
//            case GameMode.HIVE_MODE:
//                strGameMode = "Hive Mode. ";
//                break;
//            default:
//                break;
//        }

//        string _content = "I got " + _highScore + " score in Wooh Puzzle " + strGameMode + "Come and play with me to see if you can beat my high score!";
//        //isProcessing = true;

//        //call createChooser method of activity class
//        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//        AndroidJavaObject currentActivity =
//            unity.GetStatic<AndroidJavaObject>("currentActivity");

//        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
//        shareMessage = "https://play.google.com/store/apps/details?id=" + context.Call<string>("getPackageName");
//        string finalMessage = string.Format("{0}\n{1}", _content, shareMessage);

//        //Create intent for action send
//        AndroidJavaClass intentClass =
//            new AndroidJavaClass("android.content.Intent");
//        AndroidJavaObject intentObject =
//            new AndroidJavaObject("android.content.Intent");
//        intentObject.Call<AndroidJavaObject>
//            ("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

//        //put text and subject extra
//        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
//        intentObject.Call<AndroidJavaObject>
//            ("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
//        intentObject.Call<AndroidJavaObject>
//            ("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), finalMessage);


//        AndroidJavaObject chooser =
//            intentClass.CallStatic<AndroidJavaObject>
//            ("createChooser", intentObject, "Share your high score");
//        currentActivity.Call("startActivity", chooser);

//        //yield return new WaitUntil(() => isFocus);
//        //isProcessing = false;
//#endif
//    }
    public static void RateApp()
    {
#if UNITY_ANDROID
        AndroidJavaClass appUtilClz = new AndroidJavaClass("com.bstech.bsgame.core.AppUtil");

        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        appUtilClz.CallStatic("openAppStoreMarket", context);
#endif
    }


    public static void ShowShortToast(string msg)
    {
        _ShowToast(msg, 0);
    }
    public static void ShowLongToast(string msg)
    {
        _ShowToast(msg, 1);
    }

    private static void _ShowToast(string msg, int type)
    {
#if UNITY_ANDROID
        AndroidJavaClass appUtilClz = new AndroidJavaClass("com.bstech.bsgame.core.AppUtil");

        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        if (type > 0)
        {
            appUtilClz.CallStatic("showLongToast", context, msg);
        }
        else
        {
            appUtilClz.CallStatic("showShortToast", context, msg);
        }
#endif
    }
}

