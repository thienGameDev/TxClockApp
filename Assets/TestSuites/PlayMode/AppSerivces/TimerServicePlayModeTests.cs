using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Zenject;
using Services;
using System;
using UnityEngine;

public class TimerServicePlayModeTests : ZenjectIntegrationTestFixture {
    [Inject] private ITimerService _timerService;
    
    [SetUp]
    public void CommonInstall() {
        PreInstall();
        Container.Bind<ITimeProvider>().To<RealtimeProvider>().AsSingle();
        Container.Bind<ITimerService>().To<TimerService>().AsSingle();
        PostInstall();
    }

    [TearDown]
    public void Teardown() {
        _timerService.Dispose();
    }
    
    [UnityTest]
    public IEnumerator Start_DecreasesRemainingTimeOverTime() {
        // Arrange
        var initialDuration = TimeSpan.FromSeconds(3);

        // Act
        _timerService.Start(initialDuration);
        yield return new WaitForSecondsRealtime(1.5f);

        // Assert
        Assert.Less(_timerService.RemainingTime.Value.TotalSeconds, initialDuration.TotalSeconds);
        Assert.Greater(_timerService.RemainingTime.Value.TotalSeconds, 1.0f); // ~1.5s elapsed
    }

    [UnityTest]
    public IEnumerator Pause_StopsTimeDecrement() {
        // Arrange
        _timerService.Start(TimeSpan.FromSeconds(3));

        // Act
        yield return new WaitForSecondsRealtime(1.0f);
        var timeBeforePause = _timerService.RemainingTime.Value;
        _timerService.Pause();
        yield return new WaitForSecondsRealtime(1.0f);

        // Assert
        Assert.AreEqual(TimerState.Paused, _timerService.State.Value);
        Assert.AreEqual(timeBeforePause, _timerService.RemainingTime.Value);
    }

    [UnityTest]
    public IEnumerator Resume_ContinuesTimeDecrement() {
        // Arrange
        _timerService.Start(TimeSpan.FromSeconds(3));
        _timerService.Pause();
        var pausedTime = _timerService.RemainingTime.Value;

        // Act
        _timerService.Resume();
        yield return new WaitForSecondsRealtime(1.0f);

        // Assert
        Assert.Less(_timerService.RemainingTime.Value.TotalSeconds, pausedTime.TotalSeconds);
    }

    [UnityTest]
    public IEnumerator Stop_ResetsRemainingTimeToZero() {
        // Arrange
        _timerService.Start(TimeSpan.FromSeconds(3));

        // Act
        yield return new WaitForSecondsRealtime(1.0f);
        _timerService.Stop();

        // Assert
        Assert.AreEqual(TimeSpan.Zero, _timerService.RemainingTime.Value);
        Assert.AreEqual(TimerState.Idle, _timerService.State.Value);
    }

    [UnityTest]
    public IEnumerator Reset_ReloadsInitialDuration() {
        // Arrange
        var initialDuration = TimeSpan.FromSeconds(3);
        _timerService.Start(initialDuration);
        _timerService.Pause();

        // Act
        _timerService.Reset();
        yield return null; // Allow one frame for state update

        // Assert
        Assert.AreEqual(initialDuration, _timerService.RemainingTime.Value);
        Assert.AreEqual(TimerState.Idle, _timerService.State.Value);
    }

    [UnityTest]
    public IEnumerator Timer_CompletesAndSetsStateToIdle() {
        // Arrange
        _timerService.Start(TimeSpan.FromSeconds(1));

        // Act
        yield return new WaitForSecondsRealtime(1.5f);

        // Assert
        Assert.AreEqual(TimeSpan.Zero, _timerService.RemainingTime.Value);
        Assert.AreEqual(TimerState.Idle, _timerService.State.Value);
    }
}