# Dev Log

## Structure

```
└─Runtime
   ├─Core                    sdk 核心底层接口
   │  ├─Android              Android 平台实现
   │  ├─Standalone           Windows 平台实现
   │  └─IOS                  IOS 平台实现
   ├─Extensions              接口封装
   |  └─BusinessExtensions   中台业务模块
   ├─ ...
   ...
```

整个 sdk 的结构主要就是围绕 `Core` 和 `Extension` 两个文件夹来组织的，所有和 sdk 底层*（平台相关）*的，都放在 `Core` 中处理实现，并将所有异步及部分同步接口在 `CoreExtension` 中以扩展方法的形式进行封装提供给上层使用。

而所有的中台通用业务功能都在 `BusinessExtensions` 中分各自模块实现并同样利用扩展方法为上层提供统一的接口入口。

*注意，所有非客户端需要直接调用的接口都应该用 `internal` 或 `private` 修饰。*

## Event & Callback

由于 unity 中跨语言的调用方式限制在了 `gameObject.SendMessage(method, params)` 的形式，使得不能直接通过闭包的方式来注册回调。

为此 sdk 中目前区分了三种方式注册回调的方式，三种方式都是基于上面的回调方式的扩展。

```csharp
// 这种方式用于 sdk 底层和 TPFSdkCallback 对象中的通过方法名强绑定的接口
// EventCenter 通过代码中预定好的方法名来映射派发回调（单个接口同时注册多个回调时，返回会调用最后注册的回调）
EventCenter.RegisterBindEvent(type, callback);

// 这种方式用于 sdk 底层和 TPFSdkback 中的 OnCommonResult 方法绑定的接口
// EventCenter 会通过字符串 commonEventKey 来映射派发回调（单个接口同时注册多个回调时，返回会调用最后注册的回调）
EventCenter.RegisterCommonEvent(commonEventKey, callback);

// 这种方式用于 sdk 底层和 TPFSdkback 中的 OnGameResult 方法绑定的接口
// EventCenter 会通过接口内部维护的一个 sessionId 来映射一对一回调
int sessionId = EventCenter.RegisterBusinessEvent(callback);

```

目前是区分了三种的主要是为了兼容底层 sdk 代码，后续底层重构可以统一成第三种 `sessionId` 的方式来建立一对一的映射。

## Publish

发布环境使用技术平台客户端统一部署的 *[Unity Package Registry](http://10.100.3.46:8080/)* 

发布前，修改 `package.json` 中的版本号后发布，发布一般只需要登录和发布，这里把可能需要的操作列举如下：

```
// 注册
npm adduser --registry http://10.100.3.46:8080

// 登录
npm login --registry http://10.100.3.46:8080

// 发布
npm publish --registry http://10.100.3.46:8080

// 下架
npm unpublish --force com.syyx.somepackage --registry http://10.100.3.46:8080
```



## Reference

> [run your own unity package server](https://medium.com/@markushofer/run-your-own-unity-package-server-b4fe9995704e)