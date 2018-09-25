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
          
          try
          {
            HttpClient client = new HttpClient();
            var b = await client.GetAsync("http://localhost:888/admin/page");
            //var s = await b.Content.ReadAsStringAsync();
            Console.WriteLine(b.ToString());
          }
          catch { }
          //Console.Write($"finish");
        });

        t.Start();
        
      }
      Console.ReadLine();
    }
  }
}
