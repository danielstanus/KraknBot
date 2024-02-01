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
        private MovementBehaviour _movementBehaviour;

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
                else if (_movementBehaviour.IsMoving)
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
                if (_movementBehaviour.IsMoving)
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
                if (!_movementBehaviour.IsMoving)
                {
                    MoveEntityToRandomUnblockedTile(40, 20);
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

            _movementBehaviour ??= playerObject.GetComponent<MovementBehaviour>();
            _running = true;
        }

        public void Stop()
        {
            this._running = false;
            this._boxTarget = null;
            this._movementBehaviour = null;
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

        private void MoveEntityToRandomUnblockedTile(int moveRange, int minDistanceFromPlayer)
        {
            Vector3 entityPosition = GameContext.PlayerGameObject.transform.position;
            Vector2Int entityMapField = MapUtils.GetMapField(entityPosition);

            var potentialPositions = new System.Collections.Generic.List<Vector2Int>();
            for (int x = -moveRange; x <= moveRange; x++)
            {
                for (int y = -moveRange; y <= moveRange; y++)
                {
                    Vector2Int newPosition = new Vector2Int();
                    newPosition.Set(entityMapField.x + x, entityMapField.y + y);

                    // Calculate distance from the current entity position to the new position
                    int distanceFromPlayer = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)); // Using Chebyshev distance for simplicity

                    if (!HarmonyPatches.InputController.mapView.isCoordBlocked(newPosition) && distanceFromPlayer >= minDistanceFromPlayer)
                    {
                        potentialPositions.Add(newPosition);
                    }
                }
            }

            if (potentialPositions.Count > 0)
            {
                var randomPosition = potentialPositions[UnityEngine.Random.Range(0, potentialPositions.Count)];
                Vector3 targetPosition = MapUtils.GetWorldPoint(randomPosition);
                HarmonyPatches.InputController.OnClickOnMap(targetPosition); // Ensure this method is accessible or indirectly triggered

                Log.Info("Moving to random unblocked tile: " + randomPosition.x + ", " + randomPosition.y);
            }
            else
            {
                Log.Info("No unblocked tile available to move to at the desired distance.");
            }
        }
    }
}