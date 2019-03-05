using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using System;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ParseCatagory : MonoBehaviour {


   

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
        .GetReference("Catagory")
        .ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
          {
              if (e2.DatabaseError != null)
              {
                  return;
              }
              else
              {
                  if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
                  {
                      foreach (var childSnapshot in e2.Snapshot.Children)
                      {


                          GameObject button = Instantiate(buttonPrefab) as GameObject;

                          button.transform.SetParent(buttonContainer, true);

                        // button.transform.position = new Vector3(xposition, yposition, zposition);



                        button.GetComponentInChildren<Text>().text = childSnapshot.Child("name").Value.ToString();
                          button.GetComponent<Button>().onClick.AddListener(() => btnclick(childSnapshot.Child("name").Value.ToString()));

                      }
                  }

              }

          };
       
    }
   public void btnclick(string name)
    {
        PlayerPrefs.SetString("saveName", name);
        SceneManager.LoadScene("subCatagoryScene");
        

    }
   

    // Update is called once per frame
    void Update()
    {

    }
}


