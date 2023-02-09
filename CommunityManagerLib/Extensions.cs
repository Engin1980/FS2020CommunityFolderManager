using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib
{
  public static class Extensions
  {
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var item in enumerable)
      {
        action.Invoke(item);
      }
      return enumerable;
    }

    public static BindingList<T> ToBindingList<T>(this IEnumerable<T> enumerable)
    {
      BindingList<T> ret = new BindingList<T>(enumerable.ToList());
      return ret;
    }

    public static string ToMessageString(this Exception ex)
    {
      List<string> msgs = new();
      Exception? tmp = ex;
      while (tmp != null)
      {
        msgs.Add(tmp.Message);
        tmp = tmp.InnerException;
      }

      string ret = string.Join(" // ", msgs);
      return ret;
    }
  }
}
