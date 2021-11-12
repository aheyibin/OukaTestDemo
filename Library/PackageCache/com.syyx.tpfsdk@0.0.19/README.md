# TPFSDK unity package 

[toc]

*TPFSDK* in unity 主要提供两方面的能力，一方面是单次接入 `TPFSDK` 即可对接多平台多渠道的能力，另一方面提供可快速接入 TPF 中台通用业务组件的能力。

## Install unity package 

1. 添加 *syyx-tpf* 的 registry，在 `Packages/manifest.json` 中添加以下内容：

   ```json 
     "scopedRegistries": [
       {
         "name": "syyx-tpf",
         "url": "http://10.100.3.46:8080",
         "scopes": [
           "com.syyx"
         ]
       }
     ],
   ```

2. 在 unity package manager 中选择 `My Registries/TPFSDK` 安装即可*（如没有看到 My Registry 需选择 advance/show preview packages）*。

## How to use

*TPFSDK* 在 C# 中提供的接口统一以 ``` ITPSDK.Instance.DoSomething()``` 的形式来调用。其中 C# 的接口方法名使用*Pascal* 命名，如 sdk 文档中方法名 `login` 对应的 C# 接口为 `Login`。接口参数和 sdk 文档中的描述一致，json 格式的参数以 `TPFSdkInfo` 传入。

### Callback

`sdk` 的异步调用都带有回调参数，统一以 `TPFSdkEventDelegate` 形式传入。

需要注意的是，sdk 中分为两类接口，一类是平台相关的 sdk 底层核心接口*（如登录、注册）*，一类是平台无关的对接 TPF 中台业务的接口*（如邮件、公告）*，调用 TPF 中台业务接口时会需要 `using TPFSDK.Business;`。

两类接口的区别是同一帧多次调用同一个接口时回调的机制*（准确的说是上一次调用结果未返回前再次调用）*，如调用登录在登录结果未返回前再次调用登录，且传入的回调是与上一次调用登录传入的回调不一样时，两次登录的结果返回时都会调用到最后一次登录时传入的回调。

而业务层的调用返回则会严格对应每个接口调用时传入的回调。

###  Config

#### Windows

win 端上默认要求在 `Resouces` 目录下配置名为 **`tpf_sdk_config`** 的配置文件，主要是在 unity editor 模式下开发需要。

如希望使用自己定制的资源加载方式加载配置，可实现 `ITPFSdkLoader` 实现自己的配置加载器，**并在第一次调用 `ITPFSdk.Instance` 前调用 `ITPFSdkLoader.SetCustomLoader(yourCustomLoader)` 设置定制配置加载器** 。

```json
{
  "appId": "50",
  "appKey": "mmo",
  "appSecret": "pppu",
  "areaId":"201",
  "channelId": "2",
  "tpfProxyUrl": "http://10.100.1.22:20010",
  "isUseNormalUrlLogin" : true,
  "loginAreaKey" : "205c643a637a9e2cf7792dd39e118ff8",
  "loginAppKey" : "46d92e8195b7b12847a0782934b7c4c7",
  "loginOfficialHost" : "http://192.168.10.197:10061",
  "loginHost" : "http://192.168.10.197:996/tpf-login",
}
```

#### Android

Android 端会在二次打包的时候根据后台页面配置替换各个渠道的配置。

### Build

#### Windows

win 端下可直接出包。

#### Android

安卓下出包需要分两步：

##### Raw apk

母包构建有两种途径

1. 直接 build apk ，这种方式需要在 `Assets/Plugins/Android/` 路径下配置定制`AndroidManifest` 文件*（例示文件在 `com.syyx.tpfsdk/Runtime/Plugins/Android/AndroidManifest`）*，并勾选 `PlayerSettings/Publishing Settings/Build/Custom Main Mainifest`或者是在定制的打包脚本中将 `AndroidManifest` 中的 `android:name="com.unity3d.player.UnityPlayerActivity"` 替换成 `android:name="com.tpf.sdk.unity.GameActivity"`。
2. 导出 Android 工程，在 Android Studio 中构建。这种方式需要手动修改 `UnityPlayerActivity`，参考**尚游游戏 融合sdk Unity接入手册 **中*sdk 导入* 小节。 

##### Final apk

母包构建完后直接直接到 [二次打包后台页面](http://tpf-api.syyx.com:8000/#/auth/login) 选择对应渠道配置进行打包即可。

### Environment Switching

*TPFSDK* 提供网络环境切换的功能，切换的流程是在 sdk 初始化时，尝试默认方式读取 `tpfSdk_Env` 文件切换环境，同时，sdk 提供接口 `ITPFSdk.Instance.SwitchEnv(cfg)` 来切换环境，**注意后者显式调用接口切换环境的方式总是会覆盖生效**。

- 默认的环境切换方式下，在安卓根路径下放置配置文件 `tpfSdk_Env` ，此时 sdk 内部将读取文件中对应的配置覆盖替换对应的环境配置。
- 调用 sdk 提供的切换环境接口 `ITPFSdk.Instance.SwitchEnv(cfg)` 将会覆盖当前网络配置生效，**建议需要切换环境的时候，在调用 sdk 的其他接口前统一调用一次 `ITPFSdk.Instance.SwitchEnv(cfg)` 以免出现先后向两个环境发送相关请求的错误。**

![](https://www.lucidchart.com/publicSegments/view/366fb925-00bb-4d0f-96cb-981cf44b595b/image.jpeg)

<center>TPFSDK 初始化流程</center>
如上图的初始化流程可以看到，Android 下的 sdk 初始化在 app 启动的时候就会自动地尝试读取默认方式的环境配置，并上报一些初始化事件，而 Windows 下是调用 tpfsdk 的初始化时才会尝试读取默认方式的环境配置。

环境配置格式如下：

```json
{
    "loginHost":"http://192.168.10.197:996/tpf-login",
    "loginOfficialHost": "http://192.168.10.197:10061",
    "tpfProxyUrl":"http://10.100.1.22:20010",
    "payHost":"http://192.168.10.163:80"
}
```

