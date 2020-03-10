using System;

namespace DbCore
{
	/// <summary>
	/// ��������
	/// </summary>
	public enum DataType
	{
		/// <summary>
		/// Ansi�ַ���
		/// </summary>
		AnsiString,
		/// <summary>
		/// ������
		/// </summary>
		Binary,
		/// <summary>
		/// ����
		/// </summary>
		Boolean,
		/// <summary>
		/// �ֽ�
		/// </summary>
		Byte,
		/// <summary>
		/// ����
		/// </summary>
		Currency,
		/// <summary>
		/// ����
		/// </summary>
		Date,
		/// <summary>
		/// ����ʱ��
		/// </summary>
		DateTime,
		/// <summary>
		/// ��ֵ
		/// </summary>
		Decimal,
		/// <summary>
		/// ˫����
		/// </summary>
		Double,
		/// <summary>
		/// GUID
		/// </summary>
		Guid,
		/// <summary>
		/// ������
		/// </summary>
		Short,
		/// <summary>
		/// ����
		/// </summary>
		Int,
		/// <summary>
		/// ������
		/// </summary>
		Long,
		/// <summary>
		/// ��ֵ
		/// </summary>
		Numeric,
		/// <summary>
		/// ����
		/// </summary>
		Object,
		/// <summary>
		/// �ֽ�
		/// </summary>
		SByte,
		/// <summary>
		/// ������
		/// </summary>
		Single,
		/// <summary>
		/// �ַ���
		/// </summary>
		String,
		/// <summary>
		/// �޷��Ŷ�����
		/// </summary>
		UShort,
		/// <summary>
		/// �޷�������
		/// </summary>
		UInt,
		/// <summary>
		/// �޷��ų�����
		/// </summary>
		ULong,
		/// <summary>
		/// ʱ��
		/// </summary>
		Time,
		/// <summary>
		/// Xml
		/// </summary>
		Xml
	}
	
	/// <summary>
	/// ���ݷ���
	/// </summary>
	public enum DataDirection
	{
		/// <summary>
		/// ����
		/// </summary>
		Input,
		/// <summary>
		/// ���
		/// </summary>
		Output,
		/// <summary>
		/// ����/���
		/// </summary>
		InputOutput,
		/// <summary>
		/// ����ֵ
		/// </summary>
		ReturnValue
	}
	
	/// <summary>
	/// �����ӿ�
	/// </summary>
	public interface IParameter: IDisposable
	{
		/// <summary>
		/// Sql�����ı�
		/// </summary>
		string Sql{set;}
		
		/// <summary>
		/// ���Ӳ���
		/// </summary>
		/// <param name="name">����</param>
		/// <param name="value">ֵ</param>
		/// <param name="type">��������</param>
		/// <param name="direction">���ݷ���</param>
		void Add(string name, object value, DataType type, DataDirection direction);
		
		/// <summary>
		/// ����ֵ
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object Value(string name);
		
		/// <summary>
		/// ִ��
		/// </summary>
		/// <returns></returns>
		bool Excute();
	}
}
