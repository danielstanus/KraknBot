using System.Numerics;
using ImGuiNET;

namespace KraknBot.UI;

public static class ImGuiStyleManager
{
    public static void ApplyDefaultStyle()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        style.WindowPadding = new Vector2(15, 15);
        style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.24f, 0.23f, 0.29f, 1.00f);

        // Set background to almost black
        style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.05f, 0.05f, 0.05f, 0.8f);

        // Make buttons very dark red
        style.Colors[(int)ImGuiCol.Button] = new Vector4(0.20f, 0.02f, 0.02f, 1.00f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.25f, 0.03f, 0.03f, 1.00f);
        style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.30f, 0.04f, 0.04f, 1.00f);

        // Make buttons not rounded
        style.FrameRounding = 0.0f;

        style.AntiAliasedLines = true;
        style.AntiAliasedFill = true;
    }
}