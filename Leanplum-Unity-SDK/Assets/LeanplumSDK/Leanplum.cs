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

namespace LeanplumSDK
{
    public enum LPLocationAccuracyType {
        LPLocationAccuracyIP = 0,
        LPLocationAccuracyCELL = 1,
        LPLocationAccuracyGPS = 2
    }

    /// <summary>
    ///     Leanplum Unity SDK.
    /// </summary>
    public static class Leanplum
    {
        public delegate void StartHandler(bool success);
        public delegate void SyncVariablesCompleted(bool success);
        public delegate void VariableChangedHandler();
        public delegate void VariablesChangedAndNoDownloadsPendingHandler();

        public const string PURCHASE_EVENT_NAME = "Purchase";

        #region Accessors and Mutators

        /// <summary>
        ///     Gets a value indicating whether Leanplum has finished starting.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        public static bool HasStarted
        {
            get { return LeanplumFactory.SDK.HasStarted(); }
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
        public static bool HasStartedAndRegisteredAsDeveloper
        {
            get
            {
                return LeanplumFactory.SDK.HasStartedAndRegisteredAsDeveloper();
            }
        }

        /// <summary>
        ///     Gets whether or not developer mode is enabled.
        /// </summary>
        /// <value><c>true</c> if developer mode; otherwise, <c>false</c>.</value>
        public static bool IsDeveloperModeEnabled
        {
            get
            {
                return LeanplumFactory.SDK.IsDeveloperModeEnabled();
            }
        }


        /// <summary>
        ///     Gets the includeDefaults param value.
        /// </summary>
        public static bool IncludeDefaults
        {
            get
            {
                return LeanplumFactory.SDK.GetIncludeDefaults();
            }
        }

        /// <summary>
        ///     Optional. Sets the API server. The API path is of the form
        ///     http[s]://hostname/servletName
        /// </summary>
        /// <param name="hostName"> The name of the API host, such as www.leanplum.com </param>
        /// <param name="servletName"> The name of the API servlet, such as api </param>
        /// <param name="useSSL"> Whether to use SSL </param>
        public static void SetApiConnectionSettings(string hostName, string servletName = "api",
          bool useSSL = true)
        {
            LeanplumFactory.SDK.SetApiConnectionSettings(hostName, servletName, useSSL);
        }

        /// <summary>
        ///     Optional. Sets the socket server path for Development mode. Path is of the form
        ///     hostName:port
        /// </summary>
        /// <param name="hostName"> The host name of the socket server. </param>
        /// <param name="port"> The port to connect to. </param>
        public static void SetSocketConnectionSettings(string hostName, int port)
        {
            LeanplumFactory.SDK.SetSocketConnectionSettings(hostName, port);
        }

        /// <summary>
        ///     The default timeout is 10 seconds for requests, and 15 seconds for file downloads.
        /// </summary>
        /// <param name="seconds"> Timeout in seconds for standard webrequests. </param>
        /// <param name="downloadSeconds"> Timeout in seconds for downloads. </param>
        public static void SetNetworkTimeout(int seconds, int downloadSeconds)
        {
            LeanplumFactory.SDK.SetNetworkTimeout(seconds, downloadSeconds);
        }

        /// <summary>
        ///     Sets the time interval between uploading events to server.
        ///     Default is <see cref="EventsUploadInterval.AtMost15Minutes"/>.
        /// </summary>
        /// <param name="uploadInterval"> The time between uploads. </param>
        public static void SetEventsUploadInterval(EventsUploadInterval uploadInterval)
        {
            LeanplumFactory.SDK.SetEventsUploadInterval(uploadInterval);
        }

        /// <summary>
        ///     Must call either this or SetAppIdForProductionMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your development key. </param>
        public static void SetAppIdForDevelopmentMode(string appId, string accessKey)
        {
            LeanplumFactory.SDK.SetAppIdForDevelopmentMode(appId, accessKey);
        }

        /// <summary>
        ///     Must call either this or SetAppIdForDevelopmentMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your production key. </param>
        public static void SetAppIdForProductionMode(string appId, string accessKey)
        {
            LeanplumFactory.SDK.SetAppIdForProductionMode(appId, accessKey);
        }

        /// <summary>
        ///     Set the application version to be sent to Leanplum.
        /// </summary>
        /// <param name="version">Version.</param>
        public static void SetAppVersion(string version)
        {
            LeanplumFactory.SDK.SetAppVersion(version);
        }

        /// <summary>
        ///     Sets a custom device ID. Device IDs should be unique across physical devices.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        public static void SetDeviceId(string deviceId)
        {
            LeanplumFactory.SDK.SetDeviceId(deviceId);
        }

        /// <summary>
        ///     Get device id.
        /// </summary>
        /// <returns>device id</returns>
        public static string GetDeviceId()
        {
            return LeanplumFactory.SDK.GetDeviceId();
        }

        /// <summary>
        ///     This should be your first statement in a unit test. Setting this to true
        ///     will prevent Leanplum from communicating with the server.
        /// </summary>
        public static void SetTestMode(bool testModeEnabled)
        {
            LeanplumFactory.SDK.SetTestMode(testModeEnabled);
        }

        /// <summary>
        ///     Sets whether the API should return default ("defaults in code") values
        ///     or only the overridden ones.
        ///     Used only in Development mode. Always false in production.
        /// </summary>
        /// <param name="includeDefaults"> The value for includeDefaults param. </param>
        public static void SetIncludeDefaultsInDevelopmentMode(bool includeDefaults)
        {
            LeanplumFactory.SDK.SetIncludeDefaultsInDevelopmentMode(includeDefaults);
        }

        /// <summary>
        ///     Sets whether realtime updates to the client are enabled in development mode.
        ///     This uses websockets which can have high CPU impact. Default: true.
        /// </summary>
        public static void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
        {
            LeanplumFactory.SDK.SetRealtimeUpdatesInDevelopmentModeEnabled(enabled);
        }

        /// <summary>
        ///     Registers the iOS app for remote notifications. This simply calls the canonical
        ///     UIApplication methods to register notifications and does not call anything within
        ///     the Leanplum SDK.
        ///     This will register for alerts, sounds, and badges.
        ///     If you'd like to register with different settings, you can use your own method for
        ///     registering instead.
        /// </summary>
        public static void RegisterForIOSRemoteNotifications()
        {
            LeanplumFactory.SDK.RegisterForIOSRemoteNotifications();
        }

        /// <summary>
        /// Enables Xiaomi MiPush integration. Available on Android only.
        /// </summary>
        /// <param name="miAppId"> The MiPush app id. </param>
        /// <param name="miAppKey"> The MiPush app key. </param>
        public static void SetMiPushApplication(string miAppId, string miAppKey)
        {
            LeanplumFactory.SDK.SetMiPushApplication(miAppId, miAppKey);
        }

        /// <summary>
        /// Sets the logging level.
        /// </summary>
        /// <param name="logLevel"> Level to set. </param>
        public static void SetLogLevel(Constants.LogLevel logLevel)
        {
            LeanplumFactory.SDK.SetLogLevel(logLevel);
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public static object ObjectForKeyPath(params object[] components)
        {
            return LeanplumFactory.SDK.ObjectForKeyPath(components);
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public static object ObjectForKeyPathComponents(object[] pathComponents)
        {
            return LeanplumFactory.SDK.ObjectForKeyPathComponents(pathComponents);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        public static void SetDeviceLocation(double latitude, double longitude) {
            LeanplumFactory.SDK.SetDeviceLocation(latitude, longitude);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        /// <param name="type"> Location accuracy type. </param>
        public static void SetDeviceLocation(double latitude, double longitude, LPLocationAccuracyType type) 
        {
            LeanplumFactory.SDK.SetDeviceLocation(latitude, longitude, type);
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
        public static void SetDeviceLocation(double latitude, double longitude, string city, string region, string country, LPLocationAccuracyType type)
        {
            LeanplumFactory.SDK.SetDeviceLocation(latitude, longitude, city, region, country, type);
        }

        /// <summary>
        ///    Disables collecting location automatically. Will do nothing if Leanplum-Location is 
        ///    not used. Not supported on Native.
        /// </summary>
        public static void DisableLocationCollection() {
            LeanplumFactory.SDK.DisableLocationCollection();
        }

        /// <summary>
        /// Returns an instance to the LeanplumInbox object.
        /// </summary>
        public static LeanplumInbox Inbox
        {
            get
            {
                return LeanplumFactory.SDK.Inbox;
            }
        }
        #endregion

        #region Callbacks
        /// <summary>
        ///     Invoked when the variables receive new values from the server.
        ///     This will be called on start, and also later on if the user is in an
        ///     experiment that can update in realtime.
        ///     If you subscribe to this and variables have already been received, it will be
        ///     invoked immediately.
        /// </summary>
        public static event VariableChangedHandler VariablesChanged
        {
                add
                {
                    LeanplumFactory.SDK.VariablesChanged += value;
                }
                remove {
                    LeanplumFactory.SDK.VariablesChanged -= value;
                }
        }

        /// <summary>
        ///     Invoked when no more file downloads are pending (either when
        ///     no files needed to be downloaded or all downloads have been completed).
        /// </summary>
        public static event VariablesChangedAndNoDownloadsPendingHandler
            VariablesChangedAndNoDownloadsPending
        {
            add
            {
                LeanplumFactory.SDK.VariablesChangedAndNoDownloadsPending += value;
            }
            remove
            {
                LeanplumFactory.SDK.VariablesChangedAndNoDownloadsPending -= value;
            }
        }

        /// <summary>
        ///     Invoked when the start call finishes, and variables are
        ///     returned back from the server.
        /// </summary>
        public static event StartHandler Started
        {
            add
            {
                LeanplumFactory.SDK.Started += value;
            }
            remove
            {
                LeanplumFactory.SDK.Started -= value;
            }
        }
        #endregion

        #region API Calls
        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public static void Start()
        {
            LeanplumFactory.SDK.Start(null, null, null);
        }

        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public static void Start(StartHandler callback)
        {
            LeanplumFactory.SDK.Start(null, null, callback);
        }

        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public static void Start(IDictionary<string, object> userAttributes)
        {
            LeanplumFactory.SDK.Start(null, userAttributes, null);
        }

        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public static void Start(string userId)
        {
            LeanplumFactory.SDK.Start(userId, null, null);
        }

        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public static void Start(string userId, StartHandler callback)
        {
            LeanplumFactory.SDK.Start(userId, null, callback);
        }

        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public static void Start(string userId, IDictionary<string, object> userAttributes)
        {
            LeanplumFactory.SDK.Start(userId, userAttributes, null);
        }

        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public static void Start(string userId, IDictionary<string, object> attributes,
            StartHandler startResponseAction)
        {
            LeanplumFactory.SDK.Start(userId, attributes, startResponseAction);
        }

        /// <summary>
        ///     Syncs the variables defined from code without Dashboard interaction.
        ///     Requires Development mode.
        ///     Not available on Android and iOS.
        /// </summary>
        /// <param name="completedHandler"> Handler to be called when request is completed. Returns true if sync was successful. </param>
        public static void ForceSyncVariables(SyncVariablesCompleted completedHandler)
        {
            LeanplumFactory.SDK.ForceSyncVariables(completedHandler);
        }

        public static void DefineAction(string name, Constants.ActionKind kind, ActionArgs args)
        {
            LeanplumFactory.SDK.DefineAction(name, kind, args, null, null);
        }

        public static void DefineAction(string name, Constants.ActionKind kind, ActionArgs args, IDictionary<string, object> options)
        {
            LeanplumFactory.SDK.DefineAction(name, kind, args, options, null);
        }

        public static void DefineAction(string name, Constants.ActionKind kind, ActionArgs args, IDictionary<string, object> options, ActionContext.ActionResponder responder)
        {
            LeanplumFactory.SDK.DefineAction(name, kind, args, options, responder);
        }

        /// <summary>
        ///     Manually Trigger an In-App Message. Supported in Unity only.
        ///     The user must be eligible for the message and the message must be present on the device (requires a Start call).
        /// </summary>
        /// <param name="id"> The message Id. </param>
        public static bool ShowMessage(string id)
        { 
            return LeanplumFactory.SDK.ShowMessage(id);
        }

        /// <summary>
        ///     Logs in-app purchase data from Google Play.
        /// </summary>
        /// <param name="item">The name of the item purchased.</param>
        /// <param name="priceMicros">The purchase price in micros.</param>
        /// <param name="currencyCode">The ISO 4217 currency code.</param>
        /// <param name="purchaseData">Purchase data from
        /// com.android.vending.billing.util.Purchase.getOriginalJson().</param>
        /// <param name="dataSignature">Purchase data signature from
        /// com.android.vending.billing.util.Purchase.getSignature().</param>
        /// <param name="parameters">Optional event parameters.</param>
        [Obsolete("TrackGooglePlayPurchase is obsolete. Please use TrackPurchase.")]
        public static void TrackGooglePlayPurchase(string item, long priceMicros,
            string currencyCode, string purchaseData, string dataSignature,
            IDictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.TrackGooglePlayPurchase(item, priceMicros, currencyCode,
                    purchaseData, dataSignature, parameters);
        }

        /// <summary>
        ///     Automatically track in-app purchases on iOS.
        /// </summary>
        [Obsolete("TrackIOSInAppPurchases is obsolete. Please use TrackPurchase.")]
        public static void TrackIOSInAppPurchases()
        {
            LeanplumFactory.SDK.TrackIOSInAppPurchases();
        }

        /// <summary>
        ///     Logs in-app purchase data from iOS.
        /// </summary>
        /// <param name="item">
        ///     The name of the item purchased.
        /// </param>
        /// <param name="unitPrice">
        ///     The purchase price (from [[SKProduct price] doubleValue].
        /// </param>
        /// <param name="quantity">
        ///     The quantity (from [[SKPaymentTransaction payment] quantity].
        /// </param>
        /// <param name="currencyCode">
        ///     The ISO 4217 currency code (from [SKProduct priceLocale].
        /// </param>
        /// <param name="transactionIdentifier">
        ///     Transaction identifier from
        ///     [SKPaymentTransaction transactionIdentifier].
        /// </param>
        /// <param name="receiptData">
        ///     Receipt data from
        ///     [[NSData dataWithContentsOfURL:[[NSBundle mainBundle] appStoreReceiptURL]]
        ///     base64EncodedStringWithOptions:0].
        /// </param>
        /// <param name="parameters">
        ///     Optional event parameters.
        /// </param>
        [Obsolete("TrackIOSInAppPurchase is obsolete. Please use TrackPurchase.")]
        public static void TrackIOSInAppPurchase(string item, double unitPrice, int quantity,
            string currencyCode, string transactionIdentifier, string receiptData,
            IDictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.TrackIOSInAppPurchase(item, unitPrice, quantity, currencyCode,
                    transactionIdentifier, receiptData, parameters);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void TrackPurchase(string eventName, double value, string currencyCode,
            IDictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.TrackPurchase(eventName, value, currencyCode, parameters);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void Track(string eventName, double value, string info,
            IDictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.Track(eventName, value, info, parameters);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void Track(string eventName)
        {
            LeanplumFactory.SDK.Track(eventName, 0.0, String.Empty, null);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void Track(string eventName, double value)
        {
            LeanplumFactory.SDK.Track(eventName, value, String.Empty, null);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void Track(string eventName, string info)
        {
            LeanplumFactory.SDK.Track(eventName, 0.0, info, null);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void Track(string eventName, Dictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.Track(eventName, 0.0, String.Empty, parameters);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void Track(string eventName, double value,
            Dictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.Track(eventName, value, String.Empty, parameters);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public static void Track(string eventName, double value, string info)
        {
            LeanplumFactory.SDK.Track(eventName, value, info, null);
        }

        /// <summary>
        ///     Sets the traffic source info for the current user.
        ///     Keys in info must be one of: publisherId, publisherName, publisherSubPublisher,
        ///     publisherSubSite, publisherSubCampaign, publisherSubAdGroup, publisherSubAd.
        /// </summary>
        public static void SetTrafficSourceInfo(IDictionary<string, string> info)
        {
            LeanplumFactory.SDK.SetTrafficSourceInfo(info);
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public static void AdvanceTo(string state, string info,
            IDictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.AdvanceTo(state, info, parameters);
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public static void AdvanceTo(string state)
        {
            LeanplumFactory.SDK.AdvanceTo(state, String.Empty, null);
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public static void AdvanceTo(string state, string info)
        {
            LeanplumFactory.SDK.AdvanceTo(state, info, null);
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public static void AdvanceTo(string state, Dictionary<string, object> parameters)
        {
            LeanplumFactory.SDK.AdvanceTo(state, String.Empty, parameters);
        }

        /// <summary>
        ///     Updates the user ID.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        public static void SetUserId(string newUserId)
        {
            LeanplumFactory.SDK.SetUserId(newUserId);
        }

        /// <summary>
        ///     Get user id.
        /// </summary>
        /// <returns>user id</returns>
        public static string GetUserId()
        {
            return LeanplumFactory.SDK.GetUserId();
        }

        /// <summary>
        ///     Adds or modifies user attributes.
        /// </summary>
        /// <param name="value">User attributes.</param>
        public static void SetUserAttributes(IDictionary<string, object> value)
        {
            LeanplumFactory.SDK.SetUserAttributes(null, value);
        }

        /// <summary>
        ///     Updates the user ID and adds or modifies user attributes.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        /// <param name="value">User attributes.</param>
        public static void SetUserAttributes(string newUserId, IDictionary<string, object> value)
        {
            LeanplumFactory.SDK.SetUserAttributes(newUserId, value);
        }

        /// <summary>
        ///     Pauses the current state.
        ///     You can use this if your game has a "pause" mode. You shouldn't call it
        ///     when someone switches out of your app because that's done automatically.
        /// </summary>
        public static void PauseState()
        {
            LeanplumFactory.SDK.PauseState();
        }

        /// <summary>
        ///     Resumes the current state.
        /// </summary>
        public static void ResumeState()
        {
            LeanplumFactory.SDK.ResumeState();
        }

        /// <summary>
        ///     Returns variant ids.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public static List<object> Variants()
        {
            return LeanplumFactory.SDK.Variants();
        }

        public static IDictionary<string, object> Vars()
        {
            return LeanplumFactory.SDK.Vars();
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
        public static LeanplumSecuredVars SecuredVars()
        {
            return LeanplumFactory.SDK.SecuredVars();
        }

        /// <summary>
        ///     Returns metadata for all active in-app messages.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public static Dictionary<string, object> MessageMetadata()
        {
            return LeanplumFactory.SDK.MessageMetadata();
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        /// </summary>
        public static void ForceContentUpdate()
        {
            LeanplumFactory.SDK.ForceContentUpdate();
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///     The provided callback will always fire regardless
        ///     of whether the variables have changed.
        /// </summary>
        ///
        public static void ForceContentUpdate(Action callback)
        {
            LeanplumFactory.SDK.ForceContentUpdate(callback);
        }

        public static void OnAction(string actionName, ActionContext.ActionResponder handler)
        {
            LeanplumFactory.SDK.OnAction(actionName, handler);
        }

        public static ActionContext CreateActionContextForId(string actionId)
        {
            return LeanplumFactory.SDK.CreateActionContextForId(actionId);
        }

        public static bool TriggerActionForId(string actionId)
        {
            return LeanplumFactory.SDK.TriggerActionForId(actionId);
        }
        #endregion
    }
}
