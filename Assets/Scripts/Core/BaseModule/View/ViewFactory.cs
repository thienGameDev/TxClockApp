using System;
using UnityEngine;
using Zenject;

namespace Core.Framework
{
    public class ViewFactory : PlaceholderFactory<GameObject, IModuleContextModel, IView> { }
    public class ViewCustomFactory<T> : IFactory<GameObject, IModuleContextModel, IView> where T : IView
    {
        private readonly DiContainer _container;
        private readonly ViewLayerManager _layerManager;

        public ViewCustomFactory(
            DiContainer container,
            ViewLayerManager layerManager)
        {
            _container = container;
            _layerManager = layerManager;
        }
        public IView Create(GameObject prefab, IModuleContextModel model)
        {
            CheckValidAndAddRequiredComponents(prefab);

            IView view = _container.InstantiatePrefabForComponent<T>(prefab);
            view.Config = model.Config;
            SetLayerAndConfigLayout(view.RootGO, view);
            return view;
        }
        private static void CheckValidAndAddRequiredComponents(GameObject prefab)
        {
            if (!prefab.TryGetComponent(out T l))
                throw new ViewScriptSetupMissing();

            if (!prefab.TryGetComponent(out CanvasGroup c))
                prefab.AddComponent<CanvasGroup>();
        }
        private void SetLayerAndConfigLayout(GameObject pref, IView model)
        {
            pref.transform.SetParent(_layerManager.GetLayerRoot(model.Config.Layer));
            if (pref.gameObject.activeInHierarchy)
                ResetlocalPosScaleAndConfigRect(pref, model);
        }
        private void ResetlocalPosScaleAndConfigRect(GameObject pref, IView model)
        {
            pref.transform.localPosition = Vector3.zero;
            pref.transform.localScale = Vector3.one;
            if (!model.Config.SkipLayout)
                ConfigRect(pref, model);
        }
        private static void ConfigRect(GameObject pref, IView model)
        {
            RectTransform rect = pref.GetComponent<RectTransform>();
            rect.SetAnchor(model.Config.AnchorPreset);
            rect.sizeDelta = model.Config.SizeDelta;
            rect.anchoredPosition = new Vector3(model.Config.AnchorPos.x, model.Config.AnchorPos.y, 0);
        }

        private class ViewScriptSetupMissing : Exception { }
    }
}
