using System;
using System.Collections.Generic;
using Zenject;

namespace Core.Framework
{
    public partial class AppStore : IInitializable, IDisposable
    {
        private static SignalBus _signalBus;

        public State GState { get; set; }

        public AppStore(
            State gState,            
            SignalBus signalBus,
            List<IAppReducer<IModuleContextModel>> contextModelReducer,
            List<IAppReducer<ModuleName>> moduleNameReducer,
            List<IAppReducer<ScreenName>> screenNameReducer)
        {
            _signalBus = signalBus;
            GState = gState;

            new Reducer<IModuleContextModel>(contextModelReducer, signalBus);
            new Reducer<ModuleName>(moduleNameReducer, signalBus);
            new Reducer<ScreenName>(screenNameReducer, signalBus);
        }

        public void Initialize()
        {
            GState.CurrentScreen.Enter();
        }

        public void Dispose()
        {
            GState.OnQuit();
        }

        #region Helper function
        public static void CreateModule(IModuleContextModel model)
        {
            _signalBus.Fire(
               new AppActionSignal<IModuleContextModel>(AppAction.ModuleCreate, model));
        }
        public static void RemoveModule<T>(T model)
        {
            _signalBus.Fire(
               new AppActionSignal<T>(AppAction.ModuleRemove, model));
        }
        public static void ChangeScreen(ScreenName name)
        {
            _signalBus.Fire(
               new AppActionSignal<ScreenName>(AppAction.ScreenChange, name));
        }
        #endregion
    }
}
