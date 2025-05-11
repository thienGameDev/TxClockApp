using NUnit.Framework;
using Zenject;
using Services;
using System;

public class TimerServiceEditModeTests : ZenjectUnitTestFixture
{
    [Inject] private ITimerService _timer;
    [Inject] private MockTimeProvider _timeProvider;

    [SetUp]
    public void CommonInstall()
    {
        TestInstaller.Install(Container);
        Container.Bind<ITimerService>().To<TimerService>().AsSingle();
        Container.Inject(this);
    }

    [Test]
    public void Start_NewTimer_SetsRunningStateAndInitialTime()
    {
        // Arrange
        var duration = TimeSpan.FromSeconds(10);
        
        // Act
        _timer.Start(duration);
        
        // Assert
        Assert.AreEqual(TimerState.Running, _timer.State.Value);
        Assert.AreEqual(duration, _timer.RemainingTime.Value);
    }

    [Test]
    public void Start_WhileAlreadyRunning_RestartsWithNewDuration()
    {
        // Arrange
        _timer.Start(TimeSpan.FromSeconds(5));
        
        // Act
        _timer.Start(TimeSpan.FromSeconds(10));
        
        // Assert
        Assert.AreEqual(TimeSpan.FromSeconds(10), _timer.RemainingTime.Value);
    }

    [Test]
    public void Pause_WhileRunning_SetsPausedState()
    {
        // Arrange
        _timer.Start(TimeSpan.FromSeconds(5));
        
        // Act
        _timer.Pause();
        
        // Assert
        Assert.AreEqual(TimerState.Paused, _timer.State.Value);
    }

    [Test]
    public void Resume_WhilePaused_SetsRunningState()
    {
        // Arrange
        _timer.Start(TimeSpan.FromSeconds(5));
        _timer.Pause();
        
        // Act
        _timer.Resume();
        
        // Assert
        Assert.AreEqual(TimerState.Running, _timer.State.Value);
    }

    [Test]
    public void Stop_WhileRunning_ResetsToIdleStateAndZeroTime()
    {
        // Arrange
        _timer.Start(TimeSpan.FromSeconds(5));
        
        // Act
        _timer.Stop();
        
        // Assert
        Assert.AreEqual(TimerState.Idle, _timer.State.Value);
        Assert.AreEqual(TimeSpan.Zero, _timer.RemainingTime.Value);
    }

    [Test]
    public void Reset_WhilePaused_SetsIdleStateAndInitialTime()
    {
        // Arrange
        var duration = TimeSpan.FromSeconds(10);
        _timer.Start(duration);
        _timer.Pause();
        
        // Act
        _timer.Reset();
        
        // Assert
        Assert.AreEqual(TimerState.Idle, _timer.State.Value);
        Assert.AreEqual(duration, _timer.RemainingTime.Value);
    }

    [Test]
    public void Dispose_DoesNotThrowErrors()
    {
        // Arrange
        _timer.Start(TimeSpan.FromSeconds(5));
        
        // Act & Assert
        Assert.DoesNotThrow(() => _timer.Dispose());
    }
}