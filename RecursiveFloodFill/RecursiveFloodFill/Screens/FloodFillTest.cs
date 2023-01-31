using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RecursiveFloodFill.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RecursiveFloodFill.Screens
{
    class FloodFillTest : BaseScreen
    {
        private MapBase tileGrid;

        private bool showGridID;
        private string infoText = "";
        private int tilesFilled = 0;

        private Color unfilledColour = Color.LightSeaGreen;
        private Color filledColour = Color.DarkRed;
        private Color normalColour = Color.Gray;

        public FloodFillTest()
        {
            showGridID = false;
            GenerateMap();
            GenerateInfoText();
        }

        public override void Draw()
        {
            GV.SpriteBatch.Begin();
            GV.SpriteBatch.Draw(Textures.Px, GV.GameSize, Color.CornflowerBlue);


            for (int y = 0; y <= tileGrid.mapHeight - 1; y++)
            {
                for (int x = 0; x <= tileGrid.mapWidth - 1; x++)
                {
                    GV.SpriteBatch.Draw(Textures.Px, tileGrid.tileList[x, y].position, tileGrid.tileList[x, y].colour);

                    if (showGridID)
                        GV.SpriteBatch.DrawString(Fonts.SystemBold, tileGrid.tileList[x, y].id.ToString(), new Vector2(tileGrid.tileList[x, y].position.X, tileGrid.tileList[x, y].position.Y), Color.Black);
                }
            }

            GV.SpriteBatch.DrawString(Fonts.SystemBold, infoText, new Vector2(15, 32), Color.Black);

            GV.SpriteBatch.End();
        }

        private void GenerateInfoText()
        {
            infoText = "F2: Show/Hide ID" + System.Environment.NewLine +
                "F3: Generate New Map" + System.Environment.NewLine + System.Environment.NewLine +
                "Tiles Filled: " + tilesFilled;
        }


        private void GenerateMap()
        {
            tileGrid = new MapBase(20, 20);
            int _count = 0;

            //Create 20x20 grid
            for (int y = 0; y <= tileGrid.mapHeight - 1; y++)
            {
                for (int x = 0; x <= tileGrid.mapWidth - 1; x++)
                {
                    tileGrid.tileList[x, y].position = new Rectangle(256 + (x * 32), 32 + (y * 32), 32, 32);
                    tileGrid.tileList[x, y].id = _count;

                    if (GV.globalRandom.Next(0, 101) > 60)
                        tileGrid.tileList[x, y].colour = unfilledColour;
                    else
                        tileGrid.tileList[x, y].colour = normalColour;

                    _count += 1;
                }
            }
        }

        public override void HandleInput()
        {

            if (UserInput.KeyPressed(Keys.F2))
            {
                //Show or hide tile id
                showGridID = !showGridID;
            }

            if (UserInput.KeyPressed(Keys.F3))
            {
                //Generate New Map
                GenerateMap();
                tilesFilled = 0;
                GenerateInfoText();
            }

            if (UserInput.LeftMouseClick())
            {
                Point _mousePos = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                //Check Position of mouse and try to do a flood fill
                for (int y = 0; y <= tileGrid.mapHeight - 1; y++)
                {
                    for (int x = 0; x <= tileGrid.mapWidth - 1; x++)
                    {
                        if (tileGrid.tileList[x, y].position.Contains(_mousePos))
                        {
                            //If tile is on the grid perform Floodfill
                           tilesFilled += FloodFill(new Point(x, y));
                           GenerateInfoText();
                            break;
                        }

                    }
                }

            }
        }

        private int FloodFill(Point _gridPosition)
        {
            //Check to see if tile is unfilled and is valid
            if (ValidateTile(_gridPosition) && tileGrid.tileList[_gridPosition.X, _gridPosition.Y].colour == unfilledColour)
            {
                //Change current tile to filled
                tileGrid.tileList[_gridPosition.X, _gridPosition.Y].colour = filledColour;

                //Check the tiles up, down, left and right
                int _up = FloodFill(new Point(_gridPosition.X, _gridPosition.Y - 1)); //Up
                int _down = FloodFill(new Point(_gridPosition.X, _gridPosition.Y + 1)); //Down
                int _left = FloodFill(new Point(_gridPosition.X - 1, _gridPosition.Y)); //Left
                int _right = FloodFill(new Point(_gridPosition.X + 1, _gridPosition.Y)); //Right

                //Update filled tiles counter
                return _up + _down + _left + _right + 1;
            }

            return 0;
        }

        private bool ValidateTile(Point _gridPosition)
        {
            //Check that tile is not out of bounds

            if (_gridPosition.X >= 0 && _gridPosition.X < 20 & _gridPosition.Y >= 0 && _gridPosition.Y < 20)
                return true;
            return false;
        }



    }

    class MapBase
    {
        public TileProperties[,] tileList;
        public int mapWidth = 0;
        public int mapHeight = 0;

        public MapBase(int w, int h)
        {
            tileList = new TileProperties[w, h];
            mapWidth = w;
            mapHeight = h;
        }
    }

    struct TileProperties
    {
        public int id;
        public Rectangle position;
        public Color colour;
    }
}
