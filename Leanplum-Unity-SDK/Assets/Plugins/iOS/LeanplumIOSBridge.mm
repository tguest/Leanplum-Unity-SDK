//
//  Copyright (c) 2020 Leanplum. All rights reserved.
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

#import <Foundation/Foundation.h>
#import <Leanplum/Leanplum.h>
#import <Leanplum/LPActionContext.h>
#import <Leanplum/LPInternalState.h>
#import <Leanplum/LPPushNotificationsManager.h>
#import "LeanplumUnityHelper.h"
#import "LeanplumActionContextBridge.h"
#import "LeanplumIOSBridge.h"

#define LEANPLUM_CLIENT @"unity-nativeios"

typedef void (*LeanplumRequestAuthorization)
(id, SEL, unsigned long long, void (^)(BOOL, NSError *__nullable));

@interface Leanplum()
typedef void (^LeanplumHandledBlock)(BOOL success);
+ (void)setClient:(NSString *)client withVersion:(NSString *)version;
+ (LPActionContext *)createActionContextForMessageId:(NSString *)messageId;
+ (void)triggerAction:(LPActionContext *)context
         handledBlock:(nullable LeanplumHandledBlock)handledBlock;
@end

__attribute__ ((__constructor__)) static void initialize_bridge(void)
{
    [LPPushNotificationsManager sharedManager];
}

static char *__LPgameObject;
static NSMutableArray *__LPVariablesCache = [NSMutableArray array];
const char *__NativeCallbackMethod = "NativeCallback";

// Variable Delegate class
@interface LPUnityVarDelegate : NSObject <LPVarDelegate>
@end

@implementation LPUnityVarDelegate
/**
 * Called when the value of the variable changes.
 */
- (void)valueDidChange:(LPVar *)var
{
    UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                     [[NSString stringWithFormat:@"VariableValueChanged:%@", var.name] UTF8String]);
}

@end
// Variable Delegate class END

@interface LPSecuredVars (JsonKeys)
@end

@implementation LPSecuredVars (JsonKeys)
+ (NSString *)securedVarsKey
{
    return @"json";
}
+ (NSString *)securedVarsSignatureKey
{
    return @"signature";
}
@end

@implementation LeanplumIOSBridge
+ (void) sendMessageToUnity:(NSString *) messageName withKey: (NSString *)key
{
    UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                     [[NSString stringWithFormat:@"%@:%@", messageName, key] UTF8String]);
}
@end

