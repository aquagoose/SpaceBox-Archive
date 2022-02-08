using System.Drawing;
using System.Numerics;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using SpaceBox.GUI.Imgui;

namespace Spacebox.Game.Scenes;

public class WelcomeScene : Scene
{
    public override void Initialize(SpaceboxGame game)
    {
        base.Initialize(game);
        
        GL.ClearColor(Color.Black);
    }

    public override void Draw(SpaceboxGame game)
    {
        base.Draw(game);
        
        ImGuiConfig.SetImGuiConfig();
            
        ImGui.SetNextWindowSize(new Vector2(850, 700));
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - new Vector2(425, 350));
        if (ImGui.Begin("Welcome to SpaceBox!", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove))
        {
            ImGui.Text(@"Hi! Welcome to SpaceBox!

SpaceBox is an ambitious project of mine. It's a voxel building game, based on Space Engineers,
developed by Keen Software House.
It's a game I've sunk many many (too many) hours into, and it became my goal to try and create
a game like Space Engineers.

This certainly isn't it. It works, but it's rough. You can place and delete blocks, and that's about it. 
The physics are broken, and there are lots and lots of bugs (and I even tried to fix a couple before 
releasing on GitHub!). The codebase is terrible, and the 'game engine' is awful.

Even so, I put a lot of effort into this project, and I think it is worth sharing the game & codebase on 
GitHub for all to see.

It was incredibly fun to develop, and I learned a lot from developing it.
You probably won't find it much fun, but I hope you can appreciate it nonetheless! You might even
find a few bugs, some of which are (somewhat) hilarious.

So, thanks for downloading SpaceBox, and I hope you 'enjoy'!
- ohtrobinson");
            
            ImGui.SetCursorPosY(660);
            ImGui.Separator();
            if (ImGui.Button("Continue"))
                SceneManager.SetScene(new IntroScene());
                
            ImGui.End();
        }
    }
}