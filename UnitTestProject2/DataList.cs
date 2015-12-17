using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.Text;
using System.IO;
namespace ConsoleApplication3
{
    public class DataList1
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
                throw new Exception("没有设置列，数据不能添加!");
            if (RowData.Length != _colunmsSize)
                throw new Exception("添加的数据与自身数据列数不一至!");
            _data.Add(RowData);
        }
        public void Fill(IDataReader reader) {
            _colunmsSize = reader.FieldCount;
            _colunms = new string[_colunmsSize];
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
       
        public static   string ToString()
        {
            return "";
        }
        public static string Parse(object writer)
        {
            return "111";
        }
        public static string Parse(DataList1 obj)
        {
            return "111";
        }
        public static DataList1 GetParseFnWriteObject(string json)
        {
           return  new DataList1() { _colunms =new string[] { "a","b"} };
        }
         

        
    }
}
