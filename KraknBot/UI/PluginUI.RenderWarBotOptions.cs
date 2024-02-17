using ImGuiNET;

namespace KraknBot.UI;

public partial class PluginUI
{
    private void RenderWarBotOptions()
    {
        if (isBotRunning) ImGui.BeginDisabled();

        // Warbot

        if (isBotRunning) ImGui.EndDisabled();
    }
}