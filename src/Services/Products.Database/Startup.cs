﻿using AutoMapper;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Products.Database.Data;
using Products.Database.Domain;
using Products.Database.Infrastructure;
using Products.Database.Model;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Database
{
    public class Startup
    {
        private readonly ILogger _logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Products.Database", Version = "v1" });
            });
            services.Configure<Settings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });
            services.AddScoped<IProductsDbContext, ProductsDbContext>();
            services.AddSingleton(RabbitHutch.CreateBus(Configuration.GetSection("RabbitConnection:ConnectionString").Value));
            services.AddSingleton<MessageDispatcher>();
            services.AddSingleton<MessagesConsumer>();


            //            services.AddDbContext<ProductsDbContext>
            //options.UseMySQL(
            //    Configuration.GetConnectionString("DefaultConnection")),
            //    ServiceLifetime.Transient
            //    );

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper autoMapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                autoMapper.ConfigurationProvider.AssertConfigurationIsValid();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products.Database");
                //c.ConfigureOAuth2("swagger", "secret".Sha256(), "swagger");
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

            //var consumer = app.ApplicationServices.GetService<MessagesConsumer>();
            //bus.SubscribeAsync<ProductDTO>("ProductMessageService", async message =>
            //{
            //    await consumer.ConsumeAsync(message);
            //});
        }
    }
}
