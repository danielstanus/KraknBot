using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MonoMod.RuntimeDetour;
using TestPlugin.Helpers;
using TestPlugin.UI;
using UnityEngine;
using ImGuiInjection = DearImGuiInjection.DearImGuiInjection;

namespace TestPlugin;

[BepInDependency(DearImGuiInjection.Metadata.GUID)]
[BepInPlugin(Metadata.GUID, Metadata.Name, Metadata.Version)]
internal unsafe class TestPlugin : BasePlugin
{
    private static List<Hook> Hooks = new();
    private GameObject TestPluginBehaviourHolder;
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

        TestPluginBehaviourHolder = new("TestPluginBehaviourGO");
        GameObject.DontDestroyOnLoad(TestPluginBehaviourHolder);
        TestPluginBehaviourHolder.hideFlags |= HideFlags.HideAndDontSave;
        _botBehaviourInstance = TestPluginBehaviourHolder.AddComponent<BotBehaviour>();
        if (_botBehaviourInstance == null)
        {
            Log.LogInfo("Failed to add BotBehaviour component.");
            LogWindow.AddLogMessage("Failed to add BotBehaviour component.");
        }
        else
        {
            PluginUI.BotBehaviourInstance = _botBehaviourInstance;
            Log.LogInfo("BotBehaviour instance set in PluginUI.");
            LogWindow.AddLogMessage("BotBehaviour instance set in PluginUI.");
        }

        LogWindow.AddLogMessage("TestPlugin loaded.");
    }

    private static void BotUI()
    {
        PluginUI.RenderUI();
        LogWindow.Render();
    }
}