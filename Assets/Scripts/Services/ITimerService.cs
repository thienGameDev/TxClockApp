using System;
using UniRx;

namespace Services {
    public interface ITimerService : IDisposable {
        ReactiveProperty<TimerState> State { get; }
        ReactiveProperty<TimeSpan> RemainingTime { get; }
        TimeSpan GetInitialDuration();
        void Start(TimeSpan duration);
        void Pause();
        void Resume();
        void Stop();
        void Reset();
    }
    public enum TimerState { Idle, Running, Paused }
}