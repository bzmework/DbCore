using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using log4net;
using System.Globalization;

namespace DbCore
{
    /// <summary>
    /// 数据库会话
    /// </summary>
    internal class DbSession : ISession
	{
		#region 成员变量

        private static readonly ILog log = LogManager.GetLogger(typeof(DbSession));
        private string name = "";
        private string type = "";
        private string provider = "";
        private string connectionString = "";

        private DbConnection conn = null;
        private DbCommand cmd = null;
        private DbDataAdapter da = null;
        private DbTransaction trans = null;
        private Parameter param = null;

        #endregion

        #region 构造与析构

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="dbProvider">数据库提供程序</param>
        public DbSession(DataSource dataSource, DbProvider dbProvider)
        {
            //设置属性
            this.name = dataSource.Name;
            this.type = dataSource.Type;
            this.provider = dataSource.Provider;
            this.connectionString = dataSource.ConnectionString;
            
            //创建连接、命令、数据适配器、参数对象
            try
            {
                this.conn = dbProvider.Factory.CreateConnection();
                this.conn.ConnectionString = this.connectionString;
                this.cmd = dbProvider.Factory.CreateCommand();
                this.da = dbProvider.Factory.CreateDataAdapter();
                this.param = new Parameter();
            }
            catch(Exception e)
            {
                this.WriteLog(e);
                this.Dispose(da);
                this.Dispose(cmd);
                this.Dispose(conn);
                this.Dispose(param);
                throw e;
            }
        }

