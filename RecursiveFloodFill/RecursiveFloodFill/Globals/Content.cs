using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RecursiveFloodFill.Globals
{
    class Fonts
    {
        public static SpriteFont SystemBold;

        public static void Load()
        {
            SystemBold = GV.Content.Load<SpriteFont>("Fonts/SystemBold");
        }

    }

    class Textures
    {
        public static Texture2D Px;

        public static void Load()
        {
            Px = GV.Content.Load<Texture2D>("Textures/pixel");
        }

    }


}
