using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DbCore
{
    /// <summary>
    /// 类型缓存
    /// </summary>
    public class TypeCache
    {
        private static BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
        private static ConcurrentDictionary<string, Dictionary<string, FieldObject>> typeFields = new ConcurrentDictionary<string, Dictionary<string, FieldObject>>();

        /// <summary>
        /// 获得类型的字段信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, FieldObject> GetType(Type type)
        {
            if (!typeFields.TryGetValue(type.FullName, out Dictionary<string, FieldObject> fields) || fields == null)
            {
                FieldInfo[] infos = type.GetFields(bindingAttr);
                fields = new Dictionary<string, FieldObject>(infos.Length, StringComparer.OrdinalIgnoreCase);
                foreach (FieldInfo info in infos)
                {
                    var field = new FieldObject 
                    { 
                        Name = info.Name,
                        Value = info
                    };
                    fields[field.Name] = field;
                }
                typeFields[type.FullName] = fields;
            }

            return fields;
        }
    }

    /// <summary>
    /// 字段对象
    /// </summary>
    public class FieldObject
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo Value { get; set; }
    }

}
