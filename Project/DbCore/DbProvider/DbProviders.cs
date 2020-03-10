using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Data.Common;
using log4net;

namespace DbCore
{
    /// <summary>
    /// 数据库提供程序
    /// </summary>
    public class DbProviders : IEnumerable<DbProvider>
    {
        #region 成员变量

        private static readonly ILog log = LogManager.GetLogger(typeof(DbSession));
        private string providerPath = "";
        private Dictionary<string, DbProvider> providers = null;

        #endregion

        #region 属性

        /// <summary>
        /// 数据库提供程序路径
        /// </summary>
        public string ProviderPath
        {
            get
            {
                return this.providerPath;
            }
        }

        /// <summary>
        /// 数据库提供程序数量
        /// </summary>
        public int Count
        {
            get
            {
                return providers.Count;
            }
        }

        #endregion

        #region 构造与析构

        /// <summary>
        /// 构造
        /// </summary>
        public DbProviders()
        {
            this.providers = new Dictionary<string, DbProvider>();
            this.providerPath = AppDomain.CurrentDomain.BaseDirectory + "Providers";
            this.RegisterProviders();
        }

        /// <summary>
        /// 析构
        /// </summary>
        ~DbProviders()
        {
            this.providers.Clear();
        }

        #endregion

        #region 枚举器

        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DbProvider> GetEnumerator()
        {
            foreach (KeyValuePair<string, DbProvider> provider in this.providers)
            {
                yield return provider.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 数据源是否存在
        /// </summary>
        /// <param name="name">帐套名称</param>
        /// <returns>存在返回true,否则返回false</returns>
        public bool Exists(string name)
        {
            return this.providers.ContainsKey(name);
        }

        /// <summary>
        /// 数据库提供程序是否存在
        /// </summary>
        /// <param name="name">提供程序名称</param>
        /// <param name="provider">返回当前找到的提供程序</param>
        /// <returns>存在返回true,否则返回false</returns>
        public bool Exists(string name, ref DbProvider provider)
        {
            bool exist = this.providers.ContainsKey(name);
            if (exist) provider = (DbProvider)this.providers[name];
            return exist;
        }

        /// <summary>
        /// 增加一个数据库提供程序
        /// </summary>
        /// <param name="provider">数据库提供程序对象</param>
        /// <returns>成功返回真</returns>
        public bool Add(DbProvider provider)
        {
            bool isOk = false;
            if (provider != null)
            {
                if (!this.Exists(provider.Name))
                {
                    this.providers.Add(provider.Name, provider);
                    isOk = true;
                }
            }
            return isOk;
        }

        /// <summary>
        /// 删除一个数据库提供程序
        /// </summary>
        /// <param name="name">名称</param>
        public void Remove(string name)
        {
            this.providers.Remove(name);
        }

        /// <summary>
        /// 删除一个数据库提供程序
        /// </summary>
        /// <param name="provider">数据库提供程序对象</param>
        public void Remove(DbProvider provider)
        {
            this.providers.Remove(provider.Name);
        }

        /// <summary>
        /// 清除数据库提供程序列表
        /// </summary>
        public void Clear()
        {
            this.providers.Clear();
        }

        /// <summary>
        /// 刷新数据库提供程序列表
        /// </summary>
        public void Refresh()
        {
            this.RegisterProviders();
        }

        #endregion

        #region Private

        /// <summary>
        /// 注册数据库提供程序
        /// </summary>
        private void RegisterProviders()
        {
            //DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            //DbProviderFactories.RegisterFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
            //DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
            //DbProviderFactories.RegisterFactory("FirebirdSql.Data.FirebirdClient", FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
            //DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
            //DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
            //DbProviderFactories.RegisterFactory("System.Data.OracleClient", System.Data.OracleClient.OracleClientFactory.Instance);
            //DbProviderFactories.RegisterFactory("IBM.Data.DB2.Core", IBM.Data.DB2.Core.DB2Factory.Instance);
            //DbProviderFactories.RegisterFactory("AdoNetCore.AseClient", AdoNetCore.AseClient.AseClientFactory.Instance);

            if(Directory.Exists(this.providerPath))
            {
                var files = Directory.GetFiles(this.providerPath, "*.dll");
                foreach(var file in files)
                {
                    var assembly = Assembly.LoadFrom(file);
                    foreach(var typeInfo in assembly.ExportedTypes)
                    {
                        if (typeInfo.FullName.ToLower().Contains("factory")) // 是否包含数据库提供程序工厂
                        {
                            try
                            {
                                Type type = assembly.GetType(typeInfo.FullName, true, true); // 获得工厂类信息
                                if (type.GetMember("Instance").Length > 0) // 必须包含实例
                                {
                                    DbProviderFactory factory = type.InvokeMember("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetField, null, null, null) as DbProviderFactory; // 取得工程类实例
                                    var providerName = typeInfo.FullName.Substring(0, typeInfo.FullName.LastIndexOf("."));
                                    DbProvider provider = new DbProvider(providerName, assembly.FullName, file, factory);
                                    this.providers.Add(providerName, provider); // 注册
                                }
                            }
                            catch (Exception e)
                            {
                                Exception ex = new Exception($"注册数据库提供程序{typeInfo.Name}失败", e);
                                if (log.IsErrorEnabled)
                                {
                                    log.Error(ex);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
