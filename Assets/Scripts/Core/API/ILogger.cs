
namespace Core.Infrastructure
{
    public interface ILogger
    {
        void Log(object mess, params object[] args);
        void Warning(object mess, params object[] args);
        void Error(object mess, params object[] args);
    }
}
