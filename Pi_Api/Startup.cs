using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IRepository;
using IRepository.IBaseRepository;
using IServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Model.pi;
using Model.pi1;
using Repository;
using Repository.BaseRepository;
using Services;

namespace Pi_Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pi_Api", Version = "v1" });
            });
        }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
            }
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pi_Api v1"));

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Autofac引用
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //https://www.cnblogs.com/atomy/p/12834804.html
            //接口、程序集注入
            builder.RegisterType<piDBContext>().SingleInstance();
            builder.RegisterType<pi1DBContext>().SingleInstance();

            builder.RegisterType<tb_pi_test>().InstancePerDependency();
            builder.RegisterType<PiTestServices>().As<IPiTestServices>();
        }
    }
}