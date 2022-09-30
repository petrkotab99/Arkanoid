using ECSL;
using ECSL.Effects;
using ECSL.Components;

using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Arkanoid
{
    /// <summary>
    /// Represent a factory for creating backgrounds.
    /// </summary>
    class BackGroundFactory
    {

        /// <summary>
        /// Local entity pool
        /// </summary>
        private readonly EntityPool pool;
        /// <summary>
        /// Content manager
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// Determine which background will be generated.
        /// Options:
        /// 0 - SHaking lines
        /// 1 - Rotationg curves
        /// </summary>
        public Int32 Option { get; set; }

        /// <summary>
        /// Initialize new background factory
        /// </summary>
        /// <param name="pool">Local entity pool</param>
        /// <param name="content">Content manager</param>
        public BackGroundFactory(EntityPool pool, ContentManager content)
        {
            this.pool = pool;
            this.content = content;
        }

        /// <summary>
        /// Create random background entity.
        /// (ID of entity will be "background".)
        /// </summary>
        public void GenerateBackground()
        {
            Option = (new Random()).Next(2);
            CreateBackgroundEntity($"background{Option}");
        }

        /// <summary>
        /// Create background entity.
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>new Background entity.</returns>
        public Entity CreateBackgroundEntity(String id)
        {
            Entity entity = new Entity(pool, id);
            Apperance apperance = null;
            Texture2D texture;
            switch (Option)
            {
                case 0:
                    texture = content.Load<Texture2D>(@"Textures\BackgroundLines");
                    apperance = new Apperance(texture)
                    {
                        Position = new RefVector2(960f, 540f),
                        Origin = new RefVector2(texture.Width / 2f, texture.Height / 2f),
                        Color = new Color(255, 124, 124),
                        LayerDepth = 1f,
                    };
                    apperance.Effects.Add("shake", new ShakeEffect(3f, -4f, 4f));
                    break;
                case 1:
                    texture = content.Load<Texture2D>(@"Textures\BackgroundRoll");
                    apperance = new Apperance(texture)
                    {
                        Position = new RefVector2(960f, 540f),
                        Origin = new RefVector2(texture.Width / 2f, texture.Height / 2f),
                        Scale = new RefVector2(1.2f, 1.2f),
                        Color = new Color(255, 124, 124),
                        LayerDepth = 1f,
                    };
                    apperance.Effects.Add("rotation", new RotationEffect(0.3f));
                    break;
                default:
                    throw new Exception("Invalid option!");
            }
            apperance.Effects.Add("transition", new ColorTransitionEffect(1, 1, 1, new Color(254, 124, 124)));
            entity.AddComponents(apperance);
            return entity;
        }

    }
}
