using System;
using BepInEx.Unity.IL2CPP.UnityEngine;
using UnityEngine;

namespace TestPlugin;

public class BotBehaviour : MonoBehaviour
{
    public BotBehaviour(IntPtr ptr) : base(ptr) { }

    private void Update()
    {
        // UpdateMethod();
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