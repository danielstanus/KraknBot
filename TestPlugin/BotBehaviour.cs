using UnityEngine;
using UniRx;
using System;
using TestPlugin;
using TestPlugin.States;
using IDisposable = Il2CppSystem.IDisposable;

public class BotBehaviour : MonoBehaviour
{
    private readonly BotLogic _botLogic = new BotLogic();
    private IDisposable _lazyUpdateSubscription;

    public void StartCollector()
    {
        _botLogic.Start();
        _lazyUpdateSubscription = Observable.Interval(Il2CppSystem.TimeSpan.FromSeconds(1))
            .Subscribe(new Action<long>(_ =>
            {
                LazyUpdate();
            })).AddTo(this);
    }

    public void StopCollector()
    {
        _botLogic.Stop();
        _lazyUpdateSubscription?.Dispose(); // Dispose the subscription to stop LazyUpdate calls
    }

    private void LazyUpdate()
    {
        _botLogic.Update();
    }

    private void Update()
    {
        // Implement update logic here
    }
}
