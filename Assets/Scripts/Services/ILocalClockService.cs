using System;
using System.Collections.Generic;
using UniRx;

namespace Services {
    public interface ILocalClockService : IDisposable {
        List<TimeZoneInfo> SystemTimeZones { get; }
        ReactiveProperty<DateTime> LocalTime { get; }
        void SetTimeZone(TimeZoneInfo timeZone);
        List<string> GetTimeZoneDisplayNames();
        void UpdateLocalTime();
    }
}