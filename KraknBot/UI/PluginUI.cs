using System;
using Il2CppInterop.Runtime;
using ImGuiNET;
using net.bigpoint.seafight.com.module.inventory;
using Seafight;
using Seafight.GameActors;
using KraknBot.Helpers;
using UnityEngine;
using ImGuiInjection = DearImGuiInjection.DearImGuiInjection;

namespace KraknBot.UI;

public class PluginUI
{
    private byte[] bufferInputText = new byte[40];
    private static PluginUI Instance => Singleton<PluginUI>.Instance;
    public static BotBehaviour BotBehaviourInstance { get; set; }

    public static bool CollectBoxes { get; set; }
    public static bool ShootNPC { get; set; }

    private bool isBotRunning = false;
    private bool collectBoxes = false;
    private bool shootNPC = false;

    private string reviveOptions = "Emergency\0Standard\0";
    public static ReviveOption SelectedReviveOption { get; private set; } = ReviveOption.Emergency;

    public static void RenderUI()
    {
        Instance.InternalRenderUI();
    }

    private void InternalRenderUI()
    {
        System.Numerics.Vector2 windowPos = new System.Numerics.Vector2(10, 10);
        System.Numerics.Vector2 windowSize = new System.Numerics.Vector2(400, 300);
        ImGui.SetNextWindowPos(windowPos, ImGuiCond.FirstUseEver, new System.Numerics.Vector2(1.0f, 1.0f));
        ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);

        if (ImGuiInjection.IsCursorVisible)
        {
            var windowOpen = true;
            if (ImGui.Begin("KraknBot UI", ref windowOpen, (int)ImGuiWindowFlags.None))
            {
                ImGui.Text("Bot Options");

                // Toggle button for starting/stopping the bot
                if (ImGui.Button(isBotRunning ? "Stop Bot" : "Start Bot"))
                {
                    isBotRunning = !isBotRunning;
                    LogWindow.AddLogMessage(isBotRunning ? "Bot started" : "Bot stopped");

                    DearImGuiInjection.BepInEx.UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        if (isBotRunning)
                        {
                            BotBehaviourInstance.StartCollector(); // Assuming this starts the bot
                            Log.Info("Bot started");
                        }
                        else
                        {
                            BotBehaviourInstance.StopCollector(); // Assuming this stops the bot
                            Log.Info("Bot stopped");
                        }
                    });
                }

                if (isBotRunning)
                {
                    ImGui.BeginDisabled();
                }

                if (ImGui.Checkbox("Collect Boxes", ref collectBoxes))
                {
                    LogWindow.AddLogMessage(collectBoxes ? "Collect Boxes enabled" : "Collect Boxes disabled");
                    CollectBoxes = collectBoxes;
                }

                // if (ImGui.Checkbox("Collect Boxes", ref collectBoxes))
                // {
                //     LogWindow.AddLogMessage(collectBoxes ? "Collect Boxes enabled" : "Collect Boxes disabled");
                //     // Additional logic for collecting boxes can be added here
                //     // List all components of Player
                //     var entityId = HarmonyPatches.InputController.gameActorModel.playerInfoSystem.UserId;
                //     var player = HarmonyPatches.InputController.mapView.GetEntity(entityId);
                //     var playerObject = player.gameObject;
                //     Log.Info($"Component: {playerObject.name}");
                //     Log.Info($"Component: {playerObject.GetIl2CppType().Name}");
                //     var movementBehaviour = playerObject.GetComponent<MovementBehaviour>();
                //     Log.Info($"Component: {movementBehaviour.name}");
                //     Log.Info($"Component: {movementBehaviour.IsMoving}");
                //     Log.Info($"Component: {movementBehaviour.isMoving}");
                //
                //     DearImGuiInjection.BepInEx.UnityMainThreadDispatcher.Enqueue(() =>
                //     {
                //         Log.Info($"///////////////////////////////////////////////");
                //         Log.Info($"All cannonballs:");
                //         InventorySystem inventorySystem = MainInstaller.Inject<InventorySystem>();
                //         GameContext.GetCannonballAmount(inventorySystem, InventoryItemType.BALLS);
                //         Log.Info($"///////////////////////////////////////////////");
                //     });
                // }

                if (ImGui.Checkbox("Shoot NPC", ref shootNPC))
                {
                    LogWindow.AddLogMessage(shootNPC ? "Shoot NPC enabled" : "Shoot NPC disabled");
                    ShootNPC = shootNPC;
                }

                ImGui.Text("Revive Options");
                string reviveOptionsCombo = "Emergency\0Standard\0";

                int selectedOptionIndex = (int)SelectedReviveOption;
                if (ImGui.Combo("Revive Type", ref selectedOptionIndex, reviveOptionsCombo))
                {
                    SelectedReviveOption = (ReviveOption)selectedOptionIndex;
                    LogWindow.AddLogMessage($"Selected Revive Option: {SelectedReviveOption}");
                }

                if (isBotRunning)
                {
                    ImGui.EndDisabled();
                }
            }
            ImGui.End();
        }
    }
}
