using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SmiteRepository
{
     class CustomRow:Dictionary<string,object>
    {
       
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            bool f = true;
            sb.Append("{");
            foreach (var item in this)
            {
                if (f)
                    f = false;
                else
                    sb.Append(",");
                sb.AppendFormat("\"{0}\":{1}",  item.Key, getValueByType(item.Value));
               
            }
            sb.Append("}");
            return sb.ToString();

        }
        private string getValueByType(object val) {
            if (val is string || val is DateTime)
            {
                return string.Format("\"{0}\"",val);
            }
            if (val is bool)
                return val.ToString().ToLower();
            if (DBNull.Value == val)
                return "null";
            return val.ToString();
        }
        
    }
}
