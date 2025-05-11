using System;
using Services;

public class MockTimeProvider : ITimeProvider
{
    private DateTime _utcNow;
    public void SetUtcNow(DateTime time) => _utcNow = time;
    public DateTime GetUtcNow() => _utcNow;
}


