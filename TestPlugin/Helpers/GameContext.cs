using UnityEngine;

namespace TestPlugin.Helpers
{
    public static class GameContext
    {
        private static GameObject _playerGameObject;

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

        public static void ResetContext()
        {
            _playerGameObject = null;
        }

        // Additional properties for other common objects can be added here
    }
}