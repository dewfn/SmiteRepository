using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository
{
   public class ORMException : Exception
    {
       
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="message"></param>
            public ORMException(string message)
                : base(message)
            { }
            public ORMException(string message,string sql)
                : base(message)
            { this.Sql = sql; }
            private string Sql;
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="message"></param>
            /// <param name="ex"></param>
            public ORMException(string message, Exception ex)
                : base(message, ex)
            { }
        
    }
}
