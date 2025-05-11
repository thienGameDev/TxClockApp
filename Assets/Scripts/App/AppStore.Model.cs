using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Core.Framework
{
    public partial class AppStore
    {
#pragma warning disable 0618
        public class State
        {
            public Dictionary<IModuleContextModel, IBaseModule> Modules;
            public IScreenController CurrentScreen;

            public State(
                [Inject(Id = ScreenName.SessionStart)]
                IScreenController startSessionScreen)
            {
                Modules = new Dictionary<IModuleContextModel, IBaseModule>();
                CurrentScreen = startSessionScreen;
            }

            public bool HasModel(IModuleContextModel model)
            {
                return Modules.ContainsKey(model);
            }

            public bool HasModel(ModuleName moduleName)
            {
                return TryGetModel(moduleName, out var model);
            }

            public bool TryGetModel(ModuleName moduleName, out IModuleContextModel model)
            {
                model = null;
                foreach (var module in Modules)
                    if (module.Key.ModuleName == moduleName)
                        model = module.Key;
                return model != null;
            }

            public IModuleContextModel PreAddModelToAvoidDuplication(IModuleContextModel model)
            {
                Modules.Add(model, null);
                return model;
            }

            public void RemoveModel(IModuleContextModel tModel)
            {
                Modules[tModel].Remove();
                Modules.Remove(tModel);
            }

            public void RemoveModel(ModuleName moduleName)
            {
                if(TryGetModel(moduleName, out var model))
                    RemoveModel(model);
            }

            public void BindModuleToModel(IModuleContextModel model, IBaseModule module)
            {
                if(Modules.ContainsKey(model))
                    Modules[model] = module;
            }

            public void RemoveAllModules()
            {
                var items = Modules.Keys.Select(d => d).ToList();
                foreach ( var model in items)
                    RemoveModel(model);
            }

            public void OnQuit()
            {
                foreach (var module in Modules)
                    module.Value.Quit();
            }

            public bool TryGetModule(IModuleContextModel tModel, out IBaseModule module)
            {
                return Modules.TryGetValue(tModel, out module);
            }

            public bool TryGetModule(ModuleName moduleName, out IBaseModule module)
            {
                module = null;
                foreach (var m in Modules)
                    if (m.Key.ModuleName == moduleName)
                        module = m.Value;
                return module != null;
            }

            public class MissingModel : Exception { }
        }
#pragma warning restore 0618
    }
}
