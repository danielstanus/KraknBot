using System;
using BepInEx.Unity.IL2CPP.UnityEngine;
using UnityEngine;
using Input = BepInEx.Unity.IL2CPP.UnityEngine.Input;

namespace TestPlugin;

public class BotBehaviour(IntPtr ptr) : MonoBehaviour(ptr)
{
    private readonly Collector _collector = new();

    private void Update()
    {
        _collector.Update();
    }

    public void StartCollector()
    {
        _collector.Start();
    }

    public void StopCollector()
    {
        _collector.Stop();
    }
}