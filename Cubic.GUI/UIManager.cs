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
        
        public Vector2 UiScale { get; private set; }

        // True if the lists are being iterated through.
        private bool _isIterating;
        // True if the list should clear on the next update frame.
        private bool _shouldClear;
        
        public UIManager(SpriteBatch batch)
        {
            _elements = new Dictionary<string, UIElement>();
            _reversedUiElements = new List<UIElement>();

            Theme = new UITheme();
            SpriteBatch = batch;
            
            SpriteBatch.Resized += SpriteBatchOnResized;
        }

        private void SpriteBatchOnResized()
        {
            Size winSize = new Size(SpriteBatch.Width, SpriteBatch.Height);
            float refSize = winSize.Width > winSize.Height ? winSize.Height : winSize.Width;
            UiScale = new Vector2(refSize / (winSize.Width > winSize.Height
                ? ReferenceResolution.Height
                : ReferenceResolution.Width));
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
            if (!_isIterating)
            {
                // It's easier to iterate through the reversed elements because we don't need to worry about the stupid
                // keyvaluepair stuff.
                foreach (UIElement element in _reversedUiElements)
                    element.Dispose();
                _elements.Clear();
                _reversedUiElements.Clear();
            }
            else
                _shouldClear = true;
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
            _isIterating = true;
            
            bool mouseTaken = false;
            foreach (UIElement element in _reversedUiElements)
            {
                if (element.Visible)
                    element.Update(ref mouseTaken);
            }

            _isIterating = false;

            if (_shouldClear)
            {
                foreach (UIElement element in _reversedUiElements)
                    element.Dispose();
                _elements.Clear();
                _reversedUiElements.Clear();
            }
        }

        public void Draw(Matrix4 transform = default, bool begun = false)
        {
            if (!begun)
                SpriteBatch.Begin();

            _isIterating = true;
            
            foreach (KeyValuePair<string, UIElement> element in _elements)
            {
                if (element.Value.Visible)
                    element.Value.Draw();
            }

            _isIterating = false;

            if (!begun)
                SpriteBatch.End();
        }
    }
}