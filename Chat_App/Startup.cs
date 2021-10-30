using System;
using System.Text;
using Chat_App.Data;
using Chat_App.Data.DbConfig;
using Chat_App.Services.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace Chat_App
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var key = "Sercret Key sadfgswdgfwsgwreg";
            services.AddCors();

            //DB Configuration :
            var connString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ChatAppDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(connString));

            services.AddControllers()
                    .AddNewtonsoftJson(s => s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IUserRepo, UserRepo>();

            services.AddScoped<Services.Auth.IAuthenticationService, Services.Auth.AuthenticationService>();

            services.AddSignalR();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false

                };
            });

            services.AddSingleton<IJwtService>(new JwtService(key));

            services.AddMvc();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ChatAppDbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(options => options
                .WithOrigins(new[] { "http://localhost:3000" })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                );


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
