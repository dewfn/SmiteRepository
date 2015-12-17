using SmiteRepository.ORMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmiteRepository.Command
{
    internal class ConvertHelper
    {
        /// <summary>  
        /// 利用反射和泛型  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        internal static List<T> ConvertToList<T>(IDataReader reader) where T : new()
        {
          
            EntityMeta meta = EntityReflect.GetDefineInfoFromType(typeof(T));
            // 定义集合  
            List<T> ts = new List<T>();
            T t;
            IPropertyManager pm= SqlMapper.getInstance().ValueViaEmit(meta.EntityType);
            
            while (reader.Read())
            {
                 t = new T();
                 for (int i = 0; i < reader.FieldCount; i++)
                 {
                     string cName = reader.GetName(i);
                     if (cName == Define.MYROWID)
                         continue;
                     EntityColumnMeta cm = meta.Columns.Find(_ => string.Equals(_.ColumnName, cName, StringComparison.OrdinalIgnoreCase));//EntityColumnMeta 不做判断是因为 查出来的字段是设置进去的

                     object value = reader[i];
                     //如果非空，则赋给对象的属性  
                     if (value != DBNull.Value)
                         pm.SetValue(t, cm.FieldName, value);
                     //对象添加到泛型集合中  

                 }

                //对象添加到泛型集合中  
                ts.Add(t);
            }
            return ts;

        }

        internal static List<T> ConvertToList<T>(IDataReader reader, EntityMeta meta) where T : BaseEntity, new()
        {

            // 定义集合  
            List<T> ts = new List<T>();
            IPropertyManager pm = SqlMapper.getInstance().ValueViaEmit(meta.EntityType);
            T t ;
            while (reader.Read())
            {
                t = new T();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string cName = reader.GetName(i);
                    if (cName == Define.MYROWID)
                        continue;
                    EntityColumnMeta cm = meta.Columns.Find(_ => string.Equals(_.ColumnName, cName, StringComparison.OrdinalIgnoreCase));//EntityColumnMeta 不做判断是因为 查出来的字段是设置进去的

                    object value = reader[i];
                    //如果非空，则赋给对象的属性  
                    if (value != DBNull.Value)
                        pm.SetValue(t, cm.FieldName,value);
                    //对象添加到泛型集合中  

                }
                t.RemoveUpdateColumn();
                ts.Add(t);
            }
            return ts;

        }
    }
}
