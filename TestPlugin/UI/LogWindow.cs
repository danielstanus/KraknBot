using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using TestPlugin.UI;

namespace TestPlugin;

public class LogWindow
{
    private static LogWindow _instance;
    private List<(DateTime Timestamp, string Message)> logMessages = new List<(DateTime, string)>();
    private static LogWindow Instance => Singleton<LogWindow>.Instance;

    private bool shouldAutoScroll = true;

    public static void AddLogMessage(string message)
    {
        Instance.logMessages.Add((DateTime.Now, message));

        if (Instance.logMessages.Count > 100)
        {
            Instance.logMessages.RemoveAt(0);
        }

        Instance.shouldAutoScroll = true;
    }

    public static void Render()
    {
        Instance.InternalRender();
    }

    private void InternalRender()
    {
        var logMessagesCopy = new List<(DateTime, string)>(logMessages);

        Vector2 windowPos = new Vector2(ImGui.GetIO().DisplaySize.X - 10, ImGui.GetIO().DisplaySize.Y - 10);
        Vector2 windowSize = new Vector2(400, 300);
        ImGui.SetNextWindowPos(windowPos, ImGuiCond.FirstUseEver, new Vector2(1.0f, 1.0f));
        ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);

        if (ImGui.Begin("Log Window"))
        {
            foreach (var (Timestamp, Message) in logMessagesCopy)
            {
                ImGui.TextUnformatted($"[{Timestamp:HH:mm:ss}] {Message}");
            }

            if (shouldAutoScroll)
            {
                ImGui.SetScrollHereY(1.0f);
                shouldAutoScroll = false;
            }
        }
        ImGui.End();

        ImGuiStyleManager.ApplyDefaultStyle();

    }
}