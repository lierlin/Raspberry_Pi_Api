using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
            ////接口、程序集注入
            //builder.RegisterType<piDBContext>().SingleInstance();
            //builder.RegisterType<pi1DBContext>().SingleInstance();
            builder.RegisterModule(new AutofacModule.AutofacModule());
            ////加载程序集
            //var Model = Assembly.Load("Model");
            ////根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            //builder.RegisterAssemblyTypes(Model)
            //    .Where(t => t.Name.Contains("pi"))
            //    .AsImplementedInterfaces().PropertiesAutowired();

            ////加载程序集
            //var iServices = Assembly.Load("IServices");
            //var services = Assembly.Load("Services");
            ////根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            //builder.RegisterAssemblyTypes(iServices, services)
            //    .Where(t => t.Name.EndsWith("Services"))
            //    .AsImplementedInterfaces().PropertiesAutowired();

            // builder.RegisterType<tb_pi_test>().InstancePerDependency();
            //builder.RegisterType<PiTestServices>().As<IPiTestServices>();
        }
    }
}