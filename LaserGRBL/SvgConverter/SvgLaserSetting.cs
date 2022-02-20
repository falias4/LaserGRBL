using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LaserGRBL.SvgConverter
{
    public class SvgLaserSetting
    {
        public string ColorSvgRef { get; set; }
        public string ColorName { get; set; }
        public Bitmap ColorAsBitmap { get; set; }
        public int Speed { get; set; }
        public LaserMode LaserMode { get; set; }
        public int SMin { get; set; }
        public string SMinPercentage { get; set; }
        public int SMax { get; set; }
        public string SMaxPercentage { get; set; }

        public SvgLaserSetting(string colorFromSvg, GrblCore core)
        {
            ColorSvgRef = colorFromSvg;

            var color = ColorTranslator.FromHtml(colorFromSvg);
            ColorName = findKnownColorName(color);
            ColorAsBitmap = createBitmapFromColor(color, 35, 12);
            Speed = Settings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000);

            string selMode = Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3");
            if (selMode == "M3" || !core.Configuration.LaserMode)
                LaserMode = LaserMode.LaserModes[0];
            else
                LaserMode = LaserMode.LaserModes[1];

            SMin =Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", 0);
            SMinPercentage = "0 %";
            SMax =Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", (int)core.Configuration.MaxPWM);
            SMaxPercentage = "0 %";
        }

        private string findKnownColorName(Color color)
        {
            var knownColors = Enum.GetValues(typeof(KnownColor));
            foreach (int knownColor in knownColors)
            {
                var currColor = Color.FromKnownColor((KnownColor)knownColor);
                if (currColor.ToArgb() == color.ToArgb())
                {
                    return currColor.Name;
                }
            }
            return color.Name;
        }

        private Bitmap createBitmapFromColor(Color color, int width, int height)
        {
            var colorBmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(colorBmp);
            g.Clear(color);
            return colorBmp;
        }
    }
}
