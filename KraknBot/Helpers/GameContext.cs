using net.bigpoint.seafight.com.module.inventory;
using Seafight;
using Seafight.GameActors;
using UnityEngine;

namespace KraknBot.Helpers;

public static class GameContext
{
    private static GameObject _playerGameObject;
    private static MovementBehaviour _movementBehaviour;
    private static HealthBehaviour _healthBehaviour;

    public static GameObject PlayerGameObject
    {
        get
        {
            if (_playerGameObject != null && !_playerGameObject.Equals(null)) return _playerGameObject;

            var entityId = HarmonyPatches.InputController.gameActorModel.playerInfoSystem.UserId;
            var playerEntity = HarmonyPatches.InputController.mapView.GetEntity(entityId);
            _playerGameObject = playerEntity != null ? playerEntity.gameObject : null;

            return _playerGameObject;
        }
    }

    public static MovementBehaviour PlayerMovementBehaviour
    {
        get
        {
            if (_movementBehaviour != null && !_movementBehaviour.Equals(null)) return _movementBehaviour;

            var entityId = HarmonyPatches.InputController.gameActorModel.playerInfoSystem.UserId;
            var playerEntity = HarmonyPatches.InputController.mapView.GetEntity(entityId);
            _playerGameObject = playerEntity != null ? playerEntity.gameObject : null;
            if (_playerGameObject != null) _movementBehaviour = _playerGameObject.GetComponent<MovementBehaviour>();
            return _movementBehaviour;
        }
    }

    public static HealthBehaviour PlayerHealthBehaviour
    {
        get
        {
            if (_healthBehaviour != null && !_healthBehaviour.Equals(null)) return _healthBehaviour;

            var entityId = HarmonyPatches.InputController.gameActorModel.playerInfoSystem.UserId;
            var playerEntity = HarmonyPatches.InputController.mapView.GetEntity(entityId);
            _playerGameObject = playerEntity != null ? playerEntity.gameObject : null;
            if (_playerGameObject != null) _healthBehaviour = _playerGameObject.GetComponent<HealthBehaviour>();
            return _healthBehaviour;
        }
    }

    public static void ResetContext()
    {
        _playerGameObject = null;
        _movementBehaviour = null;
        _healthBehaviour = null;
    }

    public static void foo()
    {
        var entityId = HarmonyPatches.InputController.gameActorModel.playerInfoSystem.UserId;
        var player = HarmonyPatches.InputController.mapView.GetEntity(entityId);
        var playerObject = player.gameObject;
        Log.Info($"Component: {playerObject.name}");
        Log.Info($"Component: {playerObject.GetIl2CppType().Name}");
        var movementBehaviour = playerObject.GetComponent<MovementBehaviour>();
        Log.Info($"Component: {movementBehaviour.name}");
        Log.Info($"Component: {movementBehaviour.IsMoving}");
        Log.Info($"Component: {movementBehaviour.isMoving}");

        DearImGuiInjection.BepInEx.UnityMainThreadDispatcher.Enqueue(() =>
        {
            Log.Info($"///////////////////////////////////////////////");
            Log.Info($"All cannonballs:");
            InventorySystem inventorySystem = MainInstaller.Inject<InventorySystem>();
            GameContext.GetCannonballAmount(inventorySystem, InventoryItemType.BALLS);
            Log.Info($"///////////////////////////////////////////////");
        });
    }

    public static void GetCannonballAmount(InventorySystem inventorySystem, InventoryItemType cannonballType)
    {
        // Iterate over each item in the inventory
        foreach (var item in inventorySystem.Inventory)
        {
            // Check if the item is of cannonball type
            if (item.Key == cannonballType)
            {
                // item.Value is a ReactiveDictionary<int, long>
                foreach (var cannonball in item.Value)
                {
                    int cannonballId = cannonball.Key;
                    long cannonballAmount = cannonball.Value;

                    // Log the cannonball ID and its amount
                    Log.Info("Cannonball ID: " + cannonballId + ", Amount: " + cannonballAmount + " Cannonball name: " );
                }
            }
        }
    }
}