using System.Collections.Generic;
using HarmonyLib;
using net.bigpoint.seafight.com.module.inventory;
using net.bigpoint.seafight.com.module.user;
using Seafight.GameActors;
using UniRx;
using UnityEngine;
using EventType = net.bigpoint.seafight.com.module.eventsystem.EventType;

namespace TestPlugin.Helpers;

public static class TargetFinder
{
    private const float SuboptimalTargetChance = 0.2f;
    private static ReactiveDictionary<EntityId, GameActorData> actors;
    private static MovementBehaviour movementBehaviour;

    public static GameObject FindNext()
    {
        actors ??= HarmonyPatches.InputController.gameActorModel.Actors;
        movementBehaviour ??= GameContext.PlayerGameObject.GetComponent<MovementBehaviour>();

        if (actors.Count == 0)
            return null;

        var possibleTargets = new List<GameObject>();
        foreach (var actor in actors)
        {
            if (actor.Value is not { GameActorType: GameActorType.Box }) continue;

            var gameObject = GetGameObject(actor.Key);
            if (gameObject != null && gameObject.activeInHierarchy)
            {
                possibleTargets.Add(gameObject);
            }
        }

        return SelectTarget(possibleTargets);
    }

    private static GameObject GetGameObject(EntityId entityId)
    {
        var entity = HarmonyPatches.InputController.mapView.GetEntity(entityId);

        return entity != null ? entity.gameObject : null;
    }

    private static GameObject SelectTarget(List<GameObject> possibleTargets)
    {
        var findSuboptimal = Random.value < SuboptimalTargetChance;
        return findSuboptimal
            ? FindSuboptimalTarget(possibleTargets)
            : FindNearestTarget(possibleTargets);
    }

    private static GameObject FindNearestTarget(List<GameObject> targets)
    {
        GameObject nearestTarget = null;
        float minDistance = float.MaxValue;

        foreach (var target in targets)
        {
            float distance = Vector3.Distance(movementBehaviour.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }

    private static GameObject FindSuboptimalTarget(List<GameObject> targets)
    {
        GameObject nearest = FindNearestTarget(targets);
        targets.Remove(nearest);

        if (targets.Count == 0)
            return nearest;

        var randomIndex = Random.Range(0, targets.Count);
        return targets[randomIndex];
    }
}