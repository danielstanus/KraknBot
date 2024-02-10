using BepInEx.Logging;

namespace KraknBot;

internal static class InitLog
{
    internal static void Init(ManualLogSource log)
    {
        Log.Init(new BepInExLog(log));
    }
}