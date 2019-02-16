using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DownloadAssete : MonoBehaviour {

    string url = "http://importantfile.ourcuet.com/assetbundle/model_test01";
  
    public Text downloadedtext, percentageText;
    public Scrollbar scroll;
    WWW www;
    double progress;
    AssetBundle mybundle;


    IEnumerator Start()
    {
        while (!Caching.ready)
            yield return null;

        using (www = WWW.LoadFromCacheOrDownload(url, 1))
        {
            yield return www;
            if (www.isDone)
            {
                PlayerPrefs.SetInt("cacheready", 1);
                SceneManager.LoadScene("01-Main");
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
                yield return null;
            }
            var myLoadedAssetBundle = www.assetBundle;

            var mybundle = myLoadedAssetBundle.mainAsset;
        }
    }





    void Update () {

        if (PlayerPrefs.GetInt("cacheready")==0)
        {
            double temp = www.bytesDownloaded / 1000;

            downloadedtext.text = temp + "/139694 kb";
            double percentage = (temp / 139694) * 100;
            percentageText.text = string.Format("{0:F3}", percentage)+" %";
           
        }
        
      
      
    }
}
