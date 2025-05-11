using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Framework
{
    public class AddressableLoader : IBundleLoader
    {
        
        private Dictionary<string, AsyncOperationHandle> _loadedOperations = new ();

        private bool IsInitialized = false;

        public AddressableLoader()
        {
            UseNewCachePath();
            InitializeAndCheckCatalogUpdates();
        }

        private void UseNewCachePath()
        {
            string fullAppDataPath = Application.persistentDataPath + "/AssetBundles";
            if(!Directory.Exists(fullAppDataPath))
                Directory.CreateDirectory(fullAppDataPath);

            Cache cache = Caching.GetCacheByPath(fullAppDataPath);
            if (cache == null || !cache.valid)
                cache = Caching.AddCache(fullAppDataPath);

            if(cache.valid)
                Caching.currentCacheForWriting = cache;
        }

        private void InitializeAndCheckCatalogUpdates()
        {
            Addressables.InitializeAsync().Completed += op =>
            {
                CheckForCatalogUpdates();
                IsInitialized = true;
            };
        }

        private void CheckForCatalogUpdates()
        {
            if (IsInitialized)
                return;

            List<string> catalogsToUpdate = new List<string>();
            Addressables.CheckForCatalogUpdates(true).Completed += op =>
            {
                catalogsToUpdate.AddRange(op.Result);
                UpdateCatalogs(catalogsToUpdate);
            };
        }

        private void UpdateCatalogs(List<string> catalogsToUpdate)
        {
            if (catalogsToUpdate.Count > 0)
                Addressables.UpdateCatalogs(catalogsToUpdate, true);
        }
        
        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                throw new AddressableRunOnEditorMode(path);
#endif
            try
            {
                AssetReference a = new AssetReference(path);
                AsyncOperationHandle<T> handle = a.LoadAssetAsync<T>();

                while (!handle.IsDone)
                    await UniTask.NextFrame();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    CacheLoadedOperation(path, handle);
                    return handle.Result;
                }
                else if ( handle.Status == AsyncOperationStatus.Failed 
                    || handle.Status == AsyncOperationStatus.None)
                    throw new AddressableCannotLoadAsset($"Path: {path} | status: {handle.Status}");

                return null;
            }
            catch
            {
                throw new MissingAddressableAssetAtPath(path);
            }
        }

        private void CacheLoadedOperation<T>(string path, AsyncOperationHandle<T> handle) where T : Object
        {
            if (!_loadedOperations.ContainsKey(path))
                _loadedOperations.Add(path, handle);
            else
                _loadedOperations[path] = handle;
        }

        public void ReleaseAsset(string path)
        {
            if (_loadedOperations.ContainsKey(path))
            {
                if (_loadedOperations[path].IsValid())
                    Addressables.Release(_loadedOperations[path]);
            }
        }

        private class MissingAddressableAssetAtPath : System.Exception
        {
            public MissingAddressableAssetAtPath(string message) : base(message) { }
        }

        private class AddressableRunOnEditorMode : System.Exception
        {
            public AddressableRunOnEditorMode(string message) : base(message) { }
        }
        
        private class AddressableCannotLoadAsset : System.Exception
        {
            public AddressableCannotLoadAsset(string message) : base(message) { }
        }
    }
}
