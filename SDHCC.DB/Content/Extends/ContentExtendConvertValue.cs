using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SDHCC.DB.Content
{
  public static partial class ContentExtend
  {
    public static Dictionary<Type, Func<string, dynamic>> ConvertStringToTypeDictionary { get; set; } = new Dictionary<Type, Func<string, dynamic>>()
    {
      [typeof(string)] = b =>
      {
        if (b == null)
        {
          return "";
        }
        return b;
      },
      [typeof(int)] = b =>
      {
        if (b == null)
        {
          return 0;
        }
        int.TryParse(b, out var r);
        return r;
      },
      [typeof(Int16)] = b =>
      {
        if (b == null)
        {
          return 0;
        }
        Int16.TryParse(b, out var r);
        return r;
      },
      [typeof(Int32)] = b =>
      {
        if (b == null)
        {
          return 0;
        }
        Int32.TryParse(b, out var r);
        return r;
      },
      [typeof(Int64)] = b =>
      {
        if (b == null)
        {
          return 0;
        }
        Int64.TryParse(b, out var r);
        return r;
      },
      [typeof(float)] = b =>
      {
        if (b == null)
        {
          return 0f;
        }
        float.TryParse(b, out var r);
        return r;
      },
      [typeof(double)] = b =>
      {
        if (b == null)
        {
          return 0d;
        }
        double.TryParse(b, out var r);
        return r;
      },
      [typeof(decimal)] = b =>
      {
        if (b == null)
        {
          return 0m;
        }
        decimal.TryParse(b, out var r);
        return r;
      },
      [typeof(bool)] = b =>
      {
        if (b == null)
        {
          return false;
        }
        Boolean.TryParse(b, out var r);
        return r;
      },
      [typeof(DateTime)] = b =>
      {
        if (b == null)
        {
          return new DateTime();
        }
        string formats = b.Trim().Split(' ').Length == 1 ? G.DateFormat : G.DateTimeFormat;
        DateTime.TryParseExact(b, formats, new CultureInfo("en-Us"), DateTimeStyles.None, out var r);
        return r;
      },
    };
    public static Dictionary<Type, Func<object, string>> ConvertTypeToStringDictionary { get; set; } = new Dictionary<Type, Func<object, string>>()
    {
      [typeof(string)] = b =>
      {
        if (b == null)
        {
          return "";
        }
        return b.ToString();
      },
      [typeof(int)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
      [typeof(Int16)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
      [typeof(Int32)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
      [typeof(Int64)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
      [typeof(float)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
      [typeof(double)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
      [typeof(decimal)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
      [typeof(bool)] = b =>
      {
        if (b == null)
        {
          return "false";
        }
        return b.ToString();
      },
      [typeof(DateTime)] = b =>
      {
        if (b == null)
        {
          return "";
        }
        return ((DateTime)b).ToString(G.DateTimeFormat);
      },
    };
  }
}
