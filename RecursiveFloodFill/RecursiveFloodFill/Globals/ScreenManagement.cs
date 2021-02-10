using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RecursiveFloodFill.Globals
{
    public abstract class BaseScreen
    {
        public string Name = "";
        public ScreenState State = ScreenState.Active;
        public Single Position;
        public bool Focused = false;
        public bool GrabFocus = true;


        public virtual void HandleInput()
        {
            // Handle Input for keyboard events.
        }

        public virtual void Update()
        {
            // General update events.
        }

        public virtual void TickEvents()
        {
            // Tick events that happen every 3 seconds.
        }

        public virtual void Draw()
        {
            // Primary Draw events.
        }

        public virtual void DrawEffects()
        {
            // Secondary draw events (effects).
        }

        public virtual void DrawGUI()
        {
            // Anything that isn't affected by shaders.
        }

        public virtual void Unload()
        {
            // Shut down screen.
            State = ScreenState.Shutdown;
        }

    }

    public class ScreenManager
    {

        private static List<BaseScreen> Screens;
        private static List<BaseScreen> removeScreens;
        private static List<BaseScreen> newScreens;

        //Logging
        private static List<Logging> logList;
        private static List<string> totalLogList;
        private bool showFullLog;
        private int logStartPos = 0;


        //FPS
        private int fps = 0;
        private int fpsCounter = 0;
        private double fpsTimer = 0;
        private string fpsText = "";
        private Color colour = Color.White;
        private int fpsY;
        private bool showFPS;

        public ScreenManager()
        {
            Screens = new List<BaseScreen>();
            removeScreens = new List<BaseScreen>();
            newScreens = new List<BaseScreen>();

            logList = new List<Logging>();
            totalLogList = new List<string>();
            showFullLog = false;
            showFPS = false;
        }

        public void Update()
        {

            foreach (BaseScreen foundscreen in Screens)
            {
                if (foundscreen.State == ScreenState.Shutdown)
                    removeScreens.Add(foundscreen);
                else
                    foundscreen.Focused = false;
            }

            // Remove dead screens
            foreach (BaseScreen foundscreen in removeScreens)
                Screens.Remove(foundscreen);

            removeScreens.Clear();

            // Add new screens
            foreach (BaseScreen foundscreen in newScreens)
                Screens.Add(foundscreen);

            newScreens.Clear();

            // Check screen focus
            if (Screens.Count > 0)
            {
                for (int i = Screens.Count - 1; i >= 0; i += -1)
                {
                    if (Screens[i].GrabFocus)
                    {
                        Screens[i].Focused = true;
                        break;
                    }

                }

            }

            if (GV.WindowFocused)
            {
                // Handle Input for focused screen
                foreach (BaseScreen foundscreen in Screens)
                {
                    foundscreen.Update();
                    foundscreen.HandleInput();
                }

                if (UserInput.KeyPressed(Keys.F1))
                    showFPS = !showFPS;

                if (UserInput.KeyPressed(Keys.F2))
                    showFullLog = !showFullLog;

                foreach (Logging _log in logList)
                {
                    _log.Update();

                    if (_log.CanRemove())
                    {
                        logList.Remove(_log);
                        totalLogList.Add(_log.message);
                        break;
                    }
                }
            }
        }

        // Draw Screens
        public void Draw()
        {
            foreach (BaseScreen foundscreen in Screens)
            {
                if (foundscreen.State == ScreenState.Active)
                    foundscreen.Draw();
            }

            if (showFPS)
            {
                if (GV.GameTime.TotalGameTime.TotalMilliseconds >= fpsTimer)
                {
                    fps = fpsCounter;
                    fpsTimer = GV.GameTime.TotalGameTime.TotalMilliseconds + 1000;
                    fpsCounter = 1;
                    fpsText = "Fps: " + fps;
                    fpsY = GV.GameSize.Height - (int)Fonts.SystemBold.MeasureString(fpsText).Y;

                    if (fps > 50)
                        colour = Color.LightGreen;
                    else if (fps > 30)
                        colour = Color.White;
                    else if (fps > 25)
                        colour = Color.Yellow;
                    else
                        colour = Color.Red;

                }
                else
                    fpsCounter += 1;

                GV.SpriteBatch.Begin();
                GV.SpriteBatch.DrawString(Fonts.SystemBold, fpsText, new Vector2(0, fpsY), colour);
                GV.SpriteBatch.End();
            }

            if (!showFullLog)
            {
                //Draw Loglist
                for (int x = 0; x <= logList.Count - 1; x++)
                {
                    Vector2 _sMeasure = Fonts.SystemBold.MeasureString(logList[x].message);

                    GV.SpriteBatch.Begin();
                    GV.SpriteBatch.DrawString(Fonts.SystemBold, logList[x].message, new Vector2((GV.GameSize.Width - _sMeasure.X) - 25, ((GV.GameSize.Height - _sMeasure.Y) - 25) - (x * 20)), Color.White * logList[x].alpha);
                    GV.SpriteBatch.End();
                }
            }
            else
            {
                if (UserInput.KeyDown(Keys.PageDown))
                {
                    logStartPos += 1;

                    if (logStartPos > totalLogList.Count)
                        logStartPos = totalLogList.Count;
                }

                if (UserInput.KeyDown(Keys.PageUp))
                {
                    logStartPos -= 1;

                    if (logStartPos < 0)
                        logStartPos = 0;
                }

                int _restrict = 0;

                //Draw the full Log list
                for (int x = logStartPos; x <= totalLogList.Count - 1; x++)
                {
                    Vector2 _sMeasure = Fonts.SystemBold.MeasureString(totalLogList[x]);

                    GV.SpriteBatch.Begin();
                    GV.SpriteBatch.DrawString(Fonts.SystemBold, totalLogList[x], new Vector2((GV.GameSize.Width - _sMeasure.X) - 25, 25 + (x * 20)), Color.White);
                    GV.SpriteBatch.End();

                    _restrict += 1;
                    if (_restrict > 33)
                        break;
                }

            }
        }

        // Draw any effects
        public void DrawEffects()
        {
            foreach (BaseScreen foundscreen in Screens)
            {
                if (foundscreen.State == ScreenState.Active)
                    foundscreen.DrawEffects();
            }
        }


        // Add screen
        public static void AddScreen(BaseScreen _name)
        {
            bool _addScreen = true;

            foreach (BaseScreen foundscreen in Screens)
            {
                if (_name.Name == foundscreen.Name)
                {
                    _addScreen = false;
                    break;
                }
            }

            if (_addScreen)
                newScreens.Add(_name);
        }

        // Remove screen
        public static void UnloadScreen(string _name)
        {
            foreach (BaseScreen foundscreen in Screens)
            {
                if (foundscreen.Name == _name)
                {
                    foundscreen.Unload();
                    break;
                }

            }

        }

        // See if a screen is active
        public static bool QueryScreen(string _name)
        {
            foreach (BaseScreen foundscreen in Screens)
            {
                if (foundscreen.Name == _name)
                    return true;
            }
            return false;
        }

        //Add a new log message
        public static void AddNewLogMessage(string _msg)
        {
            if (logList.Count < 34 && _msg != "")
            {
                string _hour = DateTime.Now.TimeOfDay.Hours.ToString("00");
                string _minute = DateTime.Now.TimeOfDay.Minutes.ToString("00");
                string _second = DateTime.Now.TimeOfDay.Seconds.ToString("00");

                logList.Add(new Logging(_hour + ":" + _minute + ":" + _second + ": " + _msg));
            }
        }
    }

    public class Logging
    {
        public string message;
        public float alpha;

        private int lifeTime;
        private bool expired;

        public Logging(string _msg)
        {
            message = _msg;
            expired = false;
            lifeTime = 5000;
            alpha = 1f;
        }

        public void Update()
        {
            if (lifeTime < 0)
            {
                alpha -= 0.05f;

                if (alpha < 0f)
                    expired = true;
            }
            else
                lifeTime -= (int)GV.GameTime.ElapsedGameTime.TotalMilliseconds;

        }

        public void Draw(Vector2 _pos)
        {
            GV.SpriteBatch.DrawString(Fonts.SystemBold, message, _pos, Color.White);
        }

        public bool CanRemove()
        {
            return expired;
        }
    }

    public enum ScreenState
    {
        Active,
        Shutdown,
        Hidden
    }


}
