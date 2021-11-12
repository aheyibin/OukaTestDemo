//
//  TPFUnityIosDelegate
//  Shanchuan
//
//  Created by Shanchuan on 2018/12/10.
//  Copyright © 2018年 TPFUser. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "UnityInterface.h"
#import "UnityAppController.h"

// 插件：扫码相关配置
//#import "TPFPluginScan/TPFPluginScan-Swift.h"

//插件：插件管理
#import "TPFPluginMgr/TPFPluginMgr-Swift.h"

//插件：webView
#import "TPFPluginWebView/TPFPluginWebView-Swift.h"

//设备管理
//#import "TPFDeviceMgr/TPFDeviceMgr-Swift.h"

// 常量
const NSString *KEY_ERR_CODE = @"ErrorCode";

const NSString *KEY_EXTRA =@"Extra";

const NSString *KEY_CALLBACK_TOKEN = @"TPFInternalCallbackToken";

const NSString *KEY_IS_NOTIFICATION = @"IsNotification";

const NSString *KEY_QR_CODE_CONTENT = @"qrCodeContent";

static const char *CALLBACK_GAMEOBJECT_NAME = "TpfSdk_callback";

const char *CALLBACK_FUNC_NAME_ON_SCAN_RESULT = "OnScanQRCodeResult";

const char *CALLBACK_FUNC_NAME_ON_INTERNAL_RESULT = "OnInternalCallback";

// END

// 辅助方法
NSString *jsonSerialize(NSDictionary *dict) {
    NSError *err;
    NSJSONWritingOptions options;
    if (@available(iOS 11.0, *)) {
        options = NSJSONWritingSortedKeys;
    } else {
        options = NSJSONWritingPrettyPrinted;
    }
    
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:options error:&err];
    
    if (err != nil) {
        return @"";
    }
    
    NSString *jsonStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return jsonStr;
}

NSObject *objectWithJsonString(NSString *jsonString) {
   NSData *jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];

   NSError *e;
   return   [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&e];
    
}


// END

