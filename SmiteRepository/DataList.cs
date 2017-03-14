using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SmiteRepository
{
      
  public class DataList
    {
        private string[] _colunms;

        public string[] Colunms
        {
            get { return _colunms; }
        }
        private List<object[]> _data;

        public List<object[]> Data
        {
            get { return _data; }
        }
        private int _colunmsSize;

        public int ColunmsSize
        {
            get { return _colunmsSize; }
        }

        public int RowCount
        {
            get { return (_data==null)?0:_data.Count; }
        }
        private bool IsFill;
        public void SetColunms(string[] columns) {
            if (columns != null && columns.Length > 0)
            {
                _colunms = columns;
                _colunmsSize = columns.Length;
                IsFill = true;
            }
        }
        public void AddData(object[] RowData)
        {
            if (!IsFill)
                throw new ORMException("没有设置列，数据不能添加!");
            if (RowData.Length != _colunmsSize)
                throw new ORMException("添加的数据与自身数据列数不一至!");
            _data.Add(RowData);
        }
        public void Fill(IDataReader reader) {
            _colunmsSize = reader.FieldCount;
            _colunms = new string[_colunmsSize];
            _data = new List<object[]>();
            for (int i = 0; i < _colunmsSize; i++)
            {
                _colunms[i] = reader.GetName(i);
            }
            while (reader.Read())
            {
                object[] vals=new object[_colunmsSize];
                reader.GetValues(vals);
                _data.Add(vals);
            }
            IsFill = true;
        }
       

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            bool f = true;
            foreach (object[] row in _data)
            {
                if (f)
                    f = false;
                else
                    sb.Append(",");
                
                sb.Append("{");
                for (int i = 0; i < _colunmsSize; i++)
                {
                    sb.AppendFormat("{0}\"{1}\":{2}", (i == 0) ? string.Empty : ",", _colunms[i], getValueByType(row[i]));
                }
                sb.Append("}");
            }
            sb.Append("]");
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
