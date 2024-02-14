using System.Collections.Generic;
using KraknBot.Models;
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

    public static List<Ammo> CurrentAmmunitionList = new List<Ammo>();

    public static void UpdateAmmunitionList(InventorySystem inventorySystem)
    {
        CurrentAmmunitionList.Clear();
        // Assuming inventorySystem.Inventory gives you access to ammunition data
        foreach (var item in inventorySystem.Inventory)
        {
            if (item.Key == InventoryItemType.BALLS) // Assuming BALLS represents ammunition
            {
                foreach (var ammo in item.Value)
                {
                    int ammoId = ammo.Key;
                    long ammoAmount = ammo.Value;
                    ammoNames.TryGetValue(ammoId, out string ammoName); // Assuming ammoNames is accessible here

                    CurrentAmmunitionList.Add(new Ammo
                    {
                        Id = ammoId,
                        Amount = (int)ammoAmount,
                        Name = ammoName
                    });

                    // Print the cannonball ID, its amount, and name
                    Log.Info($"Cannonball ID: {ammoId}, Amount: {ammoAmount}, Name: {ammoName}");
                }
            }
        }
    }

    private static readonly Dictionary<int, string> ammoNames = new Dictionary<int, string>
    {
        {0, "No chosen ammo"},
        {1, "Chain ammunition"},
        {2, "Stone ammunition"},
        {3, "Splinter ammunition"},
        {4, "Fire ammunition"},
        {5, "Hollow ammunition"},
        {6, "Skull ammunition"},
        {18, "Copper harpoons"},
        {19, "Lead harpoons"},
        {20, "Bronze harpoons"},
        {51, "Explosive ammunition"},
        {75, "Iron harpoons"},
        {76, "Steel harpoons"},
        {77, "Damask harpoons"},
        {100, "Snow ammunition"},
        {101, "Flare ammunition"},
        {102, "Pumpkin ammunition"},
        {103, "Reinforced Explosive ammunition"},
        {104, "Soccer ammunition"},
        {120, "Repair ammunition"},
        {150, "Upgraded Shrapnel ammunition"},
        {160, "Upgraded Explosive ammunition"},
        {170, "Confetti ammunition"},
        {180, "Pyre ammunition"},
        {182, "Ice ammunition"},
        {183, "Kraken Poison ammunition"},
        {184, "Burning Ice ammunition"},
        {185, "Scrap ammunition"},
        {186, "Voodoo ammunition"},
        {187, "Voodoo Doom ammunition"},
        {188, "Soul Eater ammunition"},
        {189, "Heartbreaker ammunition"},
        {190, "Shell Shock ammunition"},
        {191, "Voodoo Blast ammunition"},
        {192, "Aqua ammunition"},
        {193, "Ignis ammunition"},
        {194, "Aer ammunition"},
        {195, "Terra ammunition"},
        {196, "Marauder ammunition"},
        {197, "Upgraded Soccer ammunition"},
        {198, "Rift ammunition"},
        {199, "Depth Charge"}
    };

    public static void GetCannonballAmount(InventorySystem inventorySystem, InventoryItemType cannonballType)
    {
        foreach (var item in inventorySystem.Inventory)
        {
            if (item.Key == cannonballType)
            {
                foreach (var cannonball in item.Value)
                {
                    int cannonballId = cannonball.Key;
                    long cannonballAmount = cannonball.Value;

                    // Attempt to get the name of the cannonball by its ID
                    ammoNames.TryGetValue(cannonballId, out string cannonballName);

                    // Print the cannonball ID, its amount, and name
                    Log.Info($"Cannonball ID: {cannonballId}, Amount: {cannonballAmount}, Name: {cannonballName}");
                }
            }
        }
    }
}