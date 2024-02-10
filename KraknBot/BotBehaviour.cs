using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using KraknBot.States;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using IDisposable = Il2CppSystem.IDisposable;

namespace KraknBot;

public class BotBehaviour : MonoBehaviour
{
    private readonly BotLogic _botLogic = new();
    private IDisposable _lazyUpdateSubscription;

    public static BotLogic Instance { get; private set; }

    void Awake()
    {
        Instance = _botLogic;
        StartCoroutine(CheckVersionCoroutine().WrapToIl2Cpp());
    }

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

    private IEnumerator CheckVersionCoroutine()
    {
        string versionCheckUrl = "https://abcdh.dev/version";
        UnityWebRequest webRequest = UnityWebRequest.Get(versionCheckUrl);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Log.Info("Error checking version: " + webRequest.error);
        }
        else
        {
            string latestVersion = webRequest.downloadHandler.text;
            if (latestVersion != Metadata.Version)
            {
                Log.Info($"An update is available for the plugin. Current version: {Metadata.Version}, Latest version: {latestVersion}");
            }
            else
            {
                Log.Info("Plugin is up to date.");
            }
        }
    }
}