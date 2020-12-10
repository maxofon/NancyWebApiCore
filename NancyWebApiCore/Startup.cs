﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using NancyWebApiCore.Helpers;

namespace NancyWebApiCore
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(_configuration, appConfig);
            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(appConfig)));
        }
    }
}