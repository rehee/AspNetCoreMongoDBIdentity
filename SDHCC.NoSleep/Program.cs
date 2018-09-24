using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SDHCC.NoSleep
{
  class Program
  {
    static void Main(string[] args)
    {
      for(var i = 0; i < 10000; i++)
      {
        Console.WriteLine($"read line {i}");
        var t = new Task(async ()=>
        {
          HttpClient client = new HttpClient();
          try
          {
            var b = await client.GetStringAsync("http://localhost:5000/admin/page");
          }
          catch { }
          //Console.Write($"finish");
        });

        t.Start();
        
      }
      

      Console.WriteLine("Hello World!");
    }
  }
}
