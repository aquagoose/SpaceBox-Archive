using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;

namespace SpaceBox.GUI.Imgui
{
    public class LoadGameWindow
    {
        private string[] _worlds;

        public string[] WorldFiles;
        
        public bool ShouldShow;

        public int SelectedWorld;

        private bool _buttonPressed;
        
        public LoadGameWindow()
        {
            string directory = Path.Combine(Data.Data.SpaceBoxFolderLocation, Data.Data.SpaceBoxFolderName,
                Data.Data.SavesFolderName);

            if (!Directory.Exists(directory))
                return;

            WorldFiles = Directory.GetFiles(directory, "*.world");
            
            List<string> worlds = new List<string>();
            foreach (string world in WorldFiles)
                worlds.Add(world.Split('\\')[^1].Replace(".world", "").Replace('_', ' '));
            _worlds = worlds.ToArray();
        }
        
        public bool Display()
        {
            _buttonPressed = false;
            ImGuiConfig.SetImGuiConfig();
            
            ImGui.SetNextWindowSize(new Vector2(500, 500));
            ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - new Vector2(250, 250));
            if (ImGui.Begin("Load Game", ref ShouldShow,
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove))
            {
                ImGui.ListBox("", ref SelectedWorld, _worlds, _worlds.Length, 10);
                
                ImGui.SetCursorPosY(460);
                ImGui.Separator();
                if (ImGui.Button("Load"))
                    _buttonPressed = true;
                
                ImGui.End();
            }
            
            ImGuiConfig.ResetImGuiConfig();

            return _buttonPressed;
        }
    }
}