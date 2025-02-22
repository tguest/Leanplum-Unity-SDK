//
// Copyright 2014, Leanplum, Inc.
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
#if UNITY_IPHONE

using System;
using UnityEngine;
using LeanplumSDK.MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LeanplumSDK.Apple;

namespace LeanplumSDK
{
    public class LeanplumApple : LeanplumSDKObject
    {
        private bool isDeveloper = false;

        [DllImport ("__Internal")]
        internal static extern void _registerForNotifications();

        [DllImport ("__Internal")]
        internal static extern void _setGameObject(string gameObject);

        [DllImport ("__Internal")]
        internal static extern void _setAppIdDeveloper(string appId, string accessKey);

        [DllImport ("__Internal")]
        internal static extern void _setAppIdProduction(string appId, string accessKey);

        [DllImport ("__Internal")]
        internal static extern bool _hasStarted();

        [DllImport ("__Internal")]
        internal static extern bool _hasStartedAndRegisteredAsDeveloper();

        [DllImport ("__Internal")]
        internal static extern void _start(string sdkVersion, string userId, string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern void _trackIOSInAppPurchases();

        [DllImport ("__Internal")]
        internal static extern void _trackPurchase(string _event, double value, string currencyCode,
          string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern void _track(string _event, double value, string info,
          string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern void _setApiHostName(string hostName, string servletName,
          int useSSL);

        [DllImport ("__Internal")]
        internal static extern void _setNetworkTimeout(int seconds, int downloadSeconds);

        [DllImport("__Internal")]
        internal static extern void _setEventsUploadInterval(int uploadInterval);

        [DllImport("__Internal")]
        internal static extern void _setLogLevel(int logLevel);

        [DllImport ("__Internal")]
        internal static extern void _setAppVersion(string version);

        [DllImport ("__Internal")]
        internal static extern void _setDeviceId(string deviceId);
        
        [DllImport ("__Internal")]
        internal static extern string _getDeviceId();

        [DllImport ("__Internal")]
        internal static extern string _getUserId();

        [DllImport ("__Internal")]
        internal static extern void _setTestModeEnabled(bool enabled);

        [DllImport ("__Internal")]
        internal static extern void _setTrafficSourceInfo(string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern void _advanceTo(string state, string info, string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern void _setUserAttributes(string newUserId, string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern void _pauseState();

        [DllImport ("__Internal")]
        internal static extern void _resumeState();

        [DllImport ("__Internal")]
        internal static extern string _variants();

        [DllImport("__Internal")]
        internal static extern string _securedVars();

        [DllImport("__Internal")]
        internal static extern string _vars();

        [DllImport ("__Internal")]
        internal static extern string _messageMetadata();

        [DllImport ("__Internal")]
        internal static extern void _forceContentUpdate();

        [DllImport ("__Internal")]
        internal static extern void _defineAction(string name, int kind, string argsJSON, string optionsJSON);

        [DllImport("__Internal")]
        internal static extern void _onAction(string name);

        [DllImport("__Internal")]
        internal static extern string _createActionContextForId(string actionId);

        [DllImport("__Internal")]
        internal static extern bool _triggerAction(string actionId);

        [DllImport ("__Internal")]
        internal static extern void _forceContentUpdateWithCallback(int key);

        [DllImport ("__Internal")]
        internal static extern string _objectForKeyPath(string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern string _objectForKeyPathComponents(string dictStringJSON);

        [DllImport ("__Internal")]
        internal static extern void _setDeviceLocationWithLatitude(double latitude, double longitude);

        [DllImport ("__Internal")]
        internal static extern void _setDeviceLocationWithLatitude(double latitude, double longitude, int type);

        [DllImport ("__Internal")]
        internal static extern void _setDeviceLocationWithLatitude(double latitude, double longitude, string city, string region, string country, int type);
       
        [DllImport ("__Internal")]
        internal static extern void _disableLocationCollection();


        private LeanplumInbox inbox;
        public override LeanplumInbox Inbox
        {
            get
            {
                if (inbox == null)
                {
                    inbox = new LeanplumInboxApple();
                    return inbox;
                }
                return inbox;
            }
        }

        public LeanplumApple() {}

        public override event Leanplum.VariableChangedHandler VariablesChanged;
        public override event Leanplum.VariablesChangedAndNoDownloadsPendingHandler VariablesChangedAndNoDownloadsPending;
        public override event Leanplum.StartHandler Started;

        private Dictionary<int, Action> ForceContentUpdateCallbackDictionary = new Dictionary<int, Action>();
        private Dictionary<string, ActionContext.ActionResponder> ActionRespondersDictionary = new Dictionary<string, ActionContext.ActionResponder>();
        private Dictionary<string, List<ActionContext.ActionResponder>> OnActionRespondersDictionary = new Dictionary<string, List<ActionContext.ActionResponder>>();
        private Dictionary<string, ActionContext> ActionContextsDictionary = new Dictionary<string, ActionContext>();

        static private int DictionaryKey = 0;

        #region Accessors and Mutators

        public override void RegisterForIOSRemoteNotifications()
        {
            _registerForNotifications();
        }

        /// <summary>
        ///     Gets a value indicating whether Leanplum has finished starting.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        public override bool HasStarted()
        {
            return _hasStarted();
        }

        /// <summary>
        ///     Gets a value indicating whether Leanplum has started and the device is registered
        ///     as a developer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if Leanplum has started and the device registered as developer;
        ///     otherwise,
        ///     <c>false</c>.
        /// </value>
        public override bool HasStartedAndRegisteredAsDeveloper()
        {
            return _hasStartedAndRegisteredAsDeveloper();
        }

        /// <summary>
        ///     Gets whether or not developer mode is enabled.
        /// </summary>
        /// <value><c>true</c> if developer mode; otherwise, <c>false</c>.</value>
        public override bool IsDeveloperModeEnabled()
        {
            return _hasStarted() && isDeveloper;
        }

        /// <summary>
        ///     Gets the includeDefaults param value.
        /// </summary>
        public override bool GetIncludeDefaults()
        {
            return false;
        }

        /// <summary>
        ///     Optional. Sets the API server. The API path is of the form
        ///     http[s]://hostname/servletName
        /// </summary>
        /// <param name="hostName"> The name of the API host, such as www.leanplum.com </param>
        /// <param name="servletName"> The name of the API servlet, such as api </param>
        /// <param name="useSSL"> Whether to use SSL </param>
        public override void SetApiConnectionSettings(string hostName, string servletName = "api",
          bool useSSL = true)
        {
            _setApiHostName(hostName, servletName, useSSL?1:0);
        }

        /// <summary>
        ///     Optional. Sets the socket server path for Development mode. Path is of the form
        ///     hostName:port
        /// </summary>
        /// <param name="hostName"> The host name of the socket server. </param>
        /// <param name="port"> The port to connect to. </param>
        public override void SetSocketConnectionSettings(string hostName, int port)
        {
            // Not supported by iOS SDK
        }

        /// <summary>
        ///     The default timeout is 10 seconds for requests, and 15 seconds for file downloads.
        /// </summary>
        /// <param name="seconds"> Timeout in seconds for standard webrequests. </param>
        /// <param name="downloadSeconds"> Timeout in seconds for downloads. </param>
        public override void SetNetworkTimeout(int seconds, int downloadSeconds)
        {
            _setNetworkTimeout(seconds, downloadSeconds);
        }

        /// <summary>
        ///     Sets the time interval between uploading events to server.
        ///     Default is <see cref="EventsUploadInterval.AtMost15Minutes"/>.
        /// </summary>
        /// <param name="uploadInterval"> The time between uploads. </param>
        public override void SetEventsUploadInterval(EventsUploadInterval uploadInterval)
        {
            _setEventsUploadInterval((int)uploadInterval);
        }

        /// <summary>
        ///     Must call either this or SetAppIdForProductionMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your development key. </param>
        public override void SetAppIdForDevelopmentMode(string appId, string accessKey)
        {
            _setAppIdDeveloper(appId, accessKey);
            isDeveloper = true;
        }

        /// <summary>
        ///     Must call either this or SetAppIdForDevelopmentMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your production key. </param>
        public override void SetAppIdForProductionMode(string appId, string accessKey)
        {
            _setAppIdProduction(appId, accessKey);
            isDeveloper = false;
        }

        /// <summary>
        ///    By default, Leanplum reports the version of your app using CFBundleVersion, which
        ///    can be used for reporting and targeting on the Leanplum dashboard.
        ///    If you wish to use CFBundleShortVersionString or any other string as the version,
        ///    you can call this before your call to [Leanplum start]
        /// </summary>
        /// <param name="version">Version.</param>
        public override void SetAppVersion(string version)
        {
            _setAppVersion(version);
        }

        public override void SetLogLevel(Constants.LogLevel logLevel)
        {
            _setLogLevel((int)logLevel);
        }

        /// <summary>
        ///     Sets a custom device ID. Device IDs should be unique across physical devices.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        public override void SetDeviceId(string deviceId)
        {
            _setDeviceId(deviceId);
        }

        /// <summary>
        ///     Gets current Device ID
        /// </summary>
        public override string GetDeviceId()
        {
            return _getDeviceId();
        }

        /// <summary>
        ///     Gets current User ID
        /// </summary>
        public override string GetUserId()
        {
            return _getUserId();
        }

        /// <summary>
        ///     This should be your first statement in a unit test. Setting this to true
        ///     will prevent Leanplum from communicating with the server.
        /// </summary>
        public override void SetTestMode(bool testModeEnabled)
        {
            _setTestModeEnabled(testModeEnabled);
        }

        /// <summary>
        ///     Sets whether the API should return default ("defaults in code") values
        ///     or only the overridden ones.
        ///     Used only in Development mode. Always false in production.
        /// </summary>
        /// <param name="includeDefaults"> The value for includeDefaults param. </param>
        public override void SetIncludeDefaultsInDevelopmentMode(bool includeDefaults)
        {
            // Not supported by iOS SDK.
        }

        /// <summary>
        ///     Sets whether realtime updates to the client are enabled in development mode.
        ///     This uses websockets which can have high CPU impact. Default: true.
        /// </summary>
        public override void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
        {
            // Not supported by iOS SDK.
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPath(params object[] components)
        {
            string jsonString = _objectForKeyPath(Json.Serialize(components));
            return Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPathComponents(object[] pathComponents)
        {
            string jsonString = _objectForKeyPathComponents(Json.Serialize(pathComponents));
            return Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        public override void SetDeviceLocation(double latitude, double longitude)
        {
          _setDeviceLocationWithLatitude(latitude, longitude);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        /// <param name="type"> Location accuracy type. </param>
        public override void SetDeviceLocation(double latitude, double longitude, LPLocationAccuracyType type) 
        {
            _setDeviceLocationWithLatitude(latitude, longitude, (int) type);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        /// <param name="city"> Location city. </param>
        /// <param name="region"> Location region. </param>
        /// <param name="country"> Country ISO code. </param>
        /// <param name="type"> Location accuracy type. </param>
        public override void SetDeviceLocation(double latitude, double longitude, string city, string region, string country, LPLocationAccuracyType type)
        {
            _setDeviceLocationWithLatitude(latitude, longitude, city, region, country, (int) type);
        }

        /// <summary>
        ///    Disables collecting location automatically. Will do nothing if Leanplum-Location is 
        ///    not used. Not supported on Native.
        /// </summary>
        public override void DisableLocationCollection()
        {
          _disableLocationCollection();
        }
        #endregion

        #region API Calls

        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public override void Start(string userId, IDictionary<string, object> attributes,
            Leanplum.StartHandler startResponseAction)
        {
            _setGameObject(LeanplumUnityHelper.Instance.gameObject.name);
            Started += startResponseAction;
            string attributesString = attributes == null ? null : Json.Serialize(attributes);
            _start(Constants.SDK_VERSION, userId, attributesString);
        }

        public override void ForceSyncVariables(Leanplum.SyncVariablesCompleted completedHandler)
        {
            // Not supported by iOS SDK.
        }

        public override void DefineAction(string name, Constants.ActionKind kind, ActionArgs args, IDictionary<string, object> options, ActionContext.ActionResponder responder)
        {
            if (name == null)
            {
                return;
            }
            if (responder != null)
            {
                ActionRespondersDictionary.Add(name, responder);
            }

            string argString = args == null ? null : args.ToJSON();
            string optionString = options == null ? null : Json.Serialize(options);

            _defineAction(name, (int) kind, argString, optionString);
        }

        public override bool ShowMessage(string id)
        {
            // Not supported by iOS SDK.
            return false;
        }

        /// <summary>
        ///     Automatically tracks InApp purchase and does server side receipt validation.
        /// </summary>
        public override void TrackIOSInAppPurchases()
        {
            _trackIOSInAppPurchases();
        }

        /// <summary>
        ///     Logs a purchase event in your application. The string can be any
        ///     value of your choosing, however in most cases you will want to use
        ///     Leanplum.PURCHASE_EVENT_NAME
        /// </summary>
        public override void TrackPurchase(string eventName, double value, string currencyCode,
            IDictionary<string, object> parameters)
        {
            string parametersString = parameters == null ? null : Json.Serialize(parameters);
            _trackPurchase(eventName, value, currencyCode, parametersString);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public override void Track(string eventName, double value, string info,
            IDictionary<string, object> parameters)
        {
            string parametersString = parameters == null ? null : Json.Serialize(parameters);
            _track(eventName, value, info, parametersString);
        }

        /// <summary>
        ///     Sets the traffic source info for the current user.
        ///     Keys in info must be one of: publisherId, publisherName, publisherSubPublisher,
        ///     publisherSubSite, publisherSubCampaign, publisherSubAdGroup, publisherSubAd.
        /// </summary>
        public override void SetTrafficSourceInfo(IDictionary<string, string> info)
        {
            string infoString = (info == null) ? null : Json.Serialize(info);
            _setTrafficSourceInfo(infoString);
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public override void AdvanceTo(string state, string info,
            IDictionary<string, object> parameters)
        {
            string parametersString = parameters == null ? null : Json.Serialize(parameters);
            _advanceTo(state, info, parametersString);
        }
        
        /// <summary>
        ///     Updates the user ID and adds or modifies user attributes.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        /// <param name="value">User attributes.</param>
        public override void SetUserAttributes(string newUserId,
            IDictionary<string, object> attributes)
        {
            string attributesString = attributes == null ? null : Json.Serialize(attributes);
            _setUserAttributes(newUserId, attributesString);
        }

        /// <summary>
        ///     Pauses the current state.
        ///     You can use this if your game has a "pause" mode. You shouldn't call it
        ///     when someone switches out of your app because that's done automatically.
        /// </summary>
        public override void PauseState()
        {
            _pauseState();
        }

        /// <summary>
        ///     Resumes the current state.
        /// </summary>
        public override void ResumeState()
        {
            _resumeState();
        }

        /// <summary>
        ///     Returns variant ids.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public override List<object> Variants()
        {
            return (List<object>)Json.Deserialize(_variants());
        }

        /// <summary>
        /// Returns the last received signed variables.
        /// If signature was not provided from server the
        /// result of this method will be null.
        /// </summary>
        /// <returns> Returns <see cref="LeanplumSecuredVars"/> instance containing
        /// variable's JSON and signature.
        /// If signature was not downloaded from server, returns null.
        /// </returns>
        public override LeanplumSecuredVars SecuredVars()
        {
            string jsonString = _securedVars();
            if (!string.IsNullOrEmpty(jsonString))
            {
                var varsDict = (Dictionary<string, object>)Json.Deserialize(jsonString);
                return LeanplumSecuredVars.FromDictionary(varsDict);
            }
            return null;
        }

        public override IDictionary<string, object> Vars()
        {
            string jsonString = _vars();
            return (Dictionary<string, object>)Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Returns metadata for all active in-app messages.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public override Dictionary<string, object> MessageMetadata()
        {
            return (Dictionary<string, object>)Json.Deserialize(_messageMetadata());
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        /// </summary>
        public override void ForceContentUpdate()
        {
            _forceContentUpdate();
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///     The provided callback will always fire regardless
        ///     of whether the variables have changed.
        /// </summary>
        ///
        public override void ForceContentUpdate(Action callback)
        {
            int key = DictionaryKey++;
            ForceContentUpdateCallbackDictionary.Add (key, callback);
            _forceContentUpdateWithCallback(key);
        }

        #endregion

        public override void NativeCallback(string message)
        {
            const string VARIABLES_CHANGED = "VariablesChanged:";
            const string VARIABLES_CHANGED_NO_DOWNLOAD_PENDING = "VariablesChangedAndNoDownloadsPending:";
            const string STARTED = "Started:";
            const string VARIABLE_VALUE_CHANGED = "VariableValueChanged:";
            const string FORCE_CONTENT_UPDATE_WITH_CALLBACK = "ForceContentUpdateWithCallback:";
            const string DEFINE_ACTION_RESPONDER = "ActionResponder:";
            const string ON_ACTION_RESPONDER = "OnAction:";
            const string RUN_ACTION_NAMED_RESPONDER = "OnRunActionNamed:";

            if (message.StartsWith(VARIABLES_CHANGED))
            {
                VariablesChanged?.Invoke();
            }
            else if (message.StartsWith(VARIABLES_CHANGED_NO_DOWNLOAD_PENDING))
            {
                VariablesChangedAndNoDownloadsPending?.Invoke();
            }
            else if (message.StartsWith(STARTED))
            {
                if (Started != null)
                {
                    bool success = message.EndsWith("1");
                    Started(success);
                }
            }
            else if (message.StartsWith(VARIABLE_VALUE_CHANGED))
            {
                // Drop the beginning of the message to get the name of the variable
                // Then dispatch to the correct variable
                LeanplumApple.VariableValueChanged(message.Substring(21));
            }
            else if (message.StartsWith(FORCE_CONTENT_UPDATE_WITH_CALLBACK))
            {
                int key = Convert.ToInt32(message.Substring(FORCE_CONTENT_UPDATE_WITH_CALLBACK.Length));
                Action callback;
                if (ForceContentUpdateCallbackDictionary.TryGetValue(key, out callback))
                {
                    callback();
                    ForceContentUpdateCallbackDictionary.Remove(key);
                }
            }
            else if (message.StartsWith(DEFINE_ACTION_RESPONDER))
            {
                string key = message.Substring(DEFINE_ACTION_RESPONDER.Length);
                string actionName = GetActionNameFromMessageKey(key);

                ActionContext.ActionResponder callback;
                if (ActionRespondersDictionary.TryGetValue(actionName, out callback))
                {
                    string messageId = GetMessageIdFromMessageKey(key);
                    var context = new ActionContextApple(key, messageId);
                    ActionContextsDictionary[key] = context;
                    callback(context);
                }
            }
            else if (message.StartsWith(ON_ACTION_RESPONDER))
            {
                string key = message.Substring(ON_ACTION_RESPONDER.Length);
                string actionName = GetActionNameFromMessageKey(key);

                if (OnActionRespondersDictionary.TryGetValue(actionName, out List<ActionContext.ActionResponder> callbacks))
                {
                    if (!ActionContextsDictionary.ContainsKey(key))
                    {
                        string messageId = GetMessageIdFromMessageKey(key);
                        var newContext = new ActionContextApple(key, messageId);
                        ActionContextsDictionary[key] = newContext;
                    }

                    ActionContext context = ActionContextsDictionary[key];
                    foreach (var callback in callbacks)
                    {
                        callback(context);
                    }
                }
            }
            else if (message.StartsWith(RUN_ACTION_NAMED_RESPONDER))
            {
                char keysSeparator = '|';
                string data = message.Substring(RUN_ACTION_NAMED_RESPONDER.Length);

                string[] keys = data.Split(new char[] { keysSeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (keys.Length != 2)
                {
                    return;
                }

                string parentKey = keys[0];
                string actionKey = keys[1];

                if (ActionContextsDictionary.TryGetValue(parentKey, out ActionContext parentContext))
                {
                    var context = new ActionContextApple(actionKey, GetMessageIdFromMessageKey(actionKey));
                    parentContext.TriggerActionNamedResponder(context);
                }
            }

            if (Inbox != null)
            {
                Inbox.NativeCallback(message);
            }
        }

        private string GetActionNameFromMessageKey(string key)
        {
            // {actionName:messageId}
            return key.Split(':')[0];
        }

        private string GetMessageIdFromMessageKey(string key)
        {
            string actionName = GetActionNameFromMessageKey(key);
            string messageId = key.Length > actionName.Length ? key.Substring(actionName.Length + 1) : string.Empty;
            return messageId;
        }

        #region Dealing with Variables

        [DllImport ("__Internal")]
        internal static extern void _defineVariable(string name, string kind, string jsonValue);

        [DllImport ("__Internal")]
        internal static extern void _registerVariableCallback(string name);

        [DllImport ("__Internal")]
        internal static extern string _getVariableValue(string name, string kind);

        public static IDictionary<string, Var> IOSVarCache = new Dictionary<string, Var>();

        private Var GetOrDefineVariable(string name, string kind, object defaultValue)
        {
            if (IOSVarCache.ContainsKey(name))
            {
                if (IOSVarCache[name].Kind != kind)
                {
                    Debug.LogError("\"" + name + "\" was already defined with a different kind");
                    return null;
                }
                return IOSVarCache[name];
            }
            _defineVariable(name, kind, Json.Serialize(defaultValue));
            return null;
        }

        public override Var<int> Define(string name, int defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<int>) cached :
                new AppleVar<int>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<long> Define(string name, long defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<long>) cached :
                new AppleVar<long>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<short> Define(string name, short defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<short>) cached :
                new AppleVar<short>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<byte> Define(string name, byte defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<byte>) cached :
                new AppleVar<byte>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<bool> Define(string name, bool defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.BOOLEAN, defaultValue);
            return (cached != null) ? (Var<bool>) cached :
                new AppleVar<bool>(name, Constants.Kinds.BOOLEAN, defaultValue);
        }

        public override Var<float> Define(string name, float defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.FLOAT, defaultValue);
            return (cached != null) ? (Var<float>) cached :
                new AppleVar<float>(name, Constants.Kinds.FLOAT, defaultValue);
        }

        public override Var<double> Define(string name, double defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.FLOAT, defaultValue);
            return (cached != null) ? (Var<double>) cached :
                new AppleVar<double>(name, Constants.Kinds.FLOAT, defaultValue);
        }

        public override Var<string> Define(string name, string defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.STRING, defaultValue);
            return (cached != null) ? (Var<string>) cached :
                new AppleVar<string>(name, Constants.Kinds.STRING, defaultValue);
        }

        public override Var<List<object>> Define(string name, List<object> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.ARRAY, defaultValue);
            return (cached != null) ? (Var<List<object>>) cached :
                new AppleVar<List<object>>(name, Constants.Kinds.ARRAY, defaultValue);
        }

        public override Var<List<string>> Define(string name, List<string> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.ARRAY, defaultValue);
            return (cached != null) ? (Var<List<string>>) cached :
                new AppleVar<List<string>>(name, Constants.Kinds.ARRAY, defaultValue);
        }

        public override Var<Dictionary<string, object>> Define(string name,
            Dictionary<string, object> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.DICTIONARY, defaultValue);
            return (cached != null) ? (Var<Dictionary<string, object>>) cached :
                new AppleVar<Dictionary<string, object>>(name, Constants.Kinds.DICTIONARY,
                    defaultValue);
        }

        public override Var<Dictionary<string, string>> Define(string name,
            Dictionary<string, string> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.DICTIONARY, defaultValue);
            return (cached != null) ? (Var<Dictionary<string, string>>) cached :
                new AppleVar<Dictionary<string, string>>(name, Constants.Kinds.DICTIONARY,
                    defaultValue);
        }

        public override Var<U> Define<U>(string name, U defaultValue)
        {
            string kind = null;
            if (defaultValue is int || defaultValue is long || defaultValue is short ||
                defaultValue is char || defaultValue is sbyte || defaultValue is byte)
            {
                kind = Constants.Kinds.INT;
            }
            else if (defaultValue is float || defaultValue is double || defaultValue is decimal)
            {
                kind = Constants.Kinds.FLOAT;
            }
            else if (defaultValue is string)
            {
                kind = Constants.Kinds.STRING;
            }
            else if (defaultValue is IList || defaultValue is Array)
            {
                kind = Constants.Kinds.ARRAY;
            }
            else if (defaultValue is IDictionary)
            {
                kind = Constants.Kinds.DICTIONARY;
            }
            else if (defaultValue is bool)
            {
                kind = Constants.Kinds.BOOLEAN;
            }
            else
            {
                Debug.LogError("Leanplum Error: Default value for \"" + name +
                    "\" not recognized or supported.");
                return null;
            }

            Var cached = GetOrDefineVariable(name, kind, defaultValue);
            return (cached != null) ? (Var<U>) cached : new AppleVar<U>(name, kind, defaultValue);
        }

        public override Var<AssetBundle> DefineAssetBundle(string name,
            bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "",
            string standaloneBundleName = "")
        {
            // TODO: Not implemented.
            return null;
        }

        public static void VariableValueChanged(string name)
        {
            Var variable = IOSVarCache[name];
            if (variable != null)
            {
                variable.OnValueChanged();
            }
        }

        public override void OnAction(string actionName, ActionContext.ActionResponder handler)
        {
            if (string.IsNullOrEmpty(actionName) || handler == null)
            {
                return;
            }

            if (!OnActionRespondersDictionary.ContainsKey(actionName))
            {
                OnActionRespondersDictionary[actionName] = new List<ActionContext.ActionResponder>();
            }

            OnActionRespondersDictionary[actionName].Add(handler);
            _onAction(actionName);
        }

        public override ActionContext CreateActionContextForId(string actionId)
        {
            if (!string.IsNullOrEmpty(actionId))
            {
                string key = _createActionContextForId(actionId);
                string messageId = GetMessageIdFromMessageKey(key);
                var context = new ActionContextApple(key, messageId);
                ActionContextsDictionary[key] = context;

                return context;
            }
            return null;
        }

        public override bool TriggerActionForId(string actionId)
        {
            return _triggerAction(actionId);
        }

        #endregion
    }
}

#endif
