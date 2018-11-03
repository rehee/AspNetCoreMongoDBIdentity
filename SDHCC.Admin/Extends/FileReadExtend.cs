using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System
{
  public static class FileReadExtend
  {
    public static Stream GetFileStreamFromPath(this string fullPath, Controller controller, int widthPx = 0)
    {
      try
      {
        if (string.IsNullOrEmpty(fullPath))
          return null;
        var path = Path.Combine(
                       Directory.GetCurrentDirectory(), fullPath);
        //var f = File.OpenWrite(path);
        Image<Rgba32> image = Image.Load(path);
        if (widthPx > 0)
        {
          int persent = image.Width / widthPx;
          image.Mutate(x => x
           .Resize(image.Width / persent, image.Height / persent)
           .Grayscale());
        }

        var memory = new MemoryStream();
        image.SaveAsJpeg(memory);
        memory.Position = 0;
        return memory;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return null;
      }

    }
    public static FileStreamResult GetFileFromPath(this string fullPath, Controller controller, int widthPx = 0)
    {
      var fileStream = fullPath.GetFileStreamFromPath(controller, widthPx);
      if (fileStream == null)
        return null;
      var memory = new MemoryStream();
      var fileName = $"{Guid.NewGuid().ToString()}.jpg";
      return controller.File(fileStream, fileName.GetContentTypeFromPath(), Path.GetFileName(fileName));
    }
  }
}
