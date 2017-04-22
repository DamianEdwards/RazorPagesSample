using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;

namespace RazorPagesWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.AddDebug())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
