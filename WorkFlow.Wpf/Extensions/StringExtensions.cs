using System;
using System.Windows.Media;

namespace WorkFlow.Extensions
{
    public static  class StringExtensions
    {

        public static Color HexToColor(this string hexaColor)
        {
            return Color.FromArgb(
                   255,
                   Convert.ToByte(hexaColor.Substring(1, 2), 16),
                   Convert.ToByte(hexaColor.Substring(3, 2), 16),
                   Convert.ToByte(hexaColor.Substring(5, 2), 16)
               );
        }
    }
}
