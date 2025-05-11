using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

#pragma warning disable 0618
namespace Core.Framework
{
    public class ModuleReducer: IAppReducer<IModuleContextModel>
    {
        private readonly DiContainer _container;
        private readonly AppStore.State GState;
        private readonly ILogger _logger;

        public ModuleReducer(
            DiContainer container, 
            AppStore.State gState,
            ILogger logger)
        {
            _container = container;
            GState = gState;
            _logger = logger;
        }
        public GameReducerInfo<IModuleContextModel>[] RegisHandler()
        {
            return new GameReducerInfo<IModuleContextModel>[]
            {
                new GameReducerInfo<IModuleContextModel>(){Action = AppAction.ModuleCreate, Handler = CreateModule },
                new GameReducerInfo<IModuleContextModel>(){Action = AppAction.ModuleRemove, Handler = RemoveModule }
            };
        }

        private async void CreateModule(IModuleContextModel model)
        {
            if (GState.HasModel(model))
                _logger.Warning($"Duplicate Found on Module: {model.ModuleName}");

            GState.PreAddModelToAvoidDuplication(model);

            IBaseModule module = await CreateModuleAndView(model);

            GState.BindModuleToModel(model, module);
        }

        private async UniTask<IBaseModule> CreateModuleAndView(IModuleContextModel model)
        {
            BaseModuleFactory baseContextFactory = _container.ResolveId<BaseModuleFactory>(model.ModuleName);
            IBaseModule module = baseContextFactory.Create();

            GameObject pref = await module.Initialize(model);
            return await CreateView(model, module, pref);
        }

        private async UniTask<IBaseModule> CreateView(IModuleContextModel model, IBaseModule module, GameObject pref)
        {
            ViewFactory viewFactory = _container.ResolveId<ViewFactory>(model.ModuleName);
            IView view = viewFactory.Create(pref, model);
            await module.CreateView(model, view);
            return module;
        }

        private void RemoveModule(IModuleContextModel model)
        {
            if (GState.HasModel(model))
                GState.RemoveModel(model);
            else
                _logger.Warning($"Try to remove non exist module: {model.ModuleName}");
        }
    }

    public class ModuleReducerByName : IAppReducer<ModuleName>
    {
        private readonly AppStore.State GState;
        private readonly ILogger _logger;

        public ModuleReducerByName(
            DiContainer container,
            AppStore.State gState,
            ILogger logger)
        {
            GState = gState;
            _logger = logger;
        }
        public GameReducerInfo<ModuleName>[] RegisHandler()
        {
            return new GameReducerInfo<ModuleName>[]
            {
                new GameReducerInfo<ModuleName>(){Action = AppAction.ModuleRemove, Handler = RemoveModule },
                new GameReducerInfo<ModuleName>(){Action = AppAction.ModuleHide, Handler = HideModule },
                new GameReducerInfo<ModuleName>(){Action = AppAction.ModuleShow, Handler = ShowModule },
                new GameReducerInfo<ModuleName>(){Action = AppAction.ModuleRemoveAll, Handler = RemoveAllModules }
            };
        }

        private void RemoveAllModules(ModuleName module)
        {
            GState.RemoveAllModules();
        }

        private void RemoveModule(ModuleName module)
        {
            if (GState.HasModel(module))
                GState.RemoveModel(module);
            else
                _logger.Warning($"Try to remove non exist module: {module}");
        }

        private void HideModule(ModuleName moduleName)
        {
            if (GState.TryGetModule(moduleName, out IBaseModule module))
                module.ContextView.Hide();
            else
                _logger.Warning($"Try to remove non exist module: {moduleName}");
        }

        private void ShowModule(ModuleName moduleName)
        {
            if (GState.TryGetModule(moduleName, out IBaseModule module))
                module.ContextView.Show();
            else
                _logger.Warning($"Try to remove non exist module: {moduleName}");
        }

    }
}
#pragma warning restore 0618
