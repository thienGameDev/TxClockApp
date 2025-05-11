using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework
{
    public abstract class View<T> : MonoBehaviour, IView where T : IBaseModule
    {
        Dictionary<Type, Tween> _cachedTween = new ();
        private ILogger _logger;

        private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup
        {
            get
            {
                if(_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();

                return _canvasGroup;
            }
        }
        
        public T Module {
            get => (T)BaseModule;
            set => BaseModule = value;
        }

        public BaseViewConfig Config { get; set; }
        public IBaseModule BaseModule { get; set; }
        public GameObject RootGO => gameObject;
        public abstract void OnReady();
        public virtual void Destroy()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
        public virtual async UniTask Show()
        {
            DoFade(1, 0.5f);
            await UniTask.Delay(500);
        }
        public virtual async UniTask Hide()
        {
            DoFade(0, 0.5f);
            await UniTask.Delay(500);
        }
        private void DoFade(
            float value, 
            float duration,
            Ease ease = Ease.Linear)
        {
            KillCurrentTweenOfType<CanvasGroup>();
            Tween tween = 
                CanvasGroup
                .DOFade(value, duration)
                .SetEase(ease);

            CacheTween<CanvasGroup>(tween);
        }
        private void KillCurrentTweenOfType<c>()
        {
            if (_cachedTween.ContainsKey(typeof(c)))
            {
                _cachedTween[typeof(c)].Pause();
                _cachedTween[typeof(c)].Kill();
                _cachedTween.Remove(typeof(c));
            }
        }
        private void CacheTween<d>(Tween tween)
        {
            _cachedTween.Add(typeof(d), tween);
        }
        
    }
}
