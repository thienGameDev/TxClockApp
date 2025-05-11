using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Core.Framework
{
    public enum ModuleName
    {
        ClockApp
    }
    public enum BundleLoaderName
    {
        Resource,
        Addressable
    }
    
    public class BaseModuleFactory : PlaceholderFactory<IBaseModule> 
    {
#pragma warning disable 0618
        private readonly IBundleLoader _bundleLoader;

        public BaseModuleFactory(
            [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader bundleLoader)
        {
            _bundleLoader = bundleLoader;
        }

        public override IBaseModule Create()
        {
            IBaseModule baseModule = base.Create();
            baseModule.SetupDependency(_bundleLoader);
            return baseModule;
        }
#pragma warning restore 0618
    }

    public abstract class BaseModule<TView, TModel>: IBaseModule, IBaseModule<TModel>
        where TView : class,IView
        where TModel : class, IModuleContextModel
    {
        public IModuleContextModel ContextModel { get; private set; }
        public TModel Model { get; set; }
        protected TView _view { get; private set; }
        public IView ContextView { get; private set; }
        private IBundleLoader _bundleLoader;
        protected abstract void OnViewReady();
        protected abstract void OnDisposed();        
        public virtual async UniTask<GameObject> Initialize(IModuleContextModel model) 
        {
            ContextModel = model;
            Model = ContextModel as TModel;
            if (Model == null)
                throw new InvalidModelException();
            GameObject pref = await LoadPrefab();
            return pref;
        }       
        public virtual async UniTask CreateView(IModuleContextModel model, IView view)
        {
            ContextView = view;
            _view = ContextView as TView;
            if (_view == null)
                throw new InvalidViewException();

            _view.BaseModule = this;
            await _view.Show();
            _view.OnReady();
            OnViewReady();                      
        }        
        public virtual async UniTask Remove()
        {
            if (_view != null)
            {
                await _view.Hide();
                _view.Destroy();
                _bundleLoader.ReleaseAsset(ContextModel.ViewId);
                OnDisposed();
            }
        }        
        public void Quit()
        {
            OnDisposed();
        }        
        public void SetupDependency(IBundleLoader bundleLoader)
        {
            _bundleLoader = bundleLoader;
        }
        private async UniTask<GameObject> LoadPrefab()
        {
            GameObject viewPref = await _bundleLoader.LoadAssetAsync<GameObject>(ContextModel.ViewId);

            return viewPref;
        }
        #region Exceptions
        private class InvalidViewException: Exception { }
        private class InvalidModelException : Exception { }
        #endregion

    }

    public abstract class BaseModuleContextModel : IModuleContextModel
    {
        public abstract string ViewId { get; }
        public abstract ModuleName ModuleName { get; }
        public virtual BaseViewConfig Config => BaseViewConfig.DefaultConfig;
        public virtual T GetConcreteModel<T>() where T: class, IModuleContextModel
        {
            if (this is T)
                return this as T;
            throw new ConcreteClassMismatchException(typeof(T).ToString());
        }
        public override bool Equals(object obj)
        {
            return Equals((BaseModuleContextModel)obj);
        }
        public bool Equals(BaseModuleContextModel p)
        {
            if (ReferenceEquals(p, null))
            {
                return false;
            }

            if (ReferenceEquals(this, p))
            {
                return true;
            }

            if (GetType() != p.GetType())
            {
                return false;
            }

            return this.ViewId == p.ViewId 
                && this.ModuleName == p.ModuleName;
        }
        public override int GetHashCode()
        {
            return ViewId.GetHashCode() + ModuleName.GetHashCode();
        }
        public override string ToString()
        {
            return $"({ViewId},{ModuleName})";
        }
        public static bool operator ==(BaseModuleContextModel l, BaseModuleContextModel r)
        {
            if (ReferenceEquals(l, null))
            {
                if (ReferenceEquals(r, null))
                {
                    return true;
                }

                return false;
            }
            return l.Equals(r);
        }
        public static bool operator !=(BaseModuleContextModel l, BaseModuleContextModel r)
        {
            return !(l == r);
        }

        private class ConcreteClassMismatchException: Exception 
        { 
            public ConcreteClassMismatchException(string message) : base(message)
            {

            }
        }
    }

    public interface IModuleContextModel
    {
        string ViewId { get; }
        ModuleName ModuleName { get; }
        BaseViewConfig Config { get; }
        T GetConcreteModel<T>() where T : class, IModuleContextModel;
    }

    public interface IBaseModule
    {
        IView ContextView { get; }
        IModuleContextModel ContextModel { get; }
        /// <summary>
        /// Only Override if needed.
        /// Called by GameStore reducers only.
        /// </summary>
        [Obsolete("Only use by GameStore reducers")]
        UniTask<GameObject> Initialize(IModuleContextModel model);
        /// <summary>
        /// Only Override if needed.
        /// Called by GameStore reducers only.
        /// </summary>
        [Obsolete("Only use by GameStore reducers")]
        UniTask CreateView(IModuleContextModel model,IView viewContext);
        /// <summary>
        /// Only Override if needed.
        /// Called by GameStore reducers only.
        /// </summary>
        [Obsolete("Only use by GameStore reducers")]
        UniTask Remove();
        /// <summary>
        /// Only Override if needed.
        /// Called by GameStore reducers only.
        /// </summary>
        [Obsolete("Only use by GameStore reducers")]
        void Quit();
        /// <summary>
        /// Only call by factory process do not use.
        /// </summary>
        [Obsolete("Only use by factory do not use")]
        void SetupDependency(IBundleLoader bundleLoader);
    }
    public interface IBaseModule<TModel>
        where TModel : class, IModuleContextModel
    {
        public TModel Model { get; }
    }

}
