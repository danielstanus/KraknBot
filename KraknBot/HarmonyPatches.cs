using System;
using HarmonyLib;
using net.bigpoint.seafight.com.module.inventory;
using Seafight;
using Seafight.GameActors;
using Seafight.Utilities;
using KraknBot.Helpers;
using KraknBot.Map;
using KraknBot.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KraknBot;

public static class HarmonyPatches
{
    private static InputController _inputController;

    public static InputController InputController
    {
        get => _inputController;
        private set
        {
            if (_inputController == null)
            {
                _inputController = value;
                Log.Info("InputController instance captured.");
            }
        }
    }

    [HarmonyPatch(typeof(InputController), nameof(InputController.Update))]
    [HarmonyPrefix]
    public static void UpdatePrefix(InputController __instance)
    {
        InputController = __instance;
    }

    [HarmonyPatch(typeof(InputController), nameof(InputController.OnClickOnMap))]
    [HarmonyPrefix]
    public static void OnClickOnMapPrefix(InputController __instance, Vector3 inputPosition)
    {
        Log.Info($"Clicked on map at {inputPosition.x}, {inputPosition.y}, {inputPosition.z}, Tile: {MapUtils.GetMapField(inputPosition).x}, {MapUtils.GetMapField(inputPosition).y}");
    }

    [HarmonyPatch(typeof(GameActorController), nameof(GameActorController.GameActorAdded))]
    [HarmonyPrefix]
    public static void GameActorAddedPrefix(GameActorController __instance, EntityId entityId, GameActorData actorData)
    {
        var entity = InputController.mapView.GetEntity(entityId);
        if (entity == null)
            return;

        // if (actorData.GameActorType == GameActorType.Tower)
        // {
        //     MapController.CreateBlockedCoordsAroundEntity(entity.gameObject, 50);
        // }

        var userId = InputController.gameActorModel.playerInfoSystem.UserId;
        if (entityId == userId)
        {
            GameContext.ResetContext();
        }
    }

    [HarmonyPatch(typeof(DeathWindowBehaviour), nameof(DeathWindowBehaviour.OnOpen))]
    [HarmonyPostfix]
    public static void OnOpenPostfix(DeathWindowBehaviour __instance)
    {
        // Check if the bot is running
        if (!BotBehaviour.Instance?.Running ?? true)
        {
            return; // Bot is not running, exit the method
        }

        InventorySystem inventorySystem = MainInstaller.Inject<InventorySystem>();

        Action reviveAction = () =>
        {
            // Check the selected revive option from the PluginUI
            ReviveOption selectedOption = PluginUI.SelectedReviveOption;

            if (selectedOption == ReviveOption.Emergency)
            {
                if (inventorySystem.Inventory.ContainsKey(InventoryItemType.NONPERISHABLE) &&
                    inventorySystem.Inventory[InventoryItemType.NONPERISHABLE].TryGetValue(99, out long amount) &&
                    amount > 0)
                {
                    __instance.OnEmergencyClicked();
                }
            }
            else // Standard
            {
                __instance.OnStandardClicked();
            }
        };

        float delayInSeconds = Random.Range(2f, 5f);
        CoroutineHandler.Instance.ExecuteDelayed(delayInSeconds, reviveAction);
    }
}