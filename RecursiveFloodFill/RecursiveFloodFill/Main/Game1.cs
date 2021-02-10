using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RecursiveFloodFill.Globals;

namespace RecursiveFloodFill
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private ScreenManager scrnMngr;

        public Game1()
        {
            GV.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            scrnMngr = new ScreenManager();
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            Window.AllowUserResizing = false;
            GV.Graphics.IsFullScreen = false;
            GV.GameSize = new Rectangle(0, 0, 1280, 720);
            //GV.GameSize = new Rectangle(0, 0, 1920, 1080);
            //GV.GameSize = new Rectangle(0, 0, 1440, 900);
            GV.Graphics.PreferredBackBufferWidth = GV.GameSize.Width;
            GV.Graphics.PreferredBackBufferHeight = GV.GameSize.Height;
            GV.Graphics.ApplyChanges();

            GV.BackBuffer = new RenderTarget2D(GV.Graphics.GraphicsDevice, GV.GameSize.Width, GV.GameSize.Height, false, SurfaceFormat.Color, DepthFormat.None);
            GV.globalRandom = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GV.SpriteBatch = new SpriteBatch(GraphicsDevice);
            GV.Content = this.Content;
            GV.viewPort = new Rectangle(GV.GameSize.X, GV.GameSize.Y, GV.GameSize.Width, GV.GameSize.Height);

            //Load Fonts, Graphics and Sounds
            Fonts.Load();
            Textures.Load();

            //Add Screen to manager
            ScreenManager.AddScreen(new RecursiveFloodFill.Screens.FloodFillTest());
        }


        protected override void Update(GameTime gameTime)
        {
            GV.WindowFocused = this.IsActive;
            GV.GameTime = gameTime;

            //Update Screens
            scrnMngr.Update();

            //Update Input
            UserInput.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GV.Graphics.GraphicsDevice.SetRenderTarget(GV.BackBuffer);
            GV.Graphics.GraphicsDevice.Clear(Color.Black);

            //Draw contents of Screen Manager
            scrnMngr.Draw();

            GV.Graphics.GraphicsDevice.SetRenderTarget(null);

            GV.SpriteBatch.Begin();
            GV.SpriteBatch.Draw(GV.BackBuffer, GV.viewPort, Color.White);
            GV.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
