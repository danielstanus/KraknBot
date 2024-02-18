using Il2CppInterop.Runtime;
using ImGuiNET;
using KraknBot.Helpers;
using KraknBot.Models;
using Seafight.GameActors;
using UniRx;
using UnityEngine;

namespace KraknBot.UI;

public partial class PluginUI
{
    private void RenderWarBotOptions()
    {
        if (GameContext.BotRunning) ImGui.BeginDisabled();

        ImGui.Text("Select a player. Then click 'Set Master' to set the selected player as the master.");

        if (GameContext.Master != null)
        {
            ImGui.Text($"Master: {GameContext.Master.name}");

            ImGui.SameLine();
            if (ImGui.Button("Clear Master"))
            {
                GameContext.Master = null;
            }
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Set text color to red
            ImGui.Text("No master selected.");
            ImGui.PopStyleColor(); // Reset text color to default
        }

        ImGui.Separator();

        var targetId = "";
        string targetName = "";

        if (HarmonyPatches.InputController.gameActorModel.TargetData != null)
        {
            if (HarmonyPatches.InputController.gameActorModel.TargetData.GameActorType == GameActorType.User)
            {
                targetId = HarmonyPatches.InputController.gameActorModel.TargetId.Value.Id.ToString();
                targetName = HarmonyPatches.InputController.gameActorModel.TargetData.components[Il2CppType.Of<GameActorNameData>()].Cast<GameActorNameData>().Name;
                ImGui.Text($"Target: {targetName} (ID: {targetId})");

                if (ImGui.Button($"Set Master"))
                {
                    var targetBehaviour = HarmonyPatches.InputController.mapView.GetEntity(HarmonyPatches.InputController.gameActorModel.TargetId.Value);
                    GameContext.Master = targetBehaviour.gameObject;
                }
            }
        }
        else
        {
            ImGui.Text("No target selected.");
        }

        ImGui.Separator();

        ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Set text color to red
        ImGui.Text("CAUTION !!!");
        ImGui.Text("This feature is experimental and may not work as expected. Use at your own risk.");
        ImGui.Text("Also by using this option you will go under the radar of BigPoint.");
        ImGui.PopStyleColor(); // Reset text color to default

        if (GameContext.BotRunning) ImGui.EndDisabled();
    }
}