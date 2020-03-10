using System;
using System.Data;
using System.Data.Common;

namespace DbCore
{
	/// <summary>
	/// ����
	/// </summary>
	internal class Parameter: IParameter
	{
		private DbCommand cmd = null;

		#region ����������

		/// <summary>
		/// ����
		/// </summary>
		public Parameter()
		{
			//
		}

		/// <summary>
		/// ж����Դ��
		/// �����������ֻ����Dispose����û�б�����ʱ�Ż����С�
		/// </summary>
		~Parameter()
		{
			// ��Ҫ�����ﴴ������(Dispose)����������Դ��
			// ����Dispose(false)�ڿɶ��ԺͿ�ά���Է��������ŵġ�
			Dispose(false);
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
					// this.cmd = null;
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

		/// <summary>
		/// �����������
		/// </summary>
		internal DbCommand Command
		{
			set
			{
				this.cmd = value;
			}
		}

		/// <summary>
		/// ����Sql�����ı�
		/// </summary>
		public string Sql
		{
			set 
			{
				this.cmd.Parameters.Clear();
				this.cmd.CommandType = CommandType.Text;
				this.cmd.CommandText = value;
			}
		}
		
		/// <summary>
		/// ���Ӳ���
		/// </summary>
		/// <param name="name">����</param>
		/// <param name="value">ֵ</param>
		/// <param name="type">��������</param>
		/// <param name="direction">���ݷ���</param>
		public void Add(string name, object value, DataType type, DataDirection direction)
		{
			DbParameter param = this.cmd.CreateParameter();
			param.ParameterName = name;
			param.DbType = this.ConvertToDbType(type);
		 	param.Value = value;
		 	param.Direction = this.ConvertToParameterDirection(direction);
		 	this.cmd.Parameters.Add(param);
		}
		
		/// <summary>
		/// ����ֵ
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object Value(string name)
		{
			DbParameter param = (DbParameter)this.cmd.Parameters[name];
			return param.Value;
		}
		
		/// <summary>
		/// ִ��
		/// </summary>
		/// <returns></returns>
		public bool Excute()
		{
			return (this.cmd.ExecuteNonQuery() != 0);
		}
        
		/// <summary>
		/// ������ת�������ݿ���������
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private DbType ConvertToDbType(DataType type)
		{
			switch(type)
			{
				case DataType.AnsiString:
					return DbType.AnsiString;
				case DataType.Boolean:
					return DbType.Boolean;
				case DataType.Byte:
					return DbType.Byte;
				case DataType.Currency:
					return DbType.Currency;
				case DataType.Date:
					return DbType.Date;
				case DataType.DateTime:
					return DbType.DateTime;
				case DataType.Decimal:
					return DbType.Decimal;
				case DataType.Double:
					return DbType.Double;
				case DataType.Guid:
					return DbType.Guid;
				case DataType.Short:
					return DbType.Int16;
				case DataType.Int:
					return DbType.Int32;
				case DataType.Long:
					return DbType.Int64;
				case DataType.Numeric:
					return DbType.VarNumeric;
				case DataType.Object:
					return DbType.Object;
				case DataType.SByte:
					return DbType.SByte;
				case DataType.Single:
					return DbType.Single;
				case DataType.String:
					return DbType.String;
				case DataType.UShort:
					return DbType.UInt16;
				case DataType.UInt:
					return DbType.UInt32;
				case DataType.ULong:
					return DbType.UInt64;
				case DataType.Time:
					return DbType.Time;
				case DataType.Xml:
					return DbType.Xml;
				default:
					return DbType.Object;
			}
		}
		
		/// <summary>
		/// ������ת���ɲ�������
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		private ParameterDirection ConvertToParameterDirection(DataDirection direction)
		{
			switch (direction)
			{
				case DataDirection.Input:
					return ParameterDirection.Input;
				case DataDirection.Output:
					return ParameterDirection.Output;
				case DataDirection.InputOutput:
					return ParameterDirection.InputOutput;
				case DataDirection.ReturnValue:
					return ParameterDirection.ReturnValue;
				default:
					return ParameterDirection.Input;
			}
		}
	}
}
