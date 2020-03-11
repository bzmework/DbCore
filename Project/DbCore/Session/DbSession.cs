using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using log4net;
using System.Globalization;

namespace DbCore
{
    /// <summary>
    /// ���ݿ�Ự
    /// </summary>
    internal class DbSession : ISession
	{
		#region ��Ա����

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

        #region ����������

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="dataSource">����Դ</param>
        /// <param name="dbProvider">���ݿ��ṩ����</param>
        public DbSession(DataSource dataSource, DbProvider dbProvider)
        {
            //��������
            this.name = dataSource.Name;
            this.type = dataSource.Type;
            this.provider = dataSource.Provider;
            this.connectionString = dataSource.ConnectionString;
            
            //�������ӡ������������������������
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
        /// ж����Դ��
        /// �����������ֻ����Dispose����û�б�����ʱ�Ż����С�
        /// </summary>
        ~DbSession()
        {
            // ��Ҫ�����ﴴ������(Dispose)����������Դ��
            // ����Dispose(false)�ڿɶ��ԺͿ�ά���Է��������ŵġ�
            Dispose(false);
        }

        /// <summary>
        /// �رջỰ
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// ���١���Ҫʹ�˷���Ϊ�鷽������Ӧ������������д�˷�����
        /// </summary>
        public void Dispose()
        {
            // ��Ҫ�����ﴴ������(Dispose)����������Դ��
            // ����Dispose(true)�ڿɶ��ԺͿ�ά���Է��������ŵġ�
            Dispose(true);

            // ����ֹ����(Finalization queue)���˳���
            // ��ֹ�˶������ֹ(finalization)����ڶ���ִ�С�
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// ��������ͬ�ĳ�����ִ��Dispose(bool Dispose)��
        /// ���dispose = true����÷����ѱ��û�����ֱ�ӻ��ӵ��á������ͷ��йܺͷ��й���Դ(Managed and unmanaged resources)��
        /// ���dispose = false��������ʱ�Ѵ��ս���(finalizer,����������)�ڲ����ø÷���������Ӧ��������������ֻ���ͷŷ��й���Դ��
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // ���disposing=true�����������������йܺͷ��й���Դ(managed and unmanaged resources)��
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

                // ���disposing=false��ֻ���ͷŷ��й���Դ(unmanaged resources)����������Ӵ����ͷŷ��й���Դ�����磺
                // CloseHandle(handle);
                // handle = IntPtr.Zero;
            }

            // ע�⣬�ⲻ���̰߳�ȫ�ġ�
            // ���ͷű�־disposed����Ϊtrue֮ǰ, ��һ���߳̿������й���Դ���ͷź�ʼ�ͷŶ���
            // �����Ҫ�̰߳�ȫ(thread safe)��������ɿͻ���(client)ʵ�ֲ���֤�̰߳�ȫ��
            disposed = true;
        }
        private bool disposed = false;

        #endregion

        #region ����

        /// <summary>
        /// ����
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// �����ṩ��
        /// </summary>
        public string Provider
        {
            get
            {
                return this.provider;
            }
        }

