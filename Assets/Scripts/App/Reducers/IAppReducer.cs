using System;

namespace Core.Framework
{
    public interface IAppReducer<T>
    {
        GameReducerInfo<T>[] RegisHandler();
    }

    public struct GameReducerInfo<T>
    {
        public AppAction Action;
        public Action<T> Handler;
    }
}
