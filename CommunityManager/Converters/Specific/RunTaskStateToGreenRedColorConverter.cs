using CommunityManagerLib.RunProcedure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CommunityManager.Converters.Specific
{
  public class RunTaskStateToGreenRedColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Color ret;
      RunTask.RunTaskState val = (RunTask.RunTaskState)value;
      switch (val)
      {
        case RunTask.RunTaskState.Running:
          ret = Colors.Purple; break;
        case RunTask.RunTaskState.Failed:
          ret = Colors.Red; break;
        case RunTask.RunTaskState.Done:
          ret = Colors.DarkGreen; break;
        case RunTask.RunTaskState.Ready:
          ret = Colors.Black; break;
        default:
          throw new NotImplementedException("Unknown RunTaskState " + val);
      }

      return new SolidColorBrush(ret);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
