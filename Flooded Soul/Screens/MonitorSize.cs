using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Flooded_Soul.Screens
{
    struct ScreenResolution
    {
        public int width;
        public int height;
    }

    internal class MonitorSize
    {
        ScreenResolution screenResolution;

        public ScreenResolution GetScreenResolution()
        {
            Screen primaryScreen = Screen.PrimaryScreen;
            Rectangle workingArea = primaryScreen.WorkingArea;

            screenResolution.width = workingArea.Width;
            screenResolution.height = workingArea.Height;

            return screenResolution;
        }
    }
}
