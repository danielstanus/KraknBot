using System;
using System.Collections.Generic;
using System.Linq;
using Bigpoint.Loca;
using Il2CppInterop.Runtime;
using ImGuiNET;
using net.bigpoint.seafight.com.module.inventory;
using Seafight;
using Seafight.GameActors;
using KraknBot.Helpers;
using KraknBot.Models;
using UniRx;
using UnityEngine;
using static KraknBot.HarmonyPatches;
using ImGuiInjection = DearImGuiInjection.DearImGuiInjection;


namespace KraknBot.UI;

public partial class PluginUI
{
    private static int newItemSelectedIndex = 0;
    private static string npcFilter = string.Empty; // For filtering NPC names
    private static string newItemName2 = string.Empty; // To store the selected NPC name


    private void RenderNPCOptions()
    {
        if (GameContext.BotRunning) ImGui.BeginDisabled();
        if (ImGui.BeginTable("NPCs", 4))
        {
            ImGui.TableSetupColumn("Active");
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Ammo", ImGuiTableColumnFlags.WidthFixed, 150.0f); // Fixed width for Ammo column
            ImGui.TableSetupColumn("Delete");
            ImGui.TableHeadersRow();

            for (int i = GameContext.npcTargetList.Count - 1; i >= 0; i--)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(100.0f);

                bool tempActive = GameContext.npcTargetList[i].Active;
                if (ImGui.Checkbox($"##active{i}", ref tempActive))
                {
                    GameContext.npcTargetList[i].Active = tempActive; // Update if changed
                }

                ImGui.TableNextColumn();
                ImGui.Text(GameContext.npcTargetList[i].Name + $" (ID: {GameContext.npcTargetList[i].Id})");

                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(150.0f);
                // Assuming GameContext.CurrentAmmunitionList is updated to use actual ammo IDs
                string ammoOptionsCombined =
                    string.Join('\0', GameContext.CurrentAmmunitionList.Select(a => $"{a.Name} (ID: {a.Id})")) + '\0';
                int currentAmmoID = GameContext.npcTargetList[i].AmmoID;
                int ammoIDIndex = GameContext.CurrentAmmunitionList.FindIndex(a => a.Id == currentAmmoID);

                if (ImGui.Combo($"##ammo{i}", ref ammoIDIndex, ammoOptionsCombined,
                        GameContext.CurrentAmmunitionList.Count))
                {
                    // Update with the selected Ammo ID
                    GameContext.npcTargetList[i].AmmoID = GameContext.CurrentAmmunitionList[ammoIDIndex].Id;
                }

                ImGui.TableNextColumn();
                if (ImGui.Button($"Delete##{i}"))
                {
                    GameContext.npcTargetList.RemoveAt(i);
                }
            }

            ImGui.EndTable();
        }

        ImGui.InputText("Filter##NPC", ref npcFilter, 100);
        ImGui.SameLine();
        if (ImGui.Button("Add"))
        {
            if (!string.IsNullOrWhiteSpace(newItemName2))
            {
                NPC selectedNPC = GameContext.npcs.FirstOrDefault(npc => npc.Name == newItemName2);
                if (selectedNPC != null)
                {
                    GameContext.npcTargetList.Add(new NPCItem
                        { Active = true, Name = newItemName2, AmmoID = 0, Id = selectedNPC.Id });
                    newItemName2 = ""; // Reset after adding
                }
            }
        }

        // Adjust the size as needed, for example, -1 for width to use the current window width,
        // and 200 pixels for height to fit approximately 10 rows at a time.
        if (ImGui.BeginChild("FilteredNPCList", new System.Numerics.Vector2(-1, 200), true,
                ImGuiWindowFlags.HorizontalScrollbar))
        {
            foreach (var npc in GameContext.npcs)
            {
                // Filtering logic: show NPC if filter is empty or if name contains the filter text
                if (string.IsNullOrEmpty(npcFilter) ||
                    npc.Name.IndexOf(npcFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ImGui.PushID(npc.Id); // Ensure unique ID for each item for ImGui internals
                    if (ImGui.Selectable(npc.Name, newItemName2 == npc.Name))
                    {
                        newItemName2 = npc.Name; // Update the selected NPC name
                        newItemSelectedIndex = npc.Id; // Update the selected index
                    }

                    ImGui.PopID();
                }
            }

            ImGui.EndChild(); // End of filtered list child frame
        }

        if (GameContext.BotRunning) ImGui.EndDisabled();
    }

    private void RenderRadarOptions()
    {
        if (GameContext.BotRunning) ImGui.BeginDisabled();

        // Ensure the ImGui logic is executed outside of the UnityMainThreadDispatcher callback
        if (ImGui.BeginTable("NPCs", 3)) // Adjusted for three columns: Name, ID, and Add
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("ID", ImGuiTableColumnFlags.WidthFixed, 100.0f); // Fixed width for ID column
            ImGui.TableSetupColumn("Add");
            ImGui.TableHeadersRow();

            foreach (var npc in GameContext.npcsToAdd)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text(npc.Name); // Display NPC Name

                ImGui.TableNextColumn();
                ImGui.Text($"{npc.Id}"); // Display NPC ID

                ImGui.TableNextColumn();
                if (ImGui.Button($"Add##{npc.Id}"))
                {
                    NPCItem newItem = new NPCItem { Active = true, Name = npc.Name, AmmoID = 0, Id = npc.Id };
                    GameContext.npcTargetList.Add(newItem);
                    GameContext.UpdateRadarList();
                }
            }

            ImGui.EndTable();
        }

        if (GameContext.BotRunning) ImGui.EndDisabled();
    }


    private string[] GetAmmoOptions()
    {
        // Placeholder method to fetch and convert ammo data from the game's inventory system
        // This should interact with GameContext or a similar mechanism to fetch real ammo names
        InventorySystem inventorySystem = MainInstaller.Inject<InventorySystem>();
        var ammoList = new List<string>();
        // Assume we have a method to fetch the actual ammo names based on game data
        GameContext.GetCannonballAmount(inventorySystem,
            InventoryItemType.BALLS); // Example call, adjust based on actual implementation
        // Populate ammoList based on fetched data, for example:
        ammoList.Add("Cannonball 1"); // Placeholder names, replace with actual data fetching
        ammoList.Add("Cannonball 2");
        // Return the ammo names as an array
        return ammoList.ToArray();
    }
}