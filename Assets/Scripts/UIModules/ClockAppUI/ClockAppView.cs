using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Services;
using TMPro;
using UIModules;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Framework {
    
    public enum ViewState {LocalClock, Stopwatch, Timer}
    public class ClockAppView : View<ClockApp> {
        [Header("Tab toggles")]
        [SerializeField] public Toggle localClockToggle;
        [SerializeField] public Toggle stopwatchToggle;
        [SerializeField] public Toggle timerToggle;
        
        [Header("Local Clock Subview")]
        [SerializeField] public Transform localClockView;
        [SerializeField] public TMP_Dropdown timeZoneDropdown;
        [SerializeField] public TMP_Text localTimeText;

        [Header("Stopwatch Subview")]
        [SerializeField] public Transform stopwatchView;
        [SerializeField] public TMP_Text timeDisplay;
        [SerializeField] public Button startStopButton;
        [SerializeField] public Button lapResetButton;
        [SerializeField] public Transform lapsContent;

        [Header("Timer Subview")]
        [SerializeField] public Transform timerView;
        [SerializeField] public TMP_InputField hoursInput;
        [SerializeField] public TMP_InputField minutesInput;
        [SerializeField] public TMP_InputField secondsInput;
        [SerializeField] public TMP_Text timerDisplayText;
        [SerializeField] public Image circleFilledImage;
        [SerializeField] public Button startPauseButton;
        [SerializeField] public Button resetButton;
        [SerializeField] public AudioClip timerCompleteSound;
        [SerializeField] public AudioSource audioSource;
        
        private readonly ReactiveProperty<ViewState> _viewState = new (ViewState.LocalClock);
        
        public override void OnReady() {
            SetupViewStateMachine();
            SetupClockView();
            SetupStopwatchView();
            SetupTimerView();
        }

        public override UniTask Show() {
            InitializeTabToggles();
            SetupDefaultView();
            return UniTask.CompletedTask;
        }

        private void SetupDefaultView() {
            _viewState.Value = ViewState.LocalClock;
            localClockToggle.Select();
        }

        private void InitializeTabToggles() {
            localClockToggle.OnValueChangedAsObservable().Subscribe(_ => {
                _viewState.Value = ViewState.LocalClock;
            }).AddTo(this);
            stopwatchToggle.OnValueChangedAsObservable().Subscribe(_ => {
                _viewState.Value = ViewState.Stopwatch;
            }).AddTo(this);
            timerToggle.OnValueChangedAsObservable().Subscribe(_ => {
                _viewState.Value = ViewState.Timer;
                EventSystem.current.SetSelectedGameObject(secondsInput.gameObject);
            }).AddTo(this);
        }
        
        private void SetupViewStateMachine() {
            _viewState.Subscribe(state => 
            {
                localClockView.gameObject.SetActive(state == ViewState.LocalClock);
                stopwatchView.gameObject.SetActive(state == ViewState.Stopwatch);
                timerView.gameObject.SetActive(state == ViewState.Timer);
            }).AddTo(this);
        }
        
        private GameObject GetFirstLapItem() {
            return lapsContent.GetChild(0).gameObject;
        }
        
    #region LocalClock
        private void SetupClockView() {
            InitializeTimeZoneDropdown();
            SetupTimeZoneSelection();
            InitializeLocalClock();
        }

        private void InitializeLocalClock() {
            Module.LocalClockService.LocalTime.Subscribe(UpdateLocalTimeDisplay).AddTo(this);
        }

        private void InitializeTimeZoneDropdown()
        {
            // Create dropdown options
            timeZoneDropdown.ClearOptions();
            timeZoneDropdown.AddOptions(Module.Model.TimeZoneDisplayNames);

            // Set initial value to local time zone
            var localTimeZoneIndex = Module.Model.TimeZones.FindIndex(tz => tz.Id == TimeZoneInfo.Local.Id);
            timeZoneDropdown.value = localTimeZoneIndex;
            Module.LocalClockService.SetTimeZone(TimeZoneInfo.Local);
        }
        
        private void SetupTimeZoneSelection()
        {
            // Observe dropdown changes
            timeZoneDropdown.onValueChanged.AsObservable()
                            .Subscribe(index =>
                            {
                                var timeZone = Module.Model.TimeZones.ElementAt(index);
                                Module.LocalClockService.SetTimeZone(timeZone);
                            })
                            .AddTo(this);
            
        }
        
        private void UpdateLocalTimeDisplay(DateTime localTime)
        {
            localTimeText.text = $"{localTime:HH:mm:ss}";
        }

    #endregion
        
    #region Stopwatch
        private void SetupStopwatchView() {
            SetupButtonInteractions();
            SetupStopwatchStateMachine();
        }
        
        private void SetupButtonInteractions()
        {
            // Start/Stop button
            startStopButton.OnClickAsObservable()
                           .Subscribe(_ => ToggleStartStop());
            // Lap/Reset button
            lapResetButton.OnClickAsObservable()
                          .Subscribe(_ => ToggleLapReset());
        }
        
        private void SetupStopwatchStateMachine() {
            Module.StopwatchService.State.Subscribe(StopwatchStateMachineSubscriber).AddTo(this);
            Module.StopwatchService.ElapsedTime.Subscribe(UpdateStopwatchDisplay).AddTo(this);
        }

        private void StopwatchStateMachineSubscriber(StopwatchState state) {
            startStopButton.GetComponentInChildren<TMP_Text>().text = 
                state == StopwatchState.Running ? "Stop" : "Start";

            lapResetButton.GetComponentInChildren<TMP_Text>().text = 
                state == StopwatchState.Running ? "Lap" : "Reset";
                
            lapResetButton.interactable = state != StopwatchState.Stopped;
                            
            GetFirstLapItem().SetActive(state != StopwatchState.Stopped);

            switch (state) {
                case StopwatchState.Running:
                    StartStopwatch();
                    break;
                case StopwatchState.Paused:
                    StopStopwatch();
                    break;
            }
        }
        
        private void StartStopwatch()
        {
            Module.StopwatchService.Start();
        }

        private void StopStopwatch()
        {
            Module.StopwatchService.Stop();
        }

        private void ToggleStartStop()
        {
            Module.StopwatchService.State.Value = 
                Module.StopwatchService.State.Value == StopwatchState.Running ? 
                StopwatchState.Paused : 
                StopwatchState.Running;
        }

        private void ToggleLapReset() {
            if (Module.StopwatchService.State.Value == StopwatchState.Running) RecordLap();
            else ResetStopwatch();
        }
        
        private void RecordLap() {
            var lapPrefab = lapsContent.GetChild(0);
            var lap = Instantiate(lapPrefab, lapsContent);
            var (lapNumber, lapTime) = Module.StopwatchService.RecordLap();
            var lapText = lap.GetComponentInChildren<TMP_Text>();
            lapText.text = FormatLapInfo(lapNumber,lapTime);
        }
        
        private void ResetStopwatch()
        {
            Module.StopwatchService.Reset();

            for (var i = 1; i < lapsContent.childCount; i++)
                Destroy(lapsContent.GetChild(i).gameObject);
            
            UpdateStopwatchDisplay(TimeSpan.Zero);
        }

        private void UpdateStopwatchDisplay(TimeSpan time)
        {
            timeDisplay.text = FormatTime(time);
            GetFirstLapItem().GetComponentInChildren<TMP_Text>().text = 
                FormatLapInfo(Module.StopwatchService.LapCount + 1, time);
        }
        
        private string FormatLapInfo(int lapNumber, TimeSpan time) {
            return $"Lap {lapNumber} \t\t {FormatTime(time)}";
        }
        
        private string FormatTime(TimeSpan time)
        {
            return $"{(int)time.TotalHours:00}:{time:mm\\:ss\\.ff}";
        }

    #endregion

    #region Timer
        private void SetupTimerView() {
            SetupInputValidation();
            SetupTimerButtonBindings();
            SetupTimerStateMachine();
        }

        private void SetupInputValidation()
    {
        hoursInput.onValueChanged.AsObservable()
            .Subscribe(_ => ValidateInputs())
            .AddTo(this);

        minutesInput.onValueChanged.AsObservable()
            .Subscribe(_ => ValidateInputs())
            .AddTo(this);

        secondsInput.onValueChanged.AsObservable()
            .Subscribe(_ => ValidateInputs())
            .AddTo(this);
    }

    private void SetupTimerButtonBindings()
    {
        startPauseButton.OnClickAsObservable()
            .Subscribe(_ => ToggleTimer()).AddTo(this);

        resetButton.OnClickAsObservable()
            .Subscribe(_ => ResetTimer()).AddTo(this);
    }

    private void SetupTimerStateMachine()
    {
        Module.TimerService.State.Subscribe(state =>
        {
            startPauseButton.GetComponentInChildren<TMP_Text>().text = 
                state == TimerState.Running ? "Pause" : "Start";
            
            resetButton.interactable = state != TimerState.Running;

            SetInputsInteractable(state == TimerState.Idle);

        }).AddTo(this);

        Module.TimerService.RemainingTime.Subscribe(UpdateTimerDisplay).AddTo(this);
    }

    private void UpdateTimerDisplay(TimeSpan time) {
        timerDisplayText.text = $"{time:hh\\:mm\\:ss}";
        circleFilledImage.fillAmount = (float) (time / Module.TimerService.GetInitialDuration());
        if (time <= TimeSpan.Zero && Module.TimerService.State.Value != TimerState.Idle)
        {
            CompleteTimer();
        }
    }
    
    private void ToggleTimer()
    {
        if (Module.TimerService.State.Value is TimerState.Idle)
        {
            if (!TryParseDuration(out var duration)) return;
            Module.TimerService.Start(duration);
        }
        else
        {
            Module.TimerService.State.Value = 
                Module.TimerService.State.Value is TimerState.Running 
                    ? TimerState.Paused 
                    : TimerState.Running;
            
            if (Module.TimerService.State.Value is TimerState.Running) Module.TimerService.Resume();
            else Module.TimerService.Pause();
        }
    }

    private void StopTimer()
    {
        Module.TimerService.Stop();
    }

    private void ResetTimer() {
        Module.TimerService.Reset();
    }

    private void CompleteTimer()
    {
        StopTimer();
        PlayCompletionSound();
    }

    private void PlayCompletionSound() {
        if (!audioSource || !timerCompleteSound) return;
        audioSource.PlayOneShot(timerCompleteSound);
        Debug.Log("Play Complete Sound");
    }

    private bool TryParseDuration(out TimeSpan duration)
    {
        duration = TimeSpan.Zero;
        
        try
        {
            var hours = string.IsNullOrEmpty(hoursInput.text) ? 0 : int.Parse(hoursInput.text);
            var minutes = string.IsNullOrEmpty(minutesInput.text) ? 0 : int.Parse(minutesInput.text);
            var seconds = string.IsNullOrEmpty(secondsInput.text) ? 0 : int.Parse(secondsInput.text);
            
            duration = new TimeSpan(hours, minutes, seconds);
            return duration > TimeSpan.Zero;
        }
        catch
        {
            return false;
        }
    }

    private void ValidateInputs()
    {
        var valid = TryParseDuration(out TimeSpan time);
        startPauseButton.interactable = valid && Module.TimerService.State.Value == TimerState.Idle;
        timerDisplayText.text = $"{(valid ? time : TimeSpan.Zero):hh\\:mm\\:ss}";
    }

    private void SetInputsInteractable(bool interactable)
    {
        hoursInput.interactable = interactable;
        minutesInput.interactable = interactable;
        secondsInput.interactable = interactable;
    }

    #endregion
    }
}