using NUnit.Framework;
using Zenject;
using Services;
using System;

[TestFixture]
public class StopwatchServiceEditModeTests : ZenjectUnitTestFixture {
    [Inject] private IStopwatchService _stopwatch;
    [Inject] private MockTimeProvider _mockTimeProvider;

    [SetUp]
    public void CommonInstall() {
        TestInstaller.Install(Container);
        Container.Bind<IStopwatchService>().To<StopwatchService>().AsSingle();
        Container.Inject(this);
    }

    [TearDown]
    public void TearDown() {
        _stopwatch.Dispose();
    }

    // Test 1: Initial state verification
    [Test]
    public void InitialState_IsStoppedWithZeroTime() {
        Assert.AreEqual(StopwatchState.Stopped, _stopwatch.State.Value);
        Assert.AreEqual(TimeSpan.Zero, _stopwatch.ElapsedTime.Value);
        Assert.AreEqual(0, _stopwatch.LapCount);
    }

    // Test 2: Start/Stop behavior
    [Test]
    public void StartStop_AccumulatesTimeCorrectly() {
        var startTime = new DateTime(2025, 1, 1, 12, 0, 0);
        _mockTimeProvider.SetUtcNow(startTime);
        
        _stopwatch.Start();
        _mockTimeProvider.SetUtcNow(startTime.AddSeconds(2));
        _stopwatch.Stop();

        Assert.AreEqual(TimeSpan.FromSeconds(2), _stopwatch.ElapsedTime.Value);
        Assert.AreEqual(StopwatchState.Paused, _stopwatch.State.Value);
    }

    // Test 3: Reset functionality
    [Test]
    public void Reset_ReturnsToInitialState() {
        _stopwatch.Start();
        _mockTimeProvider.SetUtcNow(DateTime.UtcNow.AddSeconds(1));
        _stopwatch.Reset();

        Assert.AreEqual(StopwatchState.Stopped, _stopwatch.State.Value);
        Assert.AreEqual(TimeSpan.Zero, _stopwatch.ElapsedTime.Value);
        Assert.AreEqual(0, _stopwatch.LapCount);
    }

    // Test 4: Lap recording
    [Test]
    public void RecordLap_CapturesCorrectTimings() {
        var startTime = new DateTime(2025, 1, 1, 12, 0, 0);
        _mockTimeProvider.SetUtcNow(startTime);
        _stopwatch.Start();

        // First lap at 3 seconds
        _mockTimeProvider.SetUtcNow(startTime.AddSeconds(3));
        var lap1 = _stopwatch.RecordLap(); // Lap 1: 3s

        // Second lap at 5 seconds
        _mockTimeProvider.SetUtcNow(startTime.AddSeconds(5));
        var lap2 = _stopwatch.RecordLap(); // Lap 2: 2s (5s total - 3s previous laps)

        Assert.AreEqual(3, lap1.Item2.TotalSeconds);
        Assert.AreEqual(2, lap2.Item2.TotalSeconds);
    }

    // Test 5: Multiple start/stop cycles
    [Test]
    public void MultipleStartStop_AccumulatesCorrectly() {
        var startTime = new DateTime(2025, 1, 1, 12, 0, 0);
        _mockTimeProvider.SetUtcNow(startTime);

        _stopwatch.Start();
        _mockTimeProvider.SetUtcNow(startTime.AddSeconds(2));
        _stopwatch.Stop();

        _stopwatch.Start();
        _mockTimeProvider.SetUtcNow(startTime.AddSeconds(5)); // +3 more seconds
        _stopwatch.Stop();

        Assert.AreEqual(TimeSpan.FromSeconds(5), _stopwatch.ElapsedTime.Value);
    }
}