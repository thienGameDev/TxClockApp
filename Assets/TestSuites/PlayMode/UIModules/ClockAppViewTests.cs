using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using Zenject;
using UnityEngine;
using TMPro;
using Services;
using Core.Framework;
using UIModules;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class ClockAppViewTests : ZenjectIntegrationTestFixture
{
    private ClockAppView _clockAppView;
    private EventSystem _eventSystem;
    private UIHelper _uiHelper;
    
    [Inject] private MockLocalClockService _mockLocalClockService;
    [Inject] private IStopwatchService _mockStopwatchService;
    [Inject] private ITimerService _mockTimerService;
    
    
    [SetUp]
    public void CommonInstall()
    {
        // Initialize Unity UI essentials
        _uiHelper = new UIHelper();
        _eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
        _eventSystem.gameObject.AddComponent<StandaloneInputModule>();

        // Create test services
        PreInstall();
        Container.Bind<MockLocalClockService>().AsSingle();
        Container.Bind<ILocalClockService>().To<MockLocalClockService>().FromResolve();
        Container.Bind<IStopwatchService>().To<MockStopwatchService>().AsSingle();
        Container.Bind<ITimerService>().To<MockTimerService>().AsSingle();
        Container.Bind<ClockApp>().AsSingle();
        PostInstall();

        // Create view with dependencies
        _clockAppView = new GameObject("ClockAppView").AddComponent<ClockAppView>();
        CreateUIComponents();
        _clockAppView.Module = Container.Resolve<ClockApp>();
        _clockAppView.Module.Model = CreateTestModel();
        _clockAppView.Show();
        _clockAppView.OnReady();
    }

    private ClockAppModel CreateTestModel()
    {
        return new ClockAppModel(
            new List<TimeZoneInfo> { TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time") },
            new List<string> { "Eastern Standard Time" }
        );
    }

    private void CreateUIComponents()
    {
        // Create tab toggles
        _clockAppView.localClockToggle = _uiHelper.CreateToggle("LocalClockToggle", true);
        _clockAppView.stopwatchToggle = _uiHelper.CreateToggle("StopwatchToggle");
        _clockAppView.timerToggle = _uiHelper.CreateToggle("TimerToggle");

        // Create local clock UI
        _clockAppView.localClockView = _uiHelper.CreateTransform("LocalClockView");
        _clockAppView.timeZoneDropdown = _uiHelper.CreateDropdown("TimeZoneDropdown");
        _clockAppView.localTimeText = _uiHelper.CreateText("LocalTimeText");
        
        // Create stopwatch UI
        _clockAppView.stopwatchView = _uiHelper.CreateTransform("StopwatchView");
        _clockAppView.timeDisplay = _uiHelper.CreateText("TimeDisplay");
        _clockAppView.startStopButton = _uiHelper.CreateButton("StartStopButton");
        _clockAppView.lapResetButton = _uiHelper.CreateButton("LapResetButton");
        _clockAppView.lapsContent = _uiHelper.CreateContentWithDefaultItem("LapContent");
        
        // Create timer UI
        _clockAppView.timerView = _uiHelper.CreateTransform("TimerView");
        _clockAppView.hoursInput = _uiHelper.CreateInputField("HoursInput");
        _clockAppView.minutesInput = _uiHelper.CreateInputField("MinutesInput");
        _clockAppView.secondsInput = _uiHelper.CreateInputField("SecondsInput");
        _clockAppView.timerDisplayText = _uiHelper.CreateText("TimerDisplayText");
        _clockAppView.circleFilledImage = _uiHelper.CreateImage("CircleFilledImage");
        _clockAppView.startPauseButton = _uiHelper.CreateButton("StartPauseButton");
        _clockAppView.resetButton = _uiHelper.CreateButton("ResetButton");
    }
    
    [TearDown]
    public void Teardown()
    {
        Object.Destroy(_eventSystem.gameObject);
    }

    [UnityTest]
    public IEnumerator LocalClock_UpdatesTimeDisplay()
    {
        // Arrange
        _clockAppView.localClockToggle.isOn = true;
        yield return null;

        // Act
        var testTime = new DateTime(2025, 1, 1, 12, 30, 45);
        _mockLocalClockService.LocalTime.Value = testTime;
        yield return null;

        // Assert
        Assert.AreEqual(
            "12:30:45",
            _clockAppView.localTimeText.text,
            "Time display should match service update"
        );
    }
    
    [UnityTest]
    public IEnumerator LocalClock_TimeZoneDropdown_Selection_UpdatesService()
    {
        // Arrange
        const int testIndex = 0;
        
        // Initialize Local Clock view
        _clockAppView.localClockToggle.isOn = true;
        yield return null;

        // Act - Simulate dropdown selection
        _clockAppView.timeZoneDropdown.value = testIndex;
        _clockAppView.timeZoneDropdown.onValueChanged.Invoke(testIndex);
        yield return null; // Allow service to update

        // Assert
        var expectedTimeZone = _clockAppView.Module.Model.TimeZones[testIndex];
        Assert.AreEqual(
            expectedTimeZone,
            _mockLocalClockService.CurrentTimeZone,
            "Selected timezone should be updated in service"
        );
    }
    
    [UnityTest]
    public IEnumerator Stopwatch_StartButton_TogglesServiceState()
    {
        // Arrange
        _clockAppView.stopwatchToggle.isOn = true;
        yield return null;

        // Act & Assert
        _clockAppView.startStopButton.onClick.Invoke();
        yield return null;
        Assert.AreEqual(StopwatchState.Running, _mockStopwatchService.State.Value);

        _clockAppView.startStopButton.onClick.Invoke();
        yield return null;
        Assert.AreEqual(StopwatchState.Paused, _mockStopwatchService.State.Value);
    }

    [UnityTest]
    public IEnumerator Stopwatch_RecordLap_AddsNewLapEntry_WithCorrectFormat()
    {
        // Arrange
        _clockAppView.stopwatchToggle.isOn = true;
        yield return null;
        
        // Start stopwatch
        _clockAppView.startStopButton.onClick.Invoke();
        yield return null;

        // Set test lap time
        _mockStopwatchService.RecordLap();

        // Act - Record lap
        _clockAppView.lapResetButton.onClick.Invoke();
        yield return null;

        // Assert
        Assert.AreEqual(2, _clockAppView.lapsContent.childCount, "Should have template + 1 lap entry");
    
        var lapEntry = _clockAppView.lapsContent.GetChild(1);
        var lapText = lapEntry.GetComponentInChildren<TMP_Text>();
        StringAssert.Contains("Lap 1 \t\t 00:00:01.00", lapText.text, "Lap time formatting mismatch");
    }
    
    [UnityTest]
    public IEnumerator Timer_ValidTimerInput_EnablesStartButton()
    {
        // Arrange
        _clockAppView.timerToggle.isOn = true;
        yield return null;

        // Act
        _clockAppView.hoursInput.text = "1";
        _clockAppView.minutesInput.text = "30";
        _clockAppView.secondsInput.text = "45";
        _clockAppView.hoursInput.onValueChanged.Invoke("1");
        yield return null;

        // Assert
        Assert.IsTrue(
            _clockAppView.startPauseButton.interactable,
            "Start button should be enabled with valid input"
        );
    }
    
    [UnityTest]
    public IEnumerator Timer_TabSelected_FocusesSecondsInput()
    {
        // Act
        _clockAppView.timerToggle.isOn = true;
        yield return null; // Allow UI to update

        // Assert
        Assert.AreEqual(
            _clockAppView.secondsInput.gameObject,
            EventSystem.current.currentSelectedGameObject,
            "Seconds input should be focused"
        );
    }
    
    [UnityTest]
    public IEnumerator Timer_Reset_ClearsState()
    {
        _clockAppView.timerToggle.isOn = true;
        yield return null;

        // Start timer first
        _clockAppView.secondsInput.text = "5";
        _clockAppView.startPauseButton.onClick.Invoke();
        yield return null;

        // Reset
        _clockAppView.resetButton.onClick.Invoke();
        yield return null;

        Assert.AreEqual(TimerState.Idle, _mockTimerService.State.Value,
            "Timer service should return to Idle state");
        Assert.AreEqual("00:00:05", _clockAppView.timerDisplayText.text,
            "Display should reset to initial time");
        Assert.AreEqual(1f, _clockAppView.circleFilledImage.fillAmount,
            "Progress circle should reset to full");
    }
    
    [UnityTest]
    public IEnumerator Timer_ProgressCircle_UpdatesCorrectly()
    {
        _clockAppView.timerToggle.isOn = true;
        yield return null;

        // Set 2 second duration
        _clockAppView.secondsInput.text = "2";
        _clockAppView.startPauseButton.onClick.Invoke();
        yield return null;

        // Check halfway point
        _mockTimerService.RemainingTime.Value = TimeSpan.FromSeconds(1);
        yield return null;
    
        Assert.AreEqual(0.5f, _clockAppView.circleFilledImage.fillAmount, 0.01f,
            "Progress circle should show 50% remaining");
    }
}