using System.Reflection;

namespace FilesExplorerInDB_WPF
{
    /// <summary>
    /// 使用此类前需要把[assembly: ComVisible(false)]改为true
    /// </summary>
    public class AssemblyInformation
    {
        private static AssemblyInformation _assemblyInformation;

        private AssemblyInformation()
        {
        }

        public static AssemblyInformation GetAssemblyInformation()
        {
            return _assemblyInformation ?? (_assemblyInformation = new AssemblyInformation());
        }

        #region 程序集属性访问器

        /// <summary>
        /// 标题
        /// </summary>
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// 版本
        /// </summary>
        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 说明
        /// </summary>
        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute) attributes[0]).Description;
            }
        }

        /// <summary>
        /// 产品
        /// </summary>
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute) attributes[0]).Product;
            }
        }

        /// <summary>
        /// 版权
        /// </summary>
        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// 公司
        /// </summary>
        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute) attributes[0]).Company;
            }
        }

        /// <summary>
        /// 配置
        /// </summary>
        public string AssemblyConfiguration
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyConfigurationAttribute) attributes[0]).Configuration;
            }
        }

        /// <summary>
        /// 商标
        /// </summary>
        public string AssemblyTrademark
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyTrademarkAttribute) attributes[0]).Trademark;
            }
        }

        /// <summary>
        /// 文化
        /// </summary>
        public string AssemblyCulture
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCultureAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCultureAttribute) attributes[0]).Culture;
            }
        }

        #endregion
    }
}
