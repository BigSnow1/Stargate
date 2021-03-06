﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Stargate
{
    public class Startup
    {
        public static Counter MessageIdCounter { get; set; } = new Counter();
        public static Counter ListenerIdCounter { get; set; } = new Counter();
        public static Random random { get; set; } = new Random();

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StargateDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));
            services.AddMvc();
            services.AddTransient<WebSocketPusher>();
            services.AddSingleton<IHostedService, TimedCleaner>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, StargateDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseEnforceHttps();
            }
            app.UseWebSockets();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseLanguageSwitcher();
            app.UseAiursoftAuthenticationFromConfiguration(Configuration, "Test");
        }
    }
}
