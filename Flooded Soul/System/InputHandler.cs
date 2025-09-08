using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace Flooded_Soul.System
{
    public class InputHandler
    {
        public bool IsKeyPressed(Keys key) => Game1.instance.keyboardState.IsKeyDown(key) && Game1.instance.previousState.IsKeyUp(key);
        public bool IsKeyDown(Keys key) => Game1.instance.keyboardState.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => Game1.instance.keyboardState.IsKeyUp(key);

        public bool IsLeftMouse() => Game1.instance.mouseState.LeftButton == ButtonState.Pressed && Game1.instance.prevMouse.LeftButton == ButtonState.Released;
        public bool IsRightMouse() => Game1.instance.mouseState.RightButton == ButtonState.Pressed && Game1.instance.prevMouse.RightButton == ButtonState.Released;
    }
}
