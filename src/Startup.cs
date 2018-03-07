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

namespace Aiursoft.Stargate
{
    public class Startup
    {
        public static Counter MessageIdCounter { get; set; } = new Counter();
        public static Counter ListenerIdCounter { get; set; } = new Counter();
        public static Random random { get; set; } = new Random();

        public IConfiguration Configuration { get; }
        public bool IsDevelopment { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            IsDevelopment = env.IsDevelopment();
            if (IsDevelopment)
            {
                Values.ForceRequestHttps = false;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StargateDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));
            services.AddMvc();
            services.AddTransient<WebSocketPusher>();
            services.AddTransient<DataCleaner>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, StargateDbContext dbContext, DataCleaner cleaner)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            app.UseWebSockets();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseAiursoftAuthenticationFromConfiguration(Configuration, "Test");
        }
    }
}