        /// <summary>
        /// ���Ӵ�
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
    	/// ����
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
        /// ִ��һ�����ؼ�¼������
        /// </summary>
        /// <param name="sql">SQL��������</param>
        /// <returns>��������Ӱ�쵽��������ʧ�ܷ���0</returns>
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
        /// ִ��һ�����ؼ�¼������
        /// </summary>
        /// <param name="commandType">��������(�洢���̡����SQL�����ı�)</param>
        /// <param name="commandText">�洢��������������SQL��������</param>
        /// <returns>��������Ӱ�쵽��������ʧ�ܷ���0</returns>
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
        /// ִ��SQL��ѯ
        /// </summary>
        /// <param name="sql">SQL��ѯ���</param>
        /// <returns>���ؽ������ʧ�ܷ���null</returns>
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
        /// ִ��SQL��ѯ
        /// </summary>
        /// <param name="sql">SQL��ѯ���</param>
        /// <param name="tableName">���ڱ�ӳ���Դ�������</param>
        /// <returns>���ؽ������ʧ�ܷ���null</returns>
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
        /// ִ��SQL��ѯ
        /// </summary>
        /// <param name="sql">SQL��ѯ���</param>
        /// <param name="startRecord">���俪ʼ�Ĵ��㿪ʼ�ļ�¼��</param>
        /// <param name="maxRecords">Ҫ����������¼��</param>
        /// <param name="tableName">���ڱ�ӳ���Դ�������</param>
        /// <returns>���ؽ������ʧ�ܷ���null</returns>
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
        /// ִ��SQL��ѯ, �������ת��Ϊʵ�弯��
        /// </summary>
        /// <param name="sql">SQL��ѯ���</param>
        /// <returns>ʵ�弯��</returns>
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
        /// ����SQL��ѯ,����һ��DbDataReader����
        /// </summary>
        /// <param name="sql">SQL��ѯ���</param>
        /// <returns>����һ��DbDataReader����ʧ�ܷ���null</returns>
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
        /// ����SQL��ѯ,����һ��DbDataReader����
        /// </summary>
        /// <param name="commandType">��������(�洢���̡����SQL�����ı�)</param>
        /// <param name="commandText">�洢��������������SQL��������</param>
        /// <returns>����һ��DbDataReader����ʧ�ܷ���null</returns>
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
        /// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С������������к��н������ԡ�
        /// </summary>
        /// <param name="sql">SQL��ѯ���</param>
        /// <returns>���ؽ�����е�һ�еĵ�һ�У�ʧ�ܷ���null</returns>
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
        /// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С������������к��н������ԡ�
        /// </summary>
        /// <param name="commandType">��������(�洢���̡����SQL�����ı�)</param>
        /// <param name="commandText">�洢��������������SQL��������</param>
        /// <returns>���ؽ�����е�һ�еĵ�һ�У�ʧ�ܷ���null</returns>
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
        /// ��ʼ����
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
        /// �ύ����
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
        /// �ع�����
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
        /// ������
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
        /// �ر�����
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
        /// ���ٶ���
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
        /// д������־
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
        /// ת��ExecuteReader���Ϊʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IList<T> ConvertReader<T>(IDataReader reader) where T : class
        {
            // �������
            Type type = typeof(T);
            var dict = TypeCache.GetType(type);

            // �������
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
        /// ת��ΪĿ������ֵ
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ConvertValue(object value, Type type)
        {
            if (value == null)
                return null;

            // ת�����ַ���
            if (type == typeof(string))
                return value.ToString();

            // ������ͬ��ת��
            Type valueType = value.GetType();
            Type targetType = Nullable.GetUnderlyingType(type) ?? type;
            if (valueType == targetType)
            {
                return value;
            }

            // ת����Guid
            if (targetType == typeof(Guid) && valueType == typeof(string))
            {
                return new Guid(value.ToString());
            }

            // ת����ö����
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

            // ת���ɻ�������
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

            // ���Ըı�����
            try
            {
                // ���Ըı��Ŀ������
                return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                // �ٴγ���ת����Ŀ������
                if (targetType.IsAssignableFrom(valueType))
                {
                    return value;
                }
            }

            // ת��ʧ�ܷ���Ĭ��ֵ
            return null;
        }

        /// <summary>
		/// �Ƿ�Ϊ����
		/// </summary>
		/// <param name="value">Ҫ�ж���ֵ</param>
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
        /// ��objectת����bool(System.Boolean)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����sbyte(System.SByte)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����sbyte(System.Byte)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����short(System.Int16)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����ushort(System.UInt16)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����int(System.Int32)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
                    // ����ַ����а���"."��","�ȷָ�����ʱ�ɳ���ת����double��Ȼ��ǿ��ת����int
                    if (double.TryParse(str, out var dblVal))
                    {
                        return (int)dblVal;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// ��objectת����uint(System.UInt32)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����long(System.Int64)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����long(System.UInt64)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����double(System.Double)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����float(System.Single)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����decimal(System.Decimal)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����DateTime
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����string(System.String)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת����char(System.Char)
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
        /// ��objectת�������ݿ�ǿ��ַ���
        /// </summary>
        /// <param name="value">Ҫת����ֵ</param>
        /// <returns>ת�����ֵ</returns>
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
