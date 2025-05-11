using System.Collections;
using NUnit.Framework;
using Zenject;
using Services;
using System;
using UnityEngine;
using UnityEngine.TestTools;

public class LocalClockServicePlayModeTests : ZenjectIntegrationTestFixture {
    
    [Inject] private ILocalClockService _localClockService;
    
    [SetUp]
    public void CommonInstall() {
        PreInstall();
        Container.Bind<ITimeProvider>().To<RealtimeProvider>().AsSingle();
        Container.Bind<ILocalClockService>().To<LocalClockService>().AsSingle();
        PostInstall();
    }
    
    [TearDown]
    public void Teardown()
    {
        _localClockService.Dispose();
    }
    
    [UnityTest]
    public IEnumerator LocalTime_UpdatesEverySecond() {
        
        _localClockService.SetTimeZone(TimeZoneInfo.Utc);

        var initialTime = _localClockService.LocalTime.Value;
        yield return new WaitForSecondsRealtime(1.5f);
        Assert.Greater(_localClockService.LocalTime.Value.Ticks, initialTime.Ticks);
    }

    [UnityTest]
    public IEnumerator TimeZoneChange_ReflectsInLocalTime() {
        
        var zone1 = TimeZoneInfo.Utc;
        var zone2 = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

        _localClockService.SetTimeZone(zone1);
        yield return new WaitForSecondsRealtime(1f);
        var time1 = _localClockService.LocalTime.Value;

        _localClockService.SetTimeZone(zone2);
        yield return new WaitForSecondsRealtime(1f);
        var time2 = _localClockService.LocalTime.Value;

        Assert.AreNotEqual(time1, time2);
    }
}