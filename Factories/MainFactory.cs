using ECSL;
using ECSL.Components;

using System;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Arkanoid.Components;

namespace Arkanoid
{
    /// <summary>
    /// Represent a factory for creating main game entites.
    /// </summary>
    class MainFactory
    {
        /// <summary>
        /// LOcal entity pool
        /// </summary>
        private EntityPool pool;
        /// <summary>
        /// Content manager
        /// </summary>
        private ContentManager manager;

        private readonly ArkanoidGame game;

        /// <summary>
        /// Initialize new main factory
        /// </summary>
        /// <param name="pool">Local entity pool</param>
        /// <param name="manager">Content manager</param>
        public MainFactory(EntityPool pool, ContentManager manager, ArkanoidGame game)
        {
            this.pool = pool;
            this.manager = manager;
            this.game = game;
            CreateBorders();
            CreateDeskEntity();
            CreateBallEntity();
            CreateScoreEntity();
            CreateLivesEntity();
            LoadLevel("Levels/lvl1.level");
        }

        #region entites

        /// <summary>
        /// Create desk entity
        /// </summary>
        /// <returns></returns>
        private Entity CreateDeskEntity()
        {
            Single speed = 25f;
            String id = "desk";

            Entity entity = new Entity(pool, id);
            #region components
            Texture2D texture = manager.Load<Texture2D>("Textures/Desk");
            Apperance apperance = new Apperance(texture)
            {
                Position = new RefVector2(1920 / 2f, 980),
                Scale = new RefVector2(1, 0.6f),
                Origin = new RefVector2(texture.Width / 2f, texture.Height / 2f),
            };

            Movable movement = new Movable(0f, 0f);

            Physics physics = new Physics(apperance.Position)
            {
                CollisionData = apperance.GetCollisionData(),
            };
            physics.CollisionResponds.Add(typeof(Border), e =>
            {
                movement.Speed = 0f;
            });

            Controllable controllable = new Controllable();
            controllable.PressedResponds.Add("moveright", new InputRespond(Keys.D, () =>
            {
                movement.Speed = speed;
                movement.Direction = 0f;
            }
            , true));
            controllable.PressedResponds.Add("moveleft", new InputRespond(Keys.A, () =>
            {
                movement.Speed = speed;
                movement.Direction = MathHelper.Pi;
            }
            , true));
            controllable.ReleasedResponds.Add("stopmoveright", new InputRespond(Keys.D, () =>
            {
                if (movement.Direction == 0f)
                    movement.Speed = 0f;
            }, true));
            controllable.ReleasedResponds.Add("stopmoveleft", new InputRespond(Keys.A, () => 
            {
                if (movement.Direction == MathHelper.Pi)
                    movement.Speed = 0f;
            }, true));
            #endregion
            entity.AddComponents(physics, apperance, movement, controllable);
            return entity;
        }


        private Entity CreateBrickEntity(String id, Single x, Single y,
            Single width, Single Height, Color color)
        {
            Entity entity = new Entity(pool, id);
            #region components
            Texture2D texture = manager.Load<Texture2D>("Textures/Brick");
            Apperance apperance = new Apperance(texture)
            {
                Color = color,
                Position = new RefVector2(x, y),
                Scale = new RefVector2(width / texture.Width, Height / texture.Height),
            };

            Physics physics = new Physics(apperance.Position)
            {
                CollisionData = apperance.GetCollisionData(),
            };

            Brick brick = new Brick();
            #endregion
            entity.AddComponents(physics, apperance, brick);
            return entity;
        }

