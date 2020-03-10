using System;
using System.Data;
using System.Data.Common;

namespace DbCore
{
	/// <summary>
	/// 参数
	/// </summary>
	internal class Parameter: IParameter
	{
		private DbCommand cmd = null;

		#region 构造与析构

		/// <summary>
		/// 构造
		/// </summary>
		public Parameter()
		{
			//
		}

		/// <summary>
		/// 卸载资源。
		/// 这个析构函数只有在Dispose方法没有被调用时才会运行。
		/// </summary>
		~Parameter()
		{
			// 不要在这里创建销毁(Dispose)代码清理资源。
			// 调用Dispose(false)在可读性和可维护性方面是最优的。
			Dispose(false);
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
					// this.cmd = null;
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

		/// <summary>
		/// 设置命令对象
		/// </summary>
		internal DbCommand Command
		{
			set
			{
				this.cmd = value;
			}
		}

		/// <summary>
		/// 设置Sql命令文本
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
		/// 增加参数
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">值</param>
		/// <param name="type">数据类型</param>
		/// <param name="direction">数据方向</param>
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
		/// 参数值
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object Value(string name)
		{
			DbParameter param = (DbParameter)this.cmd.Parameters[name];
			return param.Value;
		}
		
		/// <summary>
		/// 执行
		/// </summary>
		/// <returns></returns>
		public bool Excute()
		{
			return (this.cmd.ExecuteNonQuery() != 0);
		}
        
		/// <summary>
		/// 将类型转换成数据库数据类型
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
		/// 将方向转换成参数方向
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
