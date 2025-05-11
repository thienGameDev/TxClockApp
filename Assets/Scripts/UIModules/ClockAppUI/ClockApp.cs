using System;
using System.Collections.Generic;
using Services;
using Core.Framework;
using Zenject;

namespace UIModules {
    public class ClockApp : BaseModule<ClockAppView, ClockAppModel> {
        
        public class Installer : Installer<Installer> {
            public override void InstallBindings() {
                Container.Bind<ClockApp>().AsTransient();
            }
        }
        
        public readonly ILocalClockService LocalClockService;
        public readonly IStopwatchService StopwatchService;
        public readonly ITimerService TimerService;
        public ClockApp(ILocalClockService localClockService, 
                        IStopwatchService stopwatchService, 
                        ITimerService timerService) {
            LocalClockService = localClockService;
            StopwatchService = stopwatchService;
            TimerService = timerService;
        }
        
        protected override void OnViewReady() {
        }

        protected override void OnDisposed() {
        }
    }

    public class ClockAppModel : BaseModuleContextModel {
        public override string ViewId => "Assets/Bundles/Views/ClockApp/ClockAppUI.prefab";
        public override ModuleName ModuleName  => ModuleName.ClockApp;
        
        public List<TimeZoneInfo> TimeZones { get; }
        public List<string> TimeZoneDisplayNames { get; }

        public ClockAppModel(List<TimeZoneInfo> timeZones, List<string> timeZoneDisplayNames) {
            TimeZones = timeZones;
            TimeZoneDisplayNames = timeZoneDisplayNames;
        }
    }
}