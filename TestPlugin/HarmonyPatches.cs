using HarmonyLib;
using net.bigpoint.seafight.com.module.inventory;
using Seafight;
using Seafight.GameActors;
using TestPlugin.Helpers;
using TestPlugin.UI;
using UnityEngine;

namespace TestPlugin;

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

    [HarmonyPatch(typeof(GameActorController), nameof(GameActorController.GameActorAdded))]
    [HarmonyPrefix]
    public static void GameActorAddedPrefix(GameActorController __instance, EntityId entityId,
        GameActorData actorData)
    {
        // Implementation for GameActorAdded
    }

    [HarmonyPatch(typeof(DeathWindowBehaviour), nameof(DeathWindowBehaviour.OnOpen))]
    [HarmonyPostfix]
    public static void OnOpenPostfix(DeathWindowBehaviour __instance)
    {
        InventorySystem inventorySystem = MainInstaller.Inject<InventorySystem>();

        System.Action reviveAction = () =>
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