using System;
using System.Reflection;
using Autofac;
using IServices.IBaseServices;
using Model.pi;
using Model.pi1;

namespace AutofacModule
{
    /// <summary>
    /// 容器注册类
    /// </summary>
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            // builder.RegisterType<HelloService>().As<IHelloService>().InstancePerDependency().AsImplementedInterfaces();

            //加载程序集
            var Model = Assembly.Load("Model");
            //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            builder.RegisterAssemblyTypes(Model)
                .Where(t => t.Name.Contains("pi"))
                .InstancePerDependency();

            var iRepository = Assembly.Load("IRepository");
            var repository = Assembly.Load("Repository");
            //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            builder.RegisterAssemblyTypes(iRepository, repository)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            //加载程序集
            var iServices = Assembly.Load("IServices");
            var services = Assembly.Load("Services");
            //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            builder.RegisterAssemblyTypes(iServices, services)
                .Where(t => t.Name.EndsWith("Services"))
                .AsImplementedInterfaces().PropertiesAutowired(new AutowiredPropertySelector());
        }
    }
}