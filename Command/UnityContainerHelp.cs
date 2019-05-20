using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Command
{
    public static class UnityContainerHelp
    {
        private static IUnityContainer container;
        static UnityContainerHelp()
        {
            container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            container.LoadConfiguration(section, "FirstClass");
        }

        public static T GetServer<T>()
        {
            return container.Resolve<T>();
        }

        public static T GetServer<T>(string Name)
        {
            return container.Resolve<T>(Name);
        }
    }
}
