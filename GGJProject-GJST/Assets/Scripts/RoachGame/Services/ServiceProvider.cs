/// File create date:8/3/2018
using RoachGame.Utils;
using System.Collections.Generic;
/// Created By Yu.Liu
namespace RoachGame.Services {
    /// <summary>
    /// 服务提供器
    /// </summary>
    public class ServiceProvider {
        /// <summary>
        /// 服务映射表
        /// </summary>
        private Dictionary<string, IGameService> services = new Dictionary<string, IGameService>();
        public static ServiceProvider Instance { get; } = new ServiceProvider();

        private ServiceProvider() { }
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="tag">服务标签</param>
        /// <param name="service">服务具体类</param>
        public void RegisterService(string tag, IGameService service) {
            if (services.ContainsKey(tag)) {
                LogUtils.LogWarning("Duplicated Service Tag detected, please advice!");
            } else {
                services[tag] = service;
                service.InitService();
            }
        }
        /// <summary>
        /// 反注册服务
        /// </summary>
        /// <param name="tag">服务标签</param>
        public void UnregisterService(string tag) {
            if (services.ContainsKey(tag)) {
                services[tag].KillService();
                services.Remove(tag);
            }
        }
        /// <summary>
        /// 提供服务，如果找不到对应服务则新建一个
        /// </summary>
        /// <typeparam name="ServiceType">服务类型，必须是类并且实现IGameService接口</typeparam>
        /// <param name="tag">服务标签</param>
        /// <returns>服务对象</returns>
        public ServiceType ProvideService<ServiceType>(string tag) where ServiceType : class, IGameService, new() {
            ServiceType result;
            if (services.ContainsKey(tag)) {
                result = services[tag] as ServiceType;
            } else {
                result = new ServiceType();
                result.InitService();
                services[tag] = result;
            }
            return result;
        }
    }
}
