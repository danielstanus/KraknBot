using ImGuiNET;
using ImGuiInjection = DearImGuiInjection.DearImGuiInjection;

namespace TestPlugin.UI;

public class PluginUI
{
    private byte[] bufferInputText = new byte[40];
    private static PluginUI Instance => Singleton<PluginUI>.Instance;
    public static BotBehaviour BotBehaviourInstance { get; set; }

    private bool isBotRunning = false;
    private bool collectBoxes = false;
    private bool shootNPC = false;

    public static void RenderUI()
    {
        Instance.InternalRenderUI();
    }

    private void InternalRenderUI()
    {
        if (ImGuiInjection.IsCursorVisible)
        {
            var windowOpen = true;
            if (ImGui.Begin("TestPlugin UI", ref windowOpen, (int)ImGuiWindowFlags.None))
            {
                ImGui.Text("Bot Options");

                // Toggle button for starting/stopping the bot
                if (ImGui.Button(isBotRunning ? "Stop Bot" : "Start Bot"))
                {
                    isBotRunning = !isBotRunning;
                    LogWindow.AddLogMessage(isBotRunning ? "Bot started" : "Bot stopped");

                    if (isBotRunning)
                    {
                        BotBehaviourInstance.StartCollector(); // Assuming this starts the bot
                        Log.Info("Bot started");
                    }
                    else
                    {
                        BotBehaviourInstance.StopCollector(); // Assuming this stops the bot
                        Log.Info("Bot stopped");
                    }
                }

                if (isBotRunning)
                {
                    ImGui.BeginDisabled();
                }

                if (ImGui.Checkbox("Collect Boxes", ref collectBoxes))
                {
                    LogWindow.AddLogMessage(collectBoxes ? "Collect Boxes enabled" : "Collect Boxes disabled");
                    // Additional logic for collecting boxes can be added here
                }

                if (ImGui.Checkbox("Shoot NPC", ref shootNPC))
                {
                    LogWindow.AddLogMessage(shootNPC ? "Shoot NPC enabled" : "Shoot NPC disabled");
                    // Additional logic for shooting NPCs can be added here
                }

                if (isBotRunning)
                {
                    ImGui.EndDisabled();
                }
            }
            ImGui.End();
        }
    }
}
