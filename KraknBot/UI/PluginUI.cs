using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using ImGuiNET;
using net.bigpoint.seafight.com.module.inventory;
using Seafight;
using Seafight.GameActors;
using KraknBot.Helpers;
using KraknBot.Models;
using UnityEngine;
using ImGuiInjection = DearImGuiInjection.DearImGuiInjection;

namespace KraknBot.UI;

public partial class PluginUI
{
    private static readonly PluginUI instance = new PluginUI();
    public static PluginUI Instance => instance;

    private byte[] bufferInputText = new byte[40];
    public static BotBehaviour BotBehaviourInstance { get; set; }

    public static bool CollectBoxes { get; set; }
    public static bool ShootNPC { get; set; }
    public static int RepairThreshold { get; set; } = 50;

    public static ReviveOption SelectedReviveOption { get; private set; } = ReviveOption.Emergency;

    private bool isBotRunning = false;
    private bool collectBoxes = false;
    private bool shootNPC = false;
    private int currentTab = 0; // 0 for Bot Options, 1 for Other Options

    private List<NPCItem> npcItems = [];
    private string newItemName = "";
    private int selectedAmmoIndex = 0;

    public static void RenderUI()
    {
        instance.InternalRenderUI();
    }

    private void InternalRenderUI()
    {
        SetupWindow();

        if (ImGuiInjection.IsCursorVisible)
        {
            var windowOpen = true;
            if (ImGui.Begin("KraknBot UI", ref windowOpen, ImGuiWindowFlags.None))
            {
                // Check if BotBehaviour is up to date before rendering the UI
                if (BotBehaviourInstance != null && !BotBehaviour.isUpToDate)
                {
                    // If the BotBehaviour is not up to date, display a message instead of the UI
                    ImGui.TextColored(new System.Numerics.Vector4(1, 0, 0, 1), "Plugin update required!");
                    ImGui.Text("Please update the plugin to the latest version.");
                }
                else
                {
                    // If BotBehaviour is up to date, render the UI as normal
                    RenderTabs();
                    switch (currentTab)
                    {
                        case 0:
                            RenderBotOptions();
                            break;
                        case 1:
                            RenderNPCOptions();
                            break;
                        case 2:
                            RenderOtherOptions();
                            break;
                    }
                }
            }

            ImGui.End();
        }
    }


    private void SetupWindow()
    {
        var windowPos = new System.Numerics.Vector2(Screen.width / 2 - 200, Screen.height / 2 - 150);
        var windowSize = new System.Numerics.Vector2(400, 300);

        ImGui.SetNextWindowPos(windowPos, ImGuiCond.FirstUseEver, new System.Numerics.Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);
    }

    private void RenderBotOptions()
    {
        ImGui.Text("Bot Options");
        RenderCollectBoxesOption();
        RenderShootNPCOption();
        RenderReviveOptions();
        RenderRepairTreshold();
    }

    private void RenderOtherOptions()
    {
        ImGui.Text("Other Options - Coming soon");
        // Implement other options UI elements here
    }

    private void ToggleBotRunningState()
    {
        isBotRunning = !isBotRunning;
        LogWindow.AddLogMessage(isBotRunning ? "Bot started" : "Bot stopped");

        DearImGuiInjection.BepInEx.UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (isBotRunning)
            {
                BotBehaviourInstance.StartCollector();
                Log.Info("Bot started");
            }
            else
            {
                BotBehaviourInstance.StopCollector();
                Log.Info("Bot stopped");
            }
        });
    }

    private void RenderCollectBoxesOption()
    {
        if (isBotRunning) ImGui.BeginDisabled();
        if (ImGui.Checkbox("Collect Boxes", ref collectBoxes))
        {
            CollectBoxes = collectBoxes;
            LogWindow.AddLogMessage(collectBoxes ? "Collect Boxes enabled" : "Collect Boxes disabled");
        }

        if (isBotRunning) ImGui.EndDisabled();
    }

    private void RenderShootNPCOption()
    {
        if (isBotRunning) ImGui.BeginDisabled();
        if (ImGui.Checkbox("Shoot NPC", ref shootNPC))
        {
            ShootNPC = shootNPC;
            LogWindow.AddLogMessage(shootNPC ? "Shoot NPC enabled" : "Shoot NPC disabled");
        }

        if (isBotRunning) ImGui.EndDisabled();
    }

    private void RenderReviveOptions()
    {
        if (isBotRunning) ImGui.BeginDisabled();
        ImGui.Text("Revive Options");
        int selectedOptionIndex = (int)SelectedReviveOption;
        string reviveOptionsCombo = "Emergency\0Standard\0";
        if (ImGui.Combo("Revive Type", ref selectedOptionIndex, reviveOptionsCombo))
        {
            SelectedReviveOption = (ReviveOption)selectedOptionIndex;
            LogWindow.AddLogMessage($"Selected Revive Option: {SelectedReviveOption}");
        }

        if (isBotRunning) ImGui.EndDisabled();
    }

    private void RenderRepairTreshold()
    {
        if (isBotRunning) ImGui.BeginDisabled();
        ImGui.Text("Repair Treshold");
        int tempRepairThreshold = RepairThreshold;
        if (ImGui.SliderInt("##RepairThreshold", ref tempRepairThreshold, 0, 100, $"{tempRepairThreshold}%"))
        {
            RepairThreshold = tempRepairThreshold; // Update the property with the temporary variable's value
            LogWindow.AddLogMessage($"Repair Threshold set to: {RepairThreshold}%");
        }

        if (isBotRunning) ImGui.EndDisabled();
    }
}