#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import <CommonCrypto/CommonCrypto.h>
#import "NSString+URLEncode.h"
#import "InAppPurchase.h"
#import <TPFSDK/TPFSDK-Swift.h>

//@interface AppStoreDelegate : NSObject <SKPaymentTransactionObserver, SKProductsRequestDelegate>
//+ (AppStoreDelegate*)instance;
//
//// 请求商品信息，参数为商品id列表
//- (void)requestProducts:(NSSet*)skus;
//
//// 开始购买
//- (void)startPurchase:(NSString*)sku orderId: (NSString*)orderId accountId: (NSString*)accountId;
//
//// 恢复内购
//- (void)restorePurchases;
//
//// 清理掉所有的交易，防止极端情况下用户一直卡着
//- (void)clearAllTransactions;
//
//// 结束交易，只有验证成功后才能结束交易
//- (void)finishTransaction:(NSString*)sku orderId:(NSString*)orderId;
//
//// 设置充值地区白名单
//- (void)setLocaleWhitelist:(NSString*)localeWhitelist;
//
//// 设置是否允许二次校验。开启二次校验可以更加严格的防止代充
//- (void)setTwiceValid: (BOOL)enable;
//@end
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

static char* MakeStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

static NSString* ToString(const char* c_string)
{
    return c_string == NULL ? [NSString stringWithUTF8String:""] : [NSString stringWithUTF8String:c_string];
}

extern void UnitySendMessage(const char* objectName, const char* methodName, const char* param);
//void UnitySendMessage(const char* objectName, const char* methodName, const char* param)
//{
//    NSLog(@"%s, %s, %s", objectName, methodName, param);
//}

// 用tpf的监听者即可
const char* EventHandler = "TpfSdk_callback";

@implementation AppStoreDelegate

// 所有商品信息列表，发起支付请求需要有SKProduct对象 key为sku
static NSMutableDictionary* m_productMap = [[NSMutableDictionary alloc] init];

// 缓存交易对象，系统通知交易成功之后加入队列，等服务器确认收据收到后结束交易并从队列中移除，这是一个异步的过程。
// key为sku
static NSMutableDictionary* m_pendingTransactions = [[NSMutableDictionary alloc] init];

// productId和orderId的映射 key为sku
static NSMutableDictionary* m_skuToOrderId = [[NSMutableDictionary alloc] init];

// 已交易结束的订单。有的时候多次点击购买，可能会导致订单序列中存在多个交易对象，其订单id是相同的。
// 这里做好记录，重复的订单就不重复处理了。
static NSMutableSet* m_finishedTransactions = [[NSMutableSet alloc] init];

static NSString* m_currentPaySKU = @""; // 当前购买的商品
static NSString* m_currentAccountId = @""; // 当前购买的账号id
static NSSet* m_localeWhitelist = nil; // 合法的地区，如果不在白名单地区的话，不允许充值


// 是否开启二次验证，主要是防止其他国家区域代充，在交易成功之后又进行了一次国家校验。默认关闭.
// 这个可能会影响充值速度，另外苹果系统并不保证成功完成的交易和再次验证之间的事务性。
// 即玩家可能扣完钱，而再次验证信息的时候因为网络不好，交易报错。这个时候应该结束交易。
static BOOL m_enableTwiceValid = FALSE;
// 二次校验的交易队列。用来购买完毕之后，再次验证商品属性是否合法。
static NSMutableDictionary* m_purchasedTransactions = [[NSMutableDictionary alloc] init];

+ (AppStoreDelegate*)instance
{
    static AppStoreDelegate* instance = nil;
    if (!instance) {
        instance = [[AppStoreDelegate alloc] init];
    }
    return instance;
}

- (id)init
{
    self = [super init];

    // 添加监听，如果之前存在尚未完成支付的交易，则在添加监听之后系统会发起支付回调
    // 注意上层逻辑收到相关消息要判定是否已经登录，如果没有登录服务器，需要把消息缓存一下再处理
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];

    [self logResult:@"AppStoreDelegate init"];
    return self;
}

- (void)dealloc
{
    // 移除监听
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
}

// 是否可充值
+ (BOOL)canMakePayments
{
    return [SKPaymentQueue canMakePayments];
}

// 是否是越狱设备
- (BOOL)isJailBreak
{
    if ([self isJailBreakByURLScheme] ||
        [self isJailBreakByCydiaFiles] ||
        [self isJailBreakByAPPName] ||
        [self isJailBreakByEnvVar]) {
        return TRUE;
    } else {
        return FALSE;
    }
}

//1. 判断cydia的URL scheme
//这个方法也就是在判定是否存在cydia这个应用
- (BOOL)isJailBreakByURLScheme
{
    if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"cydia://"]]) {
        return YES;
    }
    return NO;
}


//2、判定常见的越狱文件是否存在
#define ARRAY_SIZE(a) sizeof(a)/sizeof(a[0])
//这个表可以尽可能的列出来，然后判定是否存在，只要有存在的就可以认为机器是越狱了。
const char* jailbreak_tool_pathes[] = {
    "/Applications/Cydia.app",
    "/Library/MobileSubstrate/MobileSubstrate.dylib",
    "/bin/bash",
    "/usr/sbin/sshd",
    "/etc/apt"
};

- (BOOL)isJailBreakByCydiaFiles
{
    for (int i = 0; i < ARRAY_SIZE(jailbreak_tool_pathes); i++) {
        if ([[NSFileManager defaultManager] fileExistsAtPath:[NSString stringWithUTF8String:jailbreak_tool_pathes[i]]]) {
            return YES;
        }
    }
    return NO;
}

