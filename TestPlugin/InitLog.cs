using BepInEx.Logging;

namespace TestPlugin;

internal static class InitLog
{
    internal static void Init(ManualLogSource log)
    {
        Log.Init(new BepInExLog(log));
    }
}