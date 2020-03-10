using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Data.Common;
using log4net;

namespace DbCore
{
    /// <summary>
    /// 数据源集合
    /// </summary>
    public class DataSources : IEnumerable<DataSource>
    {
        #region 成员变量

        private static readonly ILog log = LogManager.GetLogger(typeof(DbSession));
        private string configFile = "";
        private Dictionary<string, DataSource> dataSources = null;

        #endregion

        #region 属性

        /// <summary>
        /// 返回数据源配置文件
        /// </summary>
        public string ConfigFile
        {
            get
            {
                return this.configFile;
            }
        }

        /// <summary>
        /// 数据源数量
        /// </summary>
        public int Count
        {
            get
            {
                return dataSources.Count;
            }
        }

        #endregion

        #region 构造与析构

        /// <summary>
        /// 构造
        /// </summary>
        public DataSources()
        {
            this.dataSources = new Dictionary<string, DataSource>();
            this.configFile = AppDomain.CurrentDomain.BaseDirectory + "Config\\DataSource.config";
            this.LoadDataSource();
        }

        /// <summary>
        /// 析构
        /// </summary>
        ~DataSources()
        {
            this.dataSources.Clear();
        }

        #endregion

        #region 枚举器

        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DataSource> GetEnumerator()
        {
            foreach (KeyValuePair<string, DataSource> currentDataSource in this.dataSources)
            {
                yield return currentDataSource.Value;
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
            return this.dataSources.ContainsKey(name);
        }

        /// <summary>
        /// 数据源是否存在
        /// </summary>
        /// <param name="name">数据源名称</param>
        /// <param name="dataSource">返回当前找到的数据源</param>
        /// <returns>存在返回true,否则返回false</returns>
        public bool Exists(string name, ref DataSource dataSource)
        {
            bool exist = this.dataSources.ContainsKey(name);
            if (exist) dataSource = (DataSource)this.dataSources[name];
            return exist;
        }

        /// <summary>
        /// 增加一个数据源
        /// </summary>
        /// <param name="name">名称(必须唯一)</param>
        /// <param name="type">类型(例如:SQLServer,Access,Oracle,Sqlite,MySql,...)</param>
        /// <param name="provider">数据提供器(例如:System.Data.Odbc,System.Data.OleDb,System.Data.SqlClient,System.Data.OracleClient,System.Data.SQLite,...)</param>
        /// <param name="connectionString">连接串(关于连接串参见http://www.connectionstrings.com)</param>
        /// <returns>返回新增加的数据源对象</returns>
        public DataSource Add(string name, string type, string provider, string connectionString)
        {
            DataSource dataSource = null;
            if (this.Exists(name, ref dataSource))
            {
                return dataSource;
            }
            else
            {
                dataSource = new DataSource(name, type, provider, connectionString);
                dataSources.Add(name, dataSource);
                return dataSource;
            }
        }

        /// <summary>
        /// 增加一个数据源
        /// </summary>
        /// <param name="dataSource">数据源对象</param>
        /// <returns>成功返回真</returns>
        public bool Add(DataSource dataSource)
        {
            bool isOk = false;
            if (dataSource != null)
            {
                if (!this.Exists(dataSource.Name))
                {
                    this.dataSources.Add(dataSource.Name, dataSource);
                    isOk = true;
                }
            }
            return isOk;
        }

        /// <summary>
        /// 删除一个数据源
        /// </summary>
        /// <param name="name">名称</param>
        public void Remove(string name)
        {
            this.dataSources.Remove(name);
        }

        /// <summary>
        /// 删除一个数据源
        /// </summary>
        /// <param name="dataSource">数据源对象</param>
        public void Remove(DataSource dataSource)
        {
            this.dataSources.Remove(dataSource.Name);
        }

        /// <summary>
        /// 清除数据源列表
        /// </summary>
        public void Clear()
        {
            this.dataSources.Clear();
        }

        /// <summary>
        /// 刷新数据源列表
        /// </summary>
        public void Refresh()
        {
            this.LoadDataSource();
        }

        /// <summary>
        /// 保存数据源配置列表到配置文件
        /// </summary>
        /// <returns>成功返回true,否则返回false</returns>
        public bool Save()
        {
            if (this.dataSources.Count == 0) return false;
            if (!File.Exists(this.configFile)) return false;
            StringBuilder connfig = new StringBuilder();

            connfig.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            connfig.AppendLine("<DataSources>");
            foreach (DataSource dataSource in this.dataSources.Values)
            {
                connfig.AppendLine("<DataSource>");
                connfig.AppendFormat("<Name>{0}</Name>\r\n", dataSource.Name);
                connfig.AppendFormat("<Type>{0}</Type>\r\n", dataSource.Type);
                connfig.AppendFormat("<Provider>{0}</Provider>\r\n", dataSource.Provider);
                connfig.AppendFormat("<ConnectionString>{0}</ConnectionString>\r\n", dataSource.ConnectionString);
                connfig.AppendLine("</DataSource>");
            }
            connfig.AppendLine("</DataSources>");

            StreamWriter sw = new StreamWriter(this.configFile);
            sw.Write(connfig.ToString());
            sw.Close();
            sw.Dispose();
            return true;
        }

        #endregion

        #region Private

        /// <summary>
        /// 从配置文件加载数据源列表
        /// </summary>
        private bool LoadDataSource()
        {
            if (!File.Exists(this.configFile)) return false;
            XmlDocument xmlDom = new XmlDocument();
            xmlDom.PreserveWhitespace = false;
            xmlDom.Load(this.configFile);
            XmlNodeList xmlNodeList = xmlDom.SelectNodes("DataSources/DataSource");
            if (xmlNodeList == null) return false;
            if (xmlNodeList.Count == 0) return false;

            this.dataSources.Clear();
            foreach (XmlNode n in xmlNodeList)
            {
                DataSource dataSource = new DataSource();
                foreach (XmlNode s in n.ChildNodes)
                {
                    if (s.InnerText.Trim() != "")
                    {
                        switch (s.Name.ToLower())
                        {
                            case "name":
                                dataSource.Name = s.InnerText;
                                break;
                            case "type":
                                dataSource.Type = s.InnerText;
                                break;
                            case "provider":
                                dataSource.Provider = s.InnerText;
                                break;
                            case "connectionstring":
                                dataSource.ConnectionString = s.InnerText;
                                if(dataSource.ConnectionString.Contains("{AppPath}"))
                                {
                                    dataSource.ConnectionString.Replace("{AppPath}", AppDomain.CurrentDomain.BaseDirectory);
                                }
                                break;
                        }
                    }
                }
                if ((dataSource.Name != "") && (!this.dataSources.ContainsKey(dataSource.Name)))
                {
                    this.dataSources.Add(dataSource.Name, dataSource);
                }
            }
            return true;
        }
        #endregion
    }
}
