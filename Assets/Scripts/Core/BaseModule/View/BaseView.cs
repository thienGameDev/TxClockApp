using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Framework
{
    public interface IView
    {
        IBaseModule BaseModule { get; set; }
        BaseViewConfig Config { get; set; }
        GameObject RootGO { get;}
        void Destroy();
        UniTask Show();
        UniTask Hide();
        void OnReady();
    }

    public class BaseViewConfig
    {
        public bool SkipLayout;
        public ViewLayerManager.ViewLayer Layer = ViewLayerManager.ViewLayer.None;
        public AnchorPresets AnchorPreset = AnchorPresets.StretchAll;
        public Vector2 AnchorPos = Vector2.zero;
        public Vector2 SizeDelta = Vector2.zero;

        public static BaseViewConfig DefaultConfig
        {
            get
            {
                return new BaseViewConfig()
                {
                    SkipLayout = false,
                    Layer = ViewLayerManager.ViewLayer.Main,
                    AnchorPreset = AnchorPresets.StretchAll,
                    AnchorPos = Vector2.zero,
                    SizeDelta = Vector2.zero
                };
            }
        }
            
    }

}
