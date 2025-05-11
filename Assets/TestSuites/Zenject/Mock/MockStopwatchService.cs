using System;
using Services;
using UniRx;

public class MockStopwatchService : IStopwatchService {
    public ReactiveProperty<StopwatchState> State { get; } = new();
    public ReactiveProperty<TimeSpan> ElapsedTime { get; } = new();
    public int LapCount => 0;
    public void Start() => State.Value = StopwatchState.Running;
    public void Stop() => State.Value = StopwatchState.Paused;
    public void Reset() {}
    public (int, TimeSpan) RecordLap() => (1, TimeSpan.FromSeconds(1));
    public void Dispose() {}
}