using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Zenject;
using Services;
using System;
using UnityEngine;

public class StopwatchServicePlayModeTests : ZenjectIntegrationTestFixture {
    
    [Inject] private IStopwatchService _stopwatchService;
    
    [SetUp]
    public void CommonInstall() {
        PreInstall();
        Container.Bind<ITimeProvider>().To<RealtimeProvider>().AsSingle();
        Container.Bind<IStopwatchService>().To<StopwatchService>().AsSingle();
        PostInstall();
    }

    [TearDown]
    public void Teardown() {
        _stopwatchService.Dispose();
    }
    
    [UnityTest]
    public IEnumerator Start_IncreasesElapsedTimeOverTime() {
        // Act
        _stopwatchService.Start();
        yield return new WaitForSecondsRealtime(1.5f);

        // Assert
        Assert.Greater(_stopwatchService.ElapsedTime.Value.TotalSeconds, 1.0f);
    }

    [UnityTest]
    public IEnumerator Stop_PausesTimeUpdates() {
        // Act
        _stopwatchService.Start();
        yield return new WaitForSecondsRealtime(1.0f);
        var timeBeforeStop = _stopwatchService.ElapsedTime.Value;
        
        _stopwatchService.Stop();
        yield return new WaitForSecondsRealtime(1.0f);

        // Allow a small tolerance
        Assert.That(
            _stopwatchService.ElapsedTime.Value.TotalSeconds,
            Is.EqualTo(timeBeforeStop.TotalSeconds).Within(0.01f)
        );
    }

    [UnityTest]
    public IEnumerator Reset_ClearsAllValues() {
        // Act
        _stopwatchService.Start();
        yield return new WaitForSecondsRealtime(0.5f);
        _stopwatchService.RecordLap();
        _stopwatchService.Reset();

        // Assert
        Assert.AreEqual(StopwatchState.Stopped, _stopwatchService.State.Value);
        Assert.AreEqual(0, _stopwatchService.LapCount);
        Assert.AreEqual(TimeSpan.Zero, _stopwatchService.ElapsedTime.Value);
    }

    [UnityTest]
    public IEnumerator RecordLap_WhileRunning_StoresCorrectTime() {
        // Act & Assert
        _stopwatchService.Start();
        yield return new WaitForSecondsRealtime(1.0f);
        var lap1 = _stopwatchService.RecordLap();
        
        yield return new WaitForSecondsRealtime(1.0f);
        var lap2 = _stopwatchService.RecordLap();

        Assert.AreEqual(2, _stopwatchService.LapCount);
        Assert.Greater(lap2.Item2.TotalSeconds, 0.9f); // ~1 second
    }
}