using ImGuiNET;
using ImGuiInjection = DearImGuiInjection.DearImGuiInjection;

namespace TestPlugin.UI;

public class PluginUI
{
    private static PluginUI _instance;
    private byte[] bufferInputText = new byte[40];
    private static PluginUI Instance => Singleton<PluginUI>.Instance;

    public static void RenderUI()
    {
        Instance.InternalRenderUI();
    }

    private void InternalRenderUI()
    {
        if (ImGuiInjection.IsCursorVisible)
        {
            var dummy2 = true;
            if (ImGui.Begin("TestPlugin UI", ref dummy2, (int)ImGuiWindowFlags.None))
            {
                ImGui.Text("Bot Options");

                if (ImGui.InputText("Input Text", bufferInputText, (uint)bufferInputText.Length))
                {
                    // Handle text input
                }

                if (ImGui.Button("Collect Boxes"))
                {
                    // Button logic
                    LogWindow.AddLogMessage("Collect Boxes button clicked");

                    // Interacting with the unity api must be done from the unity main thread
                    // Can just use the dispatcher shipped with the library for that
                    DearImGuiInjection.BepInEx.UnityMainThreadDispatcher.Enqueue(() =>
                    {
                    });
                }
            }

            ImGui.End();
        }
    }
}