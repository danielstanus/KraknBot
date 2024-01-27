﻿using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MonoMod.RuntimeDetour;
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

    public override void Load()
    {
        InitLog.Init(Log);

        Harmony.CreateAndPatchAll(typeof(HarmonyPatches));

        ImGuiInjection.Render += BotUI;
        ImGuiStyleManager.ApplyDefaultStyle();


        ClassInjector.RegisterTypeInIl2Cpp<BotBehaviour>();
        TestPluginBehaviourHolder = new("TestPluginBehaviourGO");
        GameObject.DontDestroyOnLoad(TestPluginBehaviourHolder);
        TestPluginBehaviourHolder.hideFlags |= HideFlags.HideAndDontSave;
        _botBehaviourInstance = TestPluginBehaviourHolder.AddComponent<BotBehaviour>();
        PluginUI.BotBehaviourInstance = _botBehaviourInstance;

        LogWindow.AddLogMessage("TestPlugin loaded.");
    }

    private static void BotUI()
    {
        PluginUI.RenderUI();
        LogWindow.Render();
    }
}