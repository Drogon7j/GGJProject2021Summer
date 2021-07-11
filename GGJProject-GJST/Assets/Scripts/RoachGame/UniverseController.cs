using RoachGame.Services;
using UnityEngine;

namespace RoachGame {
    /// <summary>
    /// 全局控制器，单例模式，静态初始化
    /// </summary>
    public class UniverseController {

        private static bool isInitialized = false;

        public static UniverseController Instance { get; } = new UniverseController();

        private UniverseController() { }

        public static void Initialize() {
            if (!isInitialized) {
                // 生成所有的永久对象以及后台协程
                // 初始化所有游戏服务
                var eventService = new EventService();
                ServiceProvider.Instance.RegisterService(EventService.SERVICE_NAME, eventService);
                var broadcastService = new BroadcastService();
                ServiceProvider.Instance.RegisterService(BroadcastService.SERVICE_NAME, broadcastService);
                var assetService = new AssetService();
                ServiceProvider.Instance.RegisterService(AssetService.SERVICE_NAME, assetService);
                // 初始化部分动态配置
                // 关于GUI比例的计算，必须按照CanvasScaler中指定的值进行，如果是按宽度缩放，则要用屏幕宽度计算，反之则反
                CommonConfigs.GUI_ANCHORED_RATIO = Screen.width / CommonConfigs.GUI_RESOLUTION_REF;
                isInitialized = true;
                // 任何涉及到BaseEventObject子类脚本的对象初始化都必须在更新标志后进行，避免无限递归
            }
        }
    }
}
