﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public static class G
  {
    public static string DateTimeFormat { get; set; } = "yyyy-MM-dd hh:mm:ss";
    public static string DateFormat { get; set; } = "yyyy-MM-dd";
    public static T GetAsyncValue<T>(this Task<T> task)
    {
      return Task.Run(async () => await task).ConfigureAwait(false).GetAwaiter().GetResult();
    }
    public static void GetAsyncValue(this Task task)
    {
      Task.Run(async () => await task).ConfigureAwait(false).GetAwaiter().GetResult();
    }
  }
}