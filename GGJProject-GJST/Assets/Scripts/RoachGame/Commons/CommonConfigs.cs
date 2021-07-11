namespace RoachGame {
    public static class CommonConfigs {
        #region "Number Configs"
        public const int EVENT_COMMON_UI_FUNCTION = 0x0000; // 通用界面功能事件
        public const int EVENT_COMMON_CALLBACK = 0x0001; // 通用回调事件
        #endregion
        #region "String Configs"
        public const string EVENT_CONTENT_WINDOW_OPEN = "WindowOpen";
        public const string EVENT_CONTENT_CALLBACK_WINDOW = "CallbackWindow";

        public const string BROADCAST_FILTER_VIEW_SHOW = "ViewShow";
        public const string BROADCAST_FILTER_VIEW_HIDE = "ViewHide";
        public const string BROADCAST_FILTER_WINDOW_OPEN = "WindowOpen";
        public const string BROADCAST_FILTER_WINDOW_CLOSE = "WindowClose";
        public const string BROADCAST_FILTER_SCENE_LOAD = "SceneLoad";

        public const string BROADCAST_CONTENT_VIEW_BEHAVIOR_START = "ViewBehaviorStart";
        public const string BROADCAST_CONTENT_VIEW_BEHAVIOR_FINISH = "ViewBehaviorFinish";
        public const string BROADCAST_CONTENT_SWITCH_SCENE = "SwitchScene";
        public const string BROADCAST_CONTENT_APPEND_SCENE = "AppendScene";
        public const string BROADCAST_CONTENT_REMOVE_SCENE = "RemoveScene";

        public const string EXTRA_TAG_WINDOW_POSITION = "WindowPosition";
        public const string EXTRA_TAG_WINDOW_IDENTIFIER = "WindowIdentifier";
        public const string EXTRA_TAG_WINDOW_DOMAIN = "WindowDomain";
        public const string EXTRA_TAG_CALLBACK_TARGET = "CallbackTarget";
        public const string EXTRA_TAG_VIEW_NAME = "ViewName";
        public const string EXTRA_TAG_VIEW_CATEGORY = "ViewCategory";
        public const string EXTRA_TAG_SCENE_NAME = "SceneName";

        public const string TARGET_SCENE_QUIT = "EXIT_GAME";

        public const string TAG_NULL_OBJ = "NULL_OBJECT";
        public const string TAG_SYSTEM_OBJ = "GameSystem";
        #endregion
        #region "Dynamic Configs"
        public const float GUI_RESOLUTION_REF = 1920f; // 这是UGUI的参考分辨率，也就是CanvasScaler上的缩放参考，可以是宽或者高
        public static float GUI_ANCHORED_RATIO = 1f; // 这是UGUI的比例参数，用于将屏幕位置映射到GUI坐标
        #endregion
    }
}
