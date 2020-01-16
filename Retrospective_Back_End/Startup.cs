using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Retrospective_Core.Services;
using Retrospective_EFSQLRetrospectiveDbImpl;
using Retrospective_EFSQLRetrospectiveDbImpl.Seeds;
using Retrospective_Back_End.Realtime;
using Retrospective_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Retrospective_Back_End.Utils;
using Decoder = Retrospective_Back_End.Utils.Decoder;

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
            services.AddControllers().AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(isOriginAllowed: _ => true)
                    .AllowCredentials());

            });
            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddDbContext<RetroSpectiveDbContext>(options =>
	            options.UseSqlServer(
		            Configuration["Data:ConnectionString"]));

            services.AddIdentity<RetroUser, RetroRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<RetroSpectiveDbContext>()
                .AddDefaultTokenProviders();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = "https://true-lime.herokuapp.com/",
                    ValidIssuer = "https://true-lime.herokuapp.com/",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecureKey"))
                };
            });

            services.AddTransient<IRetroRespectiveRepository, EFRetrospectiveRepository>();
            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddTransient<IDecoder, Decoder>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Retrospective Tool API V1", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
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

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Retrospective Tool API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthentication();
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
