using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System
{
  public static class FileReadExtend
  {
    public static FileStreamResult GetFileFromPath(this string fullPath, Controller controller)
    {
      if (string.IsNullOrEmpty(fullPath))
        return null;

      var path = Path.Combine(
                     Directory.GetCurrentDirectory(), fullPath);

      var memory = new MemoryStream();
      using (var stream = new FileStream(path, FileMode.Open))
      {
        stream.CopyToAsync(memory).GetAsyncValue();
      }
      memory.Position = 0;
      return controller.File(memory, path.GetContentTypeFromPath(), Path.GetFileName(path));
    }
  }
}