//3. 读取系统所有应用的名称
//这个是利用不越狱的机器没有这个权限来判定的。
#define USER_APP_PATH       @"/User/Applications/"
- (BOOL)isJailBreakByAPPName
{
    if ([[NSFileManager defaultManager] fileExistsAtPath:USER_APP_PATH]) {
        NSArray *applist = [[NSFileManager defaultManager] contentsOfDirectoryAtPath:USER_APP_PATH error:nil];
        return YES;
    }
    return NO;
}

//5. 读取环境变量
//这个DYLD_INSERT_LIBRARIES环境变量，在非越狱的机器上应该是空，
//越狱的机器上基本都会有Library/MobileSubstrate/MobileSubstrate.dylib
- (BOOL)isJailBreakByEnvVar
{
    if (getenv("DYLD_INSERT_LIBRARIES")) {
        return YES;
    }
    return NO;
}

// 设置充值地区白名单
- (void)setLocaleWhitelist: (NSString*)localeWhitelist
{
    // 设置白名单，以逗号分隔
    NSArray *array = [localeWhitelist componentsSeparatedByString:@","];
    m_localeWhitelist = [NSSet setWithArray:array];
}

// 是否开启二次校验
- (void)setTwiceValid: (BOOL)enable
{
    m_enableTwiceValid = enable;
}

// 请求商品信息 用户自己维护商品列表可不用在业务层调用此接口
- (void)requestProducts:(NSSet*)skus
{
    SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers:skus];
    request.delegate = self;
    [request start];
}

