using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;
using WebMatBotV3.Shared;

namespace WebMatBotV3.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
//            services.AddDbContext<DataContext>(option => { option.UseMySql(Configuration.GetConnectionString("MainDataBase"), mysqlOptions => mysqlOptions.EnableRetryOnFailure());});

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSignalR();
//            services.AddSingleton<Services.ContextCache>();
//            services.AddScoped<Services.CannonService>();
//            services.AddHostedService<Services.WebMatBotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<Hubs.SubtitleHub>("/HubConnection");
                endpoints.MapHub<Hubs.CannonHub>("/HubCannon");
            });

            WebMatBot.Program.Start();

            //hostApp.ApplicationStopping.Register(async () => { 
            //    await Services.WebMatBotService.StopAsync();
            //    await Task.Delay(5000);
            //});
        }
    }
}
