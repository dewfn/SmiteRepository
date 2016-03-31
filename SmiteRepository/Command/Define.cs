using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SmiteRepository
{
    
    public class Define
    {
        public const string MYROWID = "SMITE_ROW_ID";
        public const string LOCKSTRING = " with(nolock) ";
    }
    internal enum OptionSqlType:byte {
      SqlServer=0,
      Oracle=2,
      Mysql=1
    }
}
