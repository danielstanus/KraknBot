using Seafight.Utilities;
using TestPlugin.Helpers;
using UnityEngine;

namespace TestPlugin.Map;

public static class MapController
{
    public static void CreateBlockedCoordsAroundEntity(GameObject entity, int blockRange)
    {
        Vector2Int entityMapField = MapUtils.GetMapField(entity.transform.position);
        Vector2Int blockedPosition = new Vector2Int();

        for (int x = -blockRange; x <= blockRange; x++)
        {
            for (int y = -blockRange; y <= blockRange; y++)
            {
                blockedPosition.Set(entityMapField.x + x, entityMapField.y + y);

                if (!HarmonyPatches.InputController.mapView.isCoordBlocked(blockedPosition))
                {
                    HarmonyPatches.InputController.mapView.currentMapBlockedCoords[blockedPosition] = true;
                    if (TestPlugin.Debug)
                    {
                        HarmonyPatches.InputController.mapView.ShowBlockedArea(blockedPosition);
                    }
                }
            }
        }
    }
}