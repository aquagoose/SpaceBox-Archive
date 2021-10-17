using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubic.Engine
{
    public class InputAction
    {
        public Keys[] ActionKeys { get; set; }
        public MouseButton[] ActionButtons { get; set; }

        public InputAction(Keys actionKey)
        {
            ActionKeys = new Keys[] { actionKey };
            ActionButtons = Array.Empty<MouseButton>();
        }

        public InputAction(MouseButton actionButton)
        {
            ActionButtons = new MouseButton[] { actionButton };
            ActionKeys = Array.Empty<Keys>();
        }

        public InputAction(IEnumerable<Keys> actionKeys)
        {
            ActionKeys = actionKeys.ToArray();
            ActionButtons = Array.Empty<MouseButton>();
        }

        public InputAction(IEnumerable<MouseButton> actionButtons)
        {
            ActionButtons = actionButtons.ToArray();
            ActionKeys = Array.Empty<Keys>();
        }

        public InputAction(IEnumerable<Keys> actionKeys, IEnumerable<MouseButton> actionButtons)
        {
            ActionKeys = actionKeys.ToArray();
            ActionButtons = actionButtons.ToArray();
        }
    }
}