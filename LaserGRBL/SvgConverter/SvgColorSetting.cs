using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LaserGRBL.SvgConverter
{
    public class SvgColorSetting
    {
        private int _maxpwm = 0;

        public string ColorSvgRef { get; set; }
        public string ColorName { get; set; }
        public Bitmap ColorAsBitmap { get; set; }
        public int Speed { get; set; }
        public LaserMode LaserMode { get; set; }
        public int SMin { get; set; }
        public string SMinPercentage { get { return getPercentageOfPower(SMin);  } }
        private int _sMax;

        public int SMax
        {
            get { return _sMax; }
            set { _sMax = Math.Min(value, _maxpwm); }
        }

        public string SMaxPercentage { get { return getPercentageOfPower(SMax); } }
        public int Passes { get; set; }

        public SvgColorSetting(string colorFromSvg, GrblCore core)
        {
            ColorSvgRef = colorFromSvg;
            _maxpwm = core?.Configuration != null ? (int)core.Configuration.MaxPWM : -1;

            // converting it to & from Win32 can lead to a 'Readable Color'
            var color = ColorTranslator.FromWin32(ColorTranslator.ToWin32(ColorTranslator.FromHtml(colorFromSvg)));
            ColorName = color.Name;
            ColorAsBitmap = createBitmapFromColor(color, 35, 12);
            Speed = Settings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000);

            string selMode = Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3");
            if (selMode == "M3" || !core.Configuration.LaserMode)
                LaserMode = LaserMode.LaserModes[0];
            else
                LaserMode = LaserMode.LaserModes[1];

            SMin = Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", 0);
            SMax = Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", (int)core.Configuration.MaxPWM);

            Passes = 1;
        }

        private string getPercentageOfPower(int power)
        {
            return _maxpwm > 0 ? (power / _maxpwm).ToString("P1") : "-";
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
