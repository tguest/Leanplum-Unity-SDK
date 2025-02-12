//
// Copyright 2013, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if !LP_UNITY_LEGACY_WWW
#if UNITY_5_5_OR_NEWER
using UnityNetworkingRequest = UnityEngine.Networking.UnityWebRequest;
using DownloadHandlerAssetBundle = UnityEngine.Networking.DownloadHandlerAssetBundle;
#else
using UnityNetworkingRequest = UnityEngine.Experimental.Networking.UnityWebRequest;
using DownloadHandlerAssetBundle = UnityEngine.Experimental.Networking.DownloadHandlerAssetBundle;
#endif
#endif

namespace LeanplumSDK
{
    /// <summary>
    ///     Provides a class that is implemented in a MonoBehaviour so that Unity functions can be
    ///     called through the GameObject.
    ///
    /// </summary>
    public class LeanplumUnityHelper : MonoBehaviour
    {
        private static LeanplumUnityHelper instance;

        internal static List<Action> delayed = new List<Action>();

        private bool developerModeEnabled;

        public static LeanplumUnityHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    var existing = FindObjectsOfType<LeanplumUnityHelper>();
                    foreach (var obj in existing)
                    {
                        Destroy(obj);
                    }

                    // Create LeanplumUnityHelper 
                    GameObject container = new GameObject("LeanplumUnityHelper"); 
                    instance = container.AddComponent<LeanplumUnityHelper>();

                    // In case instance is left, never save it to a scene file 
                    instance.hideFlags = HideFlags.DontSaveInEditor;
                }
                return instance;
            }
        }

        public void NativeCallback(string message)
        {
            LeanplumFactory.SDK.NativeCallback(message);
        }

        private void Start()
        {
            developerModeEnabled = Leanplum.IsDeveloperModeEnabled;

            // Prevent Unity from destroying this GameObject when a new scene is loaded.
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnApplicationQuit()
        {
            LeanplumNative.CompatibilityLayer.FlushSavedSettings();
            if (LeanplumNative.calledStart)
            {
                LeanplumNative.Stop();
            }
            LeanplumNative.isStopped = true;
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!LeanplumNative.calledStart)
            {
                return;
            }

            if (isPaused)
            {
                LeanplumNative.Pause();
            }
            else
            {
                LeanplumNative.Resume();
            }
        }

        private void Update()
        {
            // Workaround so that CheckVarsUpdate() is invoked on Unity's main thread.
            // This is called by Unity on every frame.
            if (VarCache.VarsNeedUpdate && developerModeEnabled && Leanplum.HasStarted)
            {
                Leanplum.ForceContentUpdate();
            }

            // Run deferred actions.
            List<Action> actions = null;
            lock (delayed)
            {
                if (delayed.Count > 0)
                {
                    actions = new List<Action>(delayed);
                    delayed.Clear();
                }
            }
            if (actions != null)
            {
                foreach (Action action in actions)
                {
                    action();
                }
            }
        }

        internal void StartRequest(string url, WWWForm wwwForm, Action<WebResponse> responseHandler,
                                   int timeout, bool isAsset = false)
        {
            StartCoroutine(RunRequest(url, wwwForm, responseHandler, timeout, isAsset));
        }

#if !LP_UNITY_LEGACY_WWW
        private static UnityNetworkingRequest CreateWebRequest(string url, WWWForm wwwForm, bool isAsset)
        {
            UnityNetworkingRequest result = null;
            if (wwwForm == null)
            {
                result = UnityNetworkingRequest.Get(url);
            }
            else if (isAsset)
            {
                result = UnityWebRequestAssetBundle.GetAssetBundle(url, 1);
            }
            else
            {
                result = UnityNetworkingRequest.Post(url, wwwForm);
            }
            return result;
        }
#else
        private static WWW CreateWww(string url, WWWForm wwwForm, bool isAsset)
        {
            WWW result = null;
            if (isAsset)
            {
                // Set an arbitrary version number - we identify different versions of assetbundles with
                // different filenames in the url.
                result = WWW.LoadFromCacheOrDownload(url, 1);
            }
            else
            {
                result = wwwForm == null ? new WWW(url) : new WWW(url, wwwForm);
            }
            return result;
        }
#endif

        private static IEnumerator RunRequest(string url, WWWForm wwwForm, Action<WebResponse> responseHandler, int timeout, bool isAsset)
        {
#if !LP_UNITY_LEGACY_WWW
            using (var request = CreateWebRequest(url, wwwForm, isAsset))
            {
                request.timeout = timeout;

                yield return request.SendWebRequest();

                while (!request.isDone)
                {
                    yield return null;
                }

                if (request.isNetworkError || request.isHttpError)
                {
                    responseHandler(new UnityWebResponse(request.responseCode, request.error, null, null));
                }
                else
                {
                    responseHandler(new UnityWebResponse(request.responseCode,
                        request.error,
                        !isAsset ? request.downloadHandler.text : null,
                        isAsset ? ((DownloadHandlerAssetBundle)request.downloadHandler) : null));
                }
            }
#else
            using (WWW www = CreateWww(url, wwwForm, isAsset))
            {
                float elapsed = 0.0f;
                while (!www.isDone && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (www.isDone)
                {
                    responseHandler(new UnityWebResponse(200, www.error,
                        (String.IsNullOrEmpty(www.error) && !isAsset) ? www.text : null,
                        (String.IsNullOrEmpty(www.error) && isAsset) ? www.assetBundle : null));
                }
                else
                {
                    responseHandler(new UnityWebResponse(408, Constants.NETWORK_TIMEOUT_MESSAGE, String.Empty, null));
                }
            }
#endif
        }

        internal static void QueueOnMainThread(Action method)
        {
            lock (delayed)
            {
                delayed.Add(method);
            }
        }
    }
}
