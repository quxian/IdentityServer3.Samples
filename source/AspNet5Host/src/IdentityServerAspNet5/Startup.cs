﻿using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Logging;
using Serilog;
using System;

namespace IdentityServerAspNet5
{
    public class Startup
    {
        public Startup(ILoggerFactory loggerFactory)
        {
            var serilogLogger = new LoggerConfiguration()
                .WriteTo
                .TextWriter(Console.Out)
                .MinimumLevel.Verbose()
                .CreateLogger();

            Log.Logger = serilogLogger;
            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddSerilog(serilogLogger);
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app, IApplicationEnvironment env)
        {
            app.UseIISPlatformHandler();
            app.UseDeveloperExceptionPage();

            var certFile = env.ApplicationBasePath + "\\idsrv3test.pfx";

            var idsrvOptions = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get()),

                SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                AuthenticationOptions = new AuthenticationOptions
                {
                    EnablePostSignOutAutoRedirect = true
                },

                RequireSsl = false
            };

            app.UseIdentityServer(idsrvOptions);
        }
    }
}