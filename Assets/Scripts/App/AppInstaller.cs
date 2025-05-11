using UnityEngine;
using System.Diagnostics;
using Zenject;
using Core.Infrastructure.Logger;
using Services;
using UIModules;

namespace Core.Framework
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Application.targetFrameRate = 60;

            Container.Bind<AppStore.State>().AsSingle();
            Container.BindInterfacesAndSelfTo<AppStore>().AsSingle();

            AppRootInstaller.Install(Container);
            UnityEngine.Debug.LogFormat("AppInstaller took {0:0.00} seconds", stopwatch.Elapsed.TotalSeconds);
            stopwatch.Stop();
        }
    }

    public class AppRootInstaller : Installer<AppRootInstaller>
    {
        public override void InstallBindings()
        {
            InstallReducers();
            InstallAppSignal();
            InstallAppState();
            InstallModules();
            InstallServices();
            InstallShareLogger();
            InstallUtilities();
        }
        
        private void InstallServices()
        {
            Container.Bind<ITimeProvider>().To<RealtimeProvider>().AsSingle();
            Container.Bind<ILocalClockService>().To<LocalClockService>().AsSingle();
            Container.Bind<IStopwatchService>().To<StopwatchService>().AsSingle();
            Container.Bind<ITimerService>().To<TimerService>().AsSingle();
        }
        
        private void InstallModules()
        {
            Container.BindFactory<IBaseModule, BaseModuleFactory>()
                .WithId(ModuleName.ClockApp).To<ClockApp>()
                .FromSubContainerResolve()
                .ByInstaller<ClockApp.Installer>();
            Container.BindFactory<GameObject, IModuleContextModel, IView, ViewFactory>()
                .WithId(ModuleName.ClockApp)
                .FromFactory<ViewCustomFactory<ClockAppView>>();
        }

        private void InstallShareLogger()
        {
            Container.BindInterfacesTo<UnityConsoleLogger>().AsSingle();
        }

        private void InstallAppState()
        {
            Container.Bind<IScreenController>().WithId(ScreenName.SessionStart).To<StartSessionScreenController>().AsSingle();
        }
        
        private void InstallUtilities() {
            Container.Bind<IBundleLoader>().WithId(BundleLoaderName.Resource).To<ResourceLoader>().AsSingle();
            Container.Bind<IBundleLoader>().WithId(BundleLoaderName.Addressable).To<AddressableLoader>().AsSingle();
        }
        
        private void InstallAppSignal()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignalWithInterfaces<AppActionSignal>().OptionalSubscriber().RunAsync();
            Container.DeclareSignalWithInterfaces<AppActionSignal<IModuleContextModel>>().OptionalSubscriber().RunAsync();
            Container.DeclareSignalWithInterfaces<AppActionSignal<ModuleName>>().OptionalSubscriber().RunAsync();
            Container.DeclareSignalWithInterfaces<AppActionSignal<ScreenName>>().OptionalSubscriber().RunAsync();
        }
        
        private void InstallReducers()
        {
            Container.BindInterfacesTo<ModuleReducer>().AsSingle();
            Container.BindInterfacesTo<ModuleReducerByName>().AsSingle();
        }
    }
}
