using System;
using System.Drawing;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceBox.Data
{
    public class SpaceboxConfig
    {
        public DisplayConfig Display { get; set; }
        public InputConfig Input { get; set; }

        public SpaceboxConfig()
        {
            Display = new DisplayConfig()
            {
                Resolution = new Size(1280, 720),
                Fullscreen = false
            };

            Input = new InputConfig()
            {
                PlaceBlock = MouseButton.Left,
                DeleteBlock = MouseButton.Right,
                
                MoveForward = Keys.W,
                MoveBackward = Keys.S,
                StrafeLeft = Keys.A,
                StrafeRight = Keys.D,
                Jump = Keys.Space,
                Sprint = Keys.LeftShift,
                CrouchOrJetpackDown = Keys.C,
                JetpackToggle = Keys.V,
                SaveGame = Keys.F5,
                TakeScreenshot = Keys.F4
            };
        }
    }

    public class DisplayConfig
    {
        public Size Resolution { get; set; }
        public bool Fullscreen { get; set; }
    }

    public class InputConfig
    {
        public MouseButton PlaceBlock { get; set; }
        public MouseButton DeleteBlock { get; set; }
        
        public Keys MoveForward { get; set; }
        public Keys MoveBackward { get; set; }
        public Keys StrafeLeft { get; set; }
        public Keys StrafeRight { get; set; }
        public Keys Jump { get; set; }
        public Keys Sprint { get; set; }
        public Keys CrouchOrJetpackDown { get; set; }
        public Keys JetpackToggle { get; set; }
        public Keys SaveGame { get; set; }
        public Keys TakeScreenshot { get; set; }
    }
}