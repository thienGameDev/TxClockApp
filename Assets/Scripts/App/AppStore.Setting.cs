using System;

namespace Core.Framework
{
    public partial class AppStore
    {
        [Serializable]
        public struct Setting
        {
            public string ChannelPrefix;
            public float ValidAudioDistance;
        }

        [Serializable]
        public struct Atlas
        {
            public string[] Atlases;
        }
    }
}