        /// <summary>
        /// 卸载资源。
        /// 这个析构函数只有在Dispose方法没有被调用时才会运行。
        /// </summary>
        ~DbSession()
        {
            // 不要在这里创建销毁(Dispose)代码清理资源。
            // 调用Dispose(false)在可读性和可维护性方面是最优的。
            Dispose(false);
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 销毁。不要使此方法为虚方法。不应让派生类能重写此方法。
        /// </summary>
        public void Dispose()
        {
            // 不要在这里创建销毁(Dispose)代码清理资源。
            // 调用Dispose(true)在可读性和可维护性方面是最优的。
            Dispose(true);

            // 从终止队列(Finalization queue)中退出，
            // 防止此对象的终止(finalization)代码第二次执行。
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 在两个不同的场景中执行Dispose(bool Dispose)：
        /// 如果dispose = true，则该方法已被用户代码直接或间接调用。可以释放托管和非托管资源(Managed and unmanaged resources)。
        /// 如果dispose = false，则运行时已从终结器(finalizer,即析构函数)内部调用该方法，您不应再引用其他对象，只能释放非托管资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // 如果disposing=true，在这里销毁所有托管和非托管资源(managed and unmanaged resources)。
                if (disposing)
                {
                    if (this.trans != null) this.trans.Dispose();
                    if (this.da != null) this.da.Dispose();
                    if (this.cmd != null) this.cmd.Dispose();
                    if (this.conn != null) this.conn.Dispose();
                    if (this.param != null) this.param.Dispose();
                    this.trans = null;
                    this.da = null;
                    this.cmd = null;
                    this.conn = null;
                    this.param = null;
                }

                // 如果disposing=false，只能释放非托管资源(unmanaged resources)。在这里添加代码释放非托管资源。例如：
                // CloseHandle(handle);
                // handle = IntPtr.Zero;
            }

            // 注意，这不是线程安全的。
            // 在释放标志disposed设置为true之前, 另一个线程可以在托管资源被释放后开始释放对象。
            // 如果想要线程安全(thread safe)，则必须由客户端(client)实现并保证线程安全。
            disposed = true;
        }
        private bool disposed = false;

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
        /// 类型
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// 数据提供器
        /// </summary>
        public string Provider
        {
            get
            {
                return this.provider;
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
    	
    	/// <summary>
    	/// 参数
    	/// </summary>
        public IParameter Parameter
        {
        	get
        	{
                this.OpenConn();
        		this.cmd.Connection = this.conn;
        		this.param.Command = this.cmd;
        		return this.param;
        	}
        }
        
        #endregion

        #region Execute

        /// <summary>
        /// 执行一不返回记录的命令
        /// </summary>
        /// <param name="sql">SQL操作命令</param>
        /// <returns>返回命令影响到的行数，失败返回0</returns>
        public int Execute(string sql)
        {
        	int ret = 0;

            try
            {
                this.OpenConn();
                this.cmd.Connection = this.conn;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.CommandText = sql;
                ret = this.cmd.ExecuteNonQuery();
                this.CloseConn();
            }
            catch (Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return ret;
        }

        /// <summary>
        /// 执行一不返回记录的命令
        /// </summary>
        /// <param name="commandType">命令类型(存储过程、表或SQL命令文本)</param>
        /// <param name="commandText">存储过程名、表名或SQL操作命令</param>
        /// <returns>返回命令影响到的行数，失败返回0</returns>
        public int Execute(CommandType commandType, string commandText)
        {
            int ret = 0;

            try
            {
                this.OpenConn();
                this.cmd.Connection = this.conn;
                this.cmd.CommandType = commandType;
                this.cmd.CommandType = CommandType.Text;
                ret = this.cmd.ExecuteNonQuery();
                this.CloseConn();
            }
            catch (Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return ret;
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>返回结果集，失败返回null</returns>
        public DataTable ExecuteQuery(string sql)
        {
        	DataTable dt = null;

            try
            {
                this.OpenConn();
                this.cmd.Connection = this.conn;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.CommandText = sql;
                this.da.SelectCommand = this.cmd;
                using (DataSet ds = new DataSet())
                {
                    this.da.Fill(ds);
                    dt = ds.Tables[0];
                }
                this.CloseConn();
            }
            catch (Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }    
            return dt;
        }

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="tableName">用于表映射的源表的名称</param>
        /// <returns>返回结果集，失败返回null</returns>
        public DataTable ExecuteQuery(string sql, string tableName)
        {
            DataTable dt = null;

            try
            {
                this.OpenConn();
                this.cmd.Connection = this.conn;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.CommandText = sql;
                this.da.SelectCommand = this.cmd;
                using (DataSet ds = new DataSet())
                {
                    this.da.Fill(ds, tableName);
                    dt = ds.Tables[0];
                }
                this.CloseConn();
            }
            catch (Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return dt;
        }
 
        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="startRecord">从其开始的从零开始的记录号</param>
        /// <param name="maxRecords">要检索的最大记录数</param>
        /// <param name="tableName">用于表映射的源表的名称</param>
        /// <returns>返回结果集，失败返回null</returns>
        public DataTable ExecuteQuery(string sql, int startRecord, int maxRecords, string tableName)
        {
            DataTable dt = null;

            try
            {
                this.OpenConn();
                this.cmd.Connection = this.conn;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.CommandText = sql;
                this.da.SelectCommand = this.cmd;
                using (DataSet ds = new DataSet())
                {
                    this.da.Fill(ds, startRecord, maxRecords, tableName);
                    dt = ds.Tables[0];
                }
                this.CloseConn();
            }
            catch (Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return dt;
        }

        /// <summary>
        /// 执行SQL查询, 将结果集转换为实体集合
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>实体集合</returns>
        public IList<T> ExecuteQuery<T>(string sql) where T : class
        {
            IList<T> list = null;

            try
            {
                this.OpenConn();
                this.cmd.Connection = this.conn;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.CommandText = sql;
                IDataReader dr = this.cmd.ExecuteReader();
                using (dr)
                {
                    list = ConvertReader<T>(dr);
                }
                this.CloseConn();
            }
            catch (Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return list;
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// 返回SQL查询,返回一个DbDataReader对象
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>返回一个DbDataReader对象，失败返回null</returns>
        public IDataReader ExecuteReader(string sql)
        {
        	IDataReader dr = null;
            
            try
            {
                this.OpenConn();
                this.cmd.Connection = this.conn;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.CommandText = sql;
                dr = this.cmd.ExecuteReader();
                this.CloseConn();
            }
            catch(Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return dr;
        }

        /// <summary>
        /// 返回SQL查询,返回一个DbDataReader对象
        /// </summary>
        /// <param name="commandType">命令类型(存储过程、表或SQL命令文本)</param>
        /// <param name="commandText">存储过程名、表名或SQL操作命令</param>
        /// <returns>返回一个DbDataReader对象，失败返回null</returns>
        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
        	IDataReader dr = null;
            try
            {
                this.OpenConn();
            	this.cmd.Connection = this.conn;
                this.cmd.CommandType = commandType;
                this.cmd.CommandText = commandText;
                dr = this.cmd.ExecuteReader();
                this.CloseConn();
            }
            catch(Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return dr;
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>返回结果集中第一行的第一列，失败返回null</returns>
        public object ExecuteScalar(string sql)
        {
        	object ret = null;
            try
            {
                this.OpenConn();
            	this.cmd.Connection = this.conn;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.CommandText = sql;
                ret = this.cmd.ExecuteScalar();
                this.CloseConn();
            }
            catch(Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return ret;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="commandType">命令类型(存储过程、表或SQL命令文本)</param>
        /// <param name="commandText">存储过程名、表名或SQL操作命令</param>
        /// <returns>返回结果集中第一行的第一列，失败返回null</returns>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
        	object ret = null;
            try
            {
                this.OpenConn();
            	this.cmd.Connection = this.conn;
                this.cmd.CommandType = commandType;
                this.cmd.CommandText = commandText;
                ret = this.cmd.ExecuteScalar();
                this.CloseConn();
            }
            catch(Exception e)
            {
                this.WriteLog(e);
                this.CloseConn();
            }
            return ret;
        }

        #endregion

        #region Transaction

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public bool BeginTrans()
        {
        	bool ret = false;
            try
            {
                this.OpenConn();
            	this.trans = this.conn.BeginTransaction();
            	ret = (this.trans != null);
            }
            catch (Exception e)
            {
                this.Dispose(trans);
                this.WriteLog(e);
                this.CloseConn();
            }
            return ret;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public bool CommitTrans()
        {
        	bool ret = false;
            try
            {
                if (this.trans != null)
                {
                    this.trans.Commit();
                    this.Dispose(trans);
                    this.CloseConn();
                    ret = true;
                }
            }
            catch (Exception e)
            {
                this.WriteLog(e);
            }
            return ret;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        public bool RollbackTrans()
        {
        	bool ret = false;
            try
            {
                if (this.trans != null)
                {
                    this.trans.Rollback();
                    this.Dispose(trans);
                    this.CloseConn();
                    ret = true;
                }
            }
            catch (Exception e)
            {
                this.WriteLog(e);
                this.Dispose(trans);
                this.CloseConn();
            }
            return ret;
        } 

        #endregion

        #region Private
        
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        private void OpenConn()
        {
            if (this.conn.State == ConnectionState.Closed)
            {
                this.conn.Open();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void CloseConn()
        {
            try
            {
                if (this.trans == null && this.conn.State == ConnectionState.Open)
                {
                    this.conn.Close();
                }
            }
            catch (Exception e)
            {
                this.WriteLog(e);
            }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="obj"></param>
        private void Dispose(IDisposable obj)
        {
            if (obj != null)
            {
                try
                {
                    obj.Dispose();
                }
                catch (Exception e)
                {
                    this.WriteLog(e);
                }
                finally
                {
                    obj = null;
                }
            }
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="e"></param>
        private void WriteLog(Exception e)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(e);
            }
        }

        /// <summary>
        /// 转换ExecuteReader结果为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IList<T> ConvertReader<T>(IDataReader reader) where T : class
        {
            // 获得类型
            Type type = typeof(T);
            var dict = TypeCache.GetType(type);

            // 获得列名
            int count = reader.FieldCount;
            string[] colNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                colNames[i] = reader.GetName(i);
            }

            //int currentIndex = 0;
            //int startIndex = pageSize * pageIndex;
            IList<T> list = new List<T>();
            while (reader.Read())
            {
                //if (startIndex > currentIndex++)
                //    continue;

                //if (pageSize > 0 && (currentIndex - startIndex) > pageSize)
                //    break;

                T obj = Activator.CreateInstance(type) as T;
                for (int i = 0; i < colNames.Length; i++)
                {
                    string name = colNames[i];
                    if (dict.TryGetValue(name, out FieldObject field))
                    {
                        object val = reader.GetValue(i);
                        if (val != null && DBNull.Value.Equals(val) == false)
                        {
                            field.Value.SetValue(obj, ConvertValue(val, field.Value.FieldType));
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// 转换为目标类型值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ConvertValue(object value, Type type)
        {
            if (value == null)
                return null;

            // 转换成字符串
            if (type == typeof(string))
                return value.ToString();

            // 类型相同不转换
            Type valueType = value.GetType();
            Type targetType = Nullable.GetUnderlyingType(type) ?? type;
            if (valueType == targetType)
            {
                return value;
            }

            // 转换成Guid
            if (targetType == typeof(Guid) && valueType == typeof(string))
            {
                return new Guid(value.ToString());
            }

            // 转换成枚举型
            if (targetType.IsEnum) 
            {
                targetType = Enum.GetUnderlyingType(targetType);
                if (value is string)
                {
                    return Enum.Parse(targetType, (string)value, true);
                }
                if (IsInteger(value))
                {
                    return Enum.ToObject(targetType, value);
                }
            }

            // 转换成基础类型
            switch (System.Type.GetTypeCode(targetType))
            {
                case TypeCode.DateTime: return C2Date(value);
                case TypeCode.Decimal: return C2Dec(value);
                case TypeCode.Double: return C2Dbl(value);
                case TypeCode.Single: return C2Flt(value);
                case TypeCode.Int64: return C2Lng(value);
                case TypeCode.Int32: return C2Int(value);
                case TypeCode.Int16: return C2Short(value);
                case TypeCode.UInt64: return C2Lng(value);
                case TypeCode.UInt32: return C2Int(value);
                case TypeCode.UInt16: return C2Short(value);
                case TypeCode.Byte: return C2Byte(value);
                case TypeCode.SByte: return C2SByte(value);
                case TypeCode.Char: return C2Chr(value);
                case TypeCode.Boolean: return C2Bool(value);
            }

            // 尝试改变类型
            try
            {
                // 尝试改变成目标类型
                return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                // 再次尝试转换成目标类型
                if (targetType.IsAssignableFrom(valueType))
                {
                    return value;
                }
            }

            // 转换失败返回默认值
            return null;
        }

        /// <summary>
		/// 是否为整型
		/// </summary>
		/// <param name="value">要判定的值</param>
		/// <returns></returns>
		private static bool IsInteger(object value)
        {
            if (value == null)
                return false;

            switch (System.Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return int.TryParse(value.ToString(), out _);
            }
        }

        /// <summary>
        /// 将object转换成bool(System.Boolean)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static bool C2Bool(object value)
        {
            if (value == null)
            {
                return false;
            }
            try
            {
                return System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                var str = value.ToString();
                if (bool.TryParse(str, out var result))
                {
                    return result;
                }
                else
                {
                    if (double.TryParse(str, out var dblVal))
                    {
                        return dblVal > 0 ? true : false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 将object转换成sbyte(System.SByte)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static sbyte C2SByte(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToSByte(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (sbyte.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成sbyte(System.Byte)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static byte C2Byte(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToByte(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (byte.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成short(System.Int16)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static short C2Short(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToInt16(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                var str = value.ToString();
                if (short.TryParse(str, out var result))
                {
                    return result;
                }
                else
                {
                    if (double.TryParse(str, out var dblVal))
                    {
                        return (short)dblVal;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成ushort(System.UInt16)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static ushort C2UShort(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                var str = value.ToString();
                if (ushort.TryParse(str, out var result))
                {
                    return result;
                }
                else
                {
                    if (double.TryParse(str, out var dblVal))
                    {
                        return (ushort)dblVal;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成int(System.Int32)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static int C2Int(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                var str = value.ToString();
                if (int.TryParse(str, out var result))
                {
                    return result;
                }
                else
                {
                    // 如果字符串中包含"."或","等分隔符号时可尝试转换成double，然后强制转换成int
                    if (double.TryParse(str, out var dblVal))
                    {
                        return (int)dblVal;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成uint(System.UInt32)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static uint C2UInt(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                var str = value.ToString();
                if (uint.TryParse(str, out var result))
                {
                    return result;
                }
                else
                {
                    if (double.TryParse(str, out var dblVal))
                    {
                        return (uint)dblVal;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成long(System.Int64)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static long C2Lng(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                var str = value.ToString();
                if (long.TryParse(str, out var result))
                {
                    return result;
                }
                else
                {
                    if (double.TryParse(str, out var dblVal))
                    {
                        return (long)dblVal;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// 将object转换成long(System.UInt64)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static ulong C2ULng(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                var str = value.ToString();
                if (ulong.TryParse(str, out var result))
                {
                    return result;
                }
                else
                {
                    if (double.TryParse(str, out var dblVal))
                    {
                        return (ulong)dblVal;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成double(System.Double)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static double C2Dbl(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (double.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成float(System.Single)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static float C2Flt(object value)
        {
            if (value == null) return 0;
            try
            {
                return System.Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (float.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成decimal(System.Decimal)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static decimal C2Dec(object value)
        {
            if (value == null) return 0;
            try
            {
                return System.Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (decimal.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将object转换成DateTime
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static DateTime C2Date(object value)
        {
            if (value == null) return default;
            try
            {
                return System.Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (DateTime.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }
            return default(DateTime);
        }

        /// <summary>
        /// 将object转换成string(System.String)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static string C2Str(object value)
        {
            if (value == null) return "";
            try
            {
                return System.Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 将object转换成char(System.Char)
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static char C2Chr(object value)
        {
            if (value == null) return default;
            try
            {
                return System.Convert.ToChar(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (char.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }
            return '\0';
        }

        /// <summary>
        /// 将object转换成数据库非空字符串
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static string C2Dbs(object value)
        {
            if (value == null) return " ";
            try
            {
                string val = System.Convert.ToString(value, CultureInfo.InvariantCulture);
                return string.IsNullOrEmpty(val) ? " " : val;
            }
            catch
            {
                return " ";
            }
        }

        #endregion
    }
}
