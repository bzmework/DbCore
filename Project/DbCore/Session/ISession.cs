using System;
using System.Data;
using System.Collections.Generic;

namespace DbCore
{
    /// <summary>
    /// 会话接口
    /// </summary>
    public interface ISession: IDisposable
    {
    	#region Property
    	
    	/// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 类型
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 数据提供器(例如:System.Data.Odbc,System.Data.OleDb,System.Data.SqlClient,System.Data.OracleClient,System.Data.SQLite,System.Data.MySql,...)
        /// </summary>
        string Provider { get; }
       
        /// <summary>
        /// 连接串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
    	/// 参数
    	/// </summary>
    	IParameter Parameter { get; }
        
        #endregion
        
        #region Execute

        /// <summary>
        /// 执行一不返回记录的命令
        /// </summary>
        /// <param name="sql">SQL操作命令</param>
        /// <returns>返回命令影响到的行数</returns>
        int Execute(string sql);

        /// <summary>
        /// 执行一不返回记录的命令
        /// </summary>
        /// <param name="commandType">命令类型(存储过程、表或SQL命令文本)</param>
        /// <param name="commandText">存储过程名、表名或SQL操作命令</param>
        /// <returns>返回命令影响到的行数</returns>
        int Execute(CommandType commandType, string commandText);

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>返回结果集</returns>
        DataTable ExecuteQuery(string sql);

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="tableName">用于表映射的源表的名称</param>
        /// <returns>返回结果集</returns>
        DataTable ExecuteQuery(string sql, string tableName);

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="startRecord">从其开始的从零开始的记录号</param>
        /// <param name="maxRecords">要检索的最大记录数</param>
        /// <param name="tableName">用于表映射的源表的名称</param>
        /// <returns>返回结果集</returns>
        DataTable ExecuteQuery(string sql, int startRecord, int maxRecords, string tableName);

        /// <summary>
        /// 执行SQL查询, 将结果集转换为实体集合
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>实体集合</returns>
        IList<T> ExecuteQuery<T>(string sql) where T : class;

        #endregion

        #region ExecuteReader

        /// <summary>
        /// 返回SQL查询,返回一个DbDataReader对象
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>返回一个DbDataReader对象</returns>
        IDataReader ExecuteReader(string sql);

        /// <summary>
        /// 返回SQL查询,返回一个DbDataReader对象
        /// </summary>
        /// <param name="commandType">命令类型(存储过程、表或SQL命令文本)</param>
        /// <param name="commandText">存储过程名、表名或SQL操作命令</param>
        /// <returns>返回一个DbDataReader对象</returns>
        IDataReader ExecuteReader(CommandType commandType, string commandText);

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>返回结果集中第一行的第一列</returns>
        object ExecuteScalar(string sql);

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="commandType">命令类型(存储过程、表或SQL命令文本)</param>
        /// <param name="commandText">存储过程名、表名或SQL操作命令</param>
        /// <returns>返回结果集中第一行的第一列</returns>
        object ExecuteScalar(CommandType commandType, string commandText);

        #endregion

        #region Transaction

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>成功返回true,失败返回false</returns>
        bool BeginTrans();

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns>成功返回true,失败返回false</returns>
        bool CommitTrans();

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns>成功返回true,失败返回false</returns>
        bool RollbackTrans();
        
        #endregion
    }
}