        private Entity CreateBallEntity()
        {
            Single speed = 18f;
            String id = "ball";

            Entity entity = new Entity(pool, id);
            #region components
            Texture2D texture = manager.Load<Texture2D>("Textures/Ball");

            Apperance apperance = new Apperance(texture)
            {
                Position = new RefVector2(1920 / 2f, 979 - texture.Height),
                Origin = new RefVector2(texture.Width / 2f, texture.Height / 2f),
            };

            Movable movement = new Movable(MathHelper.Pi / 4f, speed);

            Physics physics = new Physics(apperance.Position)
            {
                CollisionData = apperance.GetCollisionData(),
            };
            physics.CollisionResponds.Add(typeof(Border), e =>
            {
                var borderPhysics = e.Entity.GetComponent<Physics>();

                if (!(borderPhysics.CollisionData.Data.GetLength(1) > 1000))
                    movement.Direction = 2 * MathHelper.Pi - movement.Direction;
                else
                    movement.Direction = MathHelper.Pi - movement.Direction;
            });
            physics.CollisionResponds.Add(typeof(Controllable), e =>
            {
                var deskPhysics = e.Entity.GetComponent<Physics>();

                Single part = MathHelper.Pi / 2f
                    / deskPhysics.CollisionData.Data.GetLength(0);
                movement.Direction = (deskPhysics.CollisionData.Data.GetLength(0)
                    - e.Vector.X) * part + MathHelper.Pi / 4f;
            });
            physics.CollisionResponds.Add(typeof(Brick), e =>
            {
                var brickPhysics = e.Entity.GetComponent<Physics>();
                var brickApperance = e.Entity.GetComponent<Apperance>();

                if (physics.Position.X < brickPhysics.Position.X || physics.Position.X > brickPhysics.Position.X + brickApperance.Texture.Width * brickApperance.Scale.X)
                    movement.Direction = MathHelper.Pi - movement.Direction;
                else
                    movement.Direction = 2 * MathHelper.Pi - movement.Direction;

                pool.RemoveEntity(e.Entity.ID);

                var scoreText = pool.Entities["score"].GetComponent<Text>();

                Int32 score = Int32.Parse(scoreText.Content);
                score++;
                scoreText.Content = score.ToString().PadLeft(4 ,'0');

                if (pool.Entities.Keys.Where(c => c.Contains("brick")).Count() == 1)
                {
                    System.Windows.Forms.MessageBox.Show("***** You Won! *****");
                    game.Exit();
                }
            });
            physics.CollisionResponds.Add(typeof(DownBorder), e =>
            {
                var lives = pool.Entities["lives"];
                lives.GetComponent<HP>().Points--;
                if (lives.GetComponent<HP>().Points == 0)
                {
                    System.Windows.Forms.MessageBox.Show("***** Game over! *****" + Environment.NewLine + "score: " + Int32.Parse(pool.Entities["score"].GetComponent<Text>().Content));
                    game.Exit();
                }
                var livesApperance = lives.GetComponent<Apperance>();
                livesApperance.Source =
                new Rectangle(0, 0, 77 * lives.GetComponent<HP>().Points, 55);

                var ball = pool.Entities["ball"];

                var ballMovable = ball.GetComponent<Movable>();
                var ballPhysics = ball.GetComponent<Physics>();
                var ballApperance = ball.GetComponent<Apperance>();

                ballPhysics.Position.X = 1920 / 2f;
                ballPhysics.Position.Y = 1009 - ballApperance.Texture.Height;
                ballMovable.Direction = MathHelper.Pi / 4f;
            });

            #endregion
            entity.AddComponents(physics, apperance, movement);
            return entity;
        }

        private Entity CreateBorderEntity(String id, Rectangle rectangle)
        {
            Entity entity = new Entity(pool, id);
            #region components
            Boolean[,] data = new Boolean[rectangle.Width, rectangle.Height];
            for (Int32 i = 0; i < data.GetLength(0); i++)
            {
                for (Int32 j = 0; j < data.GetLength(1); j++)
                {
                    data[i, j] = true;
                }
            }
            Physics physics = new Physics(rectangle.X, rectangle.Y)
            {
                CollisionData = new CollisionData(Matrix.Identity, data),
            };

            Border border = new Border();
            #endregion
            entity.AddComponents(physics, border);
            return entity;
        }

        private Entity CreateScoreEntity()
        {
            String id = "score";

            Entity entity = new Entity(pool, id);
            #region components
            Text text = new Text("0000", manager.Load<SpriteFont>("Fonts/Rectangular"))
            {
                Color = Color.Yellow,
                Position = new Vector2(10, 1005),
            };
            #endregion
            entity.AddComponents(text);
            return entity;
        }

        private Entity CreateLivesEntity()
        {
            String id = "lives";

            Entity entity = new Entity(pool, id);
            #region components
            Texture2D texture = manager.Load<Texture2D>("Textures/Lives");
            Apperance apperance = new Apperance(texture)
            {
                Position = new RefVector2(1670, 1020),
                //Source = new Rectangle(0,0,texture.Width, texture.Height),
            };
            HP hp = new HP(3);
            #endregion
            entity.AddComponents(apperance, hp);
            return entity;
        }

        private Entity CreateDownBorderEntity()
        {
            String id = "downborder";

            Entity entity = new Entity(pool, id);
            #region components
            Boolean[,] data = new Boolean[2020, 50];
            for (Int32 i = 0; i < data.GetLength(0); i++)
            {
                for (Int32 j = 0; j < data.GetLength(1); j++)
                {
                    data[i, j] = true;
                }
            }
            Physics physics = new Physics(-50, 1130)
            {
                CollisionData = new CollisionData(Matrix.Identity, data),
            };

            DownBorder downBorder = new DownBorder();
            #endregion
            entity.AddComponents(physics, downBorder);
            return entity;
        }

        #endregion

        private void CreateBricksFromString(String source)
        {
            String[] lines = source.Split(',');

            Int32 x = lines.First().Length;
            Int32 y = lines.Length;

            Single width = 1920f / x;
            Single height = 50f;

            for (Int32 i = 0; i < y; i++)
            {
                for (Int32 j = 0; j < x; j++)
                {
                    if (lines[i][j] == '#')
                    {
                        CreateBrickEntity($"brick {i}|{j}", j * width, i * height,
                            width, height, i % 2 == 0 ? Color.Red : Color.Blue);
                    }
                }
            }
        }

        private void CreateBorders()
        {
            CreateBorderEntity("leftborder", new Rectangle(-50, -50, 50, 1180));
            CreateBorderEntity("rightborder", new Rectangle(1920, -50, 50, 1180));
            CreateBorderEntity("topborder", new Rectangle(-50, -50, 2020, 50));
            CreateDownBorderEntity();
        }

        public void LoadLevel(String path)
        {
            String source = "";

            using (StreamReader reader = new StreamReader(path))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    source += line;
                }
            }

            CreateBricksFromString(source);
        }

    }
}
