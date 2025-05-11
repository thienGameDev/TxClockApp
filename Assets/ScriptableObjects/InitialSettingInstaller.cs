using UnityEngine;
using Zenject;

namespace Core.Framework
{
    [CreateAssetMenu(fileName = "InitialSetting", menuName = "Configs/InitialConfig", order = 1)]
    public class InitialSettingInstaller: ScriptableObjectInstaller<InitialSettingInstaller>
    {
        public AppStore.Setting GameSetting;
        public AppStore.Atlas Atlas;

        public override void InstallBindings()
        {
            Container.BindInstance(GameSetting);
            Container.BindInstance(Atlas);
        }
    }
}
