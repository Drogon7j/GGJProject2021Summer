/// File create date:8/3/2018
using RoachGame.Utils;
/// Created By Yu.Liu
namespace RoachGame.Services {
    /// <summary>
    /// 空游戏服务，作为基础服务类和服务回滚
    /// </summary>
    public class EmptyService : IGameService {

        public virtual void InitService() {
            LogUtils.LogWarning("Empty Service initiated, be adviced!");
        }

        public virtual void KillService() {
            LogUtils.LogWarning("Empty Service killed, be adviced!");
        }

        public T To<T>() where T : class, IGameService {
            return this as T;
        }
    }
}
