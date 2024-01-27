using System;
using System.Collections.Generic;
using UnityEngine;
using Seafight;
using Seafight.GameActors;

namespace TestPlugin
{
    public class Collector
    {
        private bool _collectingEnabled = false;
        private GameObject _target;
        private MovementBehaviour _movementBehaviour;

        public void Update()
        {
            if (!_collectingEnabled)
                return;

            LogWindow.AddLogMessage("Tick!");

            if (_target == null || !IsTargetValid(_target))
            {
                if (!FindNextTarget())
                {
                    Log.Info("No more boxes to collect");
                    return;
                }
            }

            // if (HarmonyPatches.Player.GetComponent<MovementBehaviour>().isMoving)
            // {
            //     Log.Info("Player is moving");
            //     return;
            // }
            //
            // MoveToTarget();
        }

        public void Start()
        {
            _collectingEnabled = true;
        }

        public void Stop()
        {
            this._collectingEnabled = false;
        }

        private void MoveToTarget()
        {
            if (_target == null)
                return;

            var targetPosition = _target.transform.position;

            Vector3 movePosition;
            movePosition.x = targetPosition.x;
            movePosition.y = targetPosition.y;
            movePosition.z = -100;
            HarmonyPatches.InputController.MoveToPoint(movePosition);
        }

        private bool FindNextTarget()
        {
            var actors = HarmonyPatches.InputController.gameActorModel.Actors;
            if (actors.Count == 0)
                return false;

            foreach (var actor in actors)
            {
                if (actor.Value == null)
                    continue;

                if (actor.Value.GameActorType == GameActorType.Box)
                {
                    var gameObject = GetGameObject(actor.Key);
                    if (gameObject == null)
                        continue;

                    _target = gameObject;
                    return true;
                }
            }
            return false;
        }

        private GameObject GetGameObject(EntityId entityId)
        {
            var entity = HarmonyPatches.InputController.mapView.GetEntity(entityId);

            return entity != null ? entity.gameObject : null;
        }

        private bool IsTargetValid(GameObject target)
        {
            return target != null && target.activeInHierarchy;
        }
    }
}