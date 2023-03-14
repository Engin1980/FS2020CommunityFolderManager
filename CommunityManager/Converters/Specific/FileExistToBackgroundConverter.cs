using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CommunityManager.Converters.Specific
{
  internal class FileExistToBackgroundConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string fileName = (string)value;
      bool exist = System.IO.File.Exists(fileName);

      SolidColorBrush ret;
      if (exist)
      {
        ret = new SolidColorBrush(Colors.Transparent);
      } else
      {
        ret = new SolidColorBrush(Colors.Pink);
      }
      return ret;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
