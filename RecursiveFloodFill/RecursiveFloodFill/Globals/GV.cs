using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RecursiveFloodFill.Globals
{
    class GV
    {
        public static ContentManager Content;
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public static GameTime GameTime;
        public static bool WindowFocused;
        public static Rectangle GameSize;
        public static Rectangle viewPort;
        public static RenderTarget2D BackBuffer;

        public static Random globalRandom;
    }
}
