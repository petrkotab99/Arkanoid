using ECSL;
using ECSL.Components;

using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Arkanoid.States;

namespace Arkanoid
{
    /// <summary>
    /// Represent a factory for creating system entites.
    /// </summary>
    class SystemFactory
    {
        /// <summary>
        /// Local entity pool
        /// </summary>
        private EntityPool pool;
        /// <summary>
        /// Graphics device manager
        /// </summary>
        private GraphicsDeviceManager graphics;
        /// <summary>
        /// Game instance
        /// </summary>
        private ArkanoidGame game;
        /// <summary>
        /// Level state instance
        /// </summary>
        private LevelState level;

        /// <summary>
        /// Initialize new system factory
        /// </summary>
        /// <param name="pool">Local entity pool</param>
        /// <param name="graphics">Graphics device manager</param>
        /// <param name="game">Game instance</param>
        /// <param name="level">Level state instance</param>
        public SystemFactory(EntityPool pool, GraphicsDeviceManager graphics, ArkanoidGame game, LevelState level)
        {
            this.pool = pool;
            this.graphics = graphics;
            this.game = game;
            this.level = level;
            CreateKeysEntity();
        }

        public Entity CreateKeysEntity()
        {
            Entity entity = new Entity(pool, "keys");
            Controllable controllable = new Controllable();

            controllable.PressedResponds.Add("fullscreen", new InputRespond(Keys.F11,
                () =>
                {
                    graphics.IsFullScreen = !graphics.IsFullScreen;
                    graphics.ApplyChanges();
                }
                , true));
            controllable.PressedResponds.Add("exit", new InputRespond(Keys.Escape,
                () =>
                {
                    game.Exit();
                }
                , true));
            controllable.PressedResponds.Add("changebackground", new InputRespond(Keys.C,
                () =>
                {
                    pool.RemoveEntity($"background{level.BackGroundFactory.Option}");
                    level.BackGroundFactory.Option =
                        (level.BackGroundFactory.Option == 0) ? 1 : 0;
                    level.BackGroundFactory.CreateBackgroundEntity($"background{level.BackGroundFactory.Option}");
                }
                , true));
            controllable.PressedResponds.Add("resetball", new InputRespond(Keys.R,
                () =>
                {
                    var ball = pool.Entities["ball"];
                    var ballPhysics = ball.GetComponent<Physics>();
                    var ballApperance = ball.GetComponent<Apperance>();
                    var ballMovable = ball.GetComponent<Movable>();

                    ballPhysics.Position.X = 1920 / 2f;
                    ballPhysics.Position.Y = 1009 - ballApperance.Texture.Height;
                    ballMovable.Direction = MathHelper.Pi / 4f;
                }, true));

            entity.AddComponents(controllable);
            return entity;
        }

    }
}
