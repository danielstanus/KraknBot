using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using KraknBot.Helpers;
using KraknBot.States;
using Seafight;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using IDisposable = Il2CppSystem.IDisposable;

namespace KraknBot;

public class BotBehaviour : MonoBehaviour
{
    private readonly BotLogic _botLogic = new();
    private IDisposable _lazyUpdateSubscription;
    private IDisposable _tickSubscription;

    public static BotLogic Instance { get; private set; }
    public static bool isUpToDate = false;

    void Awake()
    {
        Instance = _botLogic;
        StartCoroutine(CheckVersionCoroutine().WrapToIl2Cpp());

        // Start the tick subscription when the GameObject is created
        Log.Info("Calling StartTick() from Awake()...");
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += new Action<Scene, LoadSceneMode>((scene, mode) =>
        {
            Log.Info("Scene loaded: " + scene.name);
            StartTick();
        });
    }

    private void StartTick()
    {
        // Define the interval for updating the ammunition list, e.g., every 5 seconds
        var updateInterval = Il2CppSystem.TimeSpan.FromSeconds(5);

        // Dispose of any existing subscription before creating a new one
        _tickSubscription?.Dispose();

        // Create a new subscription that updates the ammunition list every `updateInterval`
        _tickSubscription = Observable.Interval(updateInterval)
            .Subscribe(new Action<long>(_ =>
            {
                Log.Info("Subscribed to UpdateAmmunitionList...");

                // Ensure UpdateAmmunitionList is called on the main thread
                DearImGuiInjection.BepInEx.UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        InventorySystem inventorySystem = MainInstaller.Inject<InventorySystem>();
                        if (inventorySystem != null)
                        {
                            GameContext.UpdateAmmunitionList(inventorySystem);
                        }
                    }
                );
            })).AddTo(this);
    }

    public void StartCollector()
    {
        _botLogic.Start();
        _lazyUpdateSubscription = Observable.Interval(Il2CppSystem.TimeSpan.FromSeconds(1))
            .Subscribe(new Action<long>(_ => { LazyUpdate(); })).AddTo(this);
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
                Log.Info(
                    $"An update is available for the plugin. Current version: {Metadata.Version}, Latest version: {latestVersion}");
            }
            else
            {
                isUpToDate = true;
                Log.Info("Plugin is up to date.");
            }
        }
    }

    void OnDestroy()
    {
        // Dispose of the subscription when the GameObject is destroyed to prevent memory leaks
        _tickSubscription?.Dispose();
    }
}