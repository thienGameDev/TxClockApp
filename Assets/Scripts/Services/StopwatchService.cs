using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Services {
    public class StopwatchService : IStopwatchService {
        private readonly CompositeDisposable _disposable = new ();
        private readonly List<TimeSpan> _lapTimes = new ();
        private readonly ITimeProvider _timeProvider; 
        private readonly ReactiveProperty<StopwatchState> _state = new (StopwatchState.Stopped);
        private readonly ReactiveProperty<TimeSpan> _elapsedTime = new (TimeSpan.Zero);
        private DateTime _startTime;
        private TimeSpan _accumulatedTime = TimeSpan.Zero;

        public StopwatchService(ITimeProvider timeProvider) {
            _timeProvider = timeProvider;
        }

        public ReactiveProperty<StopwatchState> State => _state;
        public ReactiveProperty<TimeSpan> ElapsedTime => _elapsedTime;
        public int LapCount => _lapTimes.Count;
        
        public void Start() {
            _state.Value = StopwatchState.Running;
            _startTime = _timeProvider.GetUtcNow() - _accumulatedTime;
            _accumulatedTime = _elapsedTime.Value;

            Observable.EveryUpdate()
                      .Subscribe(_ => UpdateElapsedTime()).AddTo(_disposable);
        }

        public void Stop() {
            UpdateElapsedTime();
            _state.Value = StopwatchState.Paused;
            _disposable.Clear();
            _accumulatedTime = _elapsedTime.Value;
        }

        public void Reset() {
            _state.Value = StopwatchState.Stopped;
            _elapsedTime.Value = TimeSpan.Zero;
            _accumulatedTime = TimeSpan.Zero;
            _lapTimes.Clear();
        }

        public (int, TimeSpan) RecordLap() {
            UpdateElapsedTime();
            
            var lapNumber = _lapTimes.Count + 1;
            var totalTime = _elapsedTime.Value;
            var previousTotal = _lapTimes.Aggregate(TimeSpan.Zero, (current, lap) => current + lap);
            var lapTime = totalTime - previousTotal;            
            _lapTimes.Add(lapTime);
            return (lapNumber, lapTime);
        }

        private void UpdateElapsedTime() {
            var currentElapsed = _timeProvider.GetUtcNow() - _startTime;
            _elapsedTime.Value = currentElapsed;
        }
        
        public void Dispose() {
            _disposable.Dispose();
        }
    }
}

