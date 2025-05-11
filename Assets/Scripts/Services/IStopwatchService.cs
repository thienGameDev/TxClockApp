using System;
using UniRx;

namespace Services {
    public interface IStopwatchService : IDisposable {
        ReactiveProperty<StopwatchState> State { get; }
        ReactiveProperty<TimeSpan> ElapsedTime { get; }
        int LapCount { get; }
        void Start();
        void Stop();
        void Reset();
        (int, TimeSpan) RecordLap();
    }
    
    public enum StopwatchState { Stopped, Running, Paused }
}