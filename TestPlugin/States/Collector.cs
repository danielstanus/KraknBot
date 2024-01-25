using System;
using System.Collections.Generic;
using UnityEngine;
using Seafight;

namespace TestPlugin
{
    public class Collector
    {
        private bool _collectingEnabled = false;
        private GameObject _target;
        private readonly List<GameObject> _removedBoxes = new List<GameObject>();
        private float _elapsed = 0f;

        public void Update()
        {
            if (!_collectingEnabled)
                return;

            if (!FindNextTarget())
            {
                Log.Info("No more boxes to collect");
                return;
            }

            MoveToTarget();
        }

        public void Start()
        {
            this._collectingEnabled = true;
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
            HarmonyPatches.inputController.MoveToPoint(movePosition);
        }

        private bool FindNextTarget()
        {
            // Adapted logic from C++ to C#
            return false;
        }
    }
}