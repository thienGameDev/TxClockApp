using System;
using System.Linq;
using Services;
using NUnit.Framework;
using UniRx;
using Zenject;

public class LocalClockServiceEditModeTests: ZenjectUnitTestFixture
{
    [Inject] private ILocalClockService _service;
    [Inject] private MockTimeProvider _mockTimeProvider;
    
    [SetUp]
    public void CommonInstall() {
        TestInstaller.Install(Container);
        Container.Bind<ILocalClockService>().To<LocalClockService>().AsSingle();
        Container.Inject(this);
    }

    [TearDown]
    public void TearDown() {
        _service.Dispose();
    }

    // Test 1: Verify SystemTimeZones initialization and ordering
    [Test]
    public void SystemTimeZones_InitializedAndOrderedByOffset() {
        var timeZones = _service.SystemTimeZones;
        Assert.IsNotNull(timeZones);
        Assert.IsTrue(timeZones.Count > 0);
        CollectionAssert.AreEqual(
            timeZones.OrderBy(tz => tz.BaseUtcOffset).ToList(),
            timeZones
        );
    }

    // Test 2: TimeZone setting works correctly
    [Test]
    public void SetTimeZone_UpdatesSelectedTimeZone() {
        var testZone = TimeZoneInfo.Utc;
        _service.SetTimeZone(testZone);
        _service.UpdateLocalTime();
        Assert.IsNotNull(_service.LocalTime.Value);
    }

    // Test 3: Time conversion with mocked time
    [Test]
    public void UpdateLocalTime_ConvertsUtcToSelectedZone() {
        var testZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        _service.SetTimeZone(testZone);
        
        var mockUtcTime = new DateTime(2025, 1, 1, 12, 0, 0);
        _mockTimeProvider.SetUtcNow(mockUtcTime);
        
        _service.UpdateLocalTime();
        var expected = TimeZoneInfo.ConvertTimeFromUtc(mockUtcTime, testZone);
        Assert.AreEqual(expected, _service.LocalTime.Value);
    }

    // Test 4: Verify display names
    [Test]
    public void GetTimeZoneDisplayNames_ReturnsValidNames() {
        var displayNames = _service.GetTimeZoneDisplayNames();
        Assert.AreEqual(_service.SystemTimeZones.Count, displayNames.Count);
        CollectionAssert.AreEqual(
            _service.SystemTimeZones.Select(tz => tz.DisplayName),
            displayNames
        );
    }

    // Test 5: Verify disposal
    [Test]
    public void Dispose_ClearsSubscriptions() {
        _service.Dispose();
        var disposables = typeof(LocalClockService)
            .GetField("_disposables", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_service) as CompositeDisposable;
        Assert.IsTrue(disposables.IsDisposed);
    }
}
