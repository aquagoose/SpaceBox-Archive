using System;
using Cubic.Engine.GUI;
using Cubic.Engine.Render;

namespace Spacebox.Game.Scenes
{
    public abstract class Scene : IDisposable
    {
        //protected SpaceboxGame Game { get; }

        protected internal SpriteBatch SpriteBatch { get; internal set; }
        protected internal UIManager UiManager { get; internal set; }

        public virtual void Initialize(SpaceboxGame game)
        {
            
        }

        public virtual void Unload()
        {
            
        }

        public virtual void Update(SpaceboxGame game)
        {
            
        }

        public virtual void Draw(SpaceboxGame game)
        {
            
        }
        
        public void Dispose()
        {
            Unload();
            GC.SuppressFinalize(this);
            Console.WriteLine("Scene disposed.");
        }
    }
}