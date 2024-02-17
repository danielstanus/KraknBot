using System.Linq;
using CakeBox;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Reflection;
using Seafight;
using Seafight.GameActors;
using Seafight.Utilities;
using KraknBot.Helpers;
using KraknBot.Models;
using KraknBot.UI;
using net.bigpoint.seafight.com.module.ship;
using Seafight.Storage;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KraknBot.States
{
    public class BotLogic
    {
        private bool _running = false;
        private GameObject _boxTarget;
        private GameObject _npcTarget;
        private Vector3[] _areaTargets = new Vector3[4];
        private int _currentAreaIndex = 0;

        private Vector3 _previousPosition = new Vector3();

        public bool Running => _running;

        public BotLogic()
        {
            InitializeAreaTargets();
        }

        private void InitializeAreaTargets()
        {
            // Define corners within the map boundaries, slightly adjusted to be "close to corners"
            _areaTargets[0] = new Vector3(0, 0, 0); // First quarter
            _areaTargets[1] = new Vector3(60, 0, 0); // Second quarter
            _areaTargets[2] = new Vector3(0, -44, 0); // Third quarter
            _areaTargets[3] = new Vector3(60, -44, 0); // Fourth quarter
        }

        public void Update()
        {
            if (!_running)
                return;

            UpdateTargets();

            var repairResult = HandleRepair();
            if (repairResult == RepairResult.Repaired)
            {
                Log.Info("Health is above threshold");
            }
            else if (repairResult == RepairResult.CantRepair)
            {
                Log.Info("Can't repair right now");
            }
            else if (repairResult == RepairResult.Repairing)
            {
                Log.Info("Repairing...");
                return;
            }

            if (_npcTarget != null && PluginUI.ShootNPC)
            {
                Log.Info("NPC Target found");
                HarmonyPatches.InputController.SelectTarget(_npcTarget.GetComponent<GameActorBehaviour>());
                if (HarmonyPatches.InputController.mapView.IsTargetInAttackRange())
                {
                    SetAmmo(); // Set the correct ammo type for the NPC
                    HarmonyPatches.InputController.attackSystem.Attack();
                }
                else if (GameContext.PlayerMovementBehaviour.IsMoving && GettingCloserToTarget())
                {

                    Log.Info("Player is moving");
                    return;
                }
                else
                {
                    HarmonyPatches.InputController.attackSystem.MoveToTargetPosition(3f);
                }
            }
            else if (_boxTarget != null && PluginUI.CollectBoxes)
            {
                if (GameContext.PlayerMovementBehaviour.IsMoving)
                {
                    Log.Info("Player is moving");
                }
                else
                {
                    MoveToTarget();
                }
            }
            else
            {
                if (!GameContext.PlayerMovementBehaviour.IsMoving)
                {
                    MoveEntityToNextArea();
                }
            }
        }

        private bool GettingCloserToTarget()
        {
            var targetPosition = _npcTarget.transform.position;
            var playerPosition = GameContext.PlayerGameObject.transform.position;

            var previousDistanceToTarget = Vector3.Distance(_previousPosition, targetPosition); // Distance from previous position to target
            var currentDistanceToTarget = Vector3.Distance(playerPosition, targetPosition); // Distance from current position to target

            Log.Info("Previous distance to target: " + previousDistanceToTarget + " Current distance to target: " + currentDistanceToTarget);
            if (currentDistanceToTarget < previousDistanceToTarget)
            {
                _previousPosition = playerPosition; // Update previous position
                return true;
            }

            return false;
        }


        private void SetAmmo()
        {
            var clientSettingStorage = MainInstaller.Inject<ClientSettingStorage>();
            var actors = HarmonyPatches.InputController.gameActorModel.Actors;
            foreach (var a in actors)
            {
                if (a.Key == _npcTarget.GetComponent<GameActorBehaviour>().EntityId)
                {
                    Log.Info("Target found in actors list");
                    var npcId =  a.Value.components[Il2CppType.Of<NpcData>()].Cast<NpcData>().NpcId;
                    var targetNPCItem = GameContext.npcTargetList.FirstOrDefault(n => n.Id == npcId && n.Active);
                    if (targetNPCItem != null && targetNPCItem.AmmoID != 0) // Check if the NPC is in the list and active
                    {
                        // Set the ammo to the correct type based on the NPCItem's AmmoIndex
                        clientSettingStorage.UpdateClientSetting(ClientSetting.SETTING_ACTIVE_CANNONBALL, targetNPCItem.AmmoID);
                        Log.Info($"Switched to ammo index {targetNPCItem.AmmoID} for NPC {targetNPCItem.Name}");
                    }
                }
            }
        }

        private RepairResult HandleRepair()
        {
            var playerInfo = HarmonyPatches.InputController.gameActorModel.playerInfoSystem.OwnPlayer;
            var repairData = playerInfo.components[Il2CppType.Of<GameActorRepairData>()].Cast<GameActorRepairData>();
            var attacking = HarmonyPatches.InputController.attackSystem.IsAttackInProgress();
            Log.Info("IsRepairing: " + repairData.isRepairing + " Attacking: " + attacking);
            if (repairData.isRepairing || attacking) return RepairResult.CantRepair;

            float currentHealth = (float)GameContext.PlayerHealthBehaviour.currentDictionary[(AmsAttributeType.HITPOINTS)];
            float maxHealth = (float)GameContext.PlayerHealthBehaviour.permanentDictionary[(AmsAttributeType.HITPOINTS)];
            if (currentHealth < maxHealth * (PluginUI.RepairThreshold / 100f))
            {
                InitiateRepair();
                return RepairResult.Repairing;
            }

            return RepairResult.Repaired;
        }

        private void InitiateRepair()
        {
            Log.Info($"Health below threshold. Initiating repair.");
            var actionMenuShip = GameObject.Find("ActionMenuShip_Default");
            if (actionMenuShip != null)
            {
                var actionMenuPanel = actionMenuShip.GetComponent<ActionMenuPanel>();
                actionMenuPanel.OnRepairEvent();
            }
        }

        private void UpdateTargets()
        {
            if (PluginUI.ShootNPC)
            {
                if (_npcTarget == null || !IsTargetValid(_npcTarget))
                {
                    _npcTarget = TargetFinder.FindNext(GameActorType.Npc);
                    if (_npcTarget == null)
                    {
                        Log.Info("No more NPCs to shoot");
                    }
                }
                else
                {
                    Log.Info("Current NPC target: " + _npcTarget.name);
                    var actors = HarmonyPatches.InputController.gameActorModel.Actors;
                    // var npcEntity = _npcTarget.GetComponent<GameActorBehaviour>().EntityId;
                    foreach (var a in actors)
                    {
                        if (a.Key == _npcTarget.GetComponent<GameActorBehaviour>().EntityId)
                        {
                            Log.Info("Target found in actors list (NPC)");
                            Log.Info("NPC data: " + a.Value.components[Il2CppType.Of<NpcData>()].Cast<NpcData>().NpcId);

                        }
                    }
                }
            }

            if (PluginUI.CollectBoxes)
            {
                if (_boxTarget == null || !IsTargetValid(_boxTarget))
                {
                    _boxTarget = TargetFinder.FindNext(GameActorType.Box);
                    if (_boxTarget == null)
                    {
                        Log.Info("No more boxes to collect");
                    }
                }
            }
        }


        public void Start()
        {
            var entityId = HarmonyPatches.InputController.gameActorModel.playerInfoSystem.UserId;
            var player = HarmonyPatches.InputController.mapView.GetEntity(entityId);
            var playerObject = player.gameObject;

            _running = true;
        }

        public void Stop()
        {
            this._running = false;
            this._boxTarget = null;
        }

        private void MoveToTarget()
        {
            if (_boxTarget == null)
                return;

            var targetPosition = _boxTarget.transform.position;

            Vector3 movePosition;
            movePosition.x = targetPosition.x;
            movePosition.y = targetPosition.y;
            movePosition.z = -100;
            HarmonyPatches.InputController.OnClickOnMap(movePosition);
        }

        private bool IsTargetValid(GameObject target)
        {
            return target != null && target.activeInHierarchy && target.GetComponent<GameActorBehaviour>();
        }

        private void MoveEntityToNextArea()
        {
            // Get the next area to move towards in a circular manner
            Vector3 targetArea = _areaTargets[_currentAreaIndex];
            _currentAreaIndex = (_currentAreaIndex + 1) % _areaTargets.Length; // Cycle through the areas

            // Initialize a list to hold potential unblocked positions
            var potentialPosition = new Vector3();

            do
            {
                var x = Random.Range(0, 60);
                var y = Random.Range(0, -44);
                Vector3 newPosition = new Vector3(targetArea.x + x, targetArea.y + y, 0);

                // Convert the world position to map field coordinates to check if it's blocked
                Vector2Int mapField = MapUtils.GetMapField(newPosition);
                Log.Info("Trying to find new random position");
                if (!HarmonyPatches.InputController.mapView.isCoordBlocked(mapField))
                {
                    Log.Info("Found new random position");
                    potentialPosition = newPosition;
                }
            } while (potentialPosition == Vector3.zero);

            HarmonyPatches.InputController.OnClickOnMap(potentialPosition);
            Log.Info($"Moving to unblocked area: {potentialPosition.x}, {potentialPosition.y}");
        }
    }
}