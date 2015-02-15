using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Dashboard
{
    class Constants
    {
        public static String OPERATION_AFFECTEDLIST = "AFFECTEDLIST";
        public static String OPERATION_BUSDETAILS = "BUSDETAILS";
        public static String SEPARATOR_DELIMITER = "_";
        public static String COMMA_DELIMITER = ",";
        public static String SEPARATOR_COLON_FOR_SERVER_DATA = ":";
        public static Color NORMAL_LINE_COLOR = Color.FromArgb(255, 0, 255, 0);
        public static Color LINE_DOWN_COLOR = Color.FromArgb(255, 255, 0, 0);
        public static Color WARNING_LINE_COLOR = Color.Yellow;
        public static int UPDATE_CIRCUIT_INTERVAL = (15) * (1000);  // 5 seconds
        public static Color APP_BUTTON_COLOR = Color.FromArgb(155, 0, 0, 0);
        public static Size MINIMUM_PANEL_SIZE = new Size(10, 10);
        public static float X_SIZE_FACTOR = 1382 / 804;
        public static float Y_SIZE_FACTOR = 744 / 498;
        public static Size APP_BUTTON_SIZE = new Size(190, 30);
        public static Color APP_BUTTON_FORE_COLOR = Color.White;
        public static int BUTTON_STATE_NORMAL = 0;        
        public static int BUTTON_STATE_WARNING = 1;
        public static int BUTTON_STATE_DOWN = 2;
    }
}
