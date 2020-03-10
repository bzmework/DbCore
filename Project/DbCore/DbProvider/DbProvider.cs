
using System.Data.Common;

namespace DbCore
{
    /// <summary>
    /// 数据库提供程序
    /// </summary>
    public class DbProvider
    {
        #region 成员变量

        private string name = "";
        private string fullName = "";
        private string assembly = "";
        private DbProviderFactory factory = null;

        #endregion

        #region 构造与析构

        /// <summary>
        /// 实例化
        /// </summary>
        public DbProvider()
        {
            this.name = "";
            this.fullName = "";
            this.assembly = "";
            this.factory = null;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="name">数据库提供程序名称</param>
        /// <param name="fullName">数据库提供程序全称</param>
        /// <param name="assembly">数据库提供程序程序集</param>
        /// <param name="factory">数据库提供程序工厂</param>
        public DbProvider(string name, string fullName, string assembly, DbProviderFactory factory)
        {
            this.name = name;
            this.fullName = fullName;
            this.assembly = assembly;
            this.factory = factory;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 全称
        /// </summary>
        public string FullName
        {
            get
            {
                return this.fullName;
            }
        }

        /// <summary>
        /// 程序集
        /// </summary>
        public string Assembly
        {
            get
            {
                return this.assembly;
            }
        }

        /// <summary>
        /// 数据库提供程序工厂
        /// </summary>
        public DbProviderFactory Factory
        {
            get
            {
                return this.factory;
            }
            set
            {
                this.factory = value;
            }
        }

        #endregion
    }
}
