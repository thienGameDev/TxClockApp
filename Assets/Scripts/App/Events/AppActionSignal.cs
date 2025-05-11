
namespace Core.Framework
{
    public class AppActionSignal
    {
        public AppAction Action { get; private set; }

        public AppActionSignal(AppAction action)
        {
            Action = action;
        }
    }

    public class AppActionSignal<TModel>
    {
        public TModel NewModel { get; private set; }
        public AppAction Action { get; private set; }

        public AppActionSignal(AppAction action, TModel newModel)
        {
            if (newModel == null)
                throw new AppActionModelIsNull();

            Action = action;
            NewModel = newModel;
        }

        public class AppActionModelIsNull : System.Exception { }
    }

    public enum AppAction
    {
        ModuleCreate,
        ModuleRemove,
        ModuleHide,
        ModuleShow,
        ModuleRemoveAll,
        ScreenChange
    }
}
