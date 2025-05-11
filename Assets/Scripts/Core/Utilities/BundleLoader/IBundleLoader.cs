using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Framework
{
    public interface IBundleLoader
    {
        UniTask<T> LoadAssetAsync<T>(string path) where T : Object;
        void ReleaseAsset(string path);
    }
}
