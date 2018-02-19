using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

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
