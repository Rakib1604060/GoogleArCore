

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using System;
    using UnityEngine.UI;
 

    using System.Collections;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif




    public class Test01Script : MonoBehaviour
    {
        //public Variables ...


        public Camera FirstPersonCamera;
        public GameObject DetectedPlanePrefab;
        public AudioSource Audio;
        public GameObject[] arrayofObject;
        public List<AudioClip> audiolist;
        public Button startButton;
        public List<int> list;
        public Text xtext;




        //private and Other Variables Initializing

        private Boolean testPossible = false;
        private Boolean detected = false;
        GameObject andyObject;
        int totalobjectallowed = 1;
        int objectspawn = 0;
        private GameObject object1, object2, object3;
        int correctModelNumber;
        HashSet<int> hashSet = new HashSet<int>();
        private GameObject model;
        public GameObject SearchingForPlaneUI;
        private const float k_ModelRotation = 180.0f;
        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
        private bool m_IsQuitting = false;
        private string modelname;
        private string url;
        int buttonclickCounter = 0;

        int i = 0;





        //Start Function;

        private void Start()
        {
            xtext.text = 0 + "";

            while (hashSet.Count < 6)
            {
                int x = UnityEngine.Random.Range(0, 7);
                hashSet.Add(x);
            }
            list = new List<int>(hashSet);
            startButton.onClick.AddListener(() => startTest());

        }





        public void Update()
        {
            if (object1 != null)
            {
                measure();
                _PinchtoZoom();
                

            }
               
            
            _UpdateApplicationLifecycle();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);


            bool showSearchingUI = true;


            if (m_AllPlanes[0].TrackingState == TrackingState.Tracking)
            {
                showSearchingUI = false;
                detected = true;
             
                startButton.GetComponentInChildren<Text>().text = "Next";


            }
            if (showSearchingUI == false)
            {
                ShowExtentxz();
            }

            SearchingForPlaneUI.SetActive(showSearchingUI);
        }

        private void startTest()
        {
            buttonclickCounter++;

            if (object1 != null || object2 != null || object3 != null)
            {
                Destroy(object1);
                Destroy(object2);
                Destroy(object3);
            }
            correctModelNumber = list[buttonclickCounter];
            Audio.clip = audiolist[correctModelNumber];
            Audio.Play();

            if (detected == true )
            {
                int a = UnityEngine.Random.Range(0, 7);
                int b = UnityEngine.Random.Range(0, 7);

                while (a == correctModelNumber)
                {
                    a = UnityEngine.Random.Range(0, 7);
                }
                while (b == correctModelNumber)
                {
                    b = UnityEngine.Random.Range(0, 7);
                }

                Vector3 pos = m_AllPlanes[0].CenterPose.position;
                
                Vector3 pos1 = new Vector3(m_AllPlanes[0].CenterPose.position.x + 0.5f, m_AllPlanes[0].CenterPose.position.y, m_AllPlanes[0].CenterPose.position.z);
                Vector3 pos2 = new Vector3(m_AllPlanes[0].CenterPose.position.x -0.5f, m_AllPlanes[0].CenterPose.position.y, m_AllPlanes[0].CenterPose.position.z);
                
                object1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                object2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                object3.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                object1 = Instantiate(arrayofObject[correctModelNumber], pos, FirstPersonCamera.transform.rotation);
                object2 = Instantiate(arrayofObject[a], pos1, FirstPersonCamera.transform.rotation);
                object3 = Instantiate(arrayofObject[b], pos2, FirstPersonCamera.transform.rotation);



            }
           

        }

      
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

      

        public void measure()
        {
            float dist = Vector3.Distance(FirstPersonCamera.transform.position, andyObject.transform.position);
            
        }

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
                object1.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
                object2.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
                object3.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
            }
        }

       


        //show the extent of x and z axis
        private void ShowExtentxz()
        {
            float x= m_AllPlanes[0].ExtentX;
            if (x > 1.0f)
            {
                testPossible = true;

            }
          
        }

    



    }}  

