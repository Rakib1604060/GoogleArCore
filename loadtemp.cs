using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadtemp : MonoBehaviour
{

    string modelname = "BUS";

    private GameObject model;
    public Transform empty;


    string url = "http://rakib.ourcuet.com/StandaloneWindows/compact";

    IEnumerator Start()
    {
        Debug.Log("start hoise");

        while (!Caching.ready)
            yield return null;



        using (var www = WWW.LoadFromCacheOrDownload(url, 1))
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
                yield return null;
            }
            AssetBundle bundle = www.assetBundle;
            print("eitok aisse");
            model = Instantiate(bundle.mainAsset) as GameObject;
            bundle.Unload(false);
            model.transform.localScale = new Vector3(10f, 10f, 10f);
            model.transform.SetParent(empty);
        }

    }
}
