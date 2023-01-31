﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib
{
  public class NotifyPropertyChangedBase
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    private Dictionary<string, object?> propertyValues = new();

    protected T? GetProperty<T>(string propertyName)
    {
      T? ret;
      if (propertyValues.ContainsKey(propertyName))
      {
        object? val = propertyValues[propertyName];
        Trace.Assert(val == null || val is T?);
        ret = (T?)val;
      }
      else
        ret = default;
      return ret;
    }

    protected void InvokePropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));      
    }

    protected void UpdateProperty<T>(string propertyName, T value)
    {
      T? previousValue = GetProperty<T?>(propertyName);
      if (previousValue == null)
      {
        if (value == null) return;
      }
      else if (previousValue.Equals(value)) return;

      propertyValues[propertyName] = value;
      InvokePropertyChanged(propertyName);
    }
  }
}
