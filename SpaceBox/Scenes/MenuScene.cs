using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Cubic.GUI;
using Cubic.Utilities;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace Spacebox.Scenes
{
    public class MenuScene : Scene
    {
        public MenuScene(SpaceboxGame game) : base(game) { }

        private string[] _resolutions;
        private int _currentRes;

        public override void Initialize()
        {
            base.Initialize();
            
            GL.ClearColor(Color.CornflowerBlue);
            
            Game.UiManager.Add("yeet", new Button(Game.UiManager, new Position(100, 100), new Size(200, 200)));
            Game.UiManager.Add("yeete", new Button(Game.UiManager, new Position(150, 150), new Size(200, 200)));

            unsafe
            {
                VideoMode[] videoModes = GLFW.GetVideoModes(GLFW.GetPrimaryMonitor());
                List<string> resolutionStrings = new List<string>();
                Vector2i currentRes = Vector2i.Zero;
                foreach (VideoMode mode in videoModes)
                {
                    if (mode.Width == currentRes.X && mode.Height == currentRes.Y)
                        continue;
                    resolutionStrings.Add($"{mode.Width}x{mode.Height}");
                    currentRes = new Vector2i(mode.Width, mode.Height);
                }

                _resolutions = resolutionStrings.ToArray();
            }
        }

        public override void Update()
        {
            base.Update();

            ImGui.StyleColorsLight();
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 0);
            ImGui.PushFont(Game.Arial);
            
            ImGui.SetNextWindowSize(new Vector2(500, 500).ToSystemNumericsVector2());
            ImGui.SetNextWindowPos(new Vector2(Game.SpriteBatch.Width / 2f - 250, Game.SpriteBatch.Height / 2f - 250).ToSystemNumericsVector2());
            if (ImGui.Begin("Settings", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
            {
                ImGui.BeginTabBar("settingsTab");

                if (ImGui.BeginTabItem("Game"))
                {
                    ImGui.Text("Nothing to see here...");

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Graphics"))
                {
                    ImGui.Text("Display");
                    ImGui.Combo("Resolution", ref _currentRes, _resolutions, _resolutions.Length);
                    
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();

                ImGui.End();
            }
            
            ImGui.PopStyleVar(7);
            ImGui.PopFont();
            ImGui.StyleColorsDark();
        }
    }
}