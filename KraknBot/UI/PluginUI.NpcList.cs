using System;
using System.Collections.Generic;
using System.Linq;
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
    private void RenderNPCOptions()
    {
        if (ImGui.BeginTable("NPCs", 4))
        {
            ImGui.TableSetupColumn("Active");
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Ammo", ImGuiTableColumnFlags.WidthFixed, 150.0f); // Fixed width for Ammo column
            ImGui.TableSetupColumn("Delete");
            ImGui.TableHeadersRow();

            for (int i = npcItems.Count - 1; i >= 0; i--)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                bool tempActive = npcItems[i].Active;
                if (ImGui.Checkbox($"##active{i}", ref tempActive))
                {
                    npcItems[i].Active = tempActive; // Update if changed
                }

                ImGui.TableNextColumn();
                ImGui.Text(npcItems[i].Name);

                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(150.0f);
                var ammoOptions = GameContext.CurrentAmmunitionList
                    .Select(a => $"{a.Name} (ID: {a.Id}, Amount: {a.Amount})")
                    .ToArray();
                string ammoOptionsCombined = string.Join('\0', ammoOptions) + '\0';
                int tempAmmoIndex = npcItems[i].AmmoIndex; // Temporary variable
                if (ImGui.Combo($"##ammo{i}", ref tempAmmoIndex, ammoOptionsCombined, ammoOptions.Length))
                {
                    npcItems[i].AmmoIndex = tempAmmoIndex; // Update if changed
                }

                ImGui.TableNextColumn();
                if (ImGui.Button($"Delete##{i}"))
                {
                    npcItems.RemoveAt(i);
                }
            }

            ImGui.EndTable();
        }

        ImGui.InputText("Name", ref newItemName, 100);
        ImGui.SameLine();
        if (ImGui.Button("Add"))
        {
            if (!string.IsNullOrWhiteSpace(newItemName))
            {
                npcItems.Add(new NPCItem { Active = true, Name = newItemName, AmmoIndex = 0 });
                newItemName = ""; // Reset after adding
            }
        }
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