using CommunityManagerLib.Addons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace CommunityManager.Converters.Specific
{
  public class NoteVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool shouldBeSeen = false;
      AddonView av = (AddonView)value;
      if (av is SingleAddonView sav)
        shouldBeSeen = !string.IsNullOrWhiteSpace(av.Note);

      return shouldBeSeen ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
