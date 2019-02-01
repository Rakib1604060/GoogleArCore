using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using System;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class ParseSubCatagory : MonoBehaviour {



    
    private String AssetName ;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

   




    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected virtual void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();

            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

    }
    protected virtual void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;

        app.SetEditorDatabaseUrl("https://arzoo-c2f52.firebaseio.com/");
        if (app.Options.DatabaseUrl != null)
            app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
            StartListener();

    }
    void StartListener()
    {
        FirebaseDatabase.DefaultInstance
        .GetReference(PlayerPrefs.GetString("saveName"))
        .GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {

            }
            else if (task.IsCompleted)
            {
                if (task.Result != null && task.Result.ChildrenCount > 0)
                {
                    foreach (var childSnapshot in task.Result.Children)
                    {
                     

                        GameObject button = Instantiate(buttonPrefab) as GameObject;
                        button.transform.SetParent(buttonContainer,true);

                        button.GetComponentInChildren<Text>().text = childSnapshot.Child("name").Value.ToString();
                        button.GetComponent<Button>().onClick.AddListener(() => btnclick(childSnapshot.Child("name").Value.ToString()));

                    }
                }
            }
        });
    }
   public void btnclick(string name)
    {
        PlayerPrefs.SetString("saveModel", name);
        SceneManager.LoadScene("HelloAR 1");
        

    }
   

    // Update is called once per frame
    void Update()
    {

    }

   



}


