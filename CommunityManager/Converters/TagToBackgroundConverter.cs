using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CommunityManager.Converters
{
  internal class TagToBackgroundConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Brush ret;

      int seed = value == null ? 0 : value.GetHashCode();
      Random rnd = new(seed);

      byte r = (byte)rnd.Next(128, 255);
      byte g = (byte)rnd.Next(128, 255);
      byte b = (byte)rnd.Next(128, 255);

      ret = new SolidColorBrush(Color.FromRgb(r, g, b));

      return ret;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
