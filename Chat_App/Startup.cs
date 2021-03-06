using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Chat_App.BackgammonGame.Logic.Models;
using Chat_App.Data;
using Chat_App.Data.DbConfig;
using Chat_App.Services.ChatService;
using Chat_App.Services.ChatService.Hubs;
using Chat_App.Services.ChatService.Hubs.Acount;
using Chat_App.Services.GameServices;
using Chat_App.Services.Hubs.Game;
using Chat_App.Services.Hubs.Game.Models;
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
            //repositories :
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IMessageRepo, MessageRepo>();
            services.AddScoped<IRoomRepo, RoomRepo>();
            

            services.AddScoped<Services.Auth.IAuthenticationService, Services.Auth.AuthenticationService>();
            services.AddSingleton<IGameService, GameService>();

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
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chat")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSingleton<IJwtService>(new JwtService(key));


            services.AddMvc();

            services.AddSingleton<IDictionary<string, UserConnection>>(options => new Dictionary<string, UserConnection>());//Chat
            services.AddSingleton<IDictionary<string, string>>(options => new Dictionary<string, string>());//Server
            services.AddSingleton<IDictionary<string, GameUserConnections>>(options => new Dictionary<string, GameUserConnections>());//Game
            services.AddSingleton<IDictionary<GameUserConnections, GameBoard>>(options => new Dictionary<GameUserConnections, GameBoard> ());//GameBoards

            services.AddSignalR();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ChatAppDbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(builder => builder
                .WithOrigins(new[] { "http://localhost:3000", "https://chat-project-client.azurewebsites.net" })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                );


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapHub<AccountsHub>("/login");
                endpoints.MapHub<GameHub>("/game");
            });
        }
    }
}
