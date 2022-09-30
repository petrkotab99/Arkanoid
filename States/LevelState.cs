using ECSL;
using ECSL.Systems;

using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Arkanoid.States
{
    /// <summary>
    /// Represent a level state.
    /// (Main game state where game takes place.)
    /// </summary>
    class LevelState : GameState
    {

        private readonly ArkanoidGame game;

        public BackGroundFactory BackGroundFactory { get; private set; }

        public SystemFactory SystemFactory { get; private set; }

        public MainFactory MainFactory { get; private set; }

        public LevelState(ArkanoidGame game, GraphicsDeviceManager graphics)
            : base(graphics)
        {
            this.game = game;
        }

        public override void Initialize()
        {
            systems = new HashSet<ECSL.System>()
            {
                new EffectUpdaterSystem(pool),
                new KeyboardHandlerSystem(pool),
                new MovementSystem(pool),
                new CollisionSystem(pool),
            };
        }

        public override void LoadContent(ContentManager manager)
        {
            var spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            renderSystem = new RenderSystem(pool, spriteBatch);
            textRenderSystem = new TextRenderSystem(pool, spriteBatch);

            BackGroundFactory = new BackGroundFactory(pool, manager);
            BackGroundFactory.GenerateBackground();
            SystemFactory = new SystemFactory(pool, graphics, game, this);
            MainFactory = new MainFactory(pool, manager, game);
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            foreach (var system in systems)
            {
                system.Update(gameTime);
            }
            pool.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            renderSystem.Update(gameTime);
            textRenderSystem.Update(gameTime);
        }

    }
}
