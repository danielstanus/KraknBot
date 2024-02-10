using net.bigpoint.seafight.com.module.inventory;
using Seafight;
using Seafight.GameActors;
using UnityEngine;

namespace TestPlugin.Helpers;

public static class GameContext
{
    private static GameObject _playerGameObject;
    private static MovementBehaviour _movementBehaviour;


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

    public static void ResetContext()
    {
        _playerGameObject = null;
        _movementBehaviour = null;
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
                    Log.Info("Cannonball ID: " + cannonballId + ", Amount: " + cannonballAmount);
                }
            }
        }
    }
}