using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class ParseSubCatagory : MonoBehaviour {



    
    private String AssetName ;
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Text testt;

    public string[] model_array;
    string url = "http://importantfile.ourcuet.com/GoogleARCore/getSubcatagory.php";


    public void Start()
    {
        StartCoroutine(LoadSubCatagory());

    }




    IEnumerator LoadSubCatagory()
    {
        PlayerPrefs.SetString("modelName", null);


        if (PlayerPrefs.GetString("modelName").Equals(""))
        {
          
            //download data from server
            print("Data is loading From server");
            WWWForm form = new WWWForm();

            string temp = PlayerPrefs.GetString("saveName");
            if(temp[0].Equals(" "))
            {
                temp = "Bird";
            }
            form.AddField("catagory_name", temp);

            WWW catagoryitem = new WWW(url,form);
            yield return catagoryitem;
            string loadedString = catagoryitem.text;

            model_array = loadedString.Split(';');

            saveScript.SaveStringArray("model", model_array.Length - 1, model_array);
            buttonSetup();
            PlayerPrefs.SetString("modelName", "notNullnow");
            testt.text = model_array.Length + "";

        }
        else
        {
           
            model_array = saveScript.GetStringArray("model");
            buttonSetup();
        }
    }


    public void btnclick(Button btn )
    {
        PlayerPrefs.SetString("saveModel", btn.name);
        SceneManager.LoadScene("03-ARScene");
        

    }
   


    private void buttonSetup()
    {
        for (int i = 0; i < model_array.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab) as GameObject;
            Button btn = button.GetComponent<Button>();
            btn.name = model_array[i];
            button.transform.SetParent(buttonContainer, true);
            button.GetComponentInChildren<Text>().text = model_array[i];
            button.GetComponent<Button>().onClick.AddListener(() => btnclick(btn));

        }

    }



}


