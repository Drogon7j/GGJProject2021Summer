// File create date:3/25/2019
using Newtonsoft.Json;
using RoachGame.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoachGame.Services {
    /// <summary>
    /// 资源服务，提供一个自动卸载机制，按照时间戳和时间阈值来卸载资源
    /// </summary>
    public class AssetService : EmptyService {

        public const string SERVICE_NAME = "AssetService";

        public static string REMOTE_ASSET_SERVER = Application.streamingAssetsPath;
        private const string ASSET_BUNDLE_MANIFEST = "AssetBundleManifest";

        /// <summary>
        /// 自动卸载时间阈值，表示如果有资源的空闲时间超过该值则卸载，单位是秒
        /// </summary>
        public float automaticUnloadTime = 600f;

        protected AssetBundleManifest bundleManifest;
        protected AssetBundleCacheCell bundleManifestCell;
        protected Dictionary<string, string> assetMapping;
        protected Dictionary<string, AssetBundleCacheCell> bundleCache;

        public AssetService() {
            bundleCache = new Dictionary<string, AssetBundleCacheCell>();
            assetMapping = new Dictionary<string, string>();
        }

        public override void InitService() {
            LogUtils.LogNotice("Asset Service Initiated!");
        }

        public override void KillService() {
            LogUtils.LogNotice("Asset Service Destoryed!");
        }

        /// <summary>
        /// 同步加载动态资源映射信息，默认为从Resources中加载，可重写
        /// 包含具体资源对象与AssetBundle包的对应关系
        /// </summary>
        /// <param name="uri">Mapping文件路径</param>
        public virtual void LoadAssetMappingInfoSync(string uri) {
            var jsonFile = Resources.Load<TextAsset>(uri);
            assetMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFile.text);
        }

        /// <summary>
        /// 异步加载动态资源映射信息，默认为从Resources中加载，可重写
        /// 包含具体资源对象与AssetBundle包的对应关系
        /// </summary>
        /// <param name="uri">Mapping文件路径</param>
        /// <returns>异步协程</returns>
        public virtual IEnumerator LoadAssetMappingInfoAsyn(string uri) {
            var request = Resources.LoadAsync<TextAsset>(uri);
            yield return request;
            var jsonFile = request.asset as TextAsset;
            assetMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFile.text);
        }

        /// <summary>
        /// 读取AssetBundleManifest信息，包含所有Bundle的依赖关系
        /// </summary>
        /// <param name="platform">Manifest标识，一般是平台名称</param>
        /// <returns>协程枚举器</returns>
        public IEnumerator LoadManifest(string platform) {
            var manifestUri = $"{REMOTE_ASSET_SERVER}/{platform}";
            var request = UnityWebRequestAssetBundle.GetAssetBundle(manifestUri);
            yield return request.SendWebRequest();
            var manifestBundle = DownloadHandlerAssetBundle.GetContent(request);
            bundleManifestCell = new AssetBundleCacheCell(ASSET_BUNDLE_MANIFEST, manifestBundle);
            bundleManifest = manifestBundle.LoadAsset<AssetBundleManifest>(ASSET_BUNDLE_MANIFEST);
        }

        /// <summary>
        /// 本地同步读取AssetBundleManifest信息，包含所有Bundle的依赖关系
        /// </summary>
        /// <param name="path">Manifest文件路径</param>
        public void LoadManifestLocal(string path) {
            var manifestBundle = AssetBundle.LoadFromFile(path);
            bundleManifestCell = new AssetBundleCacheCell(ASSET_BUNDLE_MANIFEST, manifestBundle);
            bundleManifest = manifestBundle.LoadAsset<AssetBundleManifest>(ASSET_BUNDLE_MANIFEST);
        }

        /// <summary>
        /// 通用的Bundle加载方法，协程异步，可配置自动或手动管理，考虑依赖
        /// </summary>
        /// <param name="bundleUri"></param>
        /// <param name="isManual"></param>
        /// <returns></returns>
        public IEnumerator LoadBundle(string bundleUri, bool isManual) {
            var bundleName = GetNameFromPath(bundleUri);
            if (!bundleCache.ContainsKey(bundleName)) {
                // 缓存里没有指定Bundle，可能是未加载或者被自动卸载了
                LogUtils.LogNotice($"Load Bundle[{bundleName}] at [{bundleUri}]");
                var request = UnityWebRequestAssetBundle.GetAssetBundle(bundleUri);
                yield return request.SendWebRequest();
                var cell = new AssetBundleCacheCell(bundleName, DownloadHandlerAssetBundle.GetContent(request));
                cell.isAutoUnload = !isManual;
                cell.lastActiveTime = Time.realtimeSinceStartup;
                var dps = bundleManifest.GetAllDependencies(bundleName);
                cell.dpBundles.PutSet(dps);
                bundleCache[bundleName] = cell;
                foreach (var dp in dps) {
                    yield return LoadBundle($"{REMOTE_ASSET_SERVER}/{dp}", isManual);
                }
                // 至此包括其依赖在内的全部Bundle加载完成
            } else {
                // 缓存里已经存在指定的Bundle，更新一次时间即可
                bundleCache[bundleName].lastActiveTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 从本地同步加载单个Bundle，手动管理，不考虑依赖问题，请谨慎使用
        /// </summary>
        /// <param name="bundleUri">Bundle的文件路径</param>
        /// <returns>AssetBundle数据</returns>
        public AssetBundle LoadBundleLocal(string bundleUri) {
            var bundleName = GetNameFromPath(bundleUri);
            if (!bundleCache.ContainsKey(bundleName)) {
                // 缓存里没有这个Bundle，说明没加载过
                var bundle = AssetBundle.LoadFromFile(bundleUri);
                var cell = new AssetBundleCacheCell(bundleName, bundle);
                cell.isAutoUnload = false;
                cell.lastActiveTime = Time.realtimeSinceStartup;
                var dps = bundleManifest.GetAllDependencies(bundleName);
                cell.dpBundles.PutSet(dps);
                bundleCache[bundleName] = cell;
            } else {
                bundleCache[bundleName].lastActiveTime = Time.realtimeSinceStartup;
            }
            return bundleCache.TryGetElement(bundleName)?.assetBundle;
        }

        /// <summary>
        /// 从路径中提取Bundle名称
        /// </summary>
        /// <param name="path">路径字符串</param>
        /// <returns>名称字符串</returns>
        private string GetNameFromPath(string path) {
            var slashIndex = path.LastIndexOf('/');
            var bslashIndex = path.LastIndexOf('\\');
            var pointIndex = path.Length;
            var seperatorIndex = Mathf.Max(slashIndex, bslashIndex);
            var nameLength = pointIndex - seperatorIndex - 1;
            return path.Substring(seperatorIndex + 1, nameLength);
        }

        /// <summary>
        /// 加载指定名称的资源，异步协程，通过回调完成资源配置
        /// </summary>
        /// <typeparam name="ResType">资源类型</typeparam>
        /// <param name="resName">资源名称</param>
        /// <param name="callback">资源回调</param>
        /// <returns>协程枚举器</returns>
        public IEnumerator LoadAsset<ResType>(string resName, Action<ResType> callback) where ResType : UnityEngine.Object {
            if (assetMapping != null) {
                if (!string.IsNullOrEmpty(resName)) {
                    if (assetMapping.ContainsKey(resName)) {
                        // 找到了映射，获取所在的Bundle
                        var bundleName = assetMapping[resName];
                        var bundleUri = $"{REMOTE_ASSET_SERVER}/{bundleName}";
                        yield return LoadBundle(bundleUri, false);
                        // 加载过程会自动检查依赖并加载对应Bundle，所以直接回调即可
                        callback?.Invoke(bundleCache[bundleName].assetBundle.LoadAsset<ResType>(resName));
                    } else {
                        LogUtils.LogError("未能找到资源所对应的Bundle信息，请确认已正确生成资源映射表。");
                    }
                } else {
                    LogUtils.LogError("资源名为空，请检查方法调用。");
                }
            } else {
                LogUtils.LogError("资源映射表未加载，请检查加载流程。");
            }
        }

        /// <summary>
        /// 从缓存里加载资源，不会自动加载Bundle，同步配置资源
        /// </summary>
        /// <typeparam name="ResType">资源类型</typeparam>
        /// <param name="resName">资源名称</param>
        /// <returns>资源对象</returns>
        public ResType LoadAssetFromCache<ResType>(string resName) where ResType : UnityEngine.Object {
            if (assetMapping != null) {
                if (!string.IsNullOrEmpty(resName)) {
                    if (assetMapping.ContainsKey(resName)) {
                        // 找到了映射，获取所在的Bundle
                        var bundleName = assetMapping[resName];
                        UpdateBundleActiveTime(bundleName, Time.realtimeSinceStartup);
                        if (bundleCache.ContainsKey(bundleName)) {
                            return bundleCache[assetMapping[resName]].assetBundle.LoadAsset<ResType>(resName);
                        }
                    } else {
                        LogUtils.LogError("未能找到资源所对应的Bundle信息，请确认已正确生成资源映射表。");
                    }
                } else {
                    LogUtils.LogError("资源名为空，请检查方法调用。");
                }
            } else {
                LogUtils.LogError("资源映射表未加载，请检查加载流程。");
            }
            return null;
        }

        /// <summary>
        /// 根据URI信息加载资源，同步操作，不会自动加载Bundle
        /// </summary>
        /// <typeparam name="ResType">资源类型</typeparam>
        /// <param name="uri">资源的URI配置</param>
        /// <returns>资源对象</returns>
        public ResType LoadAssetByURI<ResType>(ResourceURI uri) where ResType : UnityEngine.Object {
            ResType result = null;
            switch (uri.type) {
                case ResourceType.Build_in:
                    result = ResourceUtils.LoadResource<ResType>(uri.resPath);
                    break;
                case ResourceType.Asset_Bundle:
                    result = LoadAssetFromCache<ResType>(uri.resPath);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 更新指定BundleCell的活动时间
        /// </summary>
        /// <param name="bundleName">Bundle名称</param>
        /// <param name="time">活动时间</param>
        private void UpdateBundleActiveTime(string bundleName, float time) {
            if (bundleCache.ContainsKey(bundleName)) {
                var cell = bundleCache[bundleName];
                cell.lastActiveTime = time;
                foreach (var dp in cell.dpBundles) {
                    UpdateBundleActiveTime(dp, time);
                }
            }
        }

        /// <summary>
        /// 一次性卸载所有手动管理的资源，请谨慎使用
        /// </summary>
        public void UnloadManually() {
            var bundleToDelete = new HashSet<string>();
            foreach (var name in bundleCache.Keys) {
                if (!bundleCache[name].isAutoUnload) {
                    bundleToDelete.Add(name);
                }
            }
            foreach (var name in bundleToDelete) {
                bundleCache[name].assetBundle.Unload(false);
                bundleCache.Remove(name);
            }
        }

        /// <summary>
        /// 卸载所有加载的资源，注意参数，谨慎使用
        /// </summary>
        /// <param name="forceUnload">强制卸载，启用时会将正在使用的资源一并卸载</param>
        public void UnloadAllBundle(bool forceUnload = false) {
            var bundleNames = new HashSet<string>(bundleCache.Keys);
            foreach (var name in bundleNames) {
                bundleCache[name].assetBundle.Unload(forceUnload);
                bundleCache.Remove(name);
            }
        }

        /// <summary>
        /// 自动资源监视器，需要放入定时器或协程内运行，监视缓存中的自动卸载资源
        /// 根据时间戳决定是否卸载，空闲时间阈值可以通过配置来改变，超过阈值的资源会自动卸载
        /// </summary>
        public void AutomaticAssetMonitor() {
            var bundleToDelete = new HashSet<string>();
            foreach (var name in bundleCache.Keys) {
                if (bundleCache[name].isAutoUnload) {
                    var deltaTime = Time.realtimeSinceStartup - bundleCache[name].lastActiveTime;
                    if (deltaTime >= automaticUnloadTime) {
                        bundleToDelete.Add(name);
                    }
                }
            }
            foreach (var name in bundleToDelete) {
                bundleCache[name].assetBundle.Unload(false);
                bundleCache.Remove(name);
            }
        }

        /// <summary>
        /// AssetBundle的缓存单元
        /// </summary>
        public class AssetBundleCacheCell {

            public string assetName;
            public float lastActiveTime;
            public bool isAutoUnload;
            public AssetBundle assetBundle;
            public HashSet<string> dpBundles;

            public AssetBundleCacheCell(string name, AssetBundle bundle) {
                assetName = name;
                lastActiveTime = Time.time;
                assetBundle = bundle;
                dpBundles = new HashSet<string>();
            }
        }
    }

    public enum ResourceType {
        Build_in, // Resources文件夹
        Asset_Bundle, // AssetService加载
        Addressable // 待实现
    }

    public class ResourceURI {
        public ResourceType type;
        public string resPath;

        public ResourceURI(string path, ResourceType t = ResourceType.Build_in) {
            resPath = path;
            type = t;
        }

        public override string ToString() {
            return $"From[{type.ToString()}], Path[{resPath}]";
        }
    }
}