// 获取商品真实显示价格(考虑当前地区)
- (NSString*)getLocalePrice:(SKProduct *)product {
    if (product) {
        NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
        [formatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
        [formatter setNumberStyle:NSNumberFormatterCurrencyStyle];
        [formatter setLocale:product.priceLocale];
        return [formatter stringFromNumber:product.price];
    }
    return @"";
}

// 计算账户名的hash，传递给苹果，帮助防止退款
- (NSString *)hashedValueForAccountName:(NSString*)userAccountName
{
    const int HASH_SIZE = 32;
    unsigned char hashedChars[HASH_SIZE];
    const char *accountName = [userAccountName UTF8String];
    size_t accountNameLen = strlen(accountName);

    // Confirm that the length of the user name is small enough
    // to be recast when calling the hash function.
    if (accountNameLen > UINT32_MAX) {
        NSLog(@"Account name too long to hash: %@", userAccountName);
        return nil;
    }
    CC_SHA256(accountName, (CC_LONG)accountNameLen, hashedChars);

    // Convert the array of bytes into a string showing its hex representation.
    NSMutableString *userAccountHash = [[NSMutableString alloc] init];
    for (int i = 0; i < HASH_SIZE; i++) {
        // Add a dash every four bytes, for readability.
        if (i != 0 && i%4 == 0) {
            [userAccountHash appendString:@"-"];
        }
        [userAccountHash appendFormat:@"%02x", hashedChars[i]];
    }

    return userAccountHash;
}

// 记录日志给服务器，日志内容会转换为json格式
- (void)logResult:(NSString*)text {
    [self logResult:text toJson:TRUE];
}

// 记录日志给服务器
- (void)logResult:(NSString*)text toJson:(BOOL)toJson {
    NSString* logStr = NULL;
    if (toJson) {
        NSDictionary *requestContents = [NSDictionary dictionaryWithObjectsAndKeys:
                                     text != nil ? text : @"", @"Content", nil];
        NSError *err;
        NSData *requestData = [NSJSONSerialization dataWithJSONObject:requestContents options:0 error:&err];
        if (!requestData) {
            // 这里就不给服务器抛事件了，避免循环调用
            return;
        }
        logStr = [[NSString alloc] initWithData:requestData encoding:NSUTF8StringEncoding];
    } else {
        logStr = text;
    }
//    UnitySendMessage(EventHandler, "LogResult", MakeStringCopy([logStr UTF8String]));
}

// 同步结果给服务器
- (void)onPayResult:(NSString*)sku error:(NSString*)error {
    [self onPayResult:sku error:error withTransaction:@""];
}

// 同步结果给服务器
- (void)onPayResult:(NSString*)sku error:(NSString*)error withTransaction:(NSString*)transaction {
    // 取到orderId
    NSString* orderId = [self getOrderId: sku];

    // json字段和内容遵循tpfsdk中的定义 参考 TPFParamKey.cs和TPFCode.cs
    NSDictionary *requestContents = [NSDictionary dictionaryWithObjectsAndKeys:
                                     sku != nil ? sku : @"", @"ProductName",
                                     orderId != nil ? orderId : @"", @"OrderId",   // 平台的订单id，在购买的时候会存储起来
                                     transaction != nil ? transaction : @"", @"TransactionId",
                                     [NSNumber numberWithInt:11], @"ErrorCode", // 11代表失败 TPFCODE_PAY_FAIL
                                     error != nil ? error : @"", @"ErrorMsg",   // 额外的错误信息
                                     [self isJailBreak] ? @"True" : @"False", @"JailBreak",  // 是否越狱
                                     [[UIDevice currentDevice]systemVersion], @"SystemVersion", // 操作系统版本
                                     nil];

    NSError *err;
    NSData *requestData = [NSJSONSerialization dataWithJSONObject:requestContents options:0 error:&err];
    if (!requestData) {
        [self logResult:[NSString stringWithFormat:@"convertErrorToJson Json序列化失败: sku=%@  error=%@", sku, err ? err.localizedDescription : @"未知错误"]];
        return;
    }

    NSString* jsonTransaction = [[NSString alloc] initWithData:requestData encoding:NSUTF8StringEncoding];
    UnitySendMessage(EventHandler, "OnPayResult", MakeStringCopy([jsonTransaction UTF8String]));
    
    // 所有失败信息都记录日志
//    UnitySendMessage(EventHandler, "LogResult", MakeStringCopy([jsonTransaction UTF8String]));
}

// 添加一个新的交易 (如果玩家取消交易的话，还会通知服务器移除交易)
- (void)onAddPayment:(NSString*)orderId {
    NSDictionary* contents = [NSDictionary dictionaryWithObjectsAndKeys:orderId, @"Content", nil];
    NSError *err;
    NSData *requestData = [NSJSONSerialization dataWithJSONObject:contents options:0 error:&err];
    if (!requestData) {
        return;
    }

    NSString* json = [[NSString alloc] initWithData:requestData encoding:NSUTF8StringEncoding];
    UnitySendMessage(EventHandler, "OnAddPayment", MakeStringCopy([json UTF8String]));
}

// 检查商品的地区是否合法
- (BOOL)checkLocale: (SKProduct*)skProduct withNotify:(BOOL)withNotify {
    if (!skProduct) return FALSE;

    // 用objectForKey来取，否则ios9可能会闪退
    NSString* countryCode = [skProduct.priceLocale objectForKey: NSLocaleCountryCode];
    if (!countryCode) return FALSE;

    if (m_localeWhitelist && [m_localeWhitelist count] > 0
        && ![m_localeWhitelist containsObject:countryCode]) {
        if (withNotify) {
            [self logResult:[NSString stringWithFormat:@"商品地区不在白名单内:::%@  %@", skProduct.productIdentifier, countryCode]];
        }
        return FALSE;
    }

    return TRUE;
}

// 商品信息返回
- (void)productsRequest:(SKProductsRequest*)request didReceiveResponse:(SKProductsResponse*)response
{
    NSArray* skProducts = response.products;
    for (SKProduct * skProduct in skProducts) {
        if (!skProduct) continue;

        // Format the price
        /*
        NSString *formattedPrice = [self getLocalePrice: skProduct]
        NSLocale *priceLocale = skProduct.priceLocale;
        NSString *currencyCode = [priceLocale objectForKey:NSLocaleCurrencyCode];
        NSNumber *productPrice = skProduct.price;

        // Setup sku details
        NSDictionary* skuDetails = [NSDictionary dictionaryWithObjectsAndKeys:
                                    skProduct.productIdentifier, @"sku",
                                    productPrice, @"price",
                                    currencyCode, @"currencyCode",
                                    formattedPrice, @"formattedPrice",
                                    ([skProduct.localizedTitle length] == 0) ? @"" : skProduct.localizedTitle, @"title",
                                    ([skProduct.localizedDescription length] == 0) ? @"" : skProduct.localizedDescription, @"description",
                                    nil];
        */
        NSString* sku = skProduct.productIdentifier;

        // 添加商品到缓存中
        [m_productMap setObject:skProduct forKey:sku];

        // 非白名单的地区不让充值
        if (![self checkLocale: skProduct withNotify:TRUE]) {
            [self onPayResult:sku error:@"InvalidLocale"];

            // 如果发现非法地区的话，删除交易，防止一直卡着
            SKPaymentTransaction* transaction = [m_purchasedTransactions objectForKey: sku];
            if (transaction) {
                [self finishTransactionWhenError:sku withTransaction:transaction];
            }
            return;
        }

        // 如果存在待验证的商品的话，则通知服务器进行校验。这里是二次校验的流程。
        SKPaymentTransaction* transaction = [m_purchasedTransactions objectForKey: sku];
        if (transaction) {
            [self sendResultToServer: sku withTransaction: transaction];
            continue;
        }

        // 如果是当前购买的商品，则发起购买请求。这里是正常购买的流程。
        if ([m_currentPaySKU isEqualToString:sku]) {
            SKMutablePayment* payment = [SKMutablePayment paymentWithProduct:skProduct];

            NSString* orderId = [self getOrderId: m_currentPaySKU];
            if (!orderId || [orderId length] == 0) {
                NSLog(@"Cannot get orderId for product: %@", m_currentPaySKU);
                return;
            }

            m_currentPaySKU = @"";
            if (m_currentAccountId != nil && [m_currentAccountId length] > 0) {
                payment.applicationUsername = m_currentAccountId;
            }
            [[SKPaymentQueue defaultQueue] addPayment:payment];
            // 通知业务层添加订单
            [self onAddPayment:orderId];
            // 标记开始充值，同步状态
            [self onPayResult:sku error:@"Purchasing"];
            [self logResult:[NSString stringWithFormat:@"商品信息返回::: sku=%@ orderId=%@ accountId=%@", sku, orderId, m_currentAccountId]];
        }
    }

    // 非法商品
    for (NSString *invalidIdentifier in response.invalidProductIdentifiers) {
        // Handle any invalid product identifiers.
        [self logResult:[NSString stringWithFormat:@"无效的商品信息::: sku=%@", invalidIdentifier]];
    }
}

// 不支持支付功能
- (void)request:(SKRequest*)request didFailWithError:(NSError*)error
{
    [self onPayResult:@"" error:error.localizedDescription];
}

// 开始购买
- (void)startPurchase:(NSString*)sku orderId:(NSString*)orderId accountId:(NSString*)accountId
{
    // 不支持购买
    if (![AppStoreDelegate canMakePayments]) {
        // 业务层会将PurchasingUnavailable作为错误码进行判断
        [self onPayResult:sku error:@"PurchasingUnavailable"];
        return;
    }

    [self logResult:[NSString stringWithFormat:@"startPurchase::: sku=%@ orderId=%@ accountId=%@", sku, orderId, accountId]];

    // 当前交易尚未完成，不要重复购买
    // 为了防止换国家代充，我们在苹果支付成功之后，再次验证了商品的国家是否在白名单。
    SKPaymentTransaction* transaction = [m_pendingTransactions objectForKey:sku];
    if (transaction) {
//        [self onPayResult:sku error:@"PurchasingNotFinish" withTransaction:transaction.transactionIdentifier];

        // 请求商品信息，如果信息返回了，就会自动进行校验
        if (m_enableTwiceValid) {
            [self requestProducts: [NSSet setWithObject:sku]];
        }
        [self sendResultToServer:sku withTransaction:transaction];
        return;
    }

    //记录账号id 赋值给applicationUserName，帮助appstore识别非法用户，降低退款率
    if (accountId != nil && [accountId length] > 0) {
        m_currentAccountId = [self hashedValueForAccountName: accountId];
    } else {
        m_currentAccountId = @"";
    }

    // 缓存orderId
    [self addOrderId:orderId forKey:sku];

    SKProduct* product = [m_productMap objectForKey:sku];
    if (product) {
        // 非白名单的地区不让充值
        // 这里不做提示了，如果是非法地区，统一在商品信息返回后进行提示
        if (![self checkLocale: product withNotify:FALSE]) {
            // 有可能玩家切地区了，重新请求下，看看商品信息有没有更新
            [self logResult:[NSString stringWithFormat:@"开始购买 地区限制::: sku=%@ orderId=%@ accountId=%@", sku, orderId, accountId]];

            m_currentPaySKU = sku;
            [self requestProducts: [NSSet setWithObject:sku]];
            return;
        }

        // 已经请求过商品信息了，直接发送购买请求
        SKMutablePayment *payment = [SKMutablePayment paymentWithProduct:product];
        // 这个在支付成功之后会原封不动的返回
        if (m_currentAccountId != nil && [m_currentAccountId length] > 0) {
            payment.applicationUsername = m_currentAccountId;
        }
        [[SKPaymentQueue defaultQueue] addPayment:payment];
        // 通知业务层添加订单
        [self onAddPayment:orderId];
        // 标记开始充值，同步状态
        [self onPayResult:sku error:@"Purchasing"];
        [self logResult:[NSString stringWithFormat:@"开始购买::: sku=%@ orderId=%@ accountId=%@", sku, orderId, accountId]];
    } else {
        // 没有请求过商品信息的话，先请求商品信息
        m_currentPaySKU = sku;
        [self requestProducts: [NSSet setWithObject:sku]];
    }
}

// 恢复商品 跟购买类似，非消耗型商品会用户可以恢复商品
-(void)restorePurchases
{
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

// 添加一个新的orderId
-(void)addOrderId:(NSString*)orderId forKey:(NSString*)sku {
    [m_skuToOrderId setObject:orderId forKey:sku];

    // 将orderId存储起来，防止交易完成前客户端闪退或者断网，导致后面无法向平台进行验证
    NSUserDefaults *standardUserDefaults = [NSUserDefaults standardUserDefaults];
    if (standardUserDefaults) {
        [standardUserDefaults setObject:orderId forKey:sku];
        [standardUserDefaults synchronize];
    }
}

// 移除已使用的orderId
-(void)removeOrderId:(NSString*)orderId forKey:(NSString*)sku {
    NSString* orderIdSaved = [m_skuToOrderId objectForKey:sku];
    if (orderIdSaved && [orderIdSaved isEqualToString:orderId]) {
        [m_skuToOrderId removeObjectForKey:sku];
    }

    // 删除存储的orderId
    NSUserDefaults *standardUserDefaults = [NSUserDefaults standardUserDefaults];
    if (standardUserDefaults) {
		orderIdSaved = [standardUserDefaults objectForKey:sku];
        if (orderIdSaved && [orderIdSaved isEqualToString:orderId]) {
            [self logResult:[NSString stringWithFormat:@"删除orderId sku=%@  orderId=%@", sku, orderId]];

            [standardUserDefaults removeObjectForKey:sku];
            [standardUserDefaults synchronize];
        }
    }
}

// 根据sku，获取orderId
- (NSString*)getOrderId:(NSString*)sku {
    NSString* orderId = [m_skuToOrderId objectForKey:sku];
    if (!orderId) {
        // 如果没有取到的话，可能游戏发生重启（闪退），从记录里面取
        NSUserDefaults* standardUserDefaults = [NSUserDefaults standardUserDefaults];
        if (standardUserDefaults) {
            orderId = [standardUserDefaults objectForKey:sku];
        } else {
            orderId = @"";
        }
    }
    return orderId;
}

// 获取收据，ios7以后通过这个接口获取收据
-(NSString*) getAppReceipt {
    NSBundle* bundle = [NSBundle mainBundle];
    if ([bundle respondsToSelector:@selector(appStoreReceiptURL)]) {
        NSURL *receiptURL = [bundle appStoreReceiptURL];
        if ([[NSFileManager defaultManager] fileExistsAtPath:[receiptURL path]]) {
            NSData *receipt = [NSData dataWithContentsOfURL:receiptURL];
            NSString* result = [receipt base64EncodedStringWithOptions:0];
            return result;
        }
    }
    NSLog(@"No App Receipt found");
    return @"";
}

// 发送交易结果给服务器
- (void)sendResultToServer: (NSString*) sku withTransaction:(SKPaymentTransaction*) transaction
{
    NSString* orderId = [self getOrderId: sku];
    [self logResult:[NSString stringWithFormat:@"商品地区校验成功::: sku=%@  transaction=%@  orderId=%@", sku, transaction.transactionIdentifier, orderId]];

    NSString* jsonTransaction = [self convertTransactionToJson: transaction];
    if ([jsonTransaction isEqual: @"error"]) {
        // 直接结束掉交易，防止用户一直卡着，暂不考虑这个异常情况
        // 这里还是有可能会发生的。当玩家点击多次商品，会产生多个交易对象。当第一个交易对象已经校验成功，后续的交易对象就找不到orderId了。
        // 我们通过一个已结束交易队列避免这种情况。但是如果游戏重启，则无法处理。
        [self onPayResult:sku error:@"交易异常" withTransaction: transaction.transactionIdentifier];
        [self finishTransactionWhenError:sku withTransaction: transaction];
        return;
    }

    [self logResult:[NSString stringWithFormat:@"通知平台进行校验::: sku=%@  transaction=%@  orderId=%@", sku, transaction.transactionIdentifier, orderId]];

    // 通知业务层交易成功，平台验证通过后会调用finish接口结束交易
//    UnitySendMessage(EventHandler, "OnPayResult", MakeStringCopy([jsonTransaction UTF8String]));
    [TPFSDK queryOrder:jsonTransaction callback:^(NSString * result) {
//        NSLog(@"%@", result);
        
        NSDictionary* resultData = jsonDeserialize(result);
        
        bool success = [(NSString*)resultData[@"ErrorCode"] isEqualToString:@"49"];
        
        NSDictionary *requestContents = [NSDictionary dictionaryWithObjectsAndKeys:
                                         success ? @"success" : @"failed", @"ErrorMsg",   // ios下用ProductName做标记
                                         success ? @"10" : @"11", @"ErrorCode",   // ios下用ProductName做标记
                                         nil];

        NSString* jsonStr = jsonSerialize(requestContents);
        UnitySendMessage(EventHandler, "OnPayResult", MakeStringCopy([jsonStr UTF8String]));
        if (success) {
            [self finishTransaction:sku orderId:orderId];
        }

    }];
    
    // 移除二次校验的交易对象
    [m_purchasedTransactions removeObjectForKey:sku];
}

// 交易结果返回
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    /*
    交易队列是由ios系统维护的，可能会有非预期的情形。
    虽然同一个商品同时只能购买一次，在交易结束之前无法再次购买。但是可能因为网络原因，在这个队列中存在相同sku的情况。
    比如在这个队列中同时有两个 coins_01，一个状态为交易取消，一个状态为交易成功。用户输入指纹之后在收到苹果服务器消息之前取消交易。
    或者有两个交易成功，交易id不同。这种情况比较极限，暂时先不处理，发生了就客服补单。
      如果要处理正确需要维护好orderId队列，但是那样可能会导致客服补单的订单又被恢复交易的流程重复发货。
    或者两个交易成功，交易id相同。这种情况默认处理就是正确的，只会发货一次。
    */
    for (SKPaymentTransaction *transaction in transactions)
    {
        NSString* sku = transaction.payment.productIdentifier;

        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchasing:
            case SKPaymentTransactionStateDeferred:
                // 正在交易中
                break;
            case SKPaymentTransactionStateFailed:
            {
                // 交易失败
                NSString* errMsg = nil;
                if (transaction.error == nil) {
                    // 未知失败原因
                    errMsg = @"Unknown";
                } else if (transaction.error.code == SKErrorPaymentCancelled) {
                    // 用户取消 业务层会将UserCancelled作为错误码进行判断
                    errMsg = @"UserCancelled";
                } else {
                    // 这里补充说明一下，我们会遇到"无法连接到iTunes Store"的错误。
                    // 这个主要发生在二次校验获取商品信息的时候，如果关掉二次校验就不会有用户既扣钱了，又交易Fail的情况。
                    // 即，苹果能保证一个交易的事务性。但是我们如果从逻辑上把交易拆分成几个部分，苹果是不保证事务性的。
                    // 虽然这里结束了交易，但正常支付的交易对象是没有被结束的，玩家重新启动游戏之后，会重新进行校验。
                    errMsg = [NSString stringWithFormat:@"%@ (%ld)", [transaction.error localizedDescription], transaction.error.code];
                }

                // 通知业务层支付失败
                [self onPayResult:sku error:errMsg withTransaction:transaction.transactionIdentifier];
                
                // 这里不再删除存储的orderId，可能因为某些异常，或者用户在支付的时候点了取消，触发了报错。
                // 但是后面会恢复正常交易，如果清理掉的话，会因为orderId不存在导致支付成功但是不发货。
                // if (standardUserDefaults) {
                //     [standardUserDefaults removeObjectForKey:sku];
                //     [standardUserDefaults synchronize];
                // }

                NSString* orderId = [self getOrderId: sku];
                [self logResult:[NSString stringWithFormat:@"交易失败::: sku=%@  transaction=%@  orderId=%@", sku, transaction.transactionIdentifier, orderId]];
                // 对于失败的交易可以直接结束掉
                [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
            }
                break;
            case SKPaymentTransactionStateRestored:
            {
                // 恢复交易 这里要用originalTransaction
                // 有的时候多次点击购买会有重复的交易对象，这里保证不重复处理已交易完成的对象。
                if ([m_finishedTransactions containsObject:transaction.transactionIdentifier]) {
                    [self logResult:[NSString stringWithFormat:@"重复的交易对象(restore) sku=%@  transaction=%@", sku, transaction.transactionIdentifier]];
                    continue;
                }

                // 缓存起来，用来等平台验证返回后，删除交易对象
                if (![m_pendingTransactions objectForKey:sku]) {
                    [m_pendingTransactions setObject:transaction forKey:sku];
                }

                NSString* orderId = [self getOrderId:sku];
                [self logResult:[NSString stringWithFormat:@"交易成功(restore)，开始验证::: sku=%@  transaction=%@  orderId=%@", sku, transaction.transactionIdentifier, orderId]];

                // 同步订单状态给平台
//                [self onPayResult:sku error:@"Restored" withTransaction:transaction.transactionIdentifier];

                if (m_enableTwiceValid) {
                    // 如果开启了二次校验，那么不直接发物品，而是再次校验国家是否合法
                    if ([m_purchasedTransactions objectForKey:sku]) {
                        // 如果一个商品尚未购买完毕，又进行了一次购买操作，这里可能会走到，这时暂时忽略这个交易。这个交易对象跟之前的交易对象是一致的。
                        // 这里并不结束此交易，即便有问题，最多是重复向平台发起校验请求。
                        [self logResult:[NSString stringWithFormat:@"正在进行验证，忽略此交易::: sku=%@  transaction=%@  orderId=%@", sku, transaction.transactionIdentifier, orderId]];
                    } else {
                        // 添加到校验队列里面
                        // 因为代充的人可以一开始用中国区账号登陆，然后退出账号。等充值的时候再换成美国区的。
                        // 这样就可以绕过一开始的检查，所以在充值完成之后再校验一次。如果地区不合法的话，就不会发货。
                        [m_purchasedTransactions setObject:transaction forKey:sku];

                        // 再次请求商品信息，如果商品区域合法，则可以发给平台进行校验
                        // 另外，客户端闪退之后恢复交易的时候，也需要重新请求商品对象
                        [self requestProducts:[NSSet setWithObject:sku]];
                    }
               } else {
                    // 没有开启二次校验，直接把消息发送给服务器即可
                    [self sendResultToServer:sku withTransaction: transaction];
                }
            }
                break;
            case SKPaymentTransactionStatePurchased:
            {
                // 交易成功
                // 有的时候多次点击购买会有重复的交易对象，这里保证不重复处理已交易完成的对象。
                if ([m_finishedTransactions containsObject:transaction.transactionIdentifier]) {
                    [self logResult:[NSString stringWithFormat:@"重复的交易对象 sku=%@  transaction=%@", sku, transaction.transactionIdentifier]];
                    continue;
                }

                // 缓存起来，用来等平台验证返回后，删除交易对象
                if (![m_pendingTransactions objectForKey:sku]) {
                    [m_pendingTransactions setObject:transaction forKey:sku];
                }

                NSString* orderId = [self getOrderId:sku];
                [self logResult:[NSString stringWithFormat:@"交易成功，开始验证::: sku=%@  transaction=%@  orderId=%@", sku, transaction.transactionIdentifier, orderId]];

                // 同步订单状态给平台
//                [self onPayResult:sku error:@"Purchased" withTransaction:transaction.transactionIdentifier];

                if (m_enableTwiceValid) {
                    // 如果开启了二次校验，那么不直接发物品，而是再次校验国家是否合法
                    if ([m_purchasedTransactions objectForKey:sku]) {
                        // 如果一个商品尚未购买完毕，又进行了一次购买操作，这里可能会走到，这时暂时忽略这个交易。这个交易对象跟之前的交易对象是一致的。
                        // 这里并不结束此交易，即便有问题，最多是重复向平台发起校验请求。
                        [self logResult:[NSString stringWithFormat:@"正在进行验证，忽略此交易::: sku=%@  transaction=%@  orderId=%@", sku, transaction.transactionIdentifier, orderId]];
                    } else {
                        // 添加到校验队列里面
                        // 因为代充的人可以一开始用中国区账号登陆，然后退出账号。等充值的时候再换成美国区的。
                        // 这样就可以绕过一开始的检查，所以在充值完成之后再校验一次。如果地区不合法的话，就不会发货。
                        [m_purchasedTransactions setObject:transaction forKey:sku];

                        // 再次请求商品信息，如果商品区域合法，则可以发给平台进行校验
                        // 另外，客户端闪退之后恢复交易的时候，也需要重新请求商品对象
                        [self requestProducts:[NSSet setWithObject:sku]];
                    }
                } else {
                    // 没有开启二次校验，直接把消息发送给服务器即可
                    [self sendResultToServer:sku withTransaction: transaction];
                }
            }
                break;
        }
    }
}

// 下载订阅会走这个接口
- (void)paymentQueue:(SKPaymentQueue *)queue updatedDownloads:(NSArray *)downloads
{
}

// 清理掉所有的交易
- (void)clearAllTransactions {
    SKPaymentQueue* defaultQueue = [SKPaymentQueue defaultQueue];
    NSArray* array = defaultQueue.transactions;
    for (int i = 0; i < [array count]; ++i) {
        [defaultQueue finishTransaction: [array objectAtIndex:i]];
    }
}

// 结束交易，只有验证成功后才能结束交易
- (void)finishTransaction:(NSString*)sku orderId:(NSString*)orderId {
    // 删除已使用的orderId
    [self removeOrderId:orderId forKey:sku];

    // 移除二次校验队列中的交易
    [m_purchasedTransactions removeObjectForKey:sku];

    // 结束掉这个交易
    SKPaymentTransaction* transaction = [m_pendingTransactions objectForKey:sku];
    if (transaction) {
        [self logResult:[NSString stringWithFormat:@"结束交易，删除transaction sku=%@  transaction=%@", sku, transaction.transactionIdentifier]];

        // 已完成的交易，加入到结束集合中。避免有重复交易对象时重复处理交易。
        if (transaction.transactionIdentifier && [transaction.transactionIdentifier length] > 0) {
            [m_finishedTransactions addObject:transaction.transactionIdentifier];
        }

        // 如果用户取消登录（appstore），则这里会执行失败。我们暂时无视这种情况，直接清理掉这个交易
        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];

        // 移除交易队列中的对象
        [m_pendingTransactions removeObjectForKey:sku];
    } else {
        // 找不到对应的交易，这个属于异常情况。正常来说只有当系统返回一个transaction已完成，才会通知平台进行验证，验证完毕之后才会结束掉这个交易
        // 所以当结束交易的函数调用的时候，transaction是必然存在的
        // 如果交易队列中存在相同的商品的交易对象时，这里找不到是有可能会发生的。
        // NSLog(@"Transaction %@ not found!", sku);
    }
}

