using System;

namespace Services {
    public class RealtimeProvider : ITimeProvider {
        public DateTime GetUtcNow() => DateTime.UtcNow;
    }
}
