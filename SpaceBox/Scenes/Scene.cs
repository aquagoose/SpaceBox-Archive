using System;

namespace Spacebox.Scenes
{
    public abstract class Scene : IDisposable
    {
        protected SpaceboxGame Game { get; }
        
        public Scene(SpaceboxGame game)
        {
            Game = game;
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void Unload()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            
        }
        
        public void Dispose()
        {
            Unload();
            GC.SuppressFinalize(this);
        }
    }
}