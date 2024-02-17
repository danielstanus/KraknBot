using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MonoMod.RuntimeDetour;
using KraknBot.Helpers;
using KraknBot.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using ImGuiInjection = DearImGuiInjection.DearImGuiInjection;

namespace KraknBot;

[BepInDependency(DearImGuiInjection.Metadata.GUID)]
[BepInPlugin(Metadata.GUID, Metadata.Name, Metadata.Version)]
internal unsafe class KraknBot : BasePlugin
{
    private static List<Hook> Hooks = new();
    private GameObject KraknBotBehaviourHolder;
    private BotBehaviour _botBehaviourInstance;

    public static bool Debug = true;

    public override void Load()
    {
        InitLog.Init(Log);

        Harmony.CreateAndPatchAll(typeof(HarmonyPatches));

        ImGuiInjection.Render += BotUI;

        ClassInjector.RegisterTypeInIl2Cpp<BotBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<CoroutineHandler>();
        var coroutineHandler = CoroutineHandler.Instance;

        KraknBotBehaviourHolder = new("KraknBotBehaviourGO");
        GameObject.DontDestroyOnLoad(KraknBotBehaviourHolder);
        KraknBotBehaviourHolder.hideFlags |= HideFlags.HideAndDontSave;
        _botBehaviourInstance = KraknBotBehaviourHolder.AddComponent<BotBehaviour>();
        if (_botBehaviourInstance == null)
        {
            Log.LogInfo("Failed to add BotBehaviour component.");
            LogWindow.AddLogMessage("Failed to add BotBehaviour component.");
        }
        else
        {
            PluginUI.BotBehaviourInstance = _botBehaviourInstance;
            Log.LogInfo("BotBehaviour instance set in PluginUI.");
        }

        LogWindow.AddLogMessage("Welcome to Krakn Bot v" + Metadata.Version);
    }

    private static void BotUI()
    {
        PluginUI.RenderUI();
        LogWindow.Render();
    }
}