- (void) finishTransactionWhenError: (NSString*) sku withTransaction:(SKPaymentTransaction*) transaction {
    // 注意，这里不要删除orderId，万一同时有两个相同的商品在购买（实际只会有一个交易对象，但交易队列中会存在两个这个对象），其中一个错了，第二个也无法完成交易了。
    // orderId留着没有啥问题，当有新的交易的时候，会设置为新的
    // 移除交易中队列里的交易
    SKPaymentTransaction* pendingTransaction = [m_pendingTransactions objectForKey:sku];
    if (pendingTransaction) {
        [self logResult:[NSString stringWithFormat:@"结束交易WhenError,m_pendingTransactions sku=%@  transaction=%@  pendingTransaction=%@", sku, transaction.transactionIdentifier, pendingTransaction.transactionIdentifier]];
        [m_pendingTransactions removeObjectForKey:sku];
    }

    // 移除二次校验队列里的交易
    pendingTransaction = [m_purchasedTransactions objectForKey:sku];
    if (pendingTransaction) {
        [self logResult:[NSString stringWithFormat:@"结束交易WhenError,m_purchasedTransactions sku=%@  transaction=%@  pendingTransaction=%@", sku, transaction.transactionIdentifier, pendingTransaction.transactionIdentifier]];
        [m_purchasedTransactions removeObjectForKey:sku];
    }

    [self logResult:[NSString stringWithFormat:@"finishTransactionWhenError 从队列中取消此交易: sku=%@  transaction=%@", sku, transaction.transactionIdentifier]];

    // 如果用户取消登录（appstore），则这里会执行失败。我们暂时无视这种情况，直接清理掉这个交易
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

// 将交易内容序列化为json字符串，最主要的是orderId和receipt，平台需要这两个数据进行校验
- (NSString*)convertTransactionToJson: (SKPaymentTransaction*) transaction
{
    NSString* sku = transaction.payment.productIdentifier;
    NSString* transactionId = transaction.transactionIdentifier;

    // 取到orderId
    NSString* orderId = [self getOrderId: sku];
    if (!orderId || [orderId length] == 0) {
        // 找不到orderId的话，是无效的交易
        [self logResult:[NSString stringWithFormat:@"convertTransactionToJson 无法找到orderId: sku=%@  transaction=%@", sku, transactionId]];
        return @"error";
    }

    SKProduct* product = [m_productMap objectForKey:sku];
    NSString* countryCode = nil;

    // 如果客户端强退之后恢复交易，则取不到product对象，这里暂时忽略
    if (product) {
        // 用objectForKey来取，否则ios9可能会闪退
        countryCode = [product.priceLocale objectForKey: NSLocaleCountryCode];
        // countryCode = CN
        // languageCode = zh
        // currencyCode = CNY
        if (!countryCode) {
            [self logResult:[NSString stringWithFormat:@"convertTransactionToJson 无法获取商品地区: sku=%@  transaction=%@  orderId=%@", sku, transactionId, orderId]];
        }
    }

    // json字段和内容遵循tpfsdk中的定义 参考 TPFParamKey.cs和TPFCode.cs
//    NSDictionary *requestContents = [NSDictionary dictionaryWithObjectsAndKeys:
//                                     sku, @"ProductName",   // ios下用ProductName做标记
//                                     orderId != nil ? orderId : @"", @"OrderId",   // 平台的订单id，在购买的时候会存储起来
//                                     transactionId != nil ? transactionId : @"", @"TransactionId",
//                                     [[self getAppReceipt] URLEncode], @"Receipt", // 收据，这个是base64的字符串，平台通过这个像苹果验证交易合法性
//                                     [NSNumber numberWithInt:10], @"ErrorCode", // 10代表成功 TPFCODE_PAY_SUCCESS
//                                     [self isJailBreak] ? @"True" : @"False", @"JailBreak",  // 是否越狱
//                                     [[UIDevice currentDevice]systemVersion], @"SystemVersion", // 操作系统版本
//                                     countryCode != nil ? countryCode : @"", @"Extra", // 商品地区
//                                     nil];
    NSDictionary *requestContents = [NSDictionary dictionaryWithObjectsAndKeys:
                                     orderId != nil ? orderId : @"", @"orderId",   // ios下用ProductName做标记
                                     transactionId != nil ? transactionId : @"", @"transactionId",   // ios下用ProductName做标记
                                     [self getAppReceipt], @"receiptData", // 收据，这个是base64的字符串，平台通过这个像苹果验证交易合法性
                                     nil];
    NSError *error;
    NSData *requestData = [NSJSONSerialization dataWithJSONObject:requestContents options:0 error:&error];
    if (requestData) {
        // 正常情况
        return [[NSString alloc] initWithData:requestData encoding:NSUTF8StringEncoding];
    }

    [self logResult:[NSString stringWithFormat:@"convertTransactionToJson Json序列化失败: sku=%@  transaction=%@  error=%@", sku, transactionId, error ? error.localizedDescription : @"未知错误"]];

    // 如果序列化失败的话，尝试把收据移除掉，再序列化一次。虽然不会成功。但是会有对应的日志记录。
//    requestContents = [NSDictionary dictionaryWithObjectsAndKeys:
//                                     sku, @"ProductName",   // ios下用ProductName做标记
//                                     orderId != nil ? orderId : @"", @"OrderId",   // 平台的订单id，在购买的时候会存储起来
//                                     transactionId != nil ? transactionId : @"", @"TransactionId",
//                                     @"", @"Receipt", // 收据，这个是base64的字符串，平台通过这个像苹果验证交易合法性
//                                     [NSNumber numberWithInt:10], @"ErrorCode", // 10代表成功 TPFCODE_PAY_SUCCESS
//                                     [self isJailBreak] ? @"True" : @"False", @"JailBreak",  // 是否越狱
//                                     [[UIDevice currentDevice]systemVersion], @"SystemVersion", // 操作系统版本
//                                     countryCode != nil ? countryCode : @"", @"Extra", // 商品地区
//                                     nil];
    requestContents = [NSDictionary dictionaryWithObjectsAndKeys:
                                     orderId != nil ? orderId : @"", @"orderId",   // ios下用ProductName做标记
                                     transactionId != nil ? transactionId : @"", @"transactionId",   // ios下用ProductName做标记
                                     [self getAppReceipt], @"receiptData", // 收据，这个是base64的字符串，平台通过这个像苹果验证交易合法性
                                     nil];
    requestData = [NSJSONSerialization dataWithJSONObject:requestContents options:0 error:&error];
    if (requestData) {
        return [[NSString alloc] initWithData:requestData encoding:NSUTF8StringEncoding];
    }

    [self logResult:[NSString stringWithFormat:@"convertTransactionToJson sku=%@ orderId=%@ transaction=%@ error=%@",
                    sku, orderId, transactionId, error ? error.localizedDescription : @"未知错误"]];

    NSLog(@"Got an error while creating the JSON object: %@", error);
    return @"error";
}

// 恢复交易失败
- (void)paymentQueue:(SKPaymentQueue*)queue restoreCompletedTransactionsFailedWithError:(NSError*)error
{
}

// 恢复交易完毕
- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue*)queue
{
}

