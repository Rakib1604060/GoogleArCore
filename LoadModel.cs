using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

using GoogleARCore.Examples.Common;



public class LoadModel : MonoBehaviour {

    public Camera FirstPersonCamera;
    public AudioClip[] audioclips;
    public Text testtext;
    public Button replayButton, BackButton;

    public GameObject DetectedPlanePrefab;
    int iy = 0;
    int totalobjectallowed = 10;
    int totalojectspawn = 0;
    public static GameObject model;

    float prevTouchDistance;
    float zoomSpeed = 0.2f;
    public AudioSource myaudiosource;


    private GameObject ourmodel;

    public GameObject AndyPointPrefab;

    public GameObject SearchingForPlaneUI;

    AudioClip modelaudio;

    private const float k_ModelRotation = 180.0f;

    public static AssetBundle mybundle;

    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
     
    string url = "http://importantfile.ourcuet.com/assetbundle/model_test01";
  

    WWW www;

    private bool m_IsQuitting = false;

    public void  Start()
    {
        replayButton.onClick.AddListener(() => Replay());
        BackButton.onClick.AddListener(() => Back());
        replayButton.enabled = false;
        testtext.text = "Start FUnction";
        StartCoroutine(loadModel());


    }

    IEnumerator loadModel()
    {
       
        while (!Caching.ready)
            yield return null;

        using (www = WWW.LoadFromCacheOrDownload(url, 1))
        {
            yield return www;
          
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
                yield return null;
            }


            mybundle = www.assetBundle;
            string model_name = PlayerPrefs.GetString("saveModel");

            model = (GameObject)mybundle.LoadAsset(model_name);
            testtext.text = PlayerPrefs.GetString("saveModel");
           
           
        }
    }

     

  public void Update()
    {

       
        if (Input.touchCount != 0)
        {
            _PinchtoZoom();
        }

      
        _UpdateApplicationLifecycle();

        // Hide snackbar when currently tracking at least one plane.

        Session.GetTrackables<DetectedPlane>(m_AllPlanes);
        bool showSearchingUI = true;
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                showSearchingUI = false;
                break;
            }
        }

        SearchingForPlaneUI.SetActive(showSearchingUI);

        // If the player has not touched the screen, we are done with this update.

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                
                GameObject prefab;
                if (hit.Trackable is FeaturePoint)
                {
                    prefab = model;
                }
                else
                {
                    prefab = model;
                }

                // Instantiate Andy model at the hit pose.
                if (totalojectspawn < totalobjectallowed)
                {
                    PlaySound();
                    ourmodel = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
                    ourmodel.transform.Rotate(0, k_ModelRotation, 0, Space.Self);
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);


                    // Make Andy model a child of the anchor.
                    ourmodel.transform.parent = anchor.transform;
                    totalojectspawn++;
                     }

             
               
            }
        }
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    /// 


    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

    //pinch to zoom in the AR SECTIOON
    public void _PinchtoZoom()
    {
        if (Input.touchCount == 2)

        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


            float pinchAmount = deltaMagnitudeDiff * 0.02f * Time.deltaTime;
            ourmodel.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);


        }

       
    }

    //Play the Audio from the Array

    private void PlaySound()
    {
        string model_name = PlayerPrefs.GetString("saveModel");

        for(int i = 0; i < audioclips.Length; i++)
        {
            if (model_name.Equals(audioclips[i].name,System.StringComparison.InvariantCultureIgnoreCase))
            {
                myaudiosource.clip = audioclips[i];
                replayButton.enabled = true ;

                myaudiosource.Play();
                break;
            }
        }
    }



    // Replay button click
    private void Replay()
    {
        myaudiosource.Play();
    }

    //Back Button Press Function

    private void Back()
    {
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public static void sendmodel()
    {
        string model_name = PlayerPrefs.GetString("saveModel");
        model = (GameObject)mybundle.LoadAsset(model_name);

  
    }


}


