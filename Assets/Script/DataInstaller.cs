using Zenject;

namespace MegaJumper
{
    [UnityEngine.CreateAssetMenu(menuName = "Data Installer")]
    public class DataInstaller : ScriptableObjectInstaller
    {
        public BlockContainer blockContainer;
        public Monetization.AdmobKeyStorer admobKeyStorer;
        public SettlementSettingContainer settlementSettingContainer;
        public GameProperties gameProperties;

        public override void InstallBindings()
        {
            Container.BindInstances(blockContainer);
            Container.BindInstances(gameProperties);
            Container.BindInstance(admobKeyStorer);
            Container.BindInstances(settlementSettingContainer);
        }
    }
}