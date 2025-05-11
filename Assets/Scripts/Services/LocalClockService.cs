using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Services {
    public class LocalClockService : ILocalClockService {
        private readonly ITimeProvider _timeProvider;
        private readonly CompositeDisposable _disposables = new ();
        private readonly ReactiveProperty<TimeZoneInfo> _selectedTimeZone = new ();
        private List<TimeZoneInfo> _systemTimeZones;
        private ReactiveProperty<DateTime> _localTime = new ();
        public List<TimeZoneInfo> SystemTimeZones {
            get {
                if (_systemTimeZones != null) return _systemTimeZones;
                _systemTimeZones = TimeZoneInfo.GetSystemTimeZones()
                                               .OrderBy(tz => tz.BaseUtcOffset)
                                               .ToList();
                return _systemTimeZones;
            }
        }

        public ReactiveProperty<DateTime> LocalTime => _localTime;

        public LocalClockService(ITimeProvider timeProvider) {
            _timeProvider = timeProvider;
            SetClockUpdates();
        }
        
        public void SetTimeZone(TimeZoneInfo timeZone) {
            _selectedTimeZone.Value = timeZone;
        }

        public List<string> GetTimeZoneDisplayNames() {
            return SystemTimeZones.Select(tz => tz.DisplayName).ToList();
        }

        private void SetClockUpdates() {
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .Subscribe(_ => UpdateLocalTime())
                      .AddTo(_disposables);
        }

        public void UpdateLocalTime()
        {
            if (_selectedTimeZone.Value == null) return;
            var utcNow = _timeProvider.GetUtcNow();
            _localTime.Value = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _selectedTimeZone.Value);
        }
        
        public void Dispose() {
            _disposables.Dispose();
        }
    }
}