using System;
using System.Collections;
using System.Collections.Generic;

namespace DbCore
{
    /// <summary>
    /// 持久层管理器
    /// </summary>
    public class PersistentManager
    {
        #region 成员变量
        
        private DataSources dataSources = null;
        private DbProviders providers = null;
        private static PersistentManager instance = null;
        private static readonly object objectLock = new object();

        #endregion

        #region 构造与析构

        /// <summary>
        /// 私有构造函数，防止实例化
        /// </summary>
        private PersistentManager()
        {
            log4net.Config.BasicConfigurator.Configure(true, true);
        	this.dataSources = new DataSources();
            this.providers = new DbProviders();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        ~PersistentManager()
        {
            this.dataSources.Clear();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 数据源集合
        /// </summary>
        public DataSources DataSources
        {
            get
            {
                return this.dataSources;
            }
        }

        /// <summary>
        /// 数据库提供程序集合
        /// </summary>
        public DbProviders Providers
        {
            get
            {
                return this.providers;
            }
        }

        /// <summary>
        /// 取得PersistentManager实例对象
        /// </summary>
        public static PersistentManager Instance
        {
            get
            {
                // 使用双检锁模式，保证多线程应用模式下只存在唯一实例。
                if (instance == null) // 保证了性能
                {
                    lock (objectLock) // 保证了线程安全
                    {
                        if (instance == null) // 保证了只有一个实例被创建
                        {
                            instance = new PersistentManager();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 打开一个会话
        /// </summary>
        /// <param name="name">数据源名称</param>
        /// <returns>返回打开的会话对象</returns>
        public ISession OpenSession(string name)
        {
            DataSource dataSource = null;
            DbProvider dbProvider = null;

            if (!this.dataSources.Exists(name, ref dataSource))
            {
                throw new Exception($"打开会话失败，未找到数据源：{name}");
            }
            else
            {
                if(!this.providers.Exists(dataSource.Provider, ref dbProvider))
                {
                    throw new Exception($"打开会话失败，未找到数据库提供程序：{dataSource.Provider}");
                }
                return new DbSession(dataSource, dbProvider);
            }
        }

        #endregion
    }
}
