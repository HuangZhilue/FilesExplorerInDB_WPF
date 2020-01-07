using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using Unity;

namespace Command
{
    public static class UnityContainerHelp
    {
        private static readonly IUnityContainer Container = new UnityContainer();
        static UnityContainerHelp()
        {
            //Container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            Container.LoadConfiguration(section, "FirstClass");
            Container.LoadConfiguration(section, "EF_SQL");
        }

        public static T GetServer<T>()
        {
            return Container.Resolve<T>();
        }

        public static T GetServer<T>(string name)
        {
            return Container.Resolve<T>(name);
        }
    }
}
