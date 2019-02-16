
namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using System;
   
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
 

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
       
        public Button startButton;
        public List<int> list;
        public Text scoreText;
    



        //private and Other Variables Initializing

        private Boolean testPossible = false;
        private Boolean detected = false, tested = false;
        GameObject andyObject, model1, model2, model3;
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
        private string url= "http://importantfile.ourcuet.com/assetbundle/model_test01";
        int buttonclickCounter = 0;
        private Boolean zoompossible = false;
        int scoreCounter = 0;
        int i = 0;
        private GameObject temp_model;
        WWW www;
        private Boolean found = false;
        string[] model_array;
        private AssetBundle bundle, modelbundle;

        private  string urlarray= "http://importantfile.ourcuet.com/GoogleARCore/getallmodel.php";
       



        public void Start()
        {
            
           while (hashSet.Count < 10)
            {
                int x = UnityEngine.Random.Range(0, 30);
                hashSet.Add(x);
            }
            list = new List<int>(hashSet);
            startButton.onClick.AddListener(() => startTest());
            StartCoroutine(loadModel());
           
        }





        //downloading model from server of cache
        IEnumerator loadModel()
        {
            if (PlayerPrefs.GetString("allmodel").Equals(""))
            {

                print("Data is loading From server");
                WWW catagoryitem = new WWW(urlarray);
                yield return catagoryitem;
                string loadedString = catagoryitem.text;

                model_array = loadedString.Split(';');

                saveScript.SaveStringArray("catagory", model_array.Length - 1, model_array);
                WWW audioreq = new WWW(url);

                PlayerPrefs.SetString("allmodel", "notNullnow");

            }
            else
            {
                model_array = saveScript.GetStringArray("allmodel");
            }


        }


       

        

        public void Update()
        {
            

            if (zoompossible)
            {
                 measure();
                _PinchtoZoom();
            }
           
            _UpdateApplicationLifecycle();
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);
             bool showSearchingUI = true;

            
            if (m_AllPlanes.Count>0)
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
            tested = true;
            if (buttonclickCounter == 11)
            {
                PlayerPrefs.SetInt("TotalScore", scoreCounter);
                
                SceneManager.LoadScene("03-Score");
                
            }

            if (buttonclickCounter < 12)
            {

                buttonclickCounter++;

              

                if (object1 != null || object2 != null || object3 != null)
                {
                    Destroy(object1);
                    Destroy(object2);
                    Destroy(object3);
                }
                correctModelNumber = list[buttonclickCounter];
               

                if (detected == true)
                {
                    AudioClip myclip = (AudioClip)bundle.LoadAsset(model_array[correctModelNumber]);

                    Audio.clip = myclip;
                    Audio.Play();

                    int a = UnityEngine.Random.Range(0, 30);
                    int b = UnityEngine.Random.Range(0, 30);

                    while (a == correctModelNumber)
                    {
                        a = UnityEngine.Random.Range(0, 30);
                    }
                    while (b == correctModelNumber)
                    {
                        b = UnityEngine.Random.Range(0, 30);
                    }

                    setmodel(a, b);
                    int position  = UnityEngine.Random.Range(1, 3);


                    Vector3 pos = m_AllPlanes[0].CenterPose.position;
                    Vector3 pos1 = new Vector3(m_AllPlanes[0].CenterPose.position.x+1f , m_AllPlanes[0].CenterPose.position.y, m_AllPlanes[0].CenterPose.position.z + 0.5f);
                    Vector3 pos2 = new Vector3(m_AllPlanes[0].CenterPose.position.x-1f, m_AllPlanes[0].CenterPose.position.y, m_AllPlanes[0].CenterPose.position.z - 0.5f);

                    if (position == 1)
                    {
                        object1 = Instantiate(model, pos, FirstPersonCamera.transform.rotation);
                        object2 = Instantiate(model2, pos1, FirstPersonCamera.transform.rotation);
                        object3 = Instantiate(model3, pos2, FirstPersonCamera.transform.rotation);

                    }
                    else if(position==2)
                    {
                        object1 = Instantiate(model1, pos1, FirstPersonCamera.transform.rotation);
                        object2 = Instantiate(model2, pos, FirstPersonCamera.transform.rotation);
                        object3 = Instantiate(model3, pos2, FirstPersonCamera.transform.rotation);

                    }
                    else if (position==3)
                    {
                        object1 = Instantiate(model1, pos2, FirstPersonCamera.transform.rotation);
                        object2 = Instantiate(model2, pos1, FirstPersonCamera.transform.rotation);
                        object3 = Instantiate(model3, pos, FirstPersonCamera.transform.rotation);

                    }
                    temp_model = object1;

                    //small the size..
                   
                    zoompossible = true;
                   

                }

            }

            
           

        }

      
        private void _UpdateApplicationLifecycle()
        {
            if (testPossible)
            {
                CorrectOrNot(); 

            }
       
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 60;
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

        void setmodel(int a, int b)
        {
            model1 = (GameObject)modelbundle.LoadAsset(model_array[correctModelNumber]);
            model2= (GameObject)modelbundle.LoadAsset(model_array[a]);
            model3 = (GameObject)modelbundle.LoadAsset(model_array[b]);

        }



        void CorrectOrNot()
        {

            float dist1 = Vector3.Distance(FirstPersonCamera.transform.position, object1.transform.position);
            float dist2 = Vector3.Distance(FirstPersonCamera.transform.position, object2.transform.position);
            float dist3 = Vector3.Distance(FirstPersonCamera.transform.position, object3.transform.position);

            if (dist1 < .9f&& tested==true)
            {
                  tested = false;
                
                    scoreCounter++;
                    scoreText.text = scoreCounter + "";
                  

                
               
            }
            else if (dist2 < .9f || dist3 < .9f&&tested==true)
            {
                tested = false;
            }
          

        }

      

        public void measure()
        {
            float dist = Vector3.Distance(FirstPersonCamera.transform.position, object1.transform.position);
            
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
        
    }
}  

