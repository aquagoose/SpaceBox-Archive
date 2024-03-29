﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Cubic.Engine;
using Cubic.Engine.Windowing;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.Data;
using Vector2 = System.Numerics.Vector2;

namespace SpaceBox.GUI.Imgui
{
    public class SettingsWindow
    {
        private SpaceboxConfig _config;
        private BaseGame _game;
        
        private Dictionary<string, string> _inputs;
        private List<string> _categories;

        private string[] _videoResolutions;
        private Size[] _resolutions;
        private int _selectedResolution;
        private int _originalResolution;
        private bool _fullscreen;
        private bool _originalFullscreen;

        public bool ShouldShow;

        public bool _showDialog;

        private float _startSeconds;

        public SettingsWindow(SpaceboxConfig config, BaseGame game)
        {
            _config = config;
            _game = game;
            _originalFullscreen = config.Display.Fullscreen;
            _fullscreen = config.Display.Fullscreen;
            
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

            unsafe
            {
                VideoMode[] modes = GLFW.GetVideoModes(GLFW.GetPrimaryMonitor());
                VideoMode* currentVideoMode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());
                List<Size> resolutions = new List<Size>();
                List<string> resolutionStr = new List<string>();
                int i = 0;
                foreach (VideoMode mode in modes)
                {
                    Size resolution = new Size(mode.Width, mode.Height);
                    if (resolutions.Contains(resolution))
                        continue;
                    resolutions.Add(resolution);
                    resolutionStr.Add(
                        $"{mode.Width}x{mode.Height}{(mode.Width == currentVideoMode->Width && mode.Height == currentVideoMode->Height ? "*" : "")}");
                    if (mode.Width == game.Size.Width && mode.Height == game.Size.Height)
                        _selectedResolution = i;
                    i++;
                }

                _originalResolution = _selectedResolution;

                _resolutions = resolutions.ToArray();
                _videoResolutions = resolutionStr.ToArray();
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
                                ImGui.Combo("Resolution", ref _selectedResolution, _videoResolutions,
                                    _videoResolutions.Length);
                                if (ImGui.IsItemHovered())
                                    ImGui.SetTooltip("Set the game's resolution. The resolution indicated with an asterisk (*) is your native resolution.");
                                ImGui.Checkbox("Fullscreen", ref _fullscreen);
                                break;
                            case "Input":
                                foreach (KeyValuePair<string, string> input in _inputs)
                                {
                                    ImGui.Text($"{input.Key} - {input.Value}");
                                }

                                break;
                        }
                        
                        ImGui.EndTabItem();
                        
                        ImGui.SetCursorPosY(460);
                        /*ImGui.Separator();
                        ImGui.Button("OK");
                        ImGui.SameLine();
                        if (ImGui.Button("Cancel"))
                        {
                            _showDialog = true;
                            ImGui.OpenPopup("Cancel?");
                        }

                        ImGui.SameLine();*/
                        if (ImGui.Button("Apply"))
                        {
                            if (_selectedResolution != _originalResolution || _fullscreen != _originalFullscreen)
                            {
                                _game.Fullscreen = _fullscreen;
                                _game.Size = _resolutions[_selectedResolution];
                                _startSeconds = Time.ElapsedSeconds;
                                _showDialog = true;
                                ImGui.OpenPopup("Keep this resolution?");
                            }
                        }
                        
                        if (ImGui.BeginPopupModal("Keep this resolution?", ref _showDialog,
                            ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
                        {
                            ImGui.Text($"Keep this resolution? It will change back in {(int)(_startSeconds - Time.ElapsedSeconds + 11)} seconds.");
                            ImGui.Separator();
                            if (ImGui.Button("Yes"))
                            {
                                _showDialog = false;
                                _originalResolution = _selectedResolution;
                                _config.Display.Resolution = _resolutions[_selectedResolution];
                                _config.Display.Fullscreen = _fullscreen;
                                _originalFullscreen = _fullscreen;
                                _originalResolution = _selectedResolution;
                                Data.Data.SaveSpaceBoxConfig(_config, "spacebox.cfg");
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("No"))
                            {
                                _game.Fullscreen = _originalFullscreen;
                                _fullscreen = _originalFullscreen;
                                _showDialog = false;
                                _game.Size = _resolutions[_originalResolution];
                                _selectedResolution = _originalResolution;
                            }

                            ImGui.End();

                            if ((int)(_startSeconds - Time.ElapsedSeconds + 11) <= 0)
                            {
                                _game.Fullscreen = _originalFullscreen;
                                _fullscreen = _originalFullscreen;
                                _game.Size = _resolutions[_originalResolution];
                                _showDialog = false;
                                _selectedResolution = _originalResolution;
                            }
                        }

                        if (ImGui.BeginPopupModal("Cancel?", ref _showDialog,
                        ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
                        {
                            ImGui.Text("Exit without changes?");
                            ImGui.Separator();
                            if (ImGui.Button("Yes"))
                            {
                                _showDialog = false;
                                ShouldShow = false;
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("No"))
                                _showDialog = false;
                            ImGui.End();
                        }
                    }
                }
                
                ImGui.EndTabBar();
            }
            ImGuiConfig.ResetImGuiConfig();
        }
    }
}