@end

extern "C"
{
    // 初始化，添加监听，在登录成功之后调用比较合适
    void AppStore_init()
    {
        [AppStoreDelegate instance];
    }

    // 请求商品信息，如果游戏自己维护商品信息，而不从appstore中获取的话，可不调用此接口
    void AppStore_requestProducts(const char* skus[], int skuNumber)
    {
        NSMutableSet *skuSet = [NSMutableSet set];
        for (int i = 0; i < skuNumber; ++i)
            [skuSet addObject: ToString(skus[i])];
        [[AppStoreDelegate instance] requestProducts:skuSet];
    }

    // 开始支付
    void AppStore_startPurchase(const char* sku, const char* orderId, const char* accountId)
    {
        [[AppStoreDelegate instance] startPurchase:ToString(sku) orderId:ToString(orderId) accountId:ToString(accountId)];
    }

    // 恢复已购买的商品（非消耗行商品需要执行restore，这个同样是用户主动执行的）
    void AppStore_restorePurchases()
    {
        [[AppStoreDelegate instance] restorePurchases];
    }

    // 结束交易，在交易流程结束之后调用
    void AppStore_finishTransaction(const char* sku, const char* orderId)
    {
        [[AppStoreDelegate instance] finishTransaction: ToString(sku) orderId:ToString(orderId)];
    }

    // 清理所有的交易。可能因为bug导致交易一直结束不掉。这里添加一个接口清理掉所有的交易数据，防止用户一直卡着
    void AppStore_clearAllTransactions()
    {
        [[AppStoreDelegate instance] clearAllTransactions];
    }

    // 设置可充值的地区，多个地区用逗号分隔。不在白名单的地区不允许充值
    void AppStore_setLocaleWhitelist(const char* localeWhitelist)
    {
        [[AppStoreDelegate instance] setLocaleWhitelist: ToString(localeWhitelist)];
    }

    // 设置是否开启二次校验，默认关闭
    void AppStore_setTwiceValid(BOOL enable)
    {
        [[AppStoreDelegate instance] setTwiceValid: enable];
    }
}
