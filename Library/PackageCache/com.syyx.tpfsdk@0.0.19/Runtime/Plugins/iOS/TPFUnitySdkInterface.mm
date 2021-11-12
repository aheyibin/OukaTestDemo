// #import "TPFUnitySdkInterface.h"
#import <Foundation/Foundation.h>
#import <TPFSDK/TPFSDK-Swift.h>
#import "InAppPurchase.h"
#import "UnityAppController.h"

const char *CALLBACK_GAMEOBJECT_NAME            = "TpfSdk_callback";
const char *LOGIN_CALLBACK_NAME                 = "OnLoginResult";
const char *REGISTER_CALLBACK_NAME              = "OnRegisterResult";
const char *VERIFYCODE_CALLBACK_NAME            = "OnVerifyCodeResult";
const char *FORGETPWD_CALLBACK_NAME             = "OnPasswordResult";
const char *BINDACCOUNT_CALLBACK_NAME           = "OnBindAccountResult";
const char *REALNAME_CALLBACK_NAME              = "OnRealNameResult";
const char *AD_CALLBACK_NAME                    = "OnAdResult";

static NSDictionary* jsonDeserialize(NSString* jsonStr)
{
//    NSString* infoStr = [NSString stringWithUTF8String:jsonStr];
    NSData* jsonStrData = [jsonStr dataUsingEncoding:NSUTF8StringEncoding];
    NSError* jsonError;
    NSDictionary* jsonData =  [NSJSONSerialization JSONObjectWithData:jsonStrData options:NSJSONReadingMutableContainers error:&jsonError];
    NSLog(@"%@", jsonError);
    return jsonData;
}

static NSString* jsonSerialize(NSDictionary* dict)
{
    NSError* err;
    NSJSONWritingOptions opt;
    if (@available(iOS 11.0, *)) {
        opt = NSJSONWritingSortedKeys;
    } else {
        opt = NSJSONWritingPrettyPrinted;
    }
   
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:dict options:opt error:&err];

    if (err != nil) {
        return @"{}";
    }
    
    NSString* jsonStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return jsonStr;
}

extern "C"
{

    //application初始化
    void initSDKApp(UIViewController *vc)
    {
        [TPFSDK initTPFSDK];
        [TPFSDK initAd:vc callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, AD_CALLBACK_NAME, [result UTF8String]);
        }];
        [TPFSDK initAnaylastic];
        [TPFSDK initPluginLogin:vc];
    }
    
    void onAppStart()
    {
        [TPFSDK onAppStart];
    }

    // 对应android-sdk login(String data)
    void tpf_unity_sdk_login(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_login");
        [TPFSDK login:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, LOGIN_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    //一键登录
    void tpf_unity_sdk_login_with_method(const char* data)
    {
        [TPFSDK loginWithMethod:[NSString stringWithUTF8String: data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, LOGIN_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk register(String data)
    void tpf_unity_sdk_register(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_register");
        [TPFSDK register:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, REGISTER_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk verifyCode(String data)
    void tpf_unity_sdk_verifyCode(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_verifyCode");
        [TPFSDK verifyCode:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, VERIFYCODE_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk forgetPwd(String data)
    void tpf_unity_sdk_forgetPwd(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_forgetPwd");
        [TPFSDK forgetPwd:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, FORGETPWD_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk bindAccount(String data)
    void tpf_unity_sdk_bindAccount(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_bindAccount");
        [TPFSDK bindAccount:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, BINDACCOUNT_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk realNameVerify(String data)
    void tpf_unity_sdk_realNameVerify(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_realNameVerify");
        [TPFSDK realNameVerify:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, REALNAME_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk queryRealName(String data)
    void tpf_unity_sdk_queryRealName(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_queryRealName");
        [TPFSDK checkRealName:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, REALNAME_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk queryUserBind(String data)
    void tpf_unity_sdk_queryUserBind(const char* data)
    {
        NSLog(@"%s", "tpf_unity_sdk_queryUserBind");
        [TPFSDK checkBindAccount:[NSString stringWithUTF8String:data] callback:^(NSString * result) {
            NSLog(@"%@", result);
            UnitySendMessage(CALLBACK_GAMEOBJECT_NAME, BINDACCOUNT_CALLBACK_NAME, [result UTF8String]);
        }];
    }

    // 对应android-sdk pay( String payInfo)
    void tpf_unity_sdk_pay(const char* playInfo)
    {
        NSString* infoStr = [NSString stringWithUTF8String:playInfo];
        NSDictionary* json = jsonDeserialize(infoStr);
        
        NSString* orderId = json[@"OrderId"];
        NSString* extraJsonStr = json[@"Extra"];

        NSDictionary* extraJsonData = jsonDeserialize(extraJsonStr);
        
        NSString* productId = extraJsonData[@"ProductId"];
        NSString* userId = extraJsonData[@"UserId"];
        
        [[AppStoreDelegate instance] startPurchase:productId orderId:orderId accountId:userId];
    }

    // 对应android-sdk prePay(String data)
    void tpf_unity_sdk_prePay(const char* data)
    {
        NSString* infoStr = [NSString stringWithUTF8String:data];

        NSDictionary* json = jsonDeserialize(infoStr);

        bool autoPay = [(NSString*)json[@"AutoPay"] isEqualToString:@"1"];


        [TPFSDK prePay:infoStr callback:^(NSString * result) {
            NSLog(@"%@", result);

            NSDictionary* resultData = jsonDeserialize(result);
            
            bool success = [(NSString*)resultData[@"ErrorCode"] isEqualToString:@"47"];
            NSDictionary* extraData = resultData[@"Extra"];

            NSString* extraJsonStr = jsonSerialize(extraData);
            
            UnitySendMessage("TpfSdk_callback", "OnCommonResult", [result UTF8String]);
            if (success && autoPay) {
                tpf_unity_sdk_pay([extraJsonStr UTF8String]);
            } else {
                
            }

        }];
    }
    
    //对应android-sdk customEvent()
    void tpf_unity_sdk_eventReport(const char* eventId, const char* eventType, const char* extra){
        [TPFSDK customEventReport:[NSString stringWithUTF8String:eventId]
                                 :[NSString stringWithUTF8String:eventType]
                                 :[NSString stringWithUTF8String:extra]];
	}

    // 对应android-sdk getEnvironmentConfig()
    const char* tpf_unity_sdk_getEnvironmentConfig(){
        NSString *result = [TPFSDK getEnvironmentConfig];
        return strdup([result UTF8String]);
    }
    
   // 对应android-sdk  loadAd()
    void tpf_unity_sdk_loadAd(int adType, const char* extra){
         [TPFSDK loadAd:adType:[NSString stringWithUTF8String:extra]];
    }

    // 对应android-sdk  loadAd()
    void tpf_unity_sdk_showAd(int adType, const char* extra){
        [TPFSDK showAd: adType:[NSString stringWithUTF8String:extra]];
    }

}
