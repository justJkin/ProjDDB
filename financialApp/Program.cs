using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace financialApp
{
    public class Program
    {
        public static void startexe(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
