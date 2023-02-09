using CommunityManagerLib.Addons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CommunityManager.Converters.Specific
{
  public class GroupAddonViewToSingleAddonViewsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      List<SingleAddonView> ret;
      if (value is GroupAddonView gav)
      {
        ret = gav.Addons;
      }
      else
        ret = new();
      return ret;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
