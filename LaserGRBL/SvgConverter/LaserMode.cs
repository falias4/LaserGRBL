using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserGRBL.SvgConverter
{
    public class LaserMode
    {
        // TODO FAL: Better way?
        public static LaserMode[] LaserModes = new LaserMode[] { new LaserMode("M3", "M3 - Constant Power"), new LaserMode("M4", "M4 - Dynamic Power") };

        public string GCode { get; set; }
        public string DisplayName { get; set; }

        public LaserMode Self { get { return this;  } }

        public LaserMode(string gcode, string displayName)
        {
            GCode = gcode;
            DisplayName = displayName;
        }
    }
}
