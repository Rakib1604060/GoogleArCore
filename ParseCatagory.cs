using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ParseCatagory : MonoBehaviour {
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Text test;




    public string[] catagory_array;
    string url= "http://importantfile.ourcuet.com/GoogleARCore/getcatagory.php";


    public void Start()
    {
        if (PlayerPrefs.GetInt("cacheready") == 0)
        {
            SceneManager.LoadScene("00-Model");
        }
          StartCoroutine(LoadCatagory());

    }


    IEnumerator LoadCatagory()
    {
        
        if (PlayerPrefs.GetString("catagory").Equals(""))
        {
           


            //download data from server
            print("Data is loading From server");
            WWW catagoryitem = new WWW(url);
            yield return catagoryitem;
            string loadedString = catagoryitem.text;

            catagory_array = loadedString.Split(';');

            saveScript.SaveStringArray("catagory", catagory_array.Length-1, catagory_array);
            buttonSetup();
            PlayerPrefs.SetString("catagory", "notNullnow");


        }
        else
        {
            test.text = " memory theke nicche";
            catagory_array= saveScript.GetStringArray("catagory");
           
            
            test.text = ""+catagory_array.Length;
            buttonSetup();
        }
    }

    


    private void buttonSetup()
    { 
        for(int i = 0; i < catagory_array.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab) as GameObject;
            Button btn = button.GetComponent<Button>();
            btn.name = catagory_array[i];
            button.transform.SetParent(buttonContainer, true);
            button.GetComponentInChildren<Text>().text = catagory_array[i];
            button.GetComponent<Button>().onClick.AddListener(() => btnclick(btn));

        }
       
    }
 
    
   public void btnclick(Button btn)
    {
        test.text = btn.name;
        PlayerPrefs.SetString("saveName", btn.name);
        SceneManager.LoadScene("02-Subcatagory");
        

    }
   


}


