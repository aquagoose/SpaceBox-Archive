using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cubic.Render;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Cubic.GUI
{
    /// <summary>
    /// A helper class for managing and rendering UI.
    /// </summary>
    public sealed class UIManager
    {
        private Dictionary<string, UIElement> _elements;
        private List<UIElement> _reversedUiElements;

        /// <summary>
        /// A reference resolution, used for scaling the UI.
        /// </summary>
        public Size ReferenceResolution { get; set; } = new Size(1280, 720);
        public UITheme Theme { get; set; }

        public SpriteBatch SpriteBatch;
        
        public UIManager(SpriteBatch batch)
        {
            _elements = new Dictionary<string, UIElement>();
            _reversedUiElements = new List<UIElement>();

            Theme = new UITheme();
            SpriteBatch = batch;
        }

        public void Add(string name, UIElement element)
        {
            if (_elements.ContainsKey(name))
                throw new Exception("The given UI element has already been added. Please choose a different name.");
            _elements.Add(name, element);
            ReverseUiELements();
        }

        public void Clear()
        {
            _elements.Clear();
            _reversedUiElements.Clear();
        }

        private void ReverseUiELements()
        {
            _reversedUiElements.Clear();
            for (int i = _elements.Count - 1; i > -1; i--)
                _reversedUiElements.Add(_elements.ElementAt(i).Value);
        }

        public T GetElement<T>(string name) where T : UIElement
        {
            return (T) _elements[name];
        }

        public void Update()
        {
            bool mouseTaken = false;
            foreach (UIElement element in _reversedUiElements)
                element.Update(ref mouseTaken);
        }

        public void Draw(Matrix4 transform = default, bool begun = false)
        {
            if (!begun)
                SpriteBatch.Begin();

            foreach (KeyValuePair<string, UIElement> element in _elements)
            {
                if (element.Value.Visible)
                    element.Value.Draw();
            }

            if (!begun)
                SpriteBatch.End();
        }
    }
}