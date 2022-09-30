using ECSL;
using ECSL.Systems;

using System;
using System.Linq;
using System.Collections.Generic;

using Arkanoid.States;

using Microsoft.Xna.Framework;


namespace Arkanoid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ArkanoidGame : Game
    {
        /// <summary>
        /// Graphics device manager.
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Contains all game states.
        /// </summary>
        public Dictionary<String, GameState> States { get; private set; }
        /// <summary>
        /// Current active game state.
        /// </summary>
        public GameState ActiveState { get; set; }

        /// <summary>
        /// Initialize new game.
        /// </summary>
        public ArkanoidGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            #region Default graphics setting
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            #endregion
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            States = new Dictionary<String, GameState>
            {
                { "levelstate", new LevelState(this, graphics) }
            };

            ActiveState = States.Values.First();

            foreach (var state in States.Values)
            {
                state.Initialize();
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (var state in States.Values)
            {
                state.LoadContent(Content);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (var state in States.Values)
            {
                state.UnloadContent();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ActiveState.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            ActiveState.Draw(gameTime);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Exit the game.
        /// </summary>
        public new void Exit()
        {
            base.Exit();
        }

    }
}
