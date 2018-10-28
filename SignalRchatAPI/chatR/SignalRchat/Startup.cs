using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SignalRchat.Hubs;
using SignalRchat.Services;
using SignalRchat.Services.Authentication;
using SignalRchat.Helpers;
using Swashbuckle.AspNetCore.Swagger;

namespace SignalRchat 
{
    public class Startup 
    {
        private readonly IHostingEnvironment _env;
        public Startup (IConfiguration configuration, IHostingEnvironment environment) 
        {
            _env = environment;

            Configuration = configuration;

            var builder = new ConfigurationBuilder ()
                .SetBasePath (_env.ContentRootPath)
                .AddJsonFile ($"appsettings.json", optional : false, reloadOnChange : true)
                .AddEnvironmentVariables ();

            AppSettingsService.Configuration = builder.Build ();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.Configure<Settings> (options => {
                options.ConnectionString = Configuration.GetSection ("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection ("MongoConnection:Database").Value;
            });

            #region JWT

            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,

                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    ValidIssuer = $"{AppSettingsService.Configuration["AppSettings:Authentication:IssuserName"]}",
                    ValidAudience = $"{AppSettingsService.Configuration["AppSettings:Authentication:AudienceName"]}",
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes ($"{AppSettingsService.Configuration["AppSettings:Authentication:Secret"]}")),
                    ClockSkew = TimeSpan.FromMinutes (5) //5 minute tolerance for the expiration date
                    };

                    options.Events = new JwtBearerEvents {
                        OnAuthenticationFailed = context => {
                                Console.WriteLine ("OnAuthenticationFailed: " + context.Exception.Message);
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context => {
                                Console.WriteLine ("OnTokenValidated: " + context.SecurityToken);
                                return Task.CompletedTask;
                            },
                            // We have to hook the OnMessageReceived event in order to
                            // allow the JWT authentication handler to read the access
                            // token from the query string when a WebSocket or 
                            // Server-Sent Events request comes in.
                            OnMessageReceived = context => {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty (accessToken) &&
                                    (path.StartsWithSegments ("/chat"))) {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                    };
                });

            services.AddCors (options => {
                options.AddPolicy ("CorsPolicy", builder => builder
                    .AllowAnyOrigin ()
                    .AllowAnyMethod ()
                    .AllowAnyHeader ()
                    .AllowCredentials ()
                    .SetPreflightMaxAge (TimeSpan.FromSeconds (2520))
                    .Build ());
            });
            #endregion

            services.AddAutoMapper ();
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
            services.AddSignalR (o => {
                o.EnableDetailedErrors = true;
            });
            services.AddSingleton<IMongoClient> (new MongoClient (Configuration.GetSection ("MongoConnection:ConnectionString").Value));

            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("v1", new Info { Title = "chatR API", Version = "v1" });
                c.DescribeAllEnumsAsStrings ();
                c.AddSecurityDefinition ("Bearer", new ApiKeyScheme {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                });
                c.AddSecurityRequirement (new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }
                });
            });

            #region DI

            services.AddTransient<ICipherService, CipherService> ();
            services.AddTransient<IGenerator, Generator> ();
            services.AddTransient<ITokenService, TokenService> ();
            services.AddTransient<IAuthenticationService, AuthenticationService> ();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ICipherService cipherService, ILogger<Startup> logger) 
        {
            app.SeedDatabase (cipherService);

            app.UseAuthentication ();
            if (env.IsDevelopment ()) 
            {
                app.UseDeveloperExceptionPage ();
            }
            app.UseHttpsRedirection ();
            app.UseHsts ();
            app.UseCors ("CorsPolicy");

            app.UseSignalR (routes => 
            {
                routes.MapHub<ChatHub> ("/chat");
            });

            app.UseSwagger ();
            app.UseSwaggerUI (s => {
                s.SwaggerEndpoint ("/swagger/v1/swagger.json", "API V1");
                s.RoutePrefix = "api/docs";
            });

            app.UseMvc ();
        }
    }

    public class Settings 
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}