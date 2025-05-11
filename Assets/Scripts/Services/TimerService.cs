using System;
using UniRx;

namespace Services {
    public class TimerService : ITimerService {
        private readonly ITimeProvider _timeProvider;
        private readonly CompositeDisposable _disposable = new ();
        private readonly ReactiveProperty<TimeSpan> _remainingTime = new ();
        private readonly ReactiveProperty<TimerState> _state = new (TimerState.Idle);

        private TimeSpan _initialDuration;
        private IDisposable _timerSubscription;

        public TimerService(ITimeProvider timeProvider) {
            _timeProvider = timeProvider;
        }

        public ReactiveProperty<TimerState> State => _state;
        public ReactiveProperty<TimeSpan> RemainingTime => _remainingTime;

        public TimeSpan GetInitialDuration() {
            return _initialDuration;
        }

        public void Start(TimeSpan duration) {
            _state.Value = TimerState.Running;
            _timerSubscription?.Dispose();
            
            _initialDuration = duration;
            _remainingTime.Value = _initialDuration;
            
            StartCountdown();
        }

        private void StartCountdown() {
            var startTime = _timeProvider.GetUtcNow();
            var pausedTime = _remainingTime.Value;

            _timerSubscription = Observable.EveryUpdate()
                                           .Subscribe(_ =>
                                           {
                                               var elapsed = _timeProvider.GetUtcNow() - startTime;
                                               _remainingTime.Value = pausedTime - elapsed;

                                               if (_remainingTime.Value > TimeSpan.Zero) return;
                                               _remainingTime.Value = TimeSpan.Zero;
                                               _timerSubscription?.Dispose();
                                               _state.Value = TimerState.Idle;
                                           }).AddTo(_disposable);
        }
        
        public void Pause() {
            _state.Value = TimerState.Paused;
            _timerSubscription?.Dispose();
        }

        public void Resume() {
            _state.Value = TimerState.Running;
            StartCountdown();
        }

        public void Stop() {
            _timerSubscription?.Dispose();
            _state.Value = TimerState.Idle;
            _remainingTime.Value = TimeSpan.Zero;
        }

        public void Reset() {
            _remainingTime.Value = _initialDuration;
            if (_state.Value == TimerState.Paused)
            {
                _state.Value = TimerState.Idle;
            }
        }

        public void Dispose() {
            _disposable.Dispose();
            _timerSubscription?.Dispose();
        }
    }
}