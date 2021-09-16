using ImGuiNET;
using IG = ImGuiNET.ImGui;

namespace Cubic.GUI.ImGui
{
    public static class ImGuiConfig
    {
        public static void SetImGuiConfig()
        {
           IG.StyleColorsLight();
           IG.PushStyleVar(ImGuiStyleVar.ChildRounding, 0);
           IG.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
           IG.PushStyleVar(ImGuiStyleVar.GrabRounding, 0);
           IG.PushStyleVar(ImGuiStyleVar.PopupRounding, 0);
           IG.PushStyleVar(ImGuiStyleVar.ScrollbarRounding, 0);
           IG.PushStyleVar(ImGuiStyleVar.TabRounding, 0);
           IG.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        }

        public static void ResetImGuiConfig()
        {
            IG.StyleColorsDark();
            IG.PopStyleVar(7);
        }
    }
}