extern "C" {
   
    BOOL _ex_initRes(const char *token){
        [TPFMgrDelegate initConfig:@"10" appKey:@"af4a65f522d4448bb28f004c05b5b13c" channelId:@"0"];
        
        NSString *callback=[NSString stringWithUTF8String: token];
        
        return [TPFMgrDelegate initRes: ^(NSInteger errCode, NSString *result) {
            
            NSString *errCodeStr = [NSString stringWithFormat: @"%ld", (long)errCode];
            
            NSDictionary *extra = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                   :errCodeStr, KEY_ERR_CODE, nil];
            
            NSDictionary *dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                  :callback,KEY_CALLBACK_TOKEN
                                  ,jsonSerialize(extra),KEY_EXTRA
                                  ,@"false",KEY_IS_NOTIFICATION,nil];
            NSString *tpfSdkInfoJson = jsonSerialize(dict);
            NSLog(@"%@", tpfSdkInfoJson);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, CALLBACK_FUNC_NAME_ON_INTERNAL_RESULT, [tpfSdkInfoJson UTF8String]);
        }];
    }
    
    BOOL _ex_download(const char *plugin,const char *token){
        NSString *callback=[NSString stringWithUTF8String:token];
        return [TPFMgrDelegate download:  [NSString stringWithUTF8String:plugin] callback:
                ^(NSInteger errCode, NSString *result) {
                    
                    NSString *errCodeStr = [NSString stringWithFormat: @"%ld", (long)errCode];
                    
                    
                    NSDictionary *extra = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                           :errCodeStr, KEY_ERR_CODE
                                           ,result,@"DownloadPath",nil];
                    
                    NSDictionary *dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys:
                                          jsonSerialize(extra), KEY_EXTRA
                                          ,callback,KEY_CALLBACK_TOKEN
                                          ,@"false",KEY_IS_NOTIFICATION, nil];
                    
                    NSString *tpfSdkInfoJson = jsonSerialize(dict);
                    
                    NSLog(@"%@", tpfSdkInfoJson);
                    
                    UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, CALLBACK_FUNC_NAME_ON_INTERNAL_RESULT, [tpfSdkInfoJson UTF8String]);
                }];
    }
    
    BOOL _ex_install(const char *plugin,const char  *token){
        
        NSString *callback=[NSString stringWithUTF8String:token];
        
        return [TPFMgrDelegate install: [NSString stringWithUTF8String:plugin] callback: ^(NSInteger errCode, NSString *result) {
            
            NSString *errCodeStr = [NSString stringWithFormat: @"%ld", (long)errCode];
            
            NSDictionary *extra = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                   :errCodeStr ,KEY_ERR_CODE ,nil];
            
            NSDictionary *dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys:
                                  jsonSerialize(extra), KEY_EXTRA
                                  ,callback,KEY_CALLBACK_TOKEN
                                  ,@"false",KEY_IS_NOTIFICATION, nil];
            NSString *tpfSdkInfoJson = jsonSerialize(dict);
            NSLog(@"%@", tpfSdkInfoJson);
            
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, CALLBACK_FUNC_NAME_ON_INTERNAL_RESULT, [tpfSdkInfoJson UTF8String]);
        }];
    }
    BOOL _ex_deleteGpk(const char *plugin,const char *token){
        NSString *callback=[NSString stringWithUTF8String:token];
        
        return [TPFMgrDelegate deleteGpk:[NSString stringWithUTF8String:plugin] callback:^(NSInteger errCode, NSString *result) {
            
            NSString *errCodeStr = [NSString stringWithFormat: @"%ld", (long)errCode];
            
            NSDictionary *extra = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                   :errCodeStr ,KEY_ERR_CODE ,nil];
            
            NSDictionary *dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                  :jsonSerialize(extra), KEY_EXTRA
                                  ,callback,KEY_CALLBACK_TOKEN
                                  ,@"false",KEY_IS_NOTIFICATION,nil];
            NSString *tpfSdkInfoJson = jsonSerialize(dict);
            NSLog(@"%@", tpfSdkInfoJson);
            
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, CALLBACK_FUNC_NAME_ON_INTERNAL_RESULT, [tpfSdkInfoJson UTF8String]);
        }];
    }
    
    BOOL _ex_isInstall(const char *plugin){
        return [TPFMgrDelegate isInstalled:[NSString stringWithUTF8String:plugin]];
    }
    
    const char* _ex_getPluginList(){
        NSString *result = [TPFMgrDelegate getPluginList];
        
        NSLog(@"%@", result);
        
        return strdup([result UTF8String]);
    }
    
    const char* _ex_getPluginInfo(const char *plugin){
        NSString *result = [TPFMgrDelegate getPluginInfo:[NSString stringWithUTF8String:plugin]];
        
        NSLog(@"%@", result);
        
        return strdup([result UTF8String]);
    }
    
    BOOL _ex_checkUpdate(const char *token){
        NSString *callback=[NSString stringWithUTF8String:token];
        return [TPFMgrDelegate checkUpdate:^(NSInteger errCode, NSString * result) {
            NSString *errCodeStr = [NSString stringWithFormat: @"%ld", (long)errCode];
            
            NSDictionary *extra = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                   :errCodeStr ,KEY_ERR_CODE
                                   ,objectWithJsonString(result),@"PluginInfo",nil];
            
            NSDictionary *dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                  :jsonSerialize(extra), KEY_EXTRA
                                  ,callback,KEY_CALLBACK_TOKEN
                                  ,@"false",KEY_IS_NOTIFICATION,nil];
            
            NSString *tpfSdkInfoJson = jsonSerialize(dict);
            
            NSLog(@"%@", tpfSdkInfoJson);
            
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, CALLBACK_FUNC_NAME_ON_INTERNAL_RESULT,
                             [tpfSdkInfoJson UTF8String]);
        }];
    }
    
    BOOL _ex_setConfig(const char *config){
        return [TPFMgrDelegate setConfig:[NSString stringWithUTF8String:config]];
    }
    
    BOOL _ex_runPluginCmd(const char *pluginName,const char *cmd,const char *param, const char *token){
        
        NSString *callback=[NSString stringWithUTF8String:token];
        NSString *plugin=[NSString stringWithUTF8String:pluginName];
        NSString *params=@"";
        if (param != NULL) {
            params=[NSString stringWithUTF8String:param];
        }
        if ([@"tpfpluginwebview" isEqual: plugin]) {
            
            return  [TPFWebViewDelegate call:
                     GetAppController().window.rootViewController cmd:[NSString stringWithUTF8String: cmd]
                                       param:params callback:^(NSInteger errCode, NSString * result)
                     {
                         NSString *errCodeStr = [NSString stringWithFormat: @"%ld", (long)errCode];
                         
                         NSDictionary *extra = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                                :errCodeStr,KEY_ERR_CODE
                                                ,result,@"visibility",nil];
                         
                         NSDictionary *dict = [[NSMutableDictionary alloc] initWithObjectsAndKeys
                                               :jsonSerialize(extra), KEY_EXTRA
                                               ,callback,KEY_CALLBACK_TOKEN
                                               ,@"false",KEY_IS_NOTIFICATION,nil];
                         
                         NSString *tpfSdkInfoJson = jsonSerialize(dict);
                         
                         NSLog(@"%@", tpfSdkInfoJson);
                         
                         UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, CALLBACK_FUNC_NAME_ON_INTERNAL_RESULT,
                                          [tpfSdkInfoJson UTF8String]);
                         
                     }];
        }
        return false;
    }
    
}
