using HarmonyLib;
using Seafight.GameActors;
using UnityEngine;

namespace TestPlugin;

public class HarmonyPatches
{
    public static GameObject Player;
    public static InputController InputController;

    [HarmonyPatch(typeof(GameActorController), nameof(GameActorController.GameActorAdded))]
    [HarmonyPrefix]
    public static void GameActorAddedPrefix(GameActorController __instance, EntityId entityId, GameActorData actorData)
    {
        // Implement this
        if (__instance.gameActorModel.playerInfoSystem.UserId.Id == entityId.Id)
        {
            Player = __instance.gameObject;
            Log.Info("Initialized main player: " + entityId.Id.ToString());
        }
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