using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using ImGuiNET;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace TestPlugin;

[BepInDependency(DearImGuiInjection.Metadata.GUID)]
[BepInPlugin(Metadata.GUID, Metadata.Name, Metadata.Version)]
internal unsafe class TestPlugin : BasePlugin
{
    private static bool _isMyUIOpen = true;

    private static List<Hook> Hooks = new();
    private GameObject TestPluginBehaviourHolder;
    private BotBehaviour _botBehaviourInstance;
    private static byte[] buffer_input_text = new byte[40];

    public override void Load()
    {
        InitLog.Init(Log);

        DearImGuiInjection.DearImGuiInjection.Render += MyUI;

        ClassInjector.RegisterTypeInIl2Cpp<BotBehaviour>();
        TestPluginBehaviourHolder = new("TestPluginBehaviourGO");
        GameObject.DontDestroyOnLoad(TestPluginBehaviourHolder);
        TestPluginBehaviourHolder.hideFlags |= HideFlags.HideAndDontSave;
        _botBehaviourInstance = TestPluginBehaviourHolder.AddComponent<BotBehaviour>();
    }

    private static void MyUI()
    {
        if (DearImGuiInjection.DearImGuiInjection.IsCursorVisible)
        {
            var dummy2 = true;
            if (ImGui.Begin(Metadata.GUID, ref dummy2, (int)ImGuiWindowFlags.None))
            {
                ImGui.Text("Bot Options");

                // if (ImGui.InputText("lol", buffer_input_text, (uint)buffer_input_text.Length))
                // {
                //
                // }

                if (ImGui.Button("Collect Boxes"))
                {
                    // Interacting with the unity api must be done from the unity main thread
                    // Can just use the dispatcher shipped with the library for that
                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        //var go = new GameObject();
                        //go.AddComponent<Stuff>();
                    });
                }
            }

            ImGui.End();
        }
    }
}