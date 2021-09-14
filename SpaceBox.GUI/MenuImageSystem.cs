using System;
using System.Collections.Generic;
using System.Drawing;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Mathematics;

namespace SpaceBox.GUI
{
    public class MenuImageSystem : UIElement
    {
        private List<MenuImage> _images;

        private MenuImage _currentMenuImage;
        private MenuImage _nextMenuImage;

        public MenuImageSystem(UIManager manager, IEnumerable<MenuImage> images, Color color) :
            base(manager, new Position(0, 0), new Size(0, 0), color)
        {
            _images = new List<MenuImage>(images);
            _currentMenuImage = _images[0];
            _currentMenuImage.Start();
        }

        protected override void Update(ref bool mouseTaken)
        {
            base.Update(ref mouseTaken);
            
            _currentMenuImage.Update();
        }

        protected override void Draw()
        {
            SpriteBatch.Draw(_currentMenuImage.Texture, _currentMenuImage.Position.ScreenPosition, Color.White, 0,
                _currentMenuImage.Origin, new Vector2(_currentMenuImage.Scale));
        }
    }

    public class MenuImage
    {
        public Texture2D Texture;
        public Position Position;
        public Vector2 Origin;

        public float InitialScale;
        public float FinalScale;

        internal float Scale { get; private set; }

        private const int SecondsToDisplayFor = 5;
        private float _startSeconds;

        public MenuImage(Texture2D texture, Position position, float initialScale, float finalScale)
        {
            Texture = texture;
            Position = position;
            InitialScale = initialScale;
            FinalScale = finalScale;
        }

        internal void Start()
        {
            _startSeconds = Time.ElapsedSeconds;
        }
        
        internal void Update()
        {
            Scale = Utils.Lerp(InitialScale, FinalScale, (Time.ElapsedSeconds - _startSeconds) / SecondsToDisplayFor);
        }
    }
}