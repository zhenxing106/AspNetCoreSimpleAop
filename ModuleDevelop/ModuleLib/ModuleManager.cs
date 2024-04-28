using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLib
{
    public class ModuleManager
    {
        private readonly IServiceCollection _services;

        private readonly List<IModule> modules = new List<IModule>();
        public ModuleManager()
        {
            if (_services != null)
            {
                Console.WriteLine(1);
            }
            var moduleTypes = GetModuleTypes();

            foreach (var moduleType in moduleTypes)
            {
                var moduleInstance = Activator.CreateInstance(moduleType) as IModule;
                if (moduleInstance != null && !modules.Contains(moduleInstance))
                    modules.Add(moduleInstance);
            }
        }

        public void LoadModules(IServiceCollection services)
        {
            modules.ForEach(m => m.ConfigureService(services));
        }

        public void Configures(IApplicationBuilder app)
        {
            modules.ForEach(m => m.Configure(app));
        }
        private IEnumerable<Type> GetModuleTypes()
        {
            string path = @"D:\LocalCode\GitHubStore\AspNetCoreSimpleAop\ModuleDevelop\Service\bin\Debug\net8.0\StartupService.dll";
            if (File.Exists(path))
            {
                var list = Assembly.LoadFrom(path)
              .GetTypes()
              .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                return list;
            }
            else
            {
                throw new Exception("对应的Service.dll 为找到");
            }
        }
    }
}
