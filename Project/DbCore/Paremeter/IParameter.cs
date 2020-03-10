using System;

namespace DbCore
{
	/// <summary>
	/// 数据类型
	/// </summary>
	public enum DataType
	{
		/// <summary>
		/// Ansi字符串
		/// </summary>
		AnsiString,
		/// <summary>
		/// 二进制
		/// </summary>
		Binary,
		/// <summary>
		/// 布尔
		/// </summary>
		Boolean,
		/// <summary>
		/// 字节
		/// </summary>
		Byte,
		/// <summary>
		/// 货币
		/// </summary>
		Currency,
		/// <summary>
		/// 日期
		/// </summary>
		Date,
		/// <summary>
		/// 日期时间
		/// </summary>
		DateTime,
		/// <summary>
		/// 数值
		/// </summary>
		Decimal,
		/// <summary>
		/// 双精度
		/// </summary>
		Double,
		/// <summary>
		/// GUID
		/// </summary>
		Guid,
		/// <summary>
		/// 短整型
		/// </summary>
		Short,
		/// <summary>
		/// 整型
		/// </summary>
		Int,
		/// <summary>
		/// 长整型
		/// </summary>
		Long,
		/// <summary>
		/// 数值
		/// </summary>
		Numeric,
		/// <summary>
		/// 对象
		/// </summary>
		Object,
		/// <summary>
		/// 字节
		/// </summary>
		SByte,
		/// <summary>
		/// 单精度
		/// </summary>
		Single,
		/// <summary>
		/// 字符串
		/// </summary>
		String,
		/// <summary>
		/// 无符号短整型
		/// </summary>
		UShort,
		/// <summary>
		/// 无符号整型
		/// </summary>
		UInt,
		/// <summary>
		/// 无符号长整型
		/// </summary>
		ULong,
		/// <summary>
		/// 时间
		/// </summary>
		Time,
		/// <summary>
		/// Xml
		/// </summary>
		Xml
	}
	
	/// <summary>
	/// 数据方向
	/// </summary>
	public enum DataDirection
	{
		/// <summary>
		/// 输入
		/// </summary>
		Input,
		/// <summary>
		/// 输出
		/// </summary>
		Output,
		/// <summary>
		/// 输入/输出
		/// </summary>
		InputOutput,
		/// <summary>
		/// 返回值
		/// </summary>
		ReturnValue
	}
	
	/// <summary>
	/// 参数接口
	/// </summary>
	public interface IParameter: IDisposable
	{
		/// <summary>
		/// Sql命令文本
		/// </summary>
		string Sql{set;}
		
		/// <summary>
		/// 增加参数
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">值</param>
		/// <param name="type">数据类型</param>
		/// <param name="direction">数据方向</param>
		void Add(string name, object value, DataType type, DataDirection direction);
		
		/// <summary>
		/// 参数值
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object Value(string name);
		
		/// <summary>
		/// 执行
		/// </summary>
		/// <returns></returns>
		bool Excute();
	}
}
