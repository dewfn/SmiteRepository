using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string tableName)
        {
            this.TableName = tableName;
        }
        public string TableName { get; private set; }
    }
}
