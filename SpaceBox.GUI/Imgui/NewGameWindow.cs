using System;
using System.Numerics;
using ImGuiNET;

namespace SpaceBox.GUI.Imgui
{
    public class NewGameWindow
    {
        public bool ShouldShow;
        private bool _buttonPressed;

        private string _worldName = "";
        public string WorldName => _worldName;
        
        public NewGameWindow()
        {
            
        }

        public bool Display()
        {
            _buttonPressed = false;
            ImGuiConfig.SetImGuiConfig();
            
            ImGui.SetNextWindowSize(new Vector2(500, 500));
            ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - new Vector2(250, 250));
            if (ImGui.Begin("New Game", ref ShouldShow,
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove))
            {
                ImGui.InputText("World name", ref _worldName, 100);
                
                ImGui.SetCursorPosY(460);
                ImGui.Separator();
                if (ImGui.Button("Create"))
                    _buttonPressed = true;
                
                ImGui.End();
            }
            
            ImGuiConfig.ResetImGuiConfig();

            return _buttonPressed;
        }
    }
}