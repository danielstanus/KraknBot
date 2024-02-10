using Il2CppSystem.Collections.Generic;
using Seafight;
using Seafight.GameActors;
using Seafight.Utilities;
using TestPlugin.Helpers;
using TestPlugin.UI;
using UnityEngine;

namespace TestPlugin.States
{
    public class BotLogic
    {
        private bool _running = false;
        private GameObject _boxTarget;
        private GameObject _npcTarget;
        private Vector3[] _areaTargets = new Vector3[4];
        private int _currentAreaIndex = 0;

        public bool Running => _running;

        public BotLogic()
        {
            InitializeAreaTargets();
        }

        private void InitializeAreaTargets()
        {
            // Define corners within the map boundaries, slightly adjusted to be "close to corners"
            _areaTargets[0] = new Vector3(10, -73, 0); // Bottom left
            _areaTargets[1] = new Vector3(110, -73, 0); // Bottom right
            _areaTargets[2] = new Vector3(110, -10, 0); // Top right
            _areaTargets[3] = new Vector3(10, -10, 0); // Top left
        }

        public void Update()
        {
            if (!_running)
                return;

            SetTargets();

            if (_npcTarget != null && PluginUI.ShootNPC)
            {
                Log.Info("NPC Target found");
                HarmonyPatches.InputController.SelectTarget(_npcTarget.GetComponent<GameActorBehaviour>());
                if (HarmonyPatches.InputController.mapView.IsTargetInAttackRange())
                {
                    HarmonyPatches.InputController.attackSystem.Attack();
                }
                else if (GameContext.PlayerMovementBehaviour.IsMoving)
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

        private void SetTargets()
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
            return target != null && target.activeInHierarchy;
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