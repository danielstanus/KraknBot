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
    private void RenderTabs()
    {
        // Assume these colors are defined or adjusted for the purpose of highlighting selected tabs
        System.Numerics.Vector4 normalButtonColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Button];
        System.Numerics.Vector4 selectedButtonColor = new System.Numerics.Vector4(0.35f, 0.10f, 0.10f, 1.00f); // Brighter red for selected tab

        // Settings Tab
        ApplyTabButtonColor(0 == currentTab ? selectedButtonColor : normalButtonColor);
        if (ImGui.Button("Settings"))
        {
            currentTab = 0;
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();

        // NPC List Tab
        ApplyTabButtonColor(1 == currentTab ? selectedButtonColor : normalButtonColor);
        if (ImGui.Button("NPC List"))
        {
            currentTab = 1;
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();

        // Radar
        ApplyTabButtonColor(2 == currentTab ? selectedButtonColor : normalButtonColor);
        if (ImGui.Button("Radar"))
        {
            currentTab = 2;
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();

        // BM Tab
        ApplyTabButtonColor(3 == currentTab ? selectedButtonColor : normalButtonColor);
        if (ImGui.Button("BM"))
        {
            currentTab = 3;
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();

        // BM Tab
        ApplyTabButtonColor(4 == currentTab ? selectedButtonColor : normalButtonColor);
        if (ImGui.Button("WarBot"))
        {
            currentTab = 4;
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();

        // Start/Stop Bot Tab (Dynamic color based on bot state)
        ApplyButtonColorForBotState();
        if (ImGui.Button(GameContext.BotRunning ? "Stop Bot" : "Start Bot"))
        {
            ToggleBotRunningState();
        }

        ImGui.PopStyleColor(3);
    }

    private void ApplyTabButtonColor(System.Numerics.Vector4 color)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, color);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color * new System.Numerics.Vector4(1.1f, 1.1f, 1.1f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, color * new System.Numerics.Vector4(0.9f, 0.9f, 0.9f, 1.0f));
    }

    private void ApplyButtonColorForBotState()
    {
        System.Numerics.Vector4 buttonColor = GameContext.BotRunning
            ? new System.Numerics.Vector4(0.8f, 0.2f, 0.2f, 1.0f) // Red for "stop"
            : new System.Numerics.Vector4(0.2f, 0.8f, 0.2f, 1.0f); // Green for "start"
        ApplyTabButtonColor(buttonColor);
    }
}