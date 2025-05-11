
namespace Core.Framework
{
    public interface IScreenController
    {
        ScreenName Name { get; }
        bool IsAllowChangeScreen(ScreenName newScreen);
        void Enter();

        void Out();
    }

    public enum ScreenName
    {
        SessionStart = 0,
        Restart
    }
}
