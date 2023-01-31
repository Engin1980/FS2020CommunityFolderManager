using System;
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
    public static BindingList<T> ToBindingList<T>(this IEnumerable<T> enumerable)
    {
      BindingList<T> ret = new BindingList<T>(enumerable.ToList());
      return ret;
    }

    //public static void ForEach<T>(this BindingList<T> bindingList, Action<T> action)
    //{
    //  foreach (var item in bindingList)
    //  {
    //    action.Invoke(item);
    //  }
    //}

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var item in enumerable)
      {
        action.Invoke(item);
      }
      return enumerable;
    }
  }
}
