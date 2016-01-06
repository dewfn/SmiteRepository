using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SmiteRepository.Page
{
    public class PageView 
    {
        public PageView()
        {

        }
        public PageView(NameValueCollection form)
        {
            this.PageIndex = Convert.ToInt32(form["pageIndex"]) - 1;
            this.PageSize = Convert.ToInt32(form["pageSize"]);
            this.Total = Convert.ToInt32(form["total"]);
            SortName = form["sortname"];
            SortOrder = form["sortorder"];
            Primary = form["pk"];
        }
        public PageView(int PageIndex,int PageSize,string SortName,string SortOrder,int Total)
        {
            this.PageIndex = PageIndex;
            this.PageSize = PageSize;
            this.SortName = SortName;
            this.SortOrder = SortOrder;
            this.Total = Total;
        }
        //public PageView(int pageIndex, int pageSize)
        //{
        //    PageIndex = pageIndex;
        //    PageSize = pageSize;
        //}
        public virtual int PageIndex { get; set; }

        public virtual int PageSize { get; set; }

        public virtual string SortName { get; set; }

        public string SortOrder { get; set; }

        public virtual int Total { get; set; }

        public virtual string Primary { get; set; }

        public virtual string GetSqlOrder()
        {
            if (!string.IsNullOrEmpty(SortName))
                return string.Format(" {0} {1} ", SortName, SortOrder);
            return string.Empty;
        }
    }
}
