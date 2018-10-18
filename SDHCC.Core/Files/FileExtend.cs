using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System
{
  public static class FileExtend
  {
    public static string GetContentTypeFromPath(this string path)
    {
      var ext = Path.GetExtension(path).ToLowerInvariant();
      if (MainType.ContainsKey(ext))
        return MainType[ext];
      return "text/plain";
    }

    public static Dictionary<string, string> MainType { get; set; } = new Dictionary<string, string>
    {
      {".txt", "text/plain"},
      {".pdf", "application/pdf"},
      {".doc", "application/vnd.ms-word"},
      {".docx", "application/vnd.ms-word"},
      {".xls", "application/vnd.ms-excel"},
      {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
      {".png", "image/png"},
      {".jpg", "image/jpeg"},
      {".jpeg", "image/jpeg"},
      {".gif", "image/gif"},
      {".csv", "text/csv"}
    };
  }
}
