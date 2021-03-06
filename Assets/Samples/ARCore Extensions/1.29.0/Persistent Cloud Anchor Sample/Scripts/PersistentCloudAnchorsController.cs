//-----------------------------------------------------------------------
// <copyright file="PersistentCloudAnchorsController.cs" company="Google LLC">
//
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace Google.XR.ARCoreExtensions.Samples.PersistentCloudAnchors
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    using System.Globalization;
    using UnityEngine.Networking;
    using System.Collections;
    using UnityEngine.SceneManagement;
    using Newtonsoft.Json.Linq;
    /// <summary>
    /// Controller for Persistent Cloud Anchors sample.
    /// </summary>
    public class PersistentCloudAnchorsController : MonoBehaviour
    {

        public string json;
        string url = "http://kilometretakip.site/PlumbAR/dbOperations.php";

        public Text jsonCheck;
        public GameObject jsonCheckGameObject;

        public InputField inputField;


        [Header("AR Foundation")]

        public GameObject backButtonForResolve;
        public GameObject getPipesButton;
        public pipeController pipecontroller;
        public ARTapToPlaceObject aRTapToPlace;

        public GameObject arSessionGameObject;

        public ARSession aRSession;

        public ARPlaneManager aRPlaneManager;

        public GameObject pipeUI;
        /// <summary>
        /// The active ARSessionOrigin used in the example.
        /// </summary>
        public ARSessionOrigin SessionOrigin;

        /// <summary>
        /// The ARSession used in the example.
        /// </summary>
        public ARSession SessionCore;

        /// <summary>
        /// The ARCoreExtensions used in the example.
        /// </summary>
        public ARCoreExtensions Extensions;

        /// <summary>
        /// The active ARAnchorManager used in the example.
        /// </summary>
        public ARAnchorManager AnchorManager;

        /// <summary>
        /// The active ARPlaneManager used in the example.
        /// </summary>
        public ARPlaneManager PlaneManager;

        /// <summary>
        /// The active ARRaycastManager used in the example.
        /// </summary>
        public ARRaycastManager RaycastManager;

        [Header("UI")]

        /// <summary>
        /// The home page to choose entering hosting or resolving work flow.
        /// </summary>
        public GameObject HomePage;

        /// <summary>
        /// The resolve screen that provides the options on which Cloud Anchors to be resolved.
        /// </summary>
        public GameObject ResolveMenu;

        /// <summary>
        /// The information screen that displays useful information about privacy prompt.
        /// </summary>
        public GameObject PrivacyPrompt;

        /// <summary>
        /// The AR screen which displays the AR view, hosts or resolves cloud anchors,
        /// and returns to home page.
        /// </summary>
        public GameObject ARView;

        /// <summary>
        /// The current application mode.
        /// </summary>
        [HideInInspector]
        public ApplicationMode Mode = ApplicationMode.Ready;

        /// <summary>
        /// A list of Cloud Anchors that will be used in resolving.
        /// </summary>
        public HashSet<string> ResolvingSet = new HashSet<string>();

        /// <summary>
        /// The key name used in PlayerPrefs which indicates whether the start info has displayed
        /// at least one time.
        /// </summary>
        private const string _hasDisplayedStartInfoKey = "HasDisplayedStartInfo";

        /// <summary>
        /// The key name used in PlayerPrefs which stores persistent Cloud Anchors history data.
        /// Expired data will be cleared at runtime.
        /// </summary>
        private const string _persistentCloudAnchorsStorageKey = "PersistentCloudAnchors";

        /// <summary>
        /// The limitation of how many Cloud Anchors can be stored in local storage.
        /// </summary>
        private const int _storageLimit = 40;

        private void Start()
        {
            
        }

        /// <summary>
        /// Sample application modes.
        /// </summary>
        public enum ApplicationMode
        {
            /// <summary>
            /// Ready to host or resolve.
            /// </summary>
            Ready,

            /// <summary>
            /// Hosting Cloud Anchors.
            /// </summary>
            Hosting,

            /// <summary>
            /// Resolving Cloud Anchors.
            /// </summary>
            Resolving,
        }

        /// <summary>
        /// Gets the current main camera.
        /// </summary>
        public Camera MainCamera
        {
            get
            {
                return SessionOrigin.camera;
            }
        }

        /// <summary>
        /// Callback handling "Begin to host" button click event in Home Page.
        /// </summary>
        public void OnHostButtonClicked()
        {
            Mode = ApplicationMode.Hosting;
            SwitchToPrivacyPromptClone();
        }

        /// <summary>
        /// Callback handling "Begin to resolve" button click event in Home Page.
        /// </summary>
        public void OnResolveButtonClicked()
        {
            Mode = ApplicationMode.Resolving;
            SwitchToResolveMenu();
            
        }

        /// <summary>
        /// Callback handling "Learn More" Button click event in Privacy Prompt.
        /// </summary>
        public void OnLearnMoreButtonClicked()
        {
            Application.OpenURL(
                "https://developers.google.com/ar/cloud-anchors-privacy");
        }

        /// <summary>
        /// Switch to home page, and disable all other screens.
        /// </summary>
        public void SwitchToHomePage()
        {
            ResetAllViews();
            Mode = ApplicationMode.Ready;
            ResolvingSet.Clear();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
            HomePage.SetActive(true);
            //pipeUI.SetActive(false);
        }

        public void backButton()
        {
            
            aRTapToPlace.resolveCheck = false;
            arSessionGameObject.GetComponent<ARSession>().Reset();
            jsonCheckGameObject.SetActive(false);
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
            
        }

        /// <summary>
        /// Switch to resolve menu, and disable all other screens.
        /// </summary>
        public void SwitchToResolveMenu()
        {
            ResetAllViews();
            ResolveMenu.SetActive(true);
            //getPipesButton.SetActive(true);
        }

        public void SwitchToPrivacyPromptClone()
        {
            if (PlayerPrefs.HasKey(_hasDisplayedStartInfoKey))
            {
                SwitchToARView();
                backButtonForResolve.SetActive(true);
                

                return;
            }

            ResetAllViews();
            PrivacyPrompt.SetActive(true);
        }

        /// <summary>
        /// Switch to privacy prompt, and disable all other screens.
        /// </summary>
        public void SwitchToPrivacyPrompt()
        {

            if (PlayerPrefs.HasKey(_hasDisplayedStartInfoKey))
            {
                
                StartCoroutine(selectAreaShareString());


                return;
            }

            ResetAllViews();
            PrivacyPrompt.SetActive(true);
        }

        public void listItemClicked(GameObject areaId)
        {

        }

        IEnumerator insertOwnership()
        {

            WWWForm form = new WWWForm();
            form.AddField("operation", "insertOwnership");
            form.AddField("owner_id", Login.user_id);
            form.AddField("area_id", inputField.text);
            form.AddField("ownership_type", "3");


            UnityWebRequest conn = UnityWebRequest.Post(url, form);
            yield return conn.SendWebRequest();

            
            json = conn.downloadHandler.text;
            jsonCheck.text = json;
            Debug.Log(json);
        }

        IEnumerator selectArea()
        {

            WWWForm form = new WWWForm();
            form.AddField("operation", "selectArea");
            form.AddField("area_id", inputField.text);
            


            UnityWebRequest conn = UnityWebRequest.Post(url, form);
            yield return conn.SendWebRequest();

            json = conn.downloadHandler.text;
            JObject my_json = JObject.Parse(json);

            if((String)my_json["creator_id"] != Login.user_id)
            {
                yield return StartCoroutine(insertOwnership());
            }
            //jsonCheck.text = json;
        }

        IEnumerator selectAreaShareString()
        {

            
            WWWForm form = new WWWForm();
            
            form.AddField("operation", "selectAreaShareString");
            form.AddField("area_id", inputField.text);

            UnityWebRequest conn = UnityWebRequest.Post(url, form);
            yield return conn.SendWebRequest();

            json = conn.downloadHandler.text;

            string[] inputIds = { json.Substring(0, 35) };
            ResolvingSet.UnionWith(inputIds);
            pipecontroller.pipe_data = json.Substring(35);


            SwitchToARView();
            backButtonForResolve.SetActive(true);
            getPipesButton.SetActive(true);
            aRTapToPlace.resolveCheck = true;

            form = new WWWForm();
            form.AddField("operation", "selectArea");
            form.AddField("area_id", inputField.text);



            conn = UnityWebRequest.Post(url, form);
            yield return conn.SendWebRequest();

            json = conn.downloadHandler.text;
            JObject my_json = JObject.Parse(json);

            if ((String)my_json["creator_id"] != Login.user_id)
            {
                yield return StartCoroutine(insertOwnership());
            }
            //jsonCheck.text = json;


        }

        /// <summary>
        /// Switch to AR view, and disable all other screens.
        /// </summary>
        public void SwitchToARView()
        {
            ResetAllViews();
            PlayerPrefs.SetInt(_hasDisplayedStartInfoKey, 1);
            ARView.SetActive(true);
            SetPlatformActive(true);
            
        }

        /// <summary>
        /// Load the persistent Cloud Anchors history from local storage,
        /// also remove outdated records and update local history data. 
        /// </summary>
        /// <returns>A collection of persistent Cloud Anchors history data.</returns>
        public CloudAnchorHistoryCollection LoadCloudAnchorHistory()
        {
            if (PlayerPrefs.HasKey(_persistentCloudAnchorsStorageKey))
            {
                var history = JsonUtility.FromJson<CloudAnchorHistoryCollection>(
                    PlayerPrefs.GetString(_persistentCloudAnchorsStorageKey));

                // Remove all records created more than 24 hours and update stored history.
                DateTime current = DateTime.Now;
                history.Collection.RemoveAll(
                    data => current.Subtract(data.CreatedTime).Days > 0);
                PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey,
                    JsonUtility.ToJson(history));
                return history;
            }

            return new CloudAnchorHistoryCollection();
        }

        /// <summary>
        /// Save the persistent Cloud Anchors history to local storage,
        /// also remove the oldest data if current storage has met maximal capacity.
        /// </summary>
        /// <param name="data">The Cloud Anchor history data needs to be stored.</param>
        public void SaveCloudAnchorHistory(CloudAnchorHistory data)
        {
            var history = LoadCloudAnchorHistory();

            // Sort the data from latest record to oldest record which affects the option order in
            // multiselection dropdown.
            history.Collection.Add(data);
            history.Collection.Sort((left, right) => right.CreatedTime.CompareTo(left.CreatedTime));

            // Remove the oldest data if the capacity exceeds storage limit.
            if (history.Collection.Count > _storageLimit)
            {
                history.Collection.RemoveRange(
                    _storageLimit, history.Collection.Count - _storageLimit);
            }

            PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey, JsonUtility.ToJson(history));
        }

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Lock screen to portrait.
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.Portrait;

            // Enable Persistent Cloud Anchors sample to target 60fps camera capture frame rate
            // on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;
            SwitchToHomePage();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            // On home page, pressing 'back' button quits the app.
            // Otherwise, returns to home page.
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (HomePage.activeSelf)
                {
                    Application.Quit();
                }
                else
                {
                    SwitchToHomePage();
                }
            }
        }

        private void ResetAllViews()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            SetPlatformActive(false);
            ARView.SetActive(false);
            PrivacyPrompt.SetActive(false);
            ResolveMenu.SetActive(false);
            HomePage.SetActive(false);
        }

        private void SetPlatformActive(bool active)
        {
            SessionOrigin.gameObject.SetActive(active);
            SessionCore.gameObject.SetActive(active);
            Extensions.gameObject.SetActive(active);

        }
    }
}
