using HarmonyLib;
using Seafight.GameActors;
using UnityEngine;

namespace TestPlugin;

public class HarmonyPatches
{
    public static InputController InputController;
    public static MovementBehaviour MovementBehaviour;

    [HarmonyPatch(typeof(GameActorController), nameof(GameActorController.GameActorAdded))]
    [HarmonyPrefix]
    public static void GameActorAddedPrefix(GameActorController __instance, EntityId entityId, GameActorData actorData)
    {
        // Log.Info("GameActorAddedPrefix");
    }

    [HarmonyPatch(typeof(InputController), nameof(InputController.Update))]
    [HarmonyPrefix]
    public static void InputControllerUpdatePrefix(InputController __instance)
    {
        if (InputController == null)
        {
            InputController = __instance;
            Log.Info("InputController instance captured.");
        }
    }
}