using System;
using Services;
using UniRx;

public class MockTimerService : ITimerService {
    private TimeSpan _initialDuration;
    public ReactiveProperty<TimerState> State { get; } = new();
    public ReactiveProperty<TimeSpan> RemainingTime { get; } = new();
    public TimeSpan GetInitialDuration() => _initialDuration;
    public void Start(TimeSpan duration) {
        _initialDuration = duration;
        State.Value = TimerState.Running;
    }

    public void Pause() => State.Value = TimerState.Paused;
    public void Resume() => State.Value = TimerState.Running;
    public void Stop() => State.Value = TimerState.Idle;
    public void Reset() {
        RemainingTime.Value = _initialDuration;
        State.Value = TimerState.Idle;
    }

    public void Dispose() {}
}