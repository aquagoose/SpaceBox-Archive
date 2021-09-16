using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Cubic.Render;
using ImGuiNET;
using SpaceBox.Data;

namespace SpaceBox.GUI.Imgui
{
    public class SettingsWindow
    {
        private Dictionary<string, string> _inputs;
        private List<string> _categories;

        public bool ShouldShow;

        public SettingsWindow(SpaceboxConfig config)
        {
            _categories = new List<string>();
            foreach (PropertyInfo info in config.GetType().GetProperties())
            {
                _categories.Add(info.Name);
            }
            
            _inputs = new Dictionary<string, string>();

            foreach (PropertyInfo info in config.Input.GetType().GetProperties())
            {
                string propName = info.Name.Replace("Or", "/");
                string propFriendlyName = String.Empty;
                for (int i = 0; i < propName.Length; i++)
                {
                    char c = propName[i];
                    if (i > 0 && (c >= 65 && c <= 90 || c >= 48 && c <= 57) && propName[i - 1] != '/')
                        propFriendlyName += " " + c.ToString().ToLower();
                    else
                        propFriendlyName += c;
                }

                string valueStr = info.GetValue(config.Input)?.ToString()?.Replace("Or", "/");
                if (valueStr == "Button1")
                    valueStr = "Left";
                else if (valueStr == "Button2")
                    valueStr = "Right";
                string valueFriendly = String.Empty;
                for (int i = 0; i < valueStr?.Length; i++)
                {
                    char c = valueStr[i];
                    if (i > 0 && (c >= 65 && c <= 90 || c >= 48 && c <= 57) && valueStr[i - 1] != '/')
                        valueFriendly += " " + c.ToString().ToLower();
                    else
                        valueFriendly += c;
                }
                
                _inputs.Add(propFriendlyName, valueFriendly);
            }
        }

        public void Display()
        {
            ImGuiConfig.SetImGuiConfig();
            ImGui.SetNextWindowSize(new Vector2(500, 500));
            ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - new Vector2(250, 250));
            if (ImGui.Begin("Settings", ref ShouldShow,
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove))
            {
                ImGui.BeginTabBar("tabs");
                
                foreach (string category in _categories)
                {
                    if (ImGui.BeginTabItem(category))
                    {
                        switch (category)
                        {
                            case "Display":
                                ImGui.Text("Nothing to see here.");
                                break;
                            case "Input":
                                foreach (KeyValuePair<string, string> input in _inputs)
                                {
                                    ImGui.Text($"{input.Key} - {input.Value}");
                                }

                                break;
                        }
                        
                        ImGui.EndTabItem();
                    }
                }
                
                ImGui.EndTabBar();
            }
            ImGuiConfig.ResetImGuiConfig();
        }
    }
}