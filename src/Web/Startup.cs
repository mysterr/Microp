using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using Web.Infrastructure;
using Web.Models;
using StackExchange.Redis;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Web
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //.AddApplicationPart((typeof(Web.Controllers.HomeController).Assembly));

            services.AddSingleton<IRepository<Product>, ProductRepository>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddDistributedRedisCache(options => 
            //    options.Configuration = Configuration.GetSection("Redis:ConnectionString").Value);
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetSection("Redis:ConnectionString").Value));
            services.AddSingleton<HttpClientHandler>();
            services.AddSignalR();

            var handler = services.BuildServiceProvider().GetService<HttpClientHandler>();
            handler.CookieContainer = new CookieContainer();
            // ignore SSL check 
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        System.Diagnostics.Debug.WriteLine(cert);
                        return true;
                    }
                    else
                        return policyErrors == SslPolicyErrors.None;
                };
            //handler.ClientCertificates.Add(certificate);

            services.AddHttpClient("ProductsDatabaseClient", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("DatabaseService:ConnectionString").Value);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).ConfigurePrimaryHttpMessageHandler(() => handler);

            services.AddHttpClient("ProductsQueryClient", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("QueueService:ConnectionString").Value);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).ConfigurePrimaryHttpMessageHandler(() => handler);

            services.AddSingleton<MessageDispatcher>();
            services.AddSingleton<MessagesConsumer>();
            services.AddSingleton(RabbitHutch.CreateBus(Configuration.GetSection("RabbitConnection:ConnectionString").Value));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSignalR(routes =>
            {
                routes.MapHub<StatHub>("/stats");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var bus = app.ApplicationServices.GetService<IBus>();
            var subscriber = new AutoSubscriber(bus, "ProductMessageService")
            {
                AutoSubscriberMessageDispatcher = app.ApplicationServices.GetService<MessageDispatcher>(),
            };
            // -- should use EasyNetQ version from 3.6.0 (3.0-3.5 doesn't work properly)
            try
            {
                Thread.Sleep(10000); // waiting for RabbitMQ server is loaded
                subscriber.Subscribe(new Assembly[] { GetType().Assembly });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
