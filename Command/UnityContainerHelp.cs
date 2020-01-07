using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using Resources.Properties;
using Unity;

namespace Command
{
    public static class UnityContainerHelp
    {
        private static readonly IUnityContainer Container = new UnityContainer();
        private static Settings Settings { get; } = new Settings();

        static UnityContainerHelp()
        {
            //Container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            Container.LoadConfiguration(section, "FirstClass");
            Container.LoadConfiguration(section, Settings.DBType != "MongoDB" ? "EF_SQL" : "EF_MongoDB");
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