extern "C"
{
    /**
     * Leanplum bridge public methods implementation
     */

    void _registerForNotifications()
    {
        [[LPPushNotificationsManager sharedManager] enableSystemPush];
    }

    void _setAppIdDeveloper(const char *appId, const char *accessKey)
    {
        
        NSString *NSSAppId = lp::to_nsstring(appId);
        NSString *NSSAccessKey = lp::to_nsstring(accessKey);

        [Leanplum setAppId:NSSAppId withDevelopmentKey:NSSAccessKey];
    }

    void _setAppIdProduction(const char *appId, const char *accessKey)
    {
        NSString *NSSAppId = lp::to_nsstring(appId);
        NSString *NSSAccessKey = lp::to_nsstring(accessKey);

        [Leanplum setAppId:NSSAppId withProductionKey:NSSAccessKey];
    }

    bool _hasStarted()
    {
        return [Leanplum hasStarted];
    }

    bool _hasStartedAndRegisteredAsDeveloper()
    {
        return [Leanplum hasStartedAndRegisteredAsDeveloper];
    }

    void _setApiHostName(const char *hostName, const char *servletName, int useSSL)
    {
        [Leanplum setApiHostName:lp::to_nsstring(hostName)
                 withServletName:lp::to_nsstring(servletName) usingSsl:[@(useSSL) boolValue]];
    }

    void _setNetworkTimeout(int seconds, int downloadSeconds)
    {
        [Leanplum setNetworkTimeoutSeconds:seconds forDownloads:downloadSeconds];
    }

    void _setEventsUploadInterval(int uploadInterval)
    {
        LPEventsUploadInterval interval = (LPEventsUploadInterval)uploadInterval;
        if (interval == AT_MOST_5_MINUTES || interval == AT_MOST_10_MINUTES || interval == AT_MOST_15_MINUTES) {
            [Leanplum setEventsUploadInterval:interval];
        }
    }

    void _setAppVersion(const char *version)
    {
        [Leanplum setAppVersion:lp::to_nsstring(version)];
    }

    void _setDeviceId(const char *deviceId)
    {
        [Leanplum setDeviceId:lp::to_nsstring(deviceId)];
    }

    const char *_getDeviceId()
    {
        return lp::to_string([Leanplum deviceId]);
    }

    const char *_getUserId()
    {
        return lp::to_string([Leanplum userId]);
    }

    void _setLogLevel(int logLevel)
    {
        [Leanplum setLogLevel:(LPLogLevel)logLevel];
    }

    void _setTestModeEnabled(bool isTestModeEnabled)
    {
        [Leanplum setTestModeEnabled:isTestModeEnabled];
    }

    void _setTrafficSourceInfo(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON)
                        dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setTrafficSourceInfo:dictionary];
    }

    void _advanceTo(const char *state, const char *info, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum advanceTo:lp::to_nsstring(state)
                   withInfo:lp::to_nsstring(info) andParameters:dictionary];
    }

    void _setUserAttributes(const char *newUserId, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setUserId:lp::to_nsstring(newUserId) withUserAttributes:dictionary];
    }

    void _pauseState()
    {
        [Leanplum pauseState];
    }

    void _resumeState()
    {
        [Leanplum resumeState];
    }

    const char * _variants()
    {
        return lp::to_json_string([Leanplum variants]);
    }

    const char * _securedVars()
    {   
        LPSecuredVars *securedVars = [Leanplum securedVars];
        if (securedVars) {
            NSDictionary *securedVarsDict = @{
                [LPSecuredVars securedVarsKey]: [securedVars varsJson],
                [LPSecuredVars securedVarsSignatureKey]: [securedVars varsSignature]
            };
            return lp::to_json_string(securedVarsDict);
        }
        return NULL;
    }

    const char * _vars()
    {
        return lp::to_json_string([[LPVarCache sharedCache] diffs]);
    }

    const char * _messageMetadata()
    {
        return lp::to_json_string([Leanplum messageMetadata]);
    }

    void _forceContentUpdate()
    {
        [Leanplum forceContentUpdate];
    }

    void _forceContentUpdateWithCallback(int key)
    {
        [Leanplum forceContentUpdate:^() {
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                             [[NSString stringWithFormat:@"ForceContentUpdateWithCallback:%d", key] UTF8String]);
        }];
    }
    
    void _setDeviceLocationWithLatitude(double latitude, double longitude)
    {
        [Leanplum setDeviceLocationWithLatitude: latitude
                                      longitude: longitude];
    }
    
    void _disableLocationCollection()
    {
        [Leanplum disableLocationCollection];
    }

    void _setGameObject(const char *gameObject)
    {
        __LPgameObject = (char *)malloc(strlen(gameObject) + 1);
        strcpy(__LPgameObject, gameObject);
    }

    // Leanplum start actions
    void LeanplumSetupCallbackBlocks()
    {
        [Leanplum onVariablesChanged:^{
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod, "VariablesChanged:");
        }];

        [Leanplum onVariablesChangedAndNoDownloadsPending:^{
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                             "VariablesChangedAndNoDownloadsPending:");
        }];

        [[Leanplum inbox] onChanged:^{
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                             [@"InboxOnChanged" UTF8String]);
        }];

        [[Leanplum inbox] onForceContentUpdate:^(BOOL success) {
            int res = [@(success) intValue];
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                                 [[NSString stringWithFormat:@"InboxForceContentUpdate:%d", res] UTF8String]);
        }];
    }

    void _start(const char *sdkVersion, const char *userId, const char *dictStringJSON)
    {
        [Leanplum setClient:LEANPLUM_CLIENT withVersion:lp::to_nsstring(sdkVersion)];

        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        
        NSString *userIdString = userId != NULL ? [NSString stringWithUTF8String:userId] : nil;
        [Leanplum startWithUserId:userIdString userAttributes:dictionary
                  responseHandler:^(BOOL success) {
                      int res = [@(success) intValue];
                      UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                                       [[NSString stringWithFormat:@"Started:%d", res] UTF8String]);
                  }];
        LeanplumSetupCallbackBlocks();
    }

    void _trackIOSInAppPurchases()
    {
        [Leanplum trackInAppPurchases];
    }

    void _trackPurchase(const char *event, double value, const char *currencyCode, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum trackPurchase:lp::to_nsstring(event) withValue:value andCurrencyCode:lp::to_nsstring(currencyCode)
          andParameters:dictionary];
    }

    void _track(const char *event, double value, const char *info, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum track:lp::to_nsstring(event) withValue:value andInfo:lp::to_nsstring(info)
          andParameters:dictionary];
    }

    const char *_objectForKeyPath(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        return lp::to_json_string([Leanplum objectForKeyPath:dictionary, nil]);
    }

    const char *_objectForKeyPathComponents(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        id object = [NSJSONSerialization JSONObjectWithData:data
                                                    options:NSUTF8StringEncoding
                                                      error:nil];
        return lp::to_json_string([Leanplum objectForKeyPathComponents:object]);
    }

    void _registerVariableCallback(const char *name)
    {
        NSString *varName = lp::to_nsstring(name);
        for (int i = 0; i < __LPVariablesCache.count; i++) {
            LPVar *var = [__LPVariablesCache objectAtIndex:i];
            if ([var.name isEqualToString:varName]) {
                // Create a delegate and set it to the variable.
                [var setDelegate:[[LPUnityVarDelegate alloc] init]];
                return;
            }
        }
    }

    void sendMessageActionContext(NSString *messageName, NSString *actionName, LPActionContext *context)
    {
        if (actionName != nil && context != nil) {
            NSString *key = [LeanplumActionContextBridge addActionContext:context];
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                            [[NSString stringWithFormat:@"%@:%@", messageName, key] UTF8String]);
        }
    }

    void _defineAction(const char* name, int kind, const char *args, const char *options)
    {
        if (name == nil) {
            NSLog(@"_defineAction: name provided is nil");
            return;
        }

        NSString *actionName = lp::to_nsstring(name);
        LeanplumActionKind actionKind = (LeanplumActionKind) kind;
        
        NSData *argsData = [lp::to_nsstring(args) dataUsingEncoding:NSUTF8StringEncoding];
        NSArray *argsArray = [NSJSONSerialization JSONObjectWithData:argsData
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        NSData *optionsData = [lp::to_nsstring(options) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *optionsDictionary = [NSJSONSerialization JSONObjectWithData:optionsData
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        
        NSMutableArray *arguments = [NSMutableArray new];
        
        static NSString *LP_KIND_INT = @"integer";
        static NSString *LP_KIND_FLOAT = @"float";
        static NSString *LP_KIND_STRING = @"string";
        static NSString *LP_KIND_BOOLEAN = @"bool";
        static NSString *LP_KIND_DICTIONARY = @"group";
        static NSString *LP_KIND_ARRAY = @"list";
        static NSString *LP_KIND_ACTION = @"action";
        static NSString *LP_KIND_COLOR = @"color";
        static NSString *LP_KIND_FILE = @"file";
        
        for (NSDictionary* arg in argsArray)
        {
            NSString* argName = arg[@"name"];
            NSString* argKind = arg[@"kind"];
            id defaultValue = arg[@"defaultValue"];
            
            if (argName == nil || argKind == nil || (defaultValue == nil && ![argKind isEqualToString:LP_KIND_ACTION]))
            {
                continue;
            }
            
            if ([argKind isEqualToString:LP_KIND_ACTION])
            {
                // Allow registering an Action with null default value
                // as it is done in the iOS SDK
                NSString* actionValue = (NSString*) defaultValue;
                if ([actionValue isKindOfClass:[NSNull class]])
                {
                    actionValue = nil;
                }
                [arguments addObject:[LPActionArg argNamed:argName withAction:actionValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_INT] && [defaultValue isKindOfClass:[NSNumber class]])
            {
                NSNumber* intValue = (NSNumber*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withNumber:intValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_FLOAT] && [defaultValue isKindOfClass:[NSNumber class]])
            {
                NSNumber* floatValue = (NSNumber*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withNumber:floatValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_STRING] && [defaultValue isKindOfClass:[NSString class]])
            {
                NSString* stringValue = (NSString*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withString:stringValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_BOOLEAN])
            {
                BOOL boolValue = [defaultValue boolValue];
                [arguments addObject:[LPActionArg argNamed:argName withBool:boolValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_DICTIONARY])
            {
                [arguments addObject:[LPActionArg argNamed:argName withDict:defaultValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_ARRAY])
            {
                [arguments addObject:[LPActionArg argNamed:argName withArray:defaultValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_COLOR])
            {
                long long longVal = [defaultValue longLongValue];
                [arguments addObject:[LPActionArg argNamed:argName withColor:lp::leanplum_intToColor(longVal)]];
            }
            else if ([argKind isEqualToString:LP_KIND_FILE])
            {
                [arguments addObject:[LPActionArg argNamed:argName withFile:defaultValue]];
            }
        }

        [Leanplum defineAction:actionName
                        ofKind:actionKind
                 withArguments:arguments
                   withOptions:optionsDictionary
                 withResponder:^BOOL(LPActionContext *context) {
                     // Propagate back event to unity layer
                     sendMessageActionContext(@"ActionResponder", actionName, context);
                     return YES;
                 }];
    }

    void _onAction(const char *name)
    {
        // Initialize default templates to prevent defineAction:actionResponder to override
        // the onAction that will be registered
        [LPMessageTemplatesClass sharedTemplates];
        
        NSString *actionName = lp::to_nsstring(name);
        // Register the onAction responder
        [Leanplum onAction:actionName invoke:^BOOL(LPActionContext *context) {
            sendMessageActionContext(@"OnAction", actionName, context);
            return YES;
        }];
    }

    const char * _createActionContextForId(const char *actionId)
    {
        NSString *mId = lp::to_nsstring(actionId);
        LPActionContext *context = [Leanplum createActionContextForMessageId:mId];
    
        if (!context.actionName)
        {
            // Action not found
            return NULL;
        }
        NSString *key = [LeanplumActionContextBridge addActionContext:context];
        return lp::to_string(key);
    }

    bool _triggerAction(const char *actionId)
    {
        NSString *key = lp::to_nsstring(actionId);
        LPActionContext *context = [LeanplumActionContextBridge sharedActionContexts][key];
    
        if (!context)
        {
            NSPredicate *predicate = [NSPredicate predicateWithFormat:@"SELF CONTAINS[cd] %@", key];
            NSArray *keys = [[LeanplumActionContextBridge sharedActionContexts] allKeys];
            NSUInteger index = [keys indexOfObjectPassingTest:
                         ^(id obj, NSUInteger idx, BOOL *stop) {
                           return [predicate evaluateWithObject:obj];
                         }];

              if (index != NSNotFound) {
                  context = [LeanplumActionContextBridge sharedActionContexts][keys[index]];
              } else {
                  const char * newKey = _createActionContextForId(actionId);
                  if (newKey)
                  {
                      context = [LeanplumActionContextBridge sharedActionContexts][lp::to_nsstring(newKey)];
                  }
              }
        }
    
        if (context) {
            [Leanplum triggerAction:context handledBlock:^(BOOL success) {
                [[LPInternalState sharedState].actionManager
                 recordMessageImpression:[context messageId]];
            }];
            return YES;
        }
        return NO;
    }

    // Leanplum Content
    void _defineVariable(const char *name, const char *kind, const char *jsonValue)
    {
        LPVar *var = nil;
        NSString *nameString = lp::to_nsstring(name);
        NSData *data = [lp::to_nsstring(jsonValue) dataUsingEncoding:NSUTF8StringEncoding];
        NSObject *object = [NSJSONSerialization JSONObjectWithData:data
                                                           options:NSUTF8StringEncoding error:nil];

        if (strcmp(kind, "integer") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withInteger:[(NSNumber *)object integerValue]];
        } else if (strcmp(kind, "float") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withFloat:[(NSNumber *)object floatValue]];
        } else if (strcmp(kind, "bool") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withBool:[(NSNumber *)object boolValue]];
        } else if (strcmp(kind, "file") == 0) {
            if (![object.class isSubclassOfClass:NSString.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withFile:(NSString *) object];
        } else if (strcmp(kind, "group") == 0) {
            if (![object.class isSubclassOfClass:NSDictionary.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withDictionary:(NSDictionary *)object];
        } else if (strcmp(kind, "list") == 0) {
            if (![object.class isSubclassOfClass:NSArray.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withArray:(NSArray *)object];
        } else if (strcmp(kind, "string") == 0) {
            if (![object.class isSubclassOfClass:NSString.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withString:(NSString *) object];
        } else {
            NSLog(@"Leanplum: Unsupported type %s", kind);
            return;
        }

        static LPUnityVarDelegate* delegate = nil;
        if (!delegate) {
            delegate = [[LPUnityVarDelegate alloc] init];
        }

        [__LPVariablesCache addObject:var];

        [var setDelegate:delegate];
    }

    const char *_getVariableValue(const char *name, const char *kind)
    {
        LPVar *var = [LPVar define:lp::to_nsstring(name)];

        if (var == nil) {
            return NULL;
        }
        return lp::to_json_string([var objectForKeyPath:nil]);
    }

    int _inbox_count()
    {
        return (int) [Leanplum inbox].count;
    }
    
    int _inbox_unreadCount()
    {
        return (int) [Leanplum inbox].unreadCount;
    }

    const char *_inbox_messageIds()
    {
        return lp::to_json_string([Leanplum inbox].messagesIds);
    }

    void _inbox_downloadMessages()
    {
        [[Leanplum inbox] downloadMessages];
    }

    void _inbox_downloadMessagesWithCallback()
    {
        [[Leanplum inbox] downloadMessages:^(BOOL success) {
            int res = [@(success) intValue];
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                                 [[NSString stringWithFormat:@"InboxDownloadMessages:%d", res] UTF8String]);
        }];
    }

    const char *_inbox_messages()
    {
        NSMutableArray<NSDictionary *> *messageData = [NSMutableArray new];
        NSArray<LPInboxMessage *> *messages = [Leanplum inbox].allMessages;

        NSDateFormatter *formatter = [NSDateFormatter new];
        [formatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ssZZZZZ"];

        for (LPInboxMessage *message : messages) {
            NSString *expirationTimestamp = nil;
            NSString *deliveryTimestamp = nil;

            if (message.deliveryTimestamp) {
                deliveryTimestamp = [formatter stringFromDate:message.deliveryTimestamp];
            }
            if (message.expirationTimestamp) {
                expirationTimestamp = [formatter stringFromDate:message.expirationTimestamp];
            }

            expirationTimestamp = nil;

            NSDictionary *data = @{
                @"id": message.messageId,
                @"title": message.title,
                @"subtitle": message.subtitle,
                @"imageFilePath": message.imageFilePath ?: @"",
                @"imageURL": [message.imageURL absoluteString] ?: @"",
                @"deliveryTimestamp": deliveryTimestamp ?: [NSNull null],
                @"expirationTimestamp": expirationTimestamp ?: [NSNull null],
                @"isRead": @(message.isRead),
            };
            [messageData addObject:data];
        }
        return lp::to_json_string(messageData);
    }

    void _inbox_read(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg read];
        }
    }

    void _inbox_markAsRead(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg markAsRead];
        }
    }

    void _inbox_remove(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg remove];
        }
    }

    void _inbox_disableImagePrefetching()
    {
        [[Leanplum inbox] disableImagePrefetching];
    }
} // extern "C"
