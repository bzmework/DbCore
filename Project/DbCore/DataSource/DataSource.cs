
namespace DbCore
{
    /// <summary>
    /// 数据源
    /// </summary>
    public class DataSource
    {
        #region 成员变量

        private string name = "";
        private string type = "";
        private string provider = "";
        private string connectionString = "";

        #endregion

        #region 构造与析构

        /// <summary>
        /// 构造
        /// </summary>
        public DataSource()
        {
            this.name = "";
            this.type = "";
            this.provider = "";
            this.connectionString = "";
        }

        /// <summary>
        /// 析构
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        public DataSource(string name, string type, string provider, string connectionString)
        {
            this.name = name;
            this.type = type;
            this.provider = provider;
            this.connectionString = connectionString;
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
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// 类型(例如:SQLServer,Access,Oracle,Sqlite,MySql,...)
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// 数据提供器(例如:System.Data.Odbc,System.Data.OleDb,System.Data.SqlClient,System.Data.OracleClient,System.Data.SQLite,...)
        /// </summary>
        public string Provider
        {
            get
            {
                return this.provider;
            }
            set
            {
                this.provider = value;
            }
        }

        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }

        #endregion
    }
}
