using System;
using VisitorStatistics.Models;
using VisitorStatistics.Services;
using VisitorStatistics.Middleware;
using VisitorStatistics.Services.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace VisitorStatistics
{
    /// <summary>
    /// This class registers the base settings for the application that will apply on startup.
    /// </summary>
    public class Startup
    {
        // Inject dependencies to reach the settings specified in appsettings.json:
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false).AddSessionStateTempDataProvider();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register the database context:
            services.AddDbContext<VisitorStatisticsContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DBConnect")));

            services.AddTransient<IVisitorHandler, VisitorHandler>();

            services.AddHostedService<TimedHostedService>();

            services.ConfigureWritable<AppSettings>(Configuration.GetSection("IpDeletionDays"));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();
            app.UseMiddleware<VisitorStatisticsServer>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvcWithDefaultRoute();
        }

    } // End class.
} // End namespace.
