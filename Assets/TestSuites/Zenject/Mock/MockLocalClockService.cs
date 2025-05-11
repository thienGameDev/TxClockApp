using System;
using System.Collections.Generic;
using Services;
using UniRx;

public class MockLocalClockService : ILocalClockService {
    public TimeZoneInfo CurrentTimeZone; 
    public ReactiveProperty<DateTime> LocalTime { get; } = new();
    private List<string> _displayNames = new();
    public List<TimeZoneInfo> SystemTimeZones { get; } = new ();

    public void SetTimeZone(TimeZoneInfo timeZone) {
        CurrentTimeZone = timeZone;
    }
    public List<string> GetTimeZoneDisplayNames() => _displayNames;
    public void UpdateLocalTime() {}

    public void Dispose() {}
}