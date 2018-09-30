using System;
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
      //for (var i = 0; i < Time; i++)
      //{
      //  System.Threading.Thread.Sleep(TimeSpend);
      //  if (task.IsCompleted != true)
      //    continue;
      //  var value = task.Result;
      //  if (value == null)
      //    return default(T);
      //  return (T)Convert.ChangeType(value, typeof(T));
      //}
      //return default(T);
      return Task.Run(async () => await task).ConfigureAwait(false).GetAwaiter().GetResult();
    }
  }
}
