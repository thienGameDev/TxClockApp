using Services;
using Zenject;
public class TestInstaller : Installer<TestInstaller> {
    public override void InstallBindings() {
        Container.Bind<MockTimeProvider>().AsSingle();
        Container.Bind<ITimeProvider>().To<MockTimeProvider>().FromResolve();
    }
}
