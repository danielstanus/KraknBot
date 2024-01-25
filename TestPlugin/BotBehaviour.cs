using System;
using BepInEx.Unity.IL2CPP.UnityEngine;
using UnityEngine;
using Input = BepInEx.Unity.IL2CPP.UnityEngine.Input;

namespace TestPlugin;

public class BotBehaviour : MonoBehaviour
{
    private Collector _collector;

    public BotBehaviour(IntPtr ptr) : base(ptr)
    {
        _collector = new Collector();
    }

    private void Update()
    {
        // UpdateMethod();
        _collector.Update();
    }

    private static void UpdateMethod()
    {
        if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.F6))
        {
            Screen.SetResolution(1280, 720, false);
        }

        if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.F7))
        {
            Screen.SetResolution(1920, 1080, false);
        }
    }
}