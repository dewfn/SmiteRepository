using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository.Page
{
    public class PagedList<T> 
    {
        private List<T> _dataList;
        public virtual List<T> DataList
        {
            get
            {
                if (_dataList == null) _dataList = new List<T>();
                return _dataList;
            }
            set
            {
                _dataList = value;
            }
        }
        public virtual int PageSize
        {
            get;
            set;
        }
        public virtual int PageIndex
        {
            get;
            set;
        }
        public virtual int Total
        { get; set; }
    }
}
