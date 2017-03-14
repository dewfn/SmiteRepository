using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapFieldAttribute : Attribute
    {
        public MapFieldAttribute(string mapField)
        {
            this.MapFieldName = mapField;
        }
        public string MapFieldName { get; set; }
    }
    /// <summary>
    /// 可为空的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]    
    public class NullableAttribute : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Property)]
    public class IdentityAttribute : Attribute
    {

    }   
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute():this(0)
        { 
        }
        public PrimaryKeyAttribute(int pkIndex)
        {
            this.PKIndex = pkIndex;
        }
        public int PKIndex { get; set; }
    }

    /// <summary>
    /// 与数据库忽略的字段
    /// </summary>
    public class IgnoreAttribute : Attribute
    { 
        
    }  
}
