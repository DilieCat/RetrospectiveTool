using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Retrospective_Core.Services;
using Retrospective_EFSQLRetrospectiveDbImpl;
using Retrospective_EFSQLRetrospectiveDbImpl.Seeds;
using Retrospective_Back_End.Realtime;
using Retrospective_Core.Models;

namespace Retrospective_Back_End
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
            options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(isOriginAllowed: _ => true)
                .AllowCredentials());

            });
            services.AddSignalR();
            _ = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddDbContext<RetroSpectiveDbContext>(options =>
	            options.UseSqlServer(
		            Configuration["Data:ConnectionString"]));

            services.AddIdentity<RetroUser, RetroRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<RetroSpectiveDbContext>();

            services.AddTransient<IRetroRespectiveRepository, EFRetrospectiveRepository>();
            services.AddControllersWithViews().AddNewtonsoftJson(options =>options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider service)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
	            app.UseHsts();
            }

            app.UseFileServer();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotifyHub>("/api/notify");
            });
            app.UseMvc();
            SeedData.Initialize(service);
        }
    }
}
