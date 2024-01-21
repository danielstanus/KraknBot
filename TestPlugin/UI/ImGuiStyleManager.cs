using System.Numerics;
using ImGuiNET;

namespace TestPlugin.UI;

public static class ImGuiStyleManager
{
    public static void ApplyDefaultStyle()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        ImGui.GetStyle().WindowPadding = new Vector2(15, 15);
        style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.24f, 0.23f, 0.29f, 1.00f);
    }
}