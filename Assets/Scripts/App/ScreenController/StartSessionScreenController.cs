using Services;
using UIModules;

namespace Core.Framework
{
    public class StartSessionScreenController : IScreenController
    {
        public ScreenName Name => ScreenName.SessionStart;
        private readonly ILocalClockService _localClockService;
        public bool IsAllowChangeScreen(ScreenName newScreen)
        {
            return newScreen != ScreenName.Restart;
        }
        
        public StartSessionScreenController(ILocalClockService localClockService) {
            _localClockService = localClockService;
        }

        public void Enter()
        {
            AppStore.CreateModule(new ClockAppModel(
                _localClockService.SystemTimeZones,
                _localClockService.GetTimeZoneDisplayNames()
                ));
        }

        public void Out()
        {
            return;
        }
    }
}
