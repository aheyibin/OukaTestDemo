//
//  InAppPurchase.h
//  iap_test
//
//  Created by LiDingyu on 2019/11/28.
//  Copyright © 2019 1yue. All rights reserved.
//

#ifndef InAppPurchase_h
#define InAppPurchase_h
#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface AppStoreDelegate : NSObject <SKPaymentTransactionObserver, SKProductsRequestDelegate>
+ (AppStoreDelegate*)instance;

// 请求商品信息，参数为商品id列表
- (void)requestProducts:(NSSet*)skus;

// 开始购买
- (void)startPurchase:(NSString*)sku orderId: (NSString*)orderId accountId: (NSString*)accountId;

// 恢复内购
- (void)restorePurchases;

// 清理掉所有的交易，防止极端情况下用户一直卡着
- (void)clearAllTransactions;

// 结束交易，只有验证成功后才能结束交易
- (void)finishTransaction:(NSString*)sku orderId:(NSString*)orderId;

// 设置充值地区白名单
- (void)setLocaleWhitelist:(NSString*)localeWhitelist;

// 设置是否允许二次校验。开启二次校验可以更加严格的防止代充
- (void)setTwiceValid: (BOOL)enable;
@end

extern "C"
{
    void AppStore_init();
    void AppStore_requestProducts(const char* skus[], int skuNumber);
    void AppStore_startPurchase(const char* sku, const char* orderId, const char* accountId);
    void AppStore_restorePurchases();
    void AppStore_finishTransaction(const char* sku, const char* orderId);
    void AppStore_clearAllTransactions();
    void AppStore_setLocaleWhitelist(const char* localeWhitelist);
    void AppStore_setTwiceValid(BOOL enable);
}

#endif /* InAppPurchase_